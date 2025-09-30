using UniRx;
using UnityEngine;
using Workspace.koto_thing;
using Workspace.momiji1107;

public class Police_Presenter : MonoBehaviour
{
    [SerializeField] private FlashLightFlickerModel flickerModel;
    [SerializeField] private FlashLightFlickerView flickerView;
    
    private Police_Model modelScript;
    private Police_View viewScript;
    private PoliceEmitter emitterScript;
    
    private CompositeDisposable disposables = new CompositeDisposable();
    
    void Start()
    {
        modelScript = this.GetComponent<Police_Model>();
        viewScript = this.GetComponent<Police_View>();
        emitterScript = this.GetComponent<PoliceEmitter>();
        
        SubscribeEvents();
    }
    
    void Update()
    {
        emitterScript.PlayFootStep(modelScript.GetCharacterController.velocity.magnitude);
        modelScript.Move();
        modelScript.AttackPlayer();
        emitterScript.UpdateMoanTimer();
    }

    private void SubscribeEvents()
    {
        modelScript.Battleflag
            .Subscribe(isBattle =>
            {
                if (isBattle)
                    flickerModel.SetFlickerState(FlickerState.NORMALFLICKER, flickerView.GetFlashLight);
                else
                    flickerModel.SetFlickerState(FlickerState.STABLE, flickerView.GetFlashLight);
            })
            .AddTo(disposables);
    }
}
