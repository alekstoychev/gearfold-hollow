using System;
using DialogueSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Interact
{
    public class InteractableNPC : Interactable
    {
        #region Variables
        [Header("Dialogue")]
        public Dialogue dialogue;
        
        public static event Action<Dialogue> OnDialogueInteract;
        #endregion


        public void ProgressDialogueObjective()
        {
            dialogue.ProgressObjectiveDialogue();
        }

        public void SetObjectiveIndex(int idx)
        {
            dialogue.SetObjectiveIndex(idx);
        }

        public override void Interact()
        {
            OnDialogueInteract?.Invoke(dialogue);
        }
    }
}
