using Minge2025Summer.Scripts.Utility;

namespace Minge2025Summer.Scripts.GameSetting
{
    public class GameController : SingletonWithMonoBehaviour<GameController>
    {
        public GameSettingsController gameSettingsController;
        
        protected override void OnAwakeProcess()
        {
            base.OnAwakeProcess();
            gameSettingsController = new GameSettingsController();
        }
        
        protected override void OnDestroyProcess()
        {
            base.OnDestroyProcess();
            gameSettingsController.SaveSettings();
        }
    }
}