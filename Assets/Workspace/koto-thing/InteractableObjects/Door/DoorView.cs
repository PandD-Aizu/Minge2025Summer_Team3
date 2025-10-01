using UnityEngine;
using DG.Tweening;

namespace Workspace.koto_thing
{
    public class DoorView : MonoBehaviour
    {
        [Header("可動対象(未指定なら自身)")]
        [SerializeField] private Transform movableRoot;
        [Header("スライドドア設定")]
        [SerializeField] private Vector3 slideLocalDirection = Vector3.right;
        [Header("回転ドア設定")]
        [SerializeField] private Vector3 swingLocalAxis = Vector3.up;
        [SerializeField] private bool invertSwing = false;
        [Tooltip("ヒンジ(回転中心)となるローカル位置。未設定(0,0,0)なら Transform 原点。")]
        [SerializeField] private Vector3 hingeLocalPivot = Vector3.zero;

        private Vector3 closedLocalPos;
        private Quaternion closedLocalRot;
        private Tween currentTween;
        private bool initialized;

        public void Initialize()
        {
            if (initialized) 
                return;
            
            if (movableRoot == null) 
                movableRoot = transform;
            
            closedLocalPos = movableRoot.localPosition;
            closedLocalRot = movableRoot.localRotation;
            initialized = true;
        }

        public void PlayOpen(DoorModel model)
        {
            if (!initialized) Initialize();
            if (currentTween != null && currentTween.IsActive()) currentTween.Kill();

            switch (model.DoorType)
            {
                case DoorType.Sliding:
                    PlayOpenSliding(model);
                    break;
                case DoorType.Swing:
                    PlayOpenSwing(model);
                    break;
            }
        }

        private void PlayOpenSliding(DoorModel model)
        {
            Vector3 to = closedLocalPos + slideLocalDirection.normalized * model.OpenDistance;
            currentTween = movableRoot.DOLocalMove(to, model.OpenDuration).SetEase(Ease.InOutSine);
        }

        private void PlayOpenSwing(DoorModel model)
        {
            float angle = (invertSwing ? -1f : 1f) * model.OpenAngle;
            Vector3 axis = swingLocalAxis == Vector3.zero ? Vector3.up : swingLocalAxis.normalized;

            if (hingeLocalPivot == Vector3.zero)
            {
                Quaternion toRot = closedLocalRot * Quaternion.AngleAxis(angle, axis);
                currentTween = DOTween.To(
                    () => 0f,
                    v => movableRoot.localRotation = Quaternion.Slerp(closedLocalRot, toRot, v),
                    1f,
                    model.OpenDuration
                ).SetEase(Ease.InOutSine);
            }
            else
            {
                Vector3 pivot = hingeLocalPivot;
                Quaternion fromRot = closedLocalRot;
                Quaternion toRot = closedLocalRot * Quaternion.AngleAxis(angle, axis);
                Vector3 pivotClosed = fromRot * pivot;

                currentTween = DOTween.To(
                    () => 0f,
                    v =>
                    {
                        Quaternion curRot = Quaternion.Slerp(fromRot, toRot, v);
                        Vector3 curPivot = curRot * pivot;
                        Vector3 delta = pivotClosed - curPivot;
                        movableRoot.localRotation = curRot;
                        movableRoot.localPosition = closedLocalPos + delta;
                    },
                    1f,
                    model.OpenDuration
                ).SetEase(Ease.InOutSine);
            }
        }

        public void PlayClose(float duration)
        {
            if (!initialized) Initialize();
            if (currentTween != null && currentTween.IsActive()) currentTween.Kill();

            if (hingeLocalPivot == Vector3.zero)
            {
                var fromRot = movableRoot.localRotation;
                currentTween = DOTween.Sequence()
                    .Join(movableRoot.DOLocalMove(closedLocalPos, duration).SetEase(Ease.InOutSine))
                    .Join(DOTween.To(
                        () => 0f,
                        t => movableRoot.localRotation = Quaternion.Slerp(fromRot, closedLocalRot, t),
                        1f,
                        duration
                    ).SetEase(Ease.InOutSine));
            }
            else
            {
                Vector3 pivot = hingeLocalPivot;
                Quaternion fromRot = movableRoot.localRotation;
                Quaternion toRot = closedLocalRot;
                // 不動基準は常に閉じた状態の pivot
                Vector3 pivotClosed = closedLocalRot * pivot;

                currentTween = DOTween.To(
                    () => 0f,
                    t =>
                    {
                        Quaternion curRot = Quaternion.Slerp(fromRot, toRot, t);
                        Vector3 curPivot = curRot * pivot;
                        Vector3 delta = pivotClosed - curPivot;
                        movableRoot.localRotation = curRot;
                        movableRoot.localPosition = closedLocalPos + delta;
                    },
                    1f,
                    duration
                ).SetEase(Ease.InOutSine);
            }
        }

        public void Stop()
        {
            if (currentTween != null && currentTween.IsActive()) currentTween.Kill();
        }

        public void SetInstantOpen(DoorModel model)
        {
            if (!initialized) Initialize();
            if (currentTween != null && currentTween.IsActive()) currentTween.Kill();
            if (model == null) return;

            switch (model.DoorType)
            {
                case DoorType.Sliding:
                    movableRoot.localPosition = closedLocalPos + slideLocalDirection.normalized * model.OpenDistance;
                    break;
                case DoorType.Swing:
                    float angle = (invertSwing ? -1f : 1f) * model.OpenAngle;
                    Vector3 axis = swingLocalAxis == Vector3.zero ? Vector3.up : swingLocalAxis.normalized;
                    Quaternion toRot = closedLocalRot * Quaternion.AngleAxis(angle, axis);
                    if (hingeLocalPivot == Vector3.zero)
                    {
                        movableRoot.localRotation = toRot;
                    }
                    else
                    {
                        Vector3 pivot = hingeLocalPivot;
                        Vector3 pivotClosed = closedLocalRot * pivot;
                        Vector3 pivotOpen = toRot * pivot;
                        Vector3 delta = pivotClosed - pivotOpen;
                        movableRoot.localRotation = toRot;
                        movableRoot.localPosition = closedLocalPos + delta;
                    }
                    break;
            }
        }
    }
}
