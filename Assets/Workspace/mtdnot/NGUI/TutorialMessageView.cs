using UnityEngine;
using TMPro;

public class TutorialSequence : MonoBehaviour
{
    [SerializeField] private TMP_Text tutorialText;  // チュートリアルメッセージを表示するテキスト
    private int step = 0;                            // 現在のチュートリアルステップ

    /// <summary>
    /// 初期化処理。最初のメッセージを表示する。
    /// </summary>
    private void Start()
    {
        ShowMessage("WASDで移動する");
    }

    /// <summary>
    /// 毎フレームの入力を監視し、ステップごとの処理を進める。
    /// </summary>
    private void Update()
    {
        switch (step)
        {
            case 0: // WASD移動
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
                    Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
                {
                    NextStep("Shiftを押しながら移動して走る");
                }
                break;

            case 1: // Shift走り
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    NextStep("Spaceキーでしゃがむ");
                }
                break;

            case 2: // Spaceしゃがみ
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    NextStep("Eキーでアイテムを拾う");
                }
                break;

            case 3: // Eアイテム取得
                if (Input.GetKeyDown(KeyCode.E))
                {
                    NextStep("Fキーで懐中電灯を切り替える");
                }
                break;

            case 4: // F懐中電灯
                if (Input.GetKeyDown(KeyCode.F))
                {
                    NextStep("左クリックで射撃する");
                }
                break;

            case 5: // 左クリック射撃
                if (Input.GetMouseButtonDown(0))
                {
                    NextStep("右クリックで照準を合わせる");
                }
                break;

            case 6: // 右クリック照準
                if (Input.GetMouseButtonDown(1))
                {
                    NextStep("追加予定");
                }
                break;
        }
    }

    /// <summary>
        /// メッセージを表示する。
    /// </summary>
    /// <param name="message">表示するメッセージ</param>
    private void ShowMessage(string message)
    {
        tutorialText.text = message;
    }

    /// <summary>
    /// 次のステップに進み、新しいメッセージを表示する
    /// </summary>
    /// <param name="nextMessage">次のステップで表示するメッセージ</param>
    private void NextStep(string nextMessage)
    {
        step++;
        ShowMessage(nextMessage);
    }
}
