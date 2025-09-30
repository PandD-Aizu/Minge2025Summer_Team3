using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class InformationScreenView : MonoBehaviour
    {
        [SerializeField, Tooltip("情報画面の親オブジェクト")] private GameObject informationScreenParent;
        [SerializeField, Tooltip("情報画面に表示するパネル")] private List<GameObject> informationPanels;
        [SerializeField, Tooltip("情報画面上部のタイトルテキスト(パネルと同じ順にすること)")] private List<TextMeshProUGUI> titleText;
        [SerializeField, Tooltip("開始時に親を表示するか")] private bool startVisible = false;

        private int currentIndex = 0;            // 現在表示中パネルインデックス
        private bool firstShowActivated = false; // 初回表示でパネルをアクティブ化したか

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Initialize()
        {
            // Null / サイズ整合チェック
            if (informationPanels == null || informationPanels.Count == 0)
            {
                Debug.LogWarning("InformationScreenView: パネルが設定されていません", this);
                return;
            }
            if (titleText != null && titleText.Count != informationPanels.Count)
            {
                Debug.LogWarning("InformationScreenView: titleText と informationPanels の数が一致しません", this);
            }

            // 全パネル非表示 & 全タイトルを通常色へ
            for (int i = 0; i < informationPanels.Count; i++)
            {
                if (informationPanels[i]) informationPanels[i].SetActive(false);
                if (titleText != null && i < titleText.Count && titleText[i])
                    titleText[i].color = Color.white;
            }

            currentIndex = 0;
            firstShowActivated = false; // 最初に表示した瞬間にパネル0をアクティブ化する

            // 親の初期表示状態
            if (informationScreenParent)
                informationScreenParent.SetActive(startVisible);

            if (startVisible)
            {
                // 開始から表示する設定なら直ちにパネル0を有効化
                ActivatePanel(currentIndex);
                firstShowActivated = true;
            }
        }

        /// <summary>
        /// 情報画面の表示・非表示を切り替える
        /// </summary>
        public void SwitchInformationScreen()
        {
            if (!informationScreenParent)
                return;

            bool next = !informationScreenParent.activeSelf;
            informationScreenParent.SetActive(next);

            // 初めて表示された瞬間にパネルをアクティブ化
            if (next && !firstShowActivated && informationPanels.Count > 0)
            {
                ActivatePanel(currentIndex);
                firstShowActivated = true;
            }
        }

        /// <summary>
        /// 情報画面内のパネルを切り替える
        /// </summary>
        /// <param name="nextOrPrevious">+1: 次 / -1: 前</param>
        public void SwitchInformationPanel(int nextOrPrevious)
        {
            if (!informationScreenParent || !informationScreenParent.activeSelf)
                return; // 画面非表示中は無視

            if (informationPanels == null || informationPanels.Count == 0)
                return;

            if (!firstShowActivated)
            {
                // まだどれもアクティブ化していない(初表示前)なら現在インデックスをアクティブ化
                ActivatePanel(currentIndex);
                firstShowActivated = true;
                return;
            }

            int newIndex = (currentIndex + nextOrPrevious + informationPanels.Count) % informationPanels.Count;
            if (newIndex == currentIndex) return;
            ActivatePanel(newIndex);
        }

        /// <summary>
        /// 指定インデックスのパネルをアクティブ化し、他を非表示/色リセット
        /// </summary>
        private void ActivatePanel(int index)
        {
            for (int i = 0; i < informationPanels.Count; i++)
            {
                bool active = (i == index);
                if (informationPanels[i] && informationPanels[i].activeSelf != active)
                    informationPanels[i].SetActive(active);
                if (titleText != null && i < titleText.Count && titleText[i])
                    titleText[i].color = active ? Color.red : Color.white;
            }
            currentIndex = index;
        }
    }
}