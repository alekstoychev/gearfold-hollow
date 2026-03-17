using Interact;
using UnityEngine;
using UnityEngine.InputSystem;

using System.Collections;
using System.Collections.Generic;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        #region Variables
        [Header("Movement")]
        public float moveSpeed = 10.0f;
        public float horizontalDampening;

        [Header("Sprites")] 
        public Sprite walkingRightSprite;
        public Sprite walkingLeftSprite;
        
        private PlayerInput playerInput;
        private Rigidbody2D rb;
        private Collider2D collision;
        private SpriteRenderer spriteRenderer;

        private InputAction moveAction;
        private InputAction interactAction;
        
        // Heyy sorry just animating
        public Animator animator;
        public float horizontalMove = 0f;
        private Vector2 movement; 

        #endregion

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
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
        private void Update()
        {
            //changing the animation from idle to running when moving
            horizontalMove = Input.GetAxisRaw("Horizontal") * moveSpeed;
            animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

            // mirroring the animation when going left. I feel like this might not be optimal but it works
            movement = new Vector2(Input.GetAxis("Horizontal"), 0).normalized;
            bool mirrored = movement.x < 0;
            if (movement.x != 0)
            {
               this.transform.rotation = Quaternion.Euler(new Vector3(0f, mirrored ? 180f : 0f, 0f));
            }

            Move();
        }

        private void FixedUpdate()
        {
            CheckSpriteRotation();
        }

        private void OnInteractPerformed(InputAction.CallbackContext callbackContext)
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

        private void CheckSpriteRotation()
        {
            //this is where the flipped sprite with the different legs would come in handy
            if (rb.linearVelocityX > 0.1)
            {
                spriteRenderer.sprite = walkingRightSprite;
            }
            else if (rb.linearVelocityX < -0.1)
            {
                spriteRenderer.sprite = walkingLeftSprite;
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
                    rb.linearVelocityX *= horizontalDampening *  Time.deltaTime;
                }
            }
        }
        
        public void Lantern()
        {
            animator.SetBool("GotLantern", true);
        }
    }
}

