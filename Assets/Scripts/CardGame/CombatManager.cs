using System.Collections;
using UnityEngine;

namespace CardGame
{
    public class CombatManager : MonoBehaviour
    {
        [SerializeField] private FakeCardPlayer aiPlayer;
        
        private CardObject playerCard;
        private CardObject aiCard;
        
        public void PlayerSelected(CardObject player)
        {
            playerCard = player;
        }

        public void AiSelected(CardObject ai)
        {
            aiCard = ai;
        }

        private void BeginBattle()
        {
            
        }
    }
}
