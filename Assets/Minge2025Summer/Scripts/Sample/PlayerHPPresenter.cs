using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.Sample
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