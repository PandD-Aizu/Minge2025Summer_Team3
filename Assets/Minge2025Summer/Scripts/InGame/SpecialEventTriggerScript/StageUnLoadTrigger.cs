using System.Collections.Generic;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.SpecialEventTriggerScript
{
    public class StageUnLoadTrigger : MonoBehaviour
    {
        [SerializeField, Tooltip("破棄するステージのリスト")]
        private List<GameObject> unloadStageList = new　();
        
        [SerializeField, Tooltip("タグで破棄する場合は有効化する")]
        private List<string> unloadStageTagList = new ();

        [SerializeField, Tooltip("破棄後にこのコンポーネントを無効化する")]
        private bool disableAfterUnload = true;

        private bool hasTriggered;

        private void OnTriggerEnter(Collider other)
        {
            if (hasTriggered)
                return;
            
            if (!other.CompareTag("Player")) 
                return;
            
            foreach (var tag in unloadStageTagList)
            {
                var stageObject = GameObject.FindGameObjectWithTag(tag);
                if (stageObject != null)
                    unloadStageList.Add(stageObject);
            }

            hasTriggered = true;
            UnloadStages();

            if (disableAfterUnload)
                enabled = false;
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