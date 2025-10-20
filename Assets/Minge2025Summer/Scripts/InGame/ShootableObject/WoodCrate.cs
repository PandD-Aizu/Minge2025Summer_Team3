using System.Collections.Generic;
using System.Linq;
using Minge2025Summer.Scripts.InGame.ShootableObject.Interface;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ShootableObject
{
    public class WoodCrate : MonoBehaviour, IShootableObject
    {
        [SerializeField] private float destroyDelay = 2.0f;
        
        /// <summary>
        /// オブジェクトを破壊する
        /// </summary>
        public void Feedback()
        {
            gameObject.GetComponentsInChildren<Rigidbody>()
                .ToList()
                .ForEach(r => {
                    r.isKinematic = false;
                    r.transform.SetParent(null);
                    
                    var vect = new Vector3(
                        Random.Range(-3f, 3f),
                        Random.Range(0f, 3f),
                        Random.Range(-3f, 3f)
                    );
                    r.AddForce(vect, ForceMode.Impulse);
                    r.AddTorque(vect, ForceMode.Impulse);
                    
                    Destroy(r.gameObject, destroyDelay);
                });
            
            Destroy(gameObject);
        }
    }
}