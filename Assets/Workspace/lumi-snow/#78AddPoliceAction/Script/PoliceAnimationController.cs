using UnityEngine;

public class PoliceAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator; //Animator

    public void PlayIdle()   => animator.Play("Idle");
    public void PlayChase()  => animator.Play("Chase");
    public void PlaySearch() => animator.Play("Idle");//Searchのアニメーション後々追加(？)
    public void PlayAttack() => animator.Play("Attack");
}