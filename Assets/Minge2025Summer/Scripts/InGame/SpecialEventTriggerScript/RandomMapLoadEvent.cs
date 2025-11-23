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
            
            AsyncOperationHandle<GameObject> map = Addressables.InstantiateAsync(randomMapAddress);
            await map.Task;
            
            Vector3 mapPosition = new Vector3(
                mapStartMarker.transform.position.x + mapCol * mapSize,
                mapStartMarker.transform.position.y,
                mapStartMarker.transform.position.z + mapRow * mapSize);
            map.Result.transform.position = mapPosition;
            
            var generator = map.Result.GetComponent<MapGenerator>();
            generator.StartMarker = mapStartMarker.transform;
            generator.NavMeshSurfaces = mapSurfaces;
            generator.GenerateMap();
        }
    }
}