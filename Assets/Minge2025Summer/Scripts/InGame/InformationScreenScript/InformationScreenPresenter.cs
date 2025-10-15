using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.InformationScreenScript
{
    public class InformationScreenPresenter : MonoBehaviour
    {
        [SerializeField] private InformationScreenModel model;
        [SerializeField] private InformationScreenView view;

        private void Start()
        {
            view.Initialize();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.V))
                view.SwitchInformationScreen();

            if (Input.GetKeyDown(KeyCode.A))
                view.SwitchInformationPanel(-1);
            if (Input.GetKeyDown(KeyCode.D))
                view.SwitchInformationPanel(1);
        }
    }
}