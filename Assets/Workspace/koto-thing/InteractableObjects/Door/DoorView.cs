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

        // 直近のオープン設定を保持してクローズ側で利用
        private DoorModel lastModel;

        public void Initialize()
        {
            if (initialized) return;
            if (movableRoot == null) movableRoot = transform;
            closedLocalPos = movableRoot.localPosition;
            closedLocalRot = movableRoot.localRotation;
            initialized = true;
        }

        public void PlayOpen(DoorModel model)
        {
            if (!initialized) Initialize();
            if (currentTween != null && currentTween.IsActive()) currentTween.Kill();
            if (model == null) return;

            lastModel = model; // 開いた設定を保存

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

            if (lastModel == null)
            {
                // フォールバック: 単純に元へ
                currentTween = movableRoot.DOLocalMove(closedLocalPos, duration)
                    .SetEase(Ease.InOutSine);
                movableRoot.DOLocalRotateQuaternion(closedLocalRot, duration);
                return;
            }

            switch (lastModel.DoorType)
            {
                case DoorType.Sliding:
                    PlayCloseSliding(duration);
                    break;
                case DoorType.Swing:
                    PlayCloseSwing(duration, lastModel);
                    break;
            }
        }

        private void PlayCloseSliding(float duration)
        {
            currentTween = movableRoot.DOLocalMove(closedLocalPos, duration).SetEase(Ease.InOutSine);
            // 回転は開閉で変えていない前提
        }

        private void PlayCloseSwing(float duration, DoorModel model)
        {
            Vector3 axis = swingLocalAxis == Vector3.zero ? Vector3.up : swingLocalAxis.normalized;
            // 開き時に使った角度方向へ既に回転している前提で閉じは元に戻す
            if (hingeLocalPivot == Vector3.zero)
            {
                Quaternion fromRot = movableRoot.localRotation;
                Quaternion toRot = closedLocalRot;
                currentTween = DOTween.To(
                    () => 0f,
                    v => movableRoot.localRotation = Quaternion.Slerp(fromRot, toRot, v),
                    1f,
                    duration
                ).SetEase(Ease.InOutSine);
            }
            else
            {
                Vector3 pivot = hingeLocalPivot;
                Quaternion fromRot = movableRoot.localRotation;
                Quaternion toRot = closedLocalRot;
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
            lastModel = model;

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
