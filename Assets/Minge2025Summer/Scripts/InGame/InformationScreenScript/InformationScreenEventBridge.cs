using UniRx;
using UnityEngine;

namespace Minge2025Summer.Main.InGame
{
    /// <summary>
    /// MessageBroker で Publish された情報画面表示/非表示イベントを受け取り、モデルへ転送するブリッジ。
    /// </summary>
    public class InformationScreenEventBridge : MonoBehaviour
    {
        [SerializeField] private InformationScreenModel model;
        private readonly CompositeDisposable disposables = new();

        private void Awake()
        {
            if (model == null) model = GetComponent<InformationScreenModel>();
        }

        private void OnEnable()
        {
            MessageBroker.Default.Receive<ShowInformationScreen>()
                .Subscribe(msg => model?.RequestShow(msg.Address))
                .AddTo(disposables);

            MessageBroker.Default.Receive<HideInformationScreen>()
                .Subscribe(_ => model?.RequestHide())
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

