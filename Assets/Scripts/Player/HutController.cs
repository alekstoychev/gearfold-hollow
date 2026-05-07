using Interact;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class HutController : MonoBehaviour
    {
        public InteractableNPC fortuneTeller;
        
        private PlayerInput playerInput;
        private InputAction interactAction;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            playerInput = gameObject.GetComponent<PlayerInput>();
            if (playerInput)
            {
                interactAction = playerInput.actions.FindAction("Player/Interact");
                
                interactAction.performed += OnInteractPerformed;
            }
            else
            {            
                Debug.LogError($"{name} did not find player input component.");
            }  
        }

        private void OnInteractPerformed(InputAction.CallbackContext obj)
        {
            fortuneTeller.Interact();
        }
    }
}
