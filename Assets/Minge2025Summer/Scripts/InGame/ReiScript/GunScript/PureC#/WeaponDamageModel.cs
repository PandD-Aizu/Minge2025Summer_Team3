using System;
using System.Collections.Generic;
using Minge2025Summer.Scripts.InGame.EnemyScript.ColliderInfo;
using Minge2025Summer.Scripts.InGame.EnemyScript.Enum;
using Minge2025Summer.Scripts.InGame.EnemyScript.Interface;
using Minge2025Summer.Scripts.InGame.FX;
using Minge2025Summer.Scripts.InGame.ShootableObject.Interface;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ReiScript.GunScript.PureC_
{
    [Serializable]
    public class WeaponDamageModel
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
                var hit = hits[i];
                var col = hit.collider;
                if (col == null) continue;

                // 敵に当たっているかどうかを判定
               if (col.TryGetComponent<IShootableObject>(out var shootableObject)) 
                   shootableObject.Feedback();
                
                var enemyHP = col.GetComponentInParent<IEnemyHP>();
                if (enemyHP == null) continue;
                
                // 既にダメージを与えた敵はスキップ
                if (damagedEnemies.Contains(enemyHP)) continue;

                float damageMultiplier = 1.0f;
                EnemyBodyParts hitPart = EnemyBodyParts.BODY;

                if (col.TryGetComponent<EnemyColliderInfo>(out var colliderInfo))
                {
                    damageMultiplier = colliderInfo.DamageMultiplier;
                    hitPart = colliderInfo.BodyParts;
                    Debug.Log($"部位: {hitPart}, 倍率: {damageMultiplier}");
                }
                else
                {
                    Debug.Log("EnemyColliderInfo が無いコライダーにヒット");
                }
                
                // ダメージ計算
                float finalDamage = attackPower * damageMultiplier;
                if (hit.distance <= pointBlankDistance) finalDamage *= pointBlankMultiplier;
                enemyHP.ReceiveDamage(finalDamage);
                
                // 部位ごとの追加効果
                var partReceiver = (enemyHP as Component)?.gameObject.GetComponentInChildren<IEnemyPartHitReceiver>();
                partReceiver?.OnPartHit(hitPart, damageMultiplier);
                
                damagedEnemies.Add(enemyHP);
                penetratedEnemies++;
                
                BloodSplatterSpawner.Spawn(
                    "BloodSplatter",
                    hit.point,
                    hit.normal,
                    speed: 5.0f,
                    lifetime: 2.0f,
                    surfaceOffset: 0.01f
                    );
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