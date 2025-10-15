using System;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ItemScript
{
    public class HintMarkerPresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private HintMarkerModel model;
        [SerializeField] private HintMarkerView view;

        private readonly CompositeDisposable disposables = new();

        private void Start()
        {
            if (model == null) 
                model = GetComponent<HintMarkerModel>();
            if (view == null) 
                view = GetComponentInChildren<HintMarkerView>();
            
            view.Initialize();
            view.SwitchVisibility(false);
        }

        private void LateUpdate()
        {
            // プレイヤー参照が無い間は位置のみ維持（表示されていればそのまま）
            var target = model.TargetRoot;
            view.UpdatePositionAndScale(
                target,
                model.VerticalOffset,
                model.UseRendererBounds,
                model.ExtraHeight,
                model.PlayerTransform,
                model.MaxDistance,
                model.ScaleWithDistance,
                model.ScaleCurve,
                model.BaseScale,
                model.MaxScaleMultiplier,
                model.PositionOffset
            );

            if (model.PlayerTransform != null)
            {
                view.RotateBillboard(model.Billboard);
                view.UpdateAlphaByDistance(model.PlayerTransform, model.MaxDistance, model.MinDistance);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            model.PlayerTransform = other.transform;
            view.SwitchVisibility(true);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            model.PlayerTransform = null;
            view.SwitchVisibility(false);
        }

        public void Dispose()
        {
            disposables.Dispose();
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}