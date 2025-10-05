using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using Random = UnityEngine.Random;

namespace RenderingScript
{
    public class FlyController : MonoBehaviour
    {
        [Header("ハエの描画関連")] 
        [SerializeField] private Mesh flyMesh;
        [SerializeField] private Material flyMaterial;
        
        public ComputeShader computeShader;
        public int flyCount = 1000;
        
        [Header("カリング設定")] 
        [SerializeField, Tooltip("フラスタムベースの簡易オクルージョンカリングを有効化")] private bool enableFrustumCulling = true;
        [SerializeField, Tooltip("フラスタムに少し余裕を持たせる半径(Boid位置)補正")] private float frustumMargin = 0.0f;

        [Header("群れの挙動パラメータ")]
        public float separationWeight = 1.0f;
        public float alignmentWeight = 1.0f;
        public float cohesionWeight = 1.0f;
        public float perceptionRadius = 2.0f;

        [Header("追加のパラメータ")] 
        [SerializeField, Tooltip("目標地点")] private Transform target;
        [SerializeField, Tooltip("目標地点への追従度合い")] private float targetWeight = 1.0f;
        [SerializeField, Tooltip("境界のサイズ")] private Vector3 boundsSize = new Vector3(20, 20, 20);
        [SerializeField, Tooltip("境界への回避度合い")] private float boundsWeight = 2.0f;
        [SerializeField, Tooltip("ノイズの強さ")] private float noiseStrength = 0.5f;

        [Header("障害物回避パラメータ")] 
        [SerializeField, Tooltip("障害物オブジェクト")] private Transform[] obstacles;
        [SerializeField, Tooltip("障害物回避の強さ")] private float obstacleAvoidanceWeight = 3.0f;
        [SerializeField, Tooltip("障害物回避の感知距離")] private float obstacleAvoidanceRadius = 1.5f;
        
        private ComputeBuffer flyDataBuffer;
        private ComputeBuffer obstacleDataBuffer;
        private ComputeBuffer argsBuffer;
        private GameObject[] flies;
        private uint[] args = new uint[5] {0, 0, 0, 0, 0};
        
        private ComputeBuffer visibleBoidBuffer; // AppendStructuredBuffer用
        private ComputeBuffer countBuffer;       // 1uintのRawカウンタ読み出し

        struct FlyData
        {
            public Vector3 position;
            public Vector3 velocity;
            public Matrix4x4 matrix;
            public int state;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ObstacleData
        {
            public Vector3 position;
            public float radius;
        }

        private void Start()
        {
            InitializeFlies();
            InitializeObstacles();
            
            // 間接描画用のバッファを設定
            argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
            args[0] = (flyMesh != null) ? flyMesh.GetIndexCount(0) : 0;
            args[1] = (uint)flyCount;
            args[2] = (flyMesh != null) ? flyMesh.GetIndexStart(0) : 0;
            args[3] = (flyMesh != null) ? flyMesh.GetBaseVertex(0) : 0;
            argsBuffer.SetData(args);

            // カリング用バッファ生成
            if (enableFrustumCulling)
            {
                int stride = Marshal.SizeOf(typeof(FlyData));
                visibleBoidBuffer = new ComputeBuffer(flyCount, stride, ComputeBufferType.Append);
                visibleBoidBuffer.SetCounterValue(0);
                countBuffer = new ComputeBuffer(1, sizeof(uint), ComputeBufferType.Raw);
            }
        }

        private void Update()
        {
            int kernelIndex = computeShader.FindKernel("CSMain");
            
            computeShader.SetBuffer(kernelIndex, "boidDataBuffer", flyDataBuffer);
            if (obstacleDataBuffer == null)
            {
                obstacleDataBuffer = new ComputeBuffer(1, Marshal.SizeOf(typeof(ObstacleData)));
                obstacleDataBuffer.SetData(new ObstacleData[1]);
            }
            computeShader.SetBuffer(kernelIndex, "obstacleDataBuffer", obstacleDataBuffer);
            computeShader.SetInt("obstacleCount", obstacles == null ? 0 : obstacles.Length);
            
            computeShader.SetFloat("deltaTime", Time.deltaTime);
            computeShader.SetInt("boidCount", flyCount);
            computeShader.SetFloat("separationWeight", separationWeight);
            computeShader.SetFloat("alignmentWeight", alignmentWeight);
            computeShader.SetFloat("cohesionWeight", cohesionWeight);
            computeShader.SetFloat("perceptionRadius", perceptionRadius);

            if (target != null)
                computeShader.SetVector("targetPosition", target.position);
            
            computeShader.SetFloat("targetWeight", targetWeight);
            computeShader.SetVector("boundsSize", boundsSize);
            computeShader.SetVector("boundsCenter", transform.position);
            computeShader.SetFloat("boundsWeight", boundsWeight);
            computeShader.SetFloat("noiseStrength", noiseStrength);
            computeShader.SetFloat("obstacleAvoidanceWeight", obstacleAvoidanceWeight);
            computeShader.SetFloat("obstacleAvoidanceRadius", obstacleAvoidanceRadius);
            
            // シミュレーションカーネル実行
            int threadGroups = Mathf.CeilToInt(flyCount / 64.0f);
            computeShader.Dispatch(kernelIndex, threadGroups, 1, 1);

            ComputeBuffer drawBuffer = flyDataBuffer; // デフォルト

            if (enableFrustumCulling)
            {
                // 可視リスト用カウンタリセット
                visibleBoidBuffer.SetCounterValue(0);
                Camera cam = Camera.main;
                if (cam != null)
                {
                    // ViewProjection行列を送る
                    Matrix4x4 vp = cam.projectionMatrix * cam.worldToCameraMatrix;
                    computeShader.SetMatrix("viewProjMatrix", vp);
                    computeShader.SetFloat("frustumMargin", frustumMargin);
                    
                    // カリングカーネル
                    int cullKernel = computeShader.FindKernel("CSCull");
                    computeShader.SetInt("boidCount", flyCount);
                    computeShader.SetBuffer(cullKernel, "boidDataBuffer", flyDataBuffer);
                    computeShader.SetBuffer(cullKernel, "visibleBoidBuffer", visibleBoidBuffer);
                    computeShader.Dispatch(cullKernel, threadGroups, 1, 1);
                    
                    // AppendカウンタをargsBuffeのinstanceCountスロットにコピー
                    // args: [0] = indexCountPerInstance, [1] = instanceCount, [2] = startIndex, [3] = baseVertex, [4] = startInstance
                    ComputeBuffer.CopyCount(visibleBoidBuffer, argsBuffer, sizeof(uint));
                    drawBuffer = visibleBoidBuffer;
                }
                else
                {
                    // カメラが無い場合は全件描画にフォールバック
                    args[1] = (uint)flyCount;
                    argsBuffer.SetData(args);
                }
            }
            else
            {
                // カリング無効: インスタンス数固定
                args[1] = (uint)flyCount;
                argsBuffer.SetData(args);
            }
            
            // GPUで描画
            flyMaterial.SetBuffer("boidDataBuffer", drawBuffer);
            var drawBounds = new Bounds(transform.position, boundsSize * 2f);
            Graphics.DrawMeshInstancedIndirect(flyMesh, 0, flyMaterial, drawBounds, argsBuffer);
        }

        /// <summary>
        /// ハエの初期化
        /// </summary>
        private void InitializeFlies()
        {
            List<FlyData> initialFlyData = new List<FlyData>(flyCount);
            for (int i = 0; i < flyCount; i++)
            {
                Vector3 pos = transform.position + Random.insideUnitSphere * 5.0f;
                Vector3 vel = Random.insideUnitSphere * 2.0f;

                FlyData flyData = new FlyData()
                {
                    position = pos,
                    velocity = vel,
                    matrix = Matrix4x4.TRS(pos, Quaternion.LookRotation(vel), Vector3.one * 0.1f)
                };
                
                initialFlyData.Add(flyData);
            }

            flyDataBuffer = new ComputeBuffer(flyCount, Marshal.SizeOf(typeof(FlyData)));
            flyDataBuffer.SetData(initialFlyData);
        }

        /// <summary>
        /// 障害物データの初期化
        /// </summary>
        private void InitializeObstacles()
        {
            if (obstacles == null || obstacles.Length == 0)
                return;

            ObstacleData[] obstacleData = new ObstacleData[obstacles.Length];
            for (int i = 0; i < obstacles.Length; i++)
            {
                obstacleData[i] = new ObstacleData()
                {
                    position = obstacles[i].position,
                    radius = obstacles[i].localScale.x / 2.0f,
                };
            }
            
            obstacleDataBuffer = new ComputeBuffer(obstacles.Length, Marshal.SizeOf(typeof(ObstacleData)));
            obstacleDataBuffer.SetData(obstacleData);
        }

        private void OnDestroy()
        {
            if (flyDataBuffer != null)
                flyDataBuffer.Release();
            
            if (obstacleDataBuffer != null)
                obstacleDataBuffer.Release();
            
            if (argsBuffer != null)
                argsBuffer.Release();

            if (visibleBoidBuffer != null)
                visibleBoidBuffer.Release();
            if (countBuffer != null)
                countBuffer.Release();
        }

        /// <summary>
        /// 境界線を表示
        /// </summary>
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, boundsSize);
        }
    }
}