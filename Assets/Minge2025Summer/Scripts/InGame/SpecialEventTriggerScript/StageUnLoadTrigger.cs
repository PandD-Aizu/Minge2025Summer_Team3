using System.Collections.Generic;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.SpecialEventTriggerScript
{
    public class StageUnLoadTrigger : MonoBehaviour
    {
        [SerializeField, Tooltip("破棄するステージのリスト")]
        private List<GameObject> unloadStageList = new　();

        [SerializeField, Tooltip("破棄後にこのコンポーネントを無効化する")]
        private bool disableAfterUnload = true;

        private bool hasTriggered; // 実行は一度だけ

        private void Start()
        {
            unloadStageList.Add(GameObject.FindGameObjectWithTag("RandomMap"));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (hasTriggered)
                return;
            
            if (!other.CompareTag("Player")) 
                return;

            hasTriggered = true;
            UnloadStages();

            if (disableAfterUnload)
                enabled = false; // 以降の処理を停止
        }

        private void UnloadStages()
        {
            if (unloadStageList == null || unloadStageList.Count == 0)
                return;
            
            foreach (var stageObject in unloadStageList)
            {
                if (stageObject == null) 
                    continue;
                
                Destroy(stageObject);
            }
        }
    }
}