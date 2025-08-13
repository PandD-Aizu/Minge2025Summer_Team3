using UnityEngine;

namespace Workspace.koto_thing
{
    public class PlayerGunModel : MonoBehaviour
    {
        // TODO: 拳銃のパラメーター決めておく
        
        /// <summary>
        /// 拳銃を発射する。
        /// </summary>
        public void ShootGun()
        {
            // Rayを飛ばして、ヒットしたオブジェクトの情報を取得する
            Physics.Raycast(Camera.main.transform.position, 
                Camera.main.transform.forward, 
                out RaycastHit hit,
                100.0f);

            // ヒットしたオブジェクトが"GunHitable"タグを持っているか確認
            if (hit.collider.CompareTag("EnemyShootable"))
            {
                Debug.Log("Hit: " + hit.collider.name);
            }
        }
    }
}