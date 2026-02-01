using UnityEngine;
using FMODUnity;
using Minge2025Summer.Scripts.InGame.AcousticsScript;
using UniRx;

public class FireAlarm : MonoBehaviour
{
    [SerializeField] private StudioEventEmitter fireAlarmEmitter;
    [SerializeField] private float soundRudius = 5.0f;
    [SerializeField] private float alarmAreaRadius = 5.0f;
    private SphereCollider sr;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = GetComponent<SphereCollider>();
        sr.radius = alarmAreaRadius;
    }

    //プレイヤーがAlarmAreaに侵入したら、音を鳴らす
    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Player")) return;

        if (!fireAlarmEmitter.IsPlaying())
        {
            fireAlarmEmitter.Play();
        }
        
        SoundEvent soundEvent = new SoundEvent(transform.position, soundRudius, SoundType.FireAlarm, gameObject);
        MessageBroker.Default.Publish(soundEvent);
    }

    //アラームを止める(PlayerGimmickControllerから呼び出す)
    public void StopFireAlarm()
    {
        fireAlarmEmitter.Stop();
    }
}
