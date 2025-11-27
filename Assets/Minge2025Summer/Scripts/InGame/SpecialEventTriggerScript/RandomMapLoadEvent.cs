using System.Collections.Generic;
using Minge2025Summer.Scripts.InGame.RandomMapGeneratorScript;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Minge2025Summer.Scripts.InGame.SpecialEventTriggerScript
{
    public class RandomMapLoadEvent : MonoBehaviour
    {
        [SerializeField] private AssetReference randomMapAddress;
        [SerializeField] private int mapRow;
        [SerializeField] private int mapCol;
        [SerializeField] private int mapSize;
        [SerializeField,Tooltip("生成する特殊部屋の数")] private int numberOfSpecialRoom;
        [SerializeField] private GameObject mapStartMarker;
        [SerializeField] private List<NavMeshSurface> mapSurfaces;
        
        private bool isAlreadyLoaded = false;
        
        private async void OnTriggerEnter(Collider other)
        {
            if (!other.transform.CompareTag("Player"))
                return;

            if (isAlreadyLoaded)
                return;

            isAlreadyLoaded = true;
            
            AsyncOperationHandle<GameObject> mapHandle = randomMapAddress.InstantiateAsync();
            await mapHandle.Task;

            if (mapHandle.Status != AsyncOperationStatus.Succeeded || mapHandle.Result == null)
            {
                Debug.LogError("[RandomMapLoadEvent] Addressables failed to instantiate map.");
                isAlreadyLoaded = false;
                return;
            }

            Vector3 mapPosition = new Vector3(
                mapStartMarker.transform.position.x + mapCol * mapSize,
                mapStartMarker.transform.position.y,
                mapStartMarker.transform.position.z + mapRow * mapSize);
            mapHandle.Result.transform.position = mapPosition;

            var generator = mapHandle.Result.GetComponent<MapGenerator>();
            if (generator == null)
            {
                Debug.LogError("[RandomMapLoadEvent] Map Generator not found on instantiated map.");
                return;
            }

            generator.StartMarker = mapStartMarker.transform;
            generator.NavMeshSurfaces = mapSurfaces;
            generator.NumberOfSpecialRoom = numberOfSpecialRoom;
            generator.GenerateMap();
        }
    }
}