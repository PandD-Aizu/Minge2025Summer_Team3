using UniRx;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class PoliceHearingPresenter : MonoBehaviour
    {
        [Header("依存関係")]
        [SerializeField] private PoliceCollisionDetectionModel detectionModel;
        [SerializeField, Tooltip("耳の位置。未設定なら自身のTransform")] private Transform earTransform;

        [Header("聴覚パラメータ")] 
        [SerializeField, Tooltip("聴覚感度: 音半径×感度以内なら聞こえる")] private float sensitivity = 1.0f;
        [SerializeField, Tooltip("自分が発した音は無視するか")] private bool ignoreSelfSource = true;

        private CompositeDisposable disposables = new CompositeDisposable();

        private void Awake()
        {
            if (earTransform == null) earTransform = transform;
            if (detectionModel == null) detectionModel = GetComponentInChildren<PoliceCollisionDetectionModel>();
        }

        private void Start()
        {
            MessageBroker.Default
                .Receive<SoundEvent>()
                .Subscribe(OnSoundEvent)
                .AddTo(disposables);
        }

        /// <summary>
        /// サンドイベントを受け取ったときの処理
        /// </summary>
        /// <param name="se">サウンドイベント構造体</param>
        private void OnSoundEvent(SoundEvent se)
        {
            if (ignoreSelfSource && se.Source == gameObject) return;
            if (earTransform == null || detectionModel == null) return;

            float hearRange = Mathf.Max(0f, se.Radius) * Mathf.Max(0.01f, sensitivity);
            float dist = Vector3.Distance(earTransform.position, se.Position);
            if (dist <= hearRange)
            {
                detectionModel.SetLastKnownPosition(se.Position);
            }
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }
    }
}
