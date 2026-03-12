using System;
using DialogueSystem;
using UnityEngine;

namespace Interact
{
    public class Interactable : MonoBehaviour
    {
        #region Variables
        [Header("Dialogue")]
        public GameObject textPopup;
        public Dialogue dialogue;
    
        [Header("Interactable")]
        public bool isInteractable;

        public static event Action<Dialogue> OnInteract;
        #endregion

        private void Awake()
        {
            textPopup.SetActive(false);
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
            OnInteract?.Invoke(dialogue);
        }
    }
}
