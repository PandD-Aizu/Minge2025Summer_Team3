using UnityEngine;

namespace Workspace.koto_thing
{
    public interface IItem
    {
        public int GetAmount { get; } // 現在残量
        public string GetDisplayName { get; }
        public string GetDescription { get; }
        public bool SetIsGet { get; set; }
        public bool GetIsApplied { get; } // 非スタック or 全量使い切り
        public Sprite GetSprite { get; }
        
        public void ApplyItem(); // 1 回使用 (内部で1減算)
        public void AddAmount(int delta); // 取得時などに加算
        public bool ConsumeOne(); // 外部で直接1個消費したい場合（残量が0になったら true を返す）
    }
}
