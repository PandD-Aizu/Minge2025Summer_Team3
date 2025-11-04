using System.Collections;
using TMPro;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.TimelineScript
{
    public class GlitchTyper : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textMeshPro;

        [Header("タイピング速度設定")] 
        [SerializeField] private float minTypeDelay = 0.03f;
        [SerializeField] private float maxTypeDelay = 0.15f;
        
        [Header("グリッチ設定")]
        [SerializeField] private float scatterAmount = 5.0f;
        
        [Header("消滅設定")]
        [Tooltip("表示完了後、消滅するまでの時間（秒）")]
        public float destroyDelay = 1.0f;

        private string fullText;
        private TMP_TextInfo textInfo;
        private TMP_MeshInfo[] cachedMeshInfo;

        private void Start()
        {
            StartTyping();
        }

        private void Update()
        {
            // メッシュ情報やキャッシュが準備できていなければ処理しない
            if (textInfo == null || cachedMeshInfo == null)
                return;

            // 表示されている文字が0なら処理しない
            int currentVisibleChars = textMeshPro.maxVisibleCharacters;
            if (currentVisibleChars == 0) 
                return;

            // 表示されているすべての文字をループ
            for (int i = 0; i < currentVisibleChars; i++)
            {
                if (i >= textInfo.characterCount) 
                    break;

                // 文字情報を取得
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) 
                    continue;

                int materialIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;

                // キャッシュしておいた、頂点情報を取得
                Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;

                // これから変更する現在の頂点配列を取得
                Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;

                // この文字専用のランダムなズレ（オフセット）を計算
                Vector3 offset = new Vector3(
                    Random.Range(-scatterAmount, scatterAmount),
                    Random.Range(-scatterAmount, scatterAmount),
                    0
                );

                // 文字を構成する4つの頂点すべてに、元の位置を基準にオフセットを加える
                destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] + offset;
                destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] + offset;
                destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] + offset;
                destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] + offset;
            }
            
            // 変更した頂点データをメッシュに反映させる
            textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
        }

        /// <summary>
        /// タイピングを開始する
        /// </summary>
        /// <param name="newText">新しくセットする文字列</param>
        public void StartTyping(string newText = null)
        {
            if (newText != null)
                textMeshPro.text = newText;

            fullText = textMeshPro.text;
            
            StopAllCoroutines();
            StartCoroutine(TypeAndGlitch());
        }

        IEnumerator TypeAndGlitch()
        {
            // 初期化
            textMeshPro.maxVisibleCharacters = 0; 
            textMeshPro.ForceMeshUpdate();
            textInfo = textMeshPro.textInfo;
            cachedMeshInfo = textInfo.CopyMeshInfoVertexData();

            // 1文字ずつランダムな間隔で表示
            int totalChars = fullText.Length;
            int visibleCount = 0;

            while (visibleCount < totalChars)
            {
                visibleCount++;
                textMeshPro.maxVisibleCharacters = visibleCount;

                float delay = Random.Range(minTypeDelay, maxTypeDelay);
                yield return new WaitForSeconds(delay);
            }
        
            // 指定した秒数だけ待機
            yield return new WaitForSeconds(destroyDelay);
        
            // このGameObject自身を破棄する
            Destroy(gameObject);
        }
    }
}