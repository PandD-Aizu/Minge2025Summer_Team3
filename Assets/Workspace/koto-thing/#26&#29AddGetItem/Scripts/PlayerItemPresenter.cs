using UniRx;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class PlayerItemPresenter : MonoBehaviour
    {
        [SerializeField] private PlayerItemModel model;
        [SerializeField] private PlayerItemView view;

        private CompositeDisposable disposable = new ();

        private void Start()
        {
            SubscribeEvents();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                model.GetItem();
            }
            
            model.UpdateItemList();
        }

        private void SubscribeEvents()
        {
            
        }

        private void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            disposable.Dispose();
        }
    }
}