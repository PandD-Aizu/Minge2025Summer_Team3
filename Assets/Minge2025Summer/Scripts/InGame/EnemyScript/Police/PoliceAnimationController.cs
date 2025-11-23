using UnityEngine;

public class PoliceAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [Header("State Names")] 
    [SerializeField] private string idleState = "Idle";
    [SerializeField] private string chaseState = "Chase";
    [SerializeField] private string searchState = "Chase";
    [SerializeField] private string attackState = "Attack";

    [Header("Attack Trigger Parameter")] 
    [SerializeField] private string attackTriggerName = "AttackTrigger";

    [Header("Cross Fade Settings")] 
    [SerializeField] private float fadeDuration = 0.1f;
    [SerializeField] private int layer = 0;

    private int _attackTriggerHash;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        _attackTriggerHash = Animator.StringToHash(attackTriggerName);
    }

    private bool IsPlayingState(string stateName)
    {
        if (animator == null) return false;
        var stateInfo = animator.GetCurrentAnimatorStateInfo(layer);
        return stateInfo.IsName(stateName);
    }

    private void CrossFadeIfNeeded(string stateName)
    {
        if (animator == null || string.IsNullOrEmpty(stateName)) 
            return;
        
        if (IsPlayingState(stateName)) 
            return;
        
        animator.CrossFadeInFixedTime(stateName, fadeDuration, layer);
    }

    public void PlayIdle()   => CrossFadeIfNeeded(idleState);
    public void PlayChase()  => CrossFadeIfNeeded(chaseState);
    public void PlaySearch() => CrossFadeIfNeeded(chaseState); // TODO: searchState未使用

    public void PlayAttack()
    {
        if (animator == null) 
            return;
        
        animator.ResetTrigger(_attackTriggerHash);
        animator.SetTrigger(_attackTriggerHash);
    }
}