using UniRx;
using UnityEngine;

namespace Minge2025Summer.InGame.Interactable
{
    public class InteractionPresenter : MonoBehaviour
    {
        [SerializeField] private PlayerInteractor playerInteractor;
        [SerializeField] private InteractionView view;

        private void Start()
        {
            if (playerInteractor != null && view != null)
            {
                playerInteractor.CurrentInteractable
                    .Subscribe(interactable =>
                    {
                        if (interactable != null)
                        {
                            view.Show(interactable.InteractionPrompt);
                        }
                        else
                        {
                            view.Hide();
                        }
                    })
                    .AddTo(this);
            }
            else
            {
                Debug.LogWarning("InteractionPresenter: Missing dependencies.");
            }
        }
    }
}
