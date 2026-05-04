using UnityEngine;

namespace Interact
{
    public abstract class Interactable : MonoBehaviour
    {
        [Header("Interactable")]
        public bool isInteractable;
        public GameObject textPopup;

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

        public abstract void Interact();
    }
}
