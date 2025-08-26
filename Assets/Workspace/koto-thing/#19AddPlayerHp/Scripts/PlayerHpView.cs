using TMPro;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class PlayerHpView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI hpText;

        /// <summary>
        /// TEMP: 一応数値として表示しておく
        /// TODO: バイオみたいに緑→黄→赤とかにするのもありかも
        /// </summary>
        /// <param name="currentHp">現在のHP</param>
        public void UpdateHpText(float currentHp, float maxHp)
        {
            if (currentHp <= maxHp * 0.3f)
            {
                hpText.text = "<color=red>" + currentHp + "</color>" + " / " + maxHp;
            }
            else if (Mathf.Approximately(currentHp, maxHp))
            {
                hpText.text = "<color=green>" + currentHp + "</color>" + " / " + maxHp;
            }
            else
            {
                hpText.text = currentHp + " / " + maxHp;
            }
        }
    }
}