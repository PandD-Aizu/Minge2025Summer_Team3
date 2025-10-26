using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.FearEffectScript
{
    public class Subliminal3DObjectDetectionArea_Model : MonoBehaviour
    {
        [SerializeField] private GameObject this3DObject; //対応するオブジェクト
        public float DisplayTime = 3.0f; //表示する時間

        //非表示にする
        public void HideThisObject()
        {
            if (this3DObject != null)
                this3DObject.gameObject.SetActive(false);
        }

        //表示する
        public void AppearThisObject()
        {
            if (this3DObject != null)
                this3DObject.gameObject.SetActive(true);
        }

        //Deleteする
        public void DestroyThisObject()
        {
            if (this3DObject != null)
                Destroy(this3DObject.gameObject);
        }
    }
}
