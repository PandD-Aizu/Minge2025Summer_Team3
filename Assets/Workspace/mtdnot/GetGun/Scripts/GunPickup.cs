using UnityEngine;

public class GunPickup : MonoBehaviour
{
    /// <summary>
    /// プレイヤーがトリガー範囲内でEキーを押すと拳銃を拾い、オブジェクトを削除する。
    /// </summary>
    /// <param name="other">接触しているCollider</param>
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("拳銃を拾った");
            Destroy(gameObject);
        }
    }
}
