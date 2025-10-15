using System;
using UniRx;

namespace Minge2025Summer.Scripts.InGame.ItemScript.Interface
{
    /// <summary>
    /// アイテムが適用完了(使用完了)を通知するためのインターフェース。
    /// IItem取得時に購読し、OnApplied発火でインベントリから消費。
    /// </summary>
    public interface IAppliable
    {
        IObservable<Unit> OnApplied { get; }
    }
}

