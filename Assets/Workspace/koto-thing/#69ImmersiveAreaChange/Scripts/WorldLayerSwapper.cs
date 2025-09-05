using UnityEngine;
using UnityEngine.Rendering;

namespace Workspace.koto_thing
{
    public class WorldLayerSwapper : MonoBehaviour
    {
        [SerializeField, Tooltip("入れ替え対象となるレイヤー名A")]
        private string layerNameA = "MainWorld";
        
        [SerializeField, Tooltip("入れ替え対象となるレイヤー名B")]
        private string layerNameB = "PortalWorld";

        private int layerA;
        private int layerB;

        private static readonly int StencilComp = Shader.PropertyToID("StencilComp");

        private void Start()
        {
            layerA = LayerMask.NameToLayer(layerNameA);
            layerB = LayerMask.NameToLayer(layerNameB);
        }

        /// <summary>
        /// すべての子オブジェクトのレイヤーを入れ替える
        /// </summary>
        public void SwapAllChildLayers()
        {
            foreach (Transform child in transform)
            {
                RecursiveSwapLayer(child);
            }
        }

        /// <summary>
        /// 再帰的に子オブジェクトのレイヤーを入れ替える
        /// </summary>
        /// <param name="target">入れ替える対象</param>
        private void RecursiveSwapLayer(Transform target)
        {
            int currentLayer = target.gameObject.layer;

            if (currentLayer == layerA)
            {
                target.gameObject.layer = layerB;
                SetMaterialStencil(target, CompareFunction.Equal);
            }
            else if (currentLayer == layerB)
            {
                target.gameObject.layer = layerA;
                SetMaterialStencil(target, CompareFunction.Always);
            }
            
            foreach (Transform child in target)
            {
                RecursiveSwapLayer(child);
            }
        }

        /// <summary>
        /// ステンシルバッファの比較関数を設定する
        /// </summary>
        /// <param name="target">入れ替える対象</param>
        /// <param name="func">入れ替える比較関数</param>
        private void SetMaterialStencil(Transform target, CompareFunction func)
        {
            var renderers = target.GetComponent<Renderer>();
            if (renderers != null)
                renderers.material.SetInt(StencilComp, (int)func);
        }
    }
}