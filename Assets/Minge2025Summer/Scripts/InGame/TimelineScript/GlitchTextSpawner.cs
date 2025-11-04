using TMPro;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.TimelineScript
{
    public class GlitchTextSpawner : MonoBehaviour
    {
        [Header("生成設定")]
        [Tooltip("生成するグリッチテキストのプレハブ")]
        public GameObject glitchyTextPrefab;

        [Tooltip("テキストを生成する範囲（このRectTransformの範囲内）")]
        public RectTransform spawnArea;

        [Tooltip("テキストを生成する最短間隔（秒）")]
        public float minSpawnInterval = 0.1f;
        [Tooltip("テキストを生成する最長間隔（秒）")]
        public float maxSpawnInterval = 0.5f;

        [Tooltip("テキストの色")] 
        public Color textColor = Color.black;

        [Header("テキスト内容")]
        [Tooltip("表示させるテキストの候補（ここからランダムに選ばれる）")]
        public string[] possibleTexts = {
            "ERROR",
            "ACCESS DENIED",
            "SYSTEM_FAILURE",
            "0xDEADBEEF",
            "Depersonalisation",
            "WARNING: MEMORY_LEAK"
        };

        private float timer;
        private float nextSpawnTime;

        void Start()
        {
            // SpawnAreaが設定されていなければ、自分自身のRectTransformを使う
            if (spawnArea == null)
            {
                spawnArea = GetComponent<RectTransform>();
            }
            SetNextSpawnTime();
        }

        void Update()
        {
            timer += Time.deltaTime;

            // 次の生成時間が来たら
            if (timer >= nextSpawnTime)
            {
                SpawnText();
                SetNextSpawnTime(); // 次の時間を再設定
                timer = 0;          // タイマーリセット
            }
        }

        void SetNextSpawnTime()
        {
            // 次にテキストを生成するまでの時間をランダムに決める
            nextSpawnTime = Random.Range(minSpawnInterval, maxSpawnInterval);
        }

        void SpawnText()
        {
            if (glitchyTextPrefab == null || spawnArea == null)
            {
                Debug.LogWarning("PrefabまたはSpawnAreaが設定されていません。");
                return;
            }

            // spawnArea のローカルの矩形サイズを取得
            Rect rect = spawnArea.rect;
            
            // 矩形内のランダムなローカル座標を計算
            float randomX = Random.Range(rect.xMin, rect.xMax);
            float randomY = Random.Range(rect.yMin, rect.yMax);
            Vector2 localPosition = new Vector2(randomX, randomY);

            // プレハブを spawnArea の子としてインスタンス化
            GameObject newTextObject = Instantiate(glitchyTextPrefab, spawnArea.transform);
            
            // ローカル座標を設定
            newTextObject.GetComponent<RectTransform>().anchoredPosition = localPosition;
            
            // テキストの色を設定
            var textComponent = newTextObject.GetComponent<TextMeshProUGUI>();
            textComponent.color = textColor;

            // ランダムなテキストを設定してタイピング開始
            GlitchTyper typer = newTextObject.GetComponent<GlitchTyper>();
            if (typer != null)
            {
                // Possible Texts 配列からランダムに1つ選ぶ
                string textToDisplay = possibleTexts[Random.Range(0, possibleTexts.Length)];
                
                // テキストをセットしてタイピングを開始させる
                typer.StartTyping(textToDisplay);
            }
        }
    }
}