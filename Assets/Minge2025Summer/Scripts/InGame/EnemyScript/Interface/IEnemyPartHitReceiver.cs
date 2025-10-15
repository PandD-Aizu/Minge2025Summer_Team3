using Minge2025Summer.Scripts.InGame.EnemyScript.Enum;

namespace Minge2025Summer.Scripts.InGame.EnemyScript.Interface
{
    /// <summary>
    /// 部位ヒット時の追加効果を受け取るインターフェース
    /// </summary>
    public interface IEnemyPartHitReceiver
    {
        /// <summary>
        /// 特定の部位に最終ダメージが適用された際に呼び出される
        /// </summary>
        /// <param name="part">被弾部位</param>
        /// <param name="finalDamage">最終ダメージ値(倍率適用後)</param>
        void OnPartHit(EnemyBodyParts part, float finalDamage);
    }
}
