using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.EnemyScript
{
    public class PoliceHPPresenter : MonoBehaviour
    {
        [SerializeField] private PoliceHPModel model;
        
        private CompositeDisposable disposables = new CompositeDisposable(); 

        private void Start()
        {
            SubscribeEvents();
        }

        private void Update()
        {
            
        }

        private void SubscribeEvents()
        {
            
        }

        private void OnDestroy()
        {
            Dispose();
        }

        private void Dispose()
        {
            disposables.Dispose();
        }
    }
}