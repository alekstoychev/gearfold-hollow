using System;
using DialogueSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        #region Variables
        [Header("Movement")]
        public float moveSpeed = 10.0f;
        public float horizontalDampening;

        [NonSerialized] public static bool isInDialogue;
        
        private PlayerInput playerInput;
        private Rigidbody2D rb;
        private Collider2D collision;
        private SpriteRenderer spriteRenderer;

        private InputAction moveAction;
        private InputAction interactAction;
        #endregion

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            rb = gameObject.GetComponent<Rigidbody2D>();
            if (!rb)
            {
                Debug.LogError($"{name} did not find rigidbody component.");
            }

            collision = gameObject.GetComponent<Collider2D>();
            if (!collision)
            {
                Debug.LogError($"{name} did not find collider2d component.");
            }
            
            playerInput = gameObject.GetComponent<PlayerInput>();
            if (playerInput)
            {
                moveAction = playerInput.actions.FindAction("Player/Move");
                interactAction = playerInput.actions.FindAction("Player/Interact");
                
                interactAction.performed += OnInteractPerformed;
            }
            else
            {            
                Debug.LogError($"{name} did not find player input component.");
            }        

            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if (!spriteRenderer)
            {
                Debug.LogError($"{name} did not find Sprite Renderer component.");
            }
        }

        // Update is called once per frame
        void Update()
        {
            Move();
        }

        void FixedUpdate()
        {
            CheckSpriteRotation();
        }

        private void OnInteractPerformed(InputAction.CallbackContext callbackContext)
        {
            if (isInDialogue)
            {
                DialogueManager.GetDialogueManager().DeactivateDialogue();
            }
            else
            {
                Collider2D[] results = new Collider2D[10];
                
                int collisions = collision.Overlap(ContactFilter2D.noFilter, results);
    
                for (int i = 0; i < collisions; i++)
                {
                    Interactable interactable = results[i].gameObject.GetComponent<Interactable>();
                    if (!interactable)
                    {
                        continue;
                    }
                    
                    interactable.Interact();
                    break;
                }
            }
        }

        private void CheckSpriteRotation()
        {
            if (rb.linearVelocityX > 0.1)
            {
                if (!spriteRenderer.flipX)
                {
                    spriteRenderer.flipX = true;
                } 
            }
            else if (rb.linearVelocityX < -0.1)
            {
                if (spriteRenderer.flipX)
                {
                    spriteRenderer.flipX = false;
                } 
            }
        }

        private void Move()
        {
            if (moveAction.IsPressed())
            {
                rb.linearVelocityX = moveAction.ReadValue<Vector2>().x * moveSpeed;
            }
            else
            {
                if (rb.linearVelocity.x != 0)
                {
                    rb.linearVelocityX *= horizontalDampening;
                }
            }
        }
    }
}
