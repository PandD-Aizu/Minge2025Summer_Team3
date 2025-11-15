using Minge2025Summer.Scripts.InGame.CaptionScript;
using Minge2025Summer.Scripts.InGame.GunScript;
using Minge2025Summer.Scripts.InGame.ReiScript.GunScript;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.SpecialEventTriggerScript
{
    public class InvisibleWallTrigger : MonoBehaviour
    {
        [Header("表示する字幕のテキスト")]
        [SerializeField] private string captionText;

        [Header("キャプションテキストのモデルクラス")] 
        [SerializeField] private CaptionModel model;

        private bool isAlreadyTriggered = false;

        private void Start()
        {
            if (model == null)
                model = FindFirstObjectByType<CaptionModel>();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("Player") && !isAlreadyTriggered)
            {
                // プレイヤーが銃を持っているかどうかを確認
                var playerGunModel = other.transform.GetComponentInChildren<WeaponModel>();
                if (playerGunModel.CurrentEquippedWeapon != null)
                {
                    // 持っている場合、不可視の壁を消す
                    var collision = transform.GetChild(0);
                    if (collision != null)
                        collision.gameObject.SetActive(false);
                    isAlreadyTriggered = true;
                }
                else
                {
                    // 持っていない場合、字幕を表示
                    model.RaiseShow(captionText);
                }
            }
        }
    }
}