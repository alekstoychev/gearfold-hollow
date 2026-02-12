using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem
{
    public class DialogueBox : MonoBehaviour
    {
        #region Variables
        [Header("Dialogue Box")]
        public Image dialogueBoxBackground;
        public Image dialogueBoxBackground2;
        public TextMeshProUGUI speakerTitleBox;
        public TextMeshProUGUI speakerTextBox;

        [Header("Typewriter settings")] 
        public float typewriterDelay;
        
        private float currentTypewriterTimer;
        
        private string fullDialogue;
        private bool doTypeWriter;

        private bool canContinueDialogue = true;
        private Dialogue currentDialogue;
        #endregion
    
        public void ShowDialogueBox(Dialogue dialogueToAdd)
        {
            if (!canContinueDialogue)
            {
                return;
            }
            
            currentDialogue = dialogueToAdd;
            
            speakerTitleBox.text = dialogueToAdd.speakerName;
            fullDialogue = dialogueToAdd.GetDialogue();
            
            dialogueBoxBackground.enabled = true;
            dialogueBoxBackground2.enabled = true;
            speakerTitleBox.enabled = true;
            speakerTextBox.enabled = true;
            speakerTextBox.text = "";
            
            doTypeWriter = true;
        }

        public void HideDialogueBox()
        {
            dialogueBoxBackground.enabled = false;
            dialogueBoxBackground2.enabled = false;
            speakerTitleBox.enabled = false;
            speakerTextBox.enabled = false;
            
            doTypeWriter = false;
            
            if (!currentDialogue.CanRemoveDialogue() && currentDialogue.GetContinueAfterText())
            {
                canContinueDialogue = false;
                StartCoroutine(ShowDialogueAfterDelay());
                Debug.Log("Doing the funny now");
            }
        }

        private void Update()
        {
            CheckTypeWriter();
        }

        private void CheckTypeWriter()
        {
            if (!doTypeWriter)
            {
                return;
            }
            
            if (currentTypewriterTimer >= typewriterDelay)
            {
                speakerTextBox.text += fullDialogue[0];
                fullDialogue = fullDialogue.Remove(0, 1);
                if (fullDialogue.Length <= 0)
                {
                    doTypeWriter = false;
                }
                
                currentTypewriterTimer = 0;
            }
            else
            {
                currentTypewriterTimer += Time.deltaTime;
            }
        }
        
        private IEnumerator ShowDialogueAfterDelay()
        {
            yield return new WaitForSeconds(currentDialogue.GetWaitTimeAfterText());
            
            ShowDialogueBox(currentDialogue);
            canContinueDialogue = true;
        }
    }
}