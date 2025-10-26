using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ObjectiveScreenScript
{
    public class ObjectiveTextEventBridge : MonoBehaviour
    {
        [SerializeField] private ObjectiveTextModel model;
        private readonly CompositeDisposable disposables = new();

        private void OnEnable()
        {
            // 表示イベント
            MessageBroker.Default
                .Receive<ShowInformationScreen>()
                .Subscribe(msg => model?.RequestShow(msg.Address))
                .AddTo(disposables);
        }

        private void OnDisable()
        {
            disposables.Clear();
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }
    }
}
