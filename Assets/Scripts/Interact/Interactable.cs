using DialogueSystem;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    #region Variables
    [Header("Dialogue")]
    public GameObject textPopup;
    public Dialogue dialogue;
    
    [Header("Interactable")]
    public bool isInteractable;
    #endregion

    void Awake()
    {
        textPopup.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isInteractable)
        {
            textPopup.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (isInteractable)
        {
            textPopup.SetActive(false);
        }
    }

    public void Interact()
    {
        DialogueManager.GetDialogueManager().TriggerDialogue(dialogue);
    }
}
