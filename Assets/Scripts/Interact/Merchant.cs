using UnityEngine;

namespace Interact
{
    public class Merchant : InteractableNPC
    {
        private bool hasWonCards = false;
        
        [SerializeField] private int playerWinDialogueIndex = -1;
        [SerializeField] private int playerLossDialogueIndex = -1;
        // Start is called once before the first execution of Update after the MonoBehaviour is created

        public void InformAboutCardGame(bool hasWon)
        {
            hasWonCards = hasWon;
            
            if (hasWonCards)
            {
                SetObjectiveIndex(playerWinDialogueIndex);
                Debug.Log("Merchant has changed dialogue for the player win");
            }
            else
            {
                SetObjectiveIndex(playerLossDialogueIndex);
                Debug.Log("Merchant has changed dialogue for the player loss");
            }
        }
    }
}
