using UnityEngine;

namespace Minge2025Summer.InGame.Interactable
{
    public interface IInteractable
    {
        /// <summary>
        /// インタラクト可能なオブジェクトのインタラクションプロンプト
        /// </summary>
        string InteractionPrompt { get; }

        /// <summary>
        /// インタラクト可能なオブジェクトと相互作用するメソッド
        /// </summary>
        /// <param name="instigator">インタラクトを行ったオブジェクト</param>
        void Interact(GameObject instigator = null);
    }
}
