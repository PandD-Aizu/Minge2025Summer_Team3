using System;
using System.Collections.Generic;
using UnityEngine;
using Workspace.momiji1107;

namespace Workspace.koto_thing
{
    [Serializable]
    public class GunDamageModel
    {
        private readonly HashSet<IEnemyHP> damagedEnemies = new();

        /// <summary>
        /// 距離ソート後、各敵について「最初に当たった EnemyColliderInfo を持つコライダー」を採用しダメージ倍率適用。
        /// EnemyColliderInfo が無い最初のヒットは基礎ダメージのみ。脚部だった場合 PolicePartHitResponder によるスロー適用。
        /// IEnemyHP は別コライダー(親など)にあり得るため GetComponentInParent で取得する。
        /// </summary>
        /// <param name="hits">RaycastNonAlloc のヒット配列</param>
        /// <param name="hitCount">実ヒット数</param>
        /// <param name="attackPower">基礎攻撃力</param>
        /// <param name="range">未使用(将来拡張用)</param>
        /// <param name="pointBlankDistance">至近距離閾値</param>
        /// <param name="pointBlankMultiplier">至近距離倍率</param>
        /// <param name="penetrationCount">貫通可能な敵数</param>
        public void ProcessHits(
            RaycastHit[] hits,
            int hitCount,
            float attackPower,
            float range,
            float pointBlankDistance,
            float pointBlankMultiplier,
            int penetrationCount)
        {
            if (hits == null || hitCount <= 0) 
                return;
            if (penetrationCount <= 0) penetrationCount = 1;

            SortHitsByDistance(hits, hitCount);

            damagedEnemies.Clear();
            int penetratedEnemies = 0;

            for (int i = 0; i < hitCount; i++)
            {
                // 貫通可能な敵数に達したら終了
                if (penetratedEnemies >= penetrationCount)
                    break;

                // ヒットしたコライダーを取得
                var col = hits[i].collider;
                if (col == null) 
                    continue;

                // まずキャラクター全体を特定し、HP管理部品を探す
                var root = col.transform.root;
                var enemyHp = root.GetComponentInChildren<IEnemyHP>();

                // 敵でなければスキップ
                if (enemyHp == null) 
                    continue;

                // 既にダメージを与えた敵であればスキップ
                if (damagedEnemies.Contains(enemyHp)) 
                    continue;

                // 次に、実際に当たった「部位」の情報を取得する
                float damageMultiplier = 1.0f;
                EnemyBodyParts? hitPart = null;
                if (col.TryGetComponent<EnemyColliderInfo>(out var colliderInfo))
                {
                    // 部位情報があれば倍率と部位タイプを更新
                    damageMultiplier = colliderInfo.DamageMultiplier;
                    hitPart = colliderInfo.BodyParts;
                }

                // 最終ダメージ計算
                float finalDamage = attackPower * damageMultiplier;
                if (hits[i].distance <= pointBlankDistance)
                    finalDamage *= pointBlankMultiplier;

                // ダメージ適用と追加効果
                enemyHp.ReceiveDamage(finalDamage);

                // 脚部に当たった場合、スロー効果を適用
                if (hitPart.HasValue && (hitPart.Value == EnemyBodyParts.LEFT_LEG || hitPart.Value == EnemyBodyParts.RIGHT_LEG))
                {
                    var partReceiver = root.GetComponentInChildren<IEnemyPartHitReceiver>();
                    partReceiver?.OnPartHit(hitPart.Value, finalDamage);
                }

                // この敵にはこれ以上ダメージを与えないようにする
                damagedEnemies.Add(enemyHp);
                penetratedEnemies++;
            }
        }
        
        /* 以下ヘルパー関数 */
        /// <summary>
        /// 距離によりソートする
        /// </summary>
        /// <param name="hits">レイキャストに引っかかったオブジェクト</param>
        /// <param name="hitCount">レイキャストに引っかかったオブジェクトの数</param>
        private void SortHitsByDistance(RaycastHit[] hits, int hitCount)
        {
            for (int i = 0; i < hitCount - 1; i++)
            {
                // 最小値探索
                int minIndex = i;
                float minDist = hits[minIndex].distance;
                for (int j = i + 1; j < hitCount; j++)
                {
                    float d = hits[j].distance;
                    if (d < minDist)
                    {
                        minDist = d;
                        minIndex = j;
                    }
                }
                
                // スワップ
                if (minIndex != i)
                {
                    (hits[i], hits[minIndex]) = (hits[minIndex], hits[i]);
                }
            }
        }
    }
}
