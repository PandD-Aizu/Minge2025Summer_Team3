using System.Collections.Generic;
using Minge2025Summer.Scripts.InGame.RandomMapGeneratorScript;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Minge2025Summer.Scripts.InGame.SpecialEventTriggerScript
{
    public class RandomMapLoadEvent : MonoBehaviour
    {
        [SerializeField] private string randomMapAddress;
        [SerializeField] private int mapRow;
        [SerializeField] private int mapCol;
        [SerializeField] private int mapSize;
        [SerializeField,Tooltip("生成する特殊部屋の数")] private int numberOfSpecialRoom;
        [SerializeField] private GameObject mapStartMarker;
        [SerializeField] private List<NavMeshSurface> mapSurfaces;
        
        private bool isAlreadyLoaded = false;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.transform.CompareTag("Player")) 
                return;
            
            if (isAlreadyLoaded) 
                return;
            
            isAlreadyLoaded = true;
            
            var map = Addressables.InstantiateAsync(randomMapAddress).WaitForCompletion();
            Vector3 mapPosition = new Vector3(
                mapStartMarker.transform.position.x + mapCol * mapSize,
                mapStartMarker.transform.position.y,
                mapStartMarker.transform.position.z + mapRow * mapSize);
            map.transform.position = mapPosition;
            
            var generator = map.GetComponent<MapGenerator>();
            generator.StartMarker = mapStartMarker.transform;
            generator.NavMeshSurfaces = mapSurfaces;
            generator.NumberOfSpecialRoom = numberOfSpecialRoom;
            generator.GenerateMap();
        }
    }
}