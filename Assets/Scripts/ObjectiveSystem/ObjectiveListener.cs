using Interact;
using UnityEngine;

namespace ObjectiveSystem
{
    public class ObjectiveListener : MonoBehaviour
    {
        [SerializeField] private string attachedNPC;

        public string AttachedNPC
        {
            get => attachedNPC;
        }

        private void Start()
        {
            UpdateOwnerName();
        }

        public void UpdateOwnerName()
        {
            Interactable interactable = gameObject.GetComponent<Interactable>();
            if (interactable == null)
            {
                attachedNPC = "None";
            }

            attachedNPC = interactable.dialogue.speakerName;
            interactable.dialogue.OnObjectiveDialogueComplete += OnObjectiveComplete;
        }

        private void OnObjectiveComplete()
        {
            ObjectiveManager.ProgressObjective();
        }

        public void InformAboutInvolvement(bool isInvolved)
        {
            gameObject.GetComponent<Interactable>().dialogue.isInvolved = isInvolved;
            Debug.Log($"{gameObject.name}: is involved: {isInvolved}");
        }
    }
}