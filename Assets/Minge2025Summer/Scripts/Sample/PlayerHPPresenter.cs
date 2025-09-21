using UnityEngine;
using UniRx;

namespace UniRxTest
{
    public class PlayerHPPresenter : MonoBehaviour
    {
        [SerializeField] private PlayerHPModel model;

        private void Start()
        {
            SubscribeEvent();
        }

        private void SubscribeEvent()
        {
            model.OnHPChanged
                .Subscribe(hp =>
                {
                    // ここでviewの更新とかを行ってもよい
                    // ifやforなども使えるが、あまり複雑にしすぎないように注意
                    Debug.Log($"Player HP changed: {hp}");
                });
        }
    }
}