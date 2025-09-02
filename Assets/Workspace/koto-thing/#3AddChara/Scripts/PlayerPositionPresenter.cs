using UnityEngine;

namespace Workspace.koto_thing
{
    public class PlayerPositionPresenter : MonoBehaviour
    {
        [SerializeField] private PlayerPositionModel model;
        [SerializeField] private PlayerPositionView view;
        [SerializeField] private PlayerPositionEmitter emitter;

        private void Start()
        {
            
        }

        private void Update()
        {
            Vector2 input = Vector2.zero;

            if (Input.GetKey(KeyCode.W)) 
                input.y += 1.0f;
            if (Input.GetKey(KeyCode.S)) 
                input.y -= 1.0f;
            if (Input.GetKey(KeyCode.D)) 
                input.x += 1.0f;
            if (Input.GetKey(KeyCode.A)) 
                input.x -= 1.0f;
            
            model.IsRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            
            model.Move(input);
            
            float speed = model.GetCharacterController.velocity.magnitude;
            emitter.PlayFootStep(speed);
        }
    }
}