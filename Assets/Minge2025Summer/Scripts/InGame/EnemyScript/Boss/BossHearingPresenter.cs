using System;
using Minge2025Summer.Scripts.InGame.AcousticsScript;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.EnemyScript.Boss
{
    public class BossHearingPresenter : MonoBehaviour
    {
        [Header("依存関係")] 
        [SerializeField] private BossCollisionDetectionModel detectionModel;
        [SerializeField, Tooltip("耳の位置, 未設定なら自身のTransform")] private Transform earTransform;
        
        [Header("聴覚パラメータ")]
        [SerializeField, Tooltip("聴覚感度: 音半径×感度以内なら聞こえる")] private float sensitivity = 1.0f;
        [SerializeField, Tooltip("自分が発した音は無視するか")] private bool ignoreSelfSource = true;

        private CompositeDisposable disposables = new ();

        private void Awake()
        {
            if (earTransform == null)
                earTransform = transform;
            
            if (detectionModel == null)
                detectionModel = GetComponentInChildren<BossCollisionDetectionModel>();
        }

        private void Start()
        {
            MessageBroker.Default
                .Receive<SoundEvent>()
                .Subscribe(OnSoundEvent)
                .AddTo(disposables);
        }

        private void OnSoundEvent(SoundEvent soundEvent)
        {
            if (ignoreSelfSource == true && soundEvent.Source == gameObject)
                return;

            if (earTransform == null || detectionModel == null)
                return;

            float hearRange = Mathf.Max(0.0f, soundEvent.Radius) * Mathf.Max(0.01f, sensitivity);
            float dist = Vector3.Distance(earTransform.position, soundEvent.Position);
            if (dist <= hearRange)
                detectionModel.SetLastKnownPosition(soundEvent.Position);
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }
    }
}