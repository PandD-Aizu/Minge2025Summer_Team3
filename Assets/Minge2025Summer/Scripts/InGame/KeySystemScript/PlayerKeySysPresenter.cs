using Minge2025Summer.Scripts.InGame.ItemScript;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.KeySystemScript
{
    public class PlayerKeySysPresenter : MonoBehaviour
    {
        [SerializeField] private PlayerKeySysModel model;
        [SerializeField] private PlayerKeySysView view;
        [SerializeField] private PlayerItemModel playerItemModel;

        private CompositeDisposable disposables = new ();

        private void Start()
        {
            SubscribeEvents();   
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                model.TryInteractKey(playerItemModel);
            }   
        }
        
        private void SubscribeEvents()
        {
            model.OnDoorOpened
                .Subscribe(door =>
                {
                    door.TryOpen();
                })
                .AddTo(disposables);

            model.OnDoorOpenFailed
                .Subscribe(_ =>
                {
                    view.UpdateCaptionText();
                })
                .AddTo(disposables);
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