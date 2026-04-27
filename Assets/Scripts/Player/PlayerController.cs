using System;
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
        public bool canMove;

        [Header("Sprites")] 
        public Sprite walkingRightSprite;
        public Sprite walkingLeftSprite;
        
        [Header("Slope Handling")]
        public float slopeRayLength = 0.8f;      
        public float maxSlopeAngle = 45f;     
        public LayerMask groundLayer;
        private Vector2 slopeNormalPerp;         
        private bool isOnSlope;
        private bool wasOnSlope;
        private float currentSlopeAngle;
        
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
            
            canMove = true;
        }

        private void OnDestroy()
        {
            if (interactAction != null)
            {
                interactAction.performed -= OnInteractPerformed;
            }
        }

        // Update is called once per frame
        private void Update()
        {
            //changing the animation from idle to running when moving
            horizontalMove = Input.GetAxisRaw("Horizontal") * moveSpeed;
            if (!canMove) horizontalMove = 0f;
            
            animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

            // mirroring the animation when going left. I feel like this might not be optimal but it works
            movement = new Vector2(Input.GetAxis("Horizontal"), 0).normalized;
            if (!canMove) horizontalMove = 0f;
            
            bool mirrored = movement.x < 0;
            if (movement.x != 0)
            {
                this.transform.rotation = Quaternion.Euler(new Vector3(0f, mirrored ? 180f : 0f, 0f));
            }
            
            Move();
        }

        private void FixedUpdate()
        {
            UpdateSlopeInfo();
            CheckSpriteRotation();
            
            if (isOnSlope && currentSlopeAngle > 15f && rb.linearVelocityY > -0.1f)
            {
                rb.AddForce(Vector2.down * 3f, ForceMode2D.Force);
            }
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

                if (interactable.isInteractable)
                {
                    interactable.Interact();
                    break;
                }
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
            if (!canMove)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }
            
            if (moveAction.IsPressed())
            {
                float inputX = moveAction.ReadValue<Vector2>().x;
                float targetVelocityX = inputX * moveSpeed;

                if (isOnSlope)
                {
                    Vector2 slopeMove = slopeNormalPerp * targetVelocityX;
                    rb.linearVelocity = new Vector2(slopeMove.x, slopeMove.y);
                }
                else
                {
                    rb.linearVelocityX = targetVelocityX;

                    if (wasOnSlope && rb.linearVelocityY > 0)
                    {
                        rb.linearVelocity = new Vector2(rb.linearVelocityX, 0);
                    }
                }
            }
            else
            {
                if (!isOnSlope)
                {
                    rb.linearVelocityX *= horizontalDampening * Time.deltaTime;
                }
                else if (isOnSlope)
                {
                    rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, horizontalDampening * Time.deltaTime);
                }
            }
            
            wasOnSlope = isOnSlope;
        }
        
        private bool IsGrounded(out RaycastHit2D hit)
        {
            Vector2 origin = collision.bounds.center - new Vector3(0, collision.bounds.extents.y);
            hit = Physics2D.Raycast(origin, Vector2.down, slopeRayLength, groundLayer);
            return hit.collider != null;
        }

        private void UpdateSlopeInfo()
        {
            RaycastHit2D hit;
            if (IsGrounded(out hit))
            {
                currentSlopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                isOnSlope = currentSlopeAngle > 0 && currentSlopeAngle <= maxSlopeAngle;
        
                if (isOnSlope)
                {
                    slopeNormalPerp = new Vector2(hit.normal.y, -hit.normal.x);
                }
            }
            else
            {
                isOnSlope = false;
                currentSlopeAngle = 0;
            }
        }
        
        public void Lantern()
        {
            animator.SetBool("GotLantern", true);
        }
    }
}

