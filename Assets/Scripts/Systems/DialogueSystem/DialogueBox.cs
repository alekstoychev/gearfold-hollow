using System;
using System.Collections;
using Interact;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DialogueSystem
{
    public class DialogueBox : MonoBehaviour
    {
        #region Variables
        [Header("Dialogue Box")]
        public Image dialogueBoxBackground;
        public TextMeshProUGUI speakerTitleBox;
        public TextMeshProUGUI speakerTextBox;
        public Image speakerImage;

        [Header("Typewriter settings")] 
        public float typewriterDelay;
        
        private float currentTypewriterTimer;
        
        private string fullDialogue;
        private bool doTypeWriter;

        private bool canContinueDialogue = true;
        private Dialogue currentDialogue;
        private bool isOnCooldown;

        public event Action<bool> OnContinueDialogue;
        #endregion
    
        public void ShowDialogueBox(Dialogue dialogueToAdd)
        {
            if (!canContinueDialogue) return;
            if (isOnCooldown) return;
            
            currentDialogue = dialogueToAdd;
            
            speakerTitleBox.text = dialogueToAdd.speakerName;
            fullDialogue = dialogueToAdd.GetDialogue();
            
            dialogueBoxBackground.enabled = true;
            speakerTitleBox.enabled = true;
            speakerTextBox.enabled = true;
            speakerTextBox.text = "";
            
            speakerImage.sprite = dialogueToAdd.GetExpressionSprite();
            if (speakerImage.sprite)
            {
                speakerImage.enabled = true;
            }
            
            doTypeWriter = true;
        }

        public void HideDialogueBox()
        {
            if (isOnCooldown) return;
            
            dialogueBoxBackground.enabled = false;
            speakerTitleBox.enabled = false;
            speakerTextBox.enabled = false;

            if (SceneManager.GetActiveScene().name != "FortuneTellerHut")
            {
                speakerImage.enabled = false;
                speakerImage.sprite = null;
            }
            
            doTypeWriter = false;
            
            //unityevent complete
            
            if (currentDialogue.GetCompleteObjectiveAfterText())
            {
                currentDialogue.TriggerEndOfDialogue();
                
                if (SceneManager.GetActiveScene().name != "FortuneTellerHut")
                {
                    PlayerController player1 = FindFirstObjectByType<PlayerController>();
                    player1.canMove = true;
                }

                return;
            }
            
            currentDialogue.TriggerEndOfDialogue();

            if (currentDialogue.IsInObjective())
            {
                canContinueDialogue = false;
                StartCoroutine(ShowDialogueAfterDelay());
                return;
            }

            if (SceneManager.GetActiveScene().name != "FortuneTellerHut")
            {
                PlayerController player = FindFirstObjectByType<PlayerController>();
                player.canMove = true;
            }
        
            
            /*
            Debug.Log($"Checking continue after dialogue {currentDialogue.GetContinueAfterText()}, {currentDialogue.GetWaitTimeAfterText()}");
            if (currentDialogue.GetContinueAfterText())
            {
                canContinueDialogue = false;
                StartCoroutine(ShowDialogueAfterDelay());
                Debug.Log($"Activating Coroutine.");
            }*/
        }

        public bool TryHideDialogueBox()
        {
            if (doTypeWriter)
            {
                speakerTextBox.text += fullDialogue;
                doTypeWriter = false;
                currentTypewriterTimer = 0;

                return false;
            }
            
            HideDialogueBox();
            return true;
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
            isOnCooldown = true;
            yield return new WaitForSeconds(1.2f);
            
            canContinueDialogue = true;
            isOnCooldown = false;
            ShowDialogueBox(currentDialogue);
            OnContinueDialogue?.Invoke(true);
        }
    }
}