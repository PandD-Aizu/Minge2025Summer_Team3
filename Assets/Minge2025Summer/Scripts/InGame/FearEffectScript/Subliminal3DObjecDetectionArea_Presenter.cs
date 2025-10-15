using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.FearEffectScript
{
    public class Subliminal3DObjectDetectionArea_Presenter : MonoBehaviour
    {
        Subliminal3DObjectDetectionArea_Model modelScript;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            modelScript = this.gameObject.GetComponent<Subliminal3DObjectDetectionArea_Model>();

            //オブジェクトを非表示にする
            modelScript.HideThisObject();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        //範囲内に入った時に対応するオブジェクトを表示する
        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                modelScript.AppearThisObject();

                //パターン１：一定時間後に消す
                modelScript.Invoke("DestroyThisObject", modelScript.DisplayTime);
            }
        }

        //パターン２：範囲外に出た時に消す
        /*public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            modelScript.DestroyThisObject();
        }
    }*/
    }
}
