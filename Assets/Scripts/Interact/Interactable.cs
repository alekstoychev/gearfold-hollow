using System;
using DialogueSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Interact
{
    public enum InteractableType
    {
        NPC,
        RandomInteractable,
        EnterPlace
    }
    
    public class Interactable : MonoBehaviour
    {
        #region Variables
        [Header("Interactable")]
        public bool isInteractable;
        public InteractableType interactableType;
        
        [Header("Dialogue")]
        public GameObject textPopup;
        public Dialogue dialogue;

        public bool hasPlayedCards = false;
        public bool hasWonCards = false;
        
        public int playerWinIndex = -1;
        public int playerLossIndex = -1;
        
        public static event Action<Dialogue> OnDialogueInteract;
        public UnityEvent OnEnterPlace;
        #endregion

        private void Awake()
        {
            textPopup.SetActive(false);

            
        }

        public void SetInteractable(bool interactable)
        {
            isInteractable = interactable;
            
            if (isInteractable)
            {
                textPopup.SetActive(true);
            }
            else if (!isInteractable)
            {
                textPopup.SetActive(false);
            }
        }

        private void Start()
        {
            if (hasPlayedCards)
            {
                if (hasWonCards)
                {
                    SetObjectiveIndex(playerWinIndex);
                    Debug.Log("Merchant has changed dialogue for the player win");
                }
                else
                {
                    SetObjectiveIndex(playerLossIndex);
                    Debug.Log("Merchant has changed dialogue for the player loss");
                }
            }
        }

        public void ProgressDialogueObjective()
        {
            dialogue.ProgressObjectiveDialogue();
        }

        public void SetObjectiveIndex(int idx)
        {
            dialogue.SetObjectiveIndex(idx);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isInteractable)
            {
                textPopup.SetActive(true);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (isInteractable)
            {
                textPopup.SetActive(false);
            }
        }

        public void Interact()
        {
            switch (interactableType)
            {
                case  InteractableType.NPC:
                    OnDialogueInteract?.Invoke(dialogue);
                    break;
                case  InteractableType.RandomInteractable:
                    OnDialogueInteract?.Invoke(dialogue); // maybe???
                    break;
                case  InteractableType.EnterPlace:
                    OnEnterPlace?.Invoke();
                    break;
            }
        }
    }
}
