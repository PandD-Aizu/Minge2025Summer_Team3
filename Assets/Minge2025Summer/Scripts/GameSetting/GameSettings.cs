using UnityEngine;

namespace Minge2025Summer.Scripts.GameSetting
{
    [System.Serializable]
    public class GameSettings
    {
        /// <summary>
        /// 設定
        /// </summary>
        // DIFFICULTY
        public int difficulty; // 0: イージー, 1: ノーマル, 2: ハード
        
        /// <summary>
        /// オプション設定
        /// </summary>
        // CONTROLS
        public bool mouseCursorLock;    // マウスカーソルロック
        public bool mouseInvertY;       // マウスY軸反転
        public bool mouseWheelInvert;   // マウスホイール反転
        public KeyCode keyMoveForward;  // 移動キー
        public KeyCode keyMoveBackward; // 移動キー
        public KeyCode keyMoveLeft;     // 移動キー
        public KeyCode keyMoveRight;    // 移動キー
        public KeyCode keyReload;       // リロードキー
        public KeyCode keyInteract;     // インタラクトキー
        public KeyCode keySprint ;      // 走るキー
        public KeyCode keyCrouch;       // しゃがむキー
        public KeyCode keyInventory;    // インベントリキー
        public KeyCode keyImportant;    // 重要アイテムキー
        
        // CAMERA
        public bool cameraInvert;             // カメラ操作反転
        public bool cameraAimingInvert;       // エイム時カメラ操作反転
        public float cameraSensitivity;       // カメラ感度
        public float cameraAimingSensitivity; // エイム時カメラ感度
        public float mouseSensitivity;        // マウス操作速度
        
        // GAME SETTINGS
        public bool aimAssist;        // エイムアシスト
        public bool tutorialEnabled;  // チュートリアル表示
        public bool displayHUD;       // HUD表示
        public bool displayCrosshair; // クロスヘア表示
        public Color crosshairColor;  // クロスヘア色
        
        // GRAPHICS
        public int presetIndex;
        public int textureIndex;
        public int antiAliasingIndex;
        public bool shadowsEnabled;
        public float shadowDistance;
        public bool ambientOcclusionEnabled;
        
        // AUDIO
        public float masterVolume;  // マスターボリューム
        public float bgmVolume;     // BGMボリューム
        public float sfxVolume;     // 効果音ボリューム
        public float voiceVolume;   // ボイスボリューム
        public float ambientVolume; // 環境音ボリューム
        public float systemVolume;  // システム音ボリューム
        
        // LANGUAGE
        public int language; // 0: 日本語, 1: 英語, 2: 中国語
        
        // ACCESSIBILITY
        public int textSize;          // 0: 小, 1: 中, 2: 大
        public Color textColor;
        public bool textBackground;
        public bool showDots;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GameSettings()
        {
            difficulty = 0;
            
            mouseCursorLock = false;
            mouseInvertY = false;
            mouseWheelInvert = false;
            keyMoveForward = KeyCode.W;
            keyMoveBackward = KeyCode.S;
            keyMoveLeft = KeyCode.A;
            keyMoveRight = KeyCode.D;
            keyReload = KeyCode.R;
            keyInteract = KeyCode.E;
            keySprint = KeyCode.LeftShift;
            keyCrouch = KeyCode.Space;
            keyInventory = KeyCode.Tab;
            keyImportant = KeyCode.V;
            
            cameraInvert = false;
            cameraAimingInvert = false;
            cameraSensitivity = 1.0f;
            cameraAimingSensitivity = 1.0f;
            mouseSensitivity = 1.0f;
            
            aimAssist = false;
            tutorialEnabled = true;
            displayHUD = true;
            displayCrosshair = true;
            crosshairColor = Color.white;
            
            presetIndex = 2;
            textureIndex = 0;
            antiAliasingIndex = 2;
            shadowsEnabled = true;
            shadowDistance = 50f;
            ambientOcclusionEnabled = true;
            
            masterVolume = 1.0f;
            bgmVolume = 1.0f;
            sfxVolume = 1.0f;
            voiceVolume = 1.0f;
            ambientVolume = 1.0f;
            systemVolume = 1.0f;
            
            language = 0;
            
            textSize = 1;
            textColor = Color.white;
            textBackground = true;
            showDots = true;
        }
        
        public void ResetKeyBindings()
        {
            keyMoveForward = KeyCode.W;
            keyMoveBackward = KeyCode.S;
            keyMoveLeft = KeyCode.A;
            keyMoveRight = KeyCode.D;
            keyReload = KeyCode.R;
            keyInteract = KeyCode.E;
            keySprint = KeyCode.LeftShift;
            keyCrouch = KeyCode.Space;
            keyInventory = KeyCode.Tab;
            keyImportant = KeyCode.V;
        }
        
        public void ResetGraphicsSettings()
        {
            presetIndex = 2;
            textureIndex = 0;
            antiAliasingIndex = 2;
            shadowsEnabled = true;
            shadowDistance = 50f;
            ambientOcclusionEnabled = true;
        }
    }   
}