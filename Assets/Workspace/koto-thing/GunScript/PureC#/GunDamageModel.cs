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
            if (hitCount <= 0) return;

            // 距離昇順ソート
            for (int i = 0; i < hitCount - 1; i++)
            {
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
                if (minIndex != i)
                {
                    (hits[i], hits[minIndex]) = (hits[minIndex], hits[i]);
                }
            }

            damagedEnemies.Clear();
            int penetrated = 0;

            for (int i = 0; i < hitCount; i++)
            {
                if (penetrated >= penetrationCount) break;
                
                Debug.Log("Hit object: " + hits[i].collider.name);

                var col = hits[i].collider;
                if (col == null) continue;
                if (!col.CompareTag("EnemyShootable")) continue; // 敵判定用タグ

                // IEnemyStatus取得
                var enemy = col.GetComponent<IEnemyHP>()
                            ?? col.GetComponentInParent<IEnemyHP>()
                            ?? col.GetComponentInChildren<IEnemyHP>();
                if (enemy == null) continue;
                if (damagedEnemies.Contains(enemy)) continue; // その敵は最初のヒットのみ採用

                float finalDamage = attackPower;
                if (hits[i].distance <= pointBlankDistance)
                    finalDamage *= pointBlankMultiplier;

                // 部位倍率
                Debug.Log("Hit enemy part: " + col.name);
                var partInfo = col.GetComponent<EnemyColliderInfo>(); // 直付けのみを見る（誤判定防止）
                EnemyBodyParts? part = null;
                if (partInfo != null)
                {
                    finalDamage *= partInfo.DamageMultiplier;
                    part = partInfo.BodyParts;
                }

                // 脚ヒット時のスロー (PolicePartHitResponder が居れば通知)
                if (part.HasValue && (part.Value == EnemyBodyParts.LEFT_LEG || part.Value == EnemyBodyParts.RIGHT_LEG))
                {
                    var responder = col.GetComponent<PolicePartHitResponder>()
                                  ?? col.GetComponentInParent<PolicePartHitResponder>()
                                  ?? col.GetComponentInChildren<PolicePartHitResponder>();
                    responder?.OnPartHit(part.Value, finalDamage);
                }

                enemy.ReceiveDamage(finalDamage);

                damagedEnemies.Add(enemy);
                penetrated++;
            }
        }
    }
}
