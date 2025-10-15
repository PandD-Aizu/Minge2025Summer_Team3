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
        
        // CAMERA
        public bool cameraInvert;           // カメラ操作反転
        public bool cameraAimingInvert;     // エイム時カメラ操作反転
        public int cameraSensitivity;       // カメラ感度
        public int cameraAimingSensitivity; // エイム時カメラ感度
        public int mouseSensitivity;        // マウス操作速度
        
        // GAME SETTINGS
        public bool aimAssist;        // エイムアシスト
        public bool tutorialEnabled;  // チュートリアル表示
        public bool displayHUD;       // HUD表示
        public bool displayCrosshair; // クロスヘア表示
        public Color crosshairColor;  // クロスヘア色
        
        // GRAPHICS
        public float brightness;           // 明るさ設定
        public bool allowHDR;              // HDR許可
        public int screenMode;             // 画面モード (0: ウィンドウ, 1: フルスクリーン, 2: ボーダーレス)
        public Vector2 screenResolution;   // 画面解像度
        public float screenFlashRate;      // 画面更新率
        public int frameRateLimit;         // フレームレート制限 (-1: 制限なし)
        public bool allowVSync;            // 垂直同期許可
        public bool fidelityFX;            // FidelityFX有効化
        public bool renderingMethod;       // レンダリング方式（false: 通常, true: 軽量）
        public float renderScale;          // レンダースケール
        public bool fidelityFXCasting;     // FidelityFXキャスティング有効化
        public int antiAliasing;           // アンチエイリアス (0: なし, 1: FXAA, 2: SMAA, 3: TAA)
        public int textureQuality;         // テクスチャ品質 (0: 高, 1: 中, 2: 低)
        public int shadowQuality;          // 影品質 (0: 高, 1: 中, 2: 低, 3: オフ)
        public int meshQuality;            // メッシュ品質 (0: 高, 1: 中, 2: 低)
        public bool ambientOcclusion;      // アンビエントオクルージョン
        public bool screenSpaceReflection; // スクリーンスペースリフレクション
        public bool subsurfaceScattering;  // サブサーフェイススキャタリング
        public bool bloom;                 // ブルーム
        public bool lensFlare;             // レンズフレア
        public bool filmGrain;             // フィルム粒子ノイズ
        public bool depthOfField;          // 被写界深度
        public bool lensDistortion;        // レンズ歪み
        public bool chromaticAberration;   // 色収差
        
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
            
            cameraInvert = false;
            cameraAimingInvert = false;
            cameraSensitivity = 5;
            cameraAimingSensitivity = 5;
            mouseSensitivity = 5;
            
            aimAssist = false;
            tutorialEnabled = true;
            displayHUD = true;
            displayCrosshair = true;
            crosshairColor = Color.white;
            
            brightness = 0.0f;
            allowHDR = false;
            screenMode = 0;
            screenResolution = new Vector2(1920, 1080);
            screenFlashRate = 60.0f;
            frameRateLimit = -1;
            allowVSync = false;
            fidelityFX = false;
            renderingMethod = false;
            renderScale = 1.0f;
            fidelityFXCasting = false;
            antiAliasing = 0;
            textureQuality = 0;
            shadowQuality = 0;
            meshQuality = 0;
            ambientOcclusion = true;
            screenSpaceReflection = false;
            subsurfaceScattering = false;
            bloom = true;
            lensFlare = true;
            filmGrain = true;
            depthOfField = true;
            lensDistortion = true;
            chromaticAberration = true;
            
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
    }   
}