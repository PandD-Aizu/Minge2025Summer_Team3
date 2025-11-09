using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using StarterAssets;

namespace Workspace.mtdnot
{
    /// <summary>
    /// インベントリ開閉時にカメラの視点移動を制御するマネージャー
    /// CinemachinePanTiltとCinemachineInputAxisControllerを制御
    /// </summary>
    public class ImprovedCameraLockManager : MonoBehaviour
    {
        [Header("Camera Components to Control")]
        [Tooltip("CinemachinePanTiltコンポーネント（青でハイライトされているオブジェクト）")]
        [SerializeField] private List<CinemachinePanTilt> panTiltComponents = new List<CinemachinePanTilt>();
        
        [Tooltip("CinemachineInputAxisControllerコンポーネント")]
        [SerializeField] private List<CinemachineInputAxisController> inputAxisControllers = new List<CinemachineInputAxisController>();
        
        [Header("Optional: Additional Controls")]
        [SerializeField] private ThirdPersonController thirdPersonController;
        [SerializeField] private StarterAssetsInputs starterAssetsInputs;
        [SerializeField] private List<GameObject> playerMovementControllers = new List<GameObject>();
        
        [Header("Settings")]
        [SerializeField] private bool debugMode = true;
        [SerializeField] private bool autoFindComponents = true;
        
        private bool isLocked = false;
        
        private void Awake()
        {
            if (autoFindComponents)
            {
                AutoFindComponents();
            }
        }
        
        private void Start()
        {
            ValidateComponents();
        }
        
        /// <summary>
        /// インベントリが開いた時に呼ぶ（OnEnableから呼ばれる想定）
        /// </summary>
        private void OnEnable()
        {
            LockCamera();
        }
        
        /// <summary>
        /// インベントリが閉じた時に呼ぶ（OnDisableから呼ばれる想定）
        /// </summary>
        private void OnDisable()
        {
            UnlockCamera();
        }
        
        /// <summary>
        /// カメラをロック（視点移動を無効化）
        /// </summary>
        public void LockCamera()
        {
            if (isLocked) return;
            
            // CinemachinePanTiltを無効化
            foreach (var panTilt in panTiltComponents)
            {
                if (panTilt != null)
                {
                    panTilt.enabled = false;
                    if (debugMode) Debug.Log($"[CameraLockManager] Disabled PanTilt: {panTilt.name}");
                }
            }
            
            // CinemachineInputAxisControllerを無効化
            foreach (var inputAxis in inputAxisControllers)
            {
                if (inputAxis != null)
                {
                    inputAxis.enabled = false;
                    if (debugMode) Debug.Log($"[CameraLockManager] Disabled InputAxisController: {inputAxis.name}");
                }
            }
            
            // プレイヤー移動を無効化
            foreach (var controller in playerMovementControllers)
            {
                if (controller != null)
                {
                    controller.SetActive(false);
                    if (debugMode) Debug.Log($"[CameraLockManager] Disabled PlayerMovement: {controller.name}");
                }
            }
            
            // ThirdPersonControllerのカメラロック
            if (thirdPersonController != null)
            {
                thirdPersonController.LockCameraPosition = true;
                if (debugMode) Debug.Log("[CameraLockManager] Locked ThirdPersonController camera");
            }
            
            // StarterAssetsInputsのマウス入力無効化
            if (starterAssetsInputs != null)
            {
                starterAssetsInputs.cursorInputForLook = false;
                if (debugMode) Debug.Log("[CameraLockManager] Disabled cursor input for look");
            }
            
            isLocked = true;
            if (debugMode) Debug.Log("[CameraLockManager] === CAMERA LOCKED ===");
        }
        
        /// <summary>
        /// カメラのロックを解除（視点移動を有効化）
        /// </summary>
        public void UnlockCamera()
        {
            if (!isLocked) return;
            
            // CinemachinePanTiltを有効化
            foreach (var panTilt in panTiltComponents)
            {
                if (panTilt != null)
                {
                    panTilt.enabled = true;
                    if (debugMode) Debug.Log($"[CameraLockManager] Enabled PanTilt: {panTilt.name}");
                }
            }
            
            // CinemachineInputAxisControllerを有効化
            foreach (var inputAxis in inputAxisControllers)
            {
                if (inputAxis != null)
                {
                    inputAxis.enabled = true;
                    if (debugMode) Debug.Log($"[CameraLockManager] Enabled InputAxisController: {inputAxis.name}");
                }
            }
            
            // プレイヤー移動を有効化
            foreach (var controller in playerMovementControllers)
            {
                if (controller != null)
                {
                    controller.SetActive(true);
                    if (debugMode) Debug.Log($"[CameraLockManager] Enabled PlayerMovement: {controller.name}");
                }
            }
            
            // ThirdPersonControllerのカメラロック解除
            if (thirdPersonController != null)
            {
                thirdPersonController.LockCameraPosition = false;
                if (debugMode) Debug.Log("[CameraLockManager] Unlocked ThirdPersonController camera");
            }
            
            // StarterAssetsInputsのマウス入力有効化
            if (starterAssetsInputs != null)
            {
                starterAssetsInputs.cursorInputForLook = true;
                if (debugMode) Debug.Log("[CameraLockManager] Enabled cursor input for look");
            }
            
            isLocked = false;
            if (debugMode) Debug.Log("[CameraLockManager] === CAMERA UNLOCKED ===");
        }
        
        /// <summary>
        /// コンポーネントを自動検索
        /// </summary>
        private void AutoFindComponents()
        {
            // CinemachinePanTiltを検索
            if (panTiltComponents.Count == 0)
            {
                var panTilts = FindObjectsOfType<CinemachinePanTilt>();
                panTiltComponents.AddRange(panTilts);
                if (debugMode && panTilts.Length > 0)
                {
                    Debug.Log($"[CameraLockManager] Auto-found {panTilts.Length} PanTilt components");
                }
            }
            
            // CinemachineInputAxisControllerを検索
            if (inputAxisControllers.Count == 0)
            {
                var inputAxes = FindObjectsOfType<CinemachineInputAxisController>();
                inputAxisControllers.AddRange(inputAxes);
                if (debugMode && inputAxes.Length > 0)
                {
                    Debug.Log($"[CameraLockManager] Auto-found {inputAxes.Length} InputAxisController components");
                }
            }
            
            // ThirdPersonControllerを検索
            if (thirdPersonController == null)
            {
                thirdPersonController = FindObjectOfType<ThirdPersonController>();
                if (debugMode && thirdPersonController != null)
                {
                    Debug.Log("[CameraLockManager] Auto-found ThirdPersonController");
                }
            }
            
            // StarterAssetsInputsを検索
            if (starterAssetsInputs == null)
            {
                starterAssetsInputs = FindObjectOfType<StarterAssetsInputs>();
                if (debugMode && starterAssetsInputs != null)
                {
                    Debug.Log("[CameraLockManager] Auto-found StarterAssetsInputs");
                }
            }
        }
        
        /// <summary>
        /// コンポーネントの検証
        /// </summary>
        private void ValidateComponents()
        {
            if (panTiltComponents.Count == 0)
            {
                Debug.LogWarning("[CameraLockManager] No CinemachinePanTilt components assigned!");
                Debug.LogWarning("[CameraLockManager] Please assign the blue highlighted object's PanTilt component!");
            }
            
            if (inputAxisControllers.Count == 0)
            {
                Debug.LogWarning("[CameraLockManager] No CinemachineInputAxisController components assigned!");
            }
            
            if (debugMode)
            {
                Debug.Log($"[CameraLockManager] Initialized with:");
                Debug.Log($"  - {panTiltComponents.Count} PanTilt components");
                Debug.Log($"  - {inputAxisControllers.Count} InputAxisController components");
                Debug.Log($"  - {playerMovementControllers.Count} PlayerMovement controllers");
                Debug.Log($"  - ThirdPersonController: {(thirdPersonController != null ? "Found" : "Not found")}");
                Debug.Log($"  - StarterAssetsInputs: {(starterAssetsInputs != null ? "Found" : "Not found")}");
            }
        }
        
        /// <summary>
        /// 手動でロック状態を切り替え
        /// </summary>
        public void ToggleLock()
        {
            if (isLocked)
                UnlockCamera();
            else
                LockCamera();
        }
    }
}