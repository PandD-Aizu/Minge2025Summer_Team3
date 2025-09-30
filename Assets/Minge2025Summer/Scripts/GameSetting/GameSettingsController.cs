using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameSetting
{
    public class GameSettingsController
    {
        public GameSettings gameSettings;

        private string saveFilePath;

        public GameSettingsController()
        {
            saveFilePath = Path.Combine(Application.persistentDataPath, "gamesettings.json");
            LoadSettings();
        }

        /// <summary>
        /// 各種設定をロードする
        /// </summary>
        public void LoadSettings()
        {
            if (File.Exists(saveFilePath))
            {
                string json = File.ReadAllText(saveFilePath);
                gameSettings = JsonUtility.FromJson<GameSettings>(json);
            }
            else
            {
                Debug.Log("Save file not found, creating default settings.");
                gameSettings = new GameSettings();
            }
        }

        /// <summary>
        /// 設定をセーブする
        /// </summary>
        public void SaveSettings()
        {
            string json = JsonUtility.ToJson(gameSettings, true);
            File.WriteAllText(saveFilePath, json);
            
            Debug.Log("Settings saved to " + saveFilePath);
        }
    }
}