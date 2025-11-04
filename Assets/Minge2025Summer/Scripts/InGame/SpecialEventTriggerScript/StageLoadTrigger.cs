using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Minge2025Summer.Scripts.InGame.SpecialEventTriggerScript
{
    public class StageLoadTrigger : MonoBehaviour
    {
        [SerializeField] private List<string> stageAddressList = new ();
        [SerializeField] private List<Transform> spawnPointList = new ();
        [SerializeField] private GameObject parentObject;

        private bool hasTriggered;

        private void Start()
        {
            if (stageAddressList.Count != spawnPointList.Count)
                Debug.LogError("StageLoadTrigger: ステージの数とスポーンポイントの数が一致しません。");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            LoadStageAsync();
            
            if (hasTriggered)
                return;
        }

        /// <summary>
        /// ステージを非同期でロードする
        /// </summary>
        private void LoadStageAsync()
        {
            for (int i = 0 ; i < stageAddressList.Count; i++)
            {
                string address = stageAddressList[i];
                Transform spawnPoint = spawnPointList[i];

                Addressables.InstantiateAsync(address, spawnPoint.position, Quaternion.identity, parentObject.transform);
            }
        }
    }
}