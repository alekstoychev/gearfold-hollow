using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CardGame
{
    [Serializable]
    public enum RockPaperScissors
    {
        Invalid,
        Rock,
        Paper,
        Scissors
    }
    
    public class CombatManager : MonoBehaviour
    {
        [Header("AI Player")]
        [SerializeField] private FakeCardPlayer aiPlayer;
        [SerializeField] private CardDeck aiDeck;
        [SerializeField] private bool hasAIPicked;
        [SerializeField] private CardObject aiCard;
        private bool isAICardDead;
        
        [Header("Human Player")]
        [SerializeField] private CardDeck playerDeck;
        [SerializeField] private bool hasPlayerPicked;
        [SerializeField] private CardObject playerCard;
        private bool isPlayerCardDead;
        
        [Header("Player Rock Paper Scissors")]
        [SerializeField] private GameObject rockButton;
        [SerializeField] private GameObject paperButton;
        [SerializeField] private GameObject scissorsButton;
        [SerializeField] private TMPro.TextMeshProUGUI minigameResultsText;
        [SerializeField] private Animator playerAnimator;
        
        [Header("Opponent Rock Paper Scissors")]
        [SerializeField] private GameObject enemyChoice;
        [SerializeField] private GameObject rockButtonPrefab;
        [SerializeField] private GameObject paperButtonPrefab;
        [SerializeField] private GameObject scissorsButtonPrefab;
        [SerializeField] private Animator opponentAnimator;
        private RockPaperScissors aiRockPaperScissors;

        [Space(10)] 
        public UnityEvent onPlayerWinGame;
        public UnityEvent onPlayerLoseGame;

        private void Awake()
        {
            playerDeck.Hand.onLostAllCards += OnPlayerLose;
            aiDeck.Hand.onLostAllCards += OnPlayerWin;
        }

        private void OnPlayerWin()
        {
            StartCoroutine(PlayerWinCoroutine());
        }

        private IEnumerator PlayerWinCoroutine()
        {
            minigameResultsText.text = "You win!";
            
            yield return new WaitForSeconds(1f);
            
            onPlayerWinGame.Invoke();
        }

        private void OnPlayerLose()
        {
            StartCoroutine(PlayerLoseCoroutine());
        }

        private IEnumerator PlayerLoseCoroutine()
        {
            minigameResultsText.text = "You lose!";
            
            yield return new WaitForSeconds(1f);
            
            onPlayerLoseGame.Invoke();
        }
        
        public IEnumerator PlayerSelected(CardObject player)
        {
            isPlayerCardDead = false;
            
            playerCard = player;
            hasPlayerPicked = true;
            player.onDeath += OnPlayerCardDead;
            
            yield return new WaitForSeconds(0.1f);
            playerDeck.DrawCardFromDeck();

            yield return new WaitForSeconds(0.5f);
            CheckPlayersSelections();
        }

        public IEnumerator AISelected(CardObject ai)
        {
            isAICardDead = false;
            
            aiCard = ai;
            hasAIPicked = true;
            ai.onDeath += OnAICardDead;
            
            yield return new WaitForSeconds(0.1f);
            aiDeck.DrawCardFromDeck();
            
            yield return new WaitForSeconds(1f);
            CheckPlayersSelections();
        }

        private void CheckPlayersSelections()
        {
            if (!hasPlayerPicked)
            {
                Debug.Log("No player card selected");
                return;
            }

            if (!hasAIPicked)
            {
                Debug.Log("No opponent card selected");
                StartCoroutine(aiPlayer.PickACard());
                return;
            }
            
            BeginRockPaperScissors();
        }

        private void BeginRockPaperScissors()
        {
            minigameResultsText.text = "";
            ShowRockPaperScissors();
            GetAIChoice();
        }

        public void PlayerRockPaperScissors(int choiceInInt)
        {
            HideRockPaperScissors();
            
            RockPaperScissors choice = (RockPaperScissors)choiceInInt;
            switch (choice)
            {
                case RockPaperScissors.Rock:
                    rockButton.SetActive(true);
                    break;
                case RockPaperScissors.Paper:
                    paperButton.SetActive(true);
                    break;
                case RockPaperScissors.Scissors:
                    scissorsButton.SetActive(true);
                    break;
                default:
                    Debug.LogError("Invalid choice");
                    return;
            }
            
            StartCoroutine(CheckResults(choice));
        }

        private void OnPlayerCardDead()
        {
            isPlayerCardDead = true;
            playerCard = null;
        }

        private void OnAICardDead()
        {
            isAICardDead = true;
            aiCard = null;
        }

        private IEnumerator CheckResults(RockPaperScissors playerChoice)
        {
            if (playerChoice == RockPaperScissors.Invalid || aiRockPaperScissors == RockPaperScissors.Invalid)
            {
                Debug.LogError("Invalid choice");
                BeginRockPaperScissors();
                yield break;
            }
            
            yield return new WaitForSeconds(1f);
            
            Debug.Log($"Player Choice:  {playerChoice}, Enemy Choice: {aiRockPaperScissors}");
            
            enemyChoice.SetActive(true);
            
            yield return new WaitForSeconds(1f);
            
            if (playerChoice == aiRockPaperScissors)
            {
                minigameResultsText.text = "Draw. Try Again";
                
                yield return new WaitForSeconds(1f);
                BeginRockPaperScissors();
                yield break;
            }

            bool isPlayerWin;
            switch (playerChoice, aiRockPaperScissors)
            {
                case (RockPaperScissors.Rock, RockPaperScissors.Scissors):
                case (RockPaperScissors.Paper, RockPaperScissors.Rock):
                case (RockPaperScissors.Scissors, RockPaperScissors.Paper):
                    minigameResultsText.text = "Player Wins";
                    isPlayerWin = true;
                    break;
                default:
                    minigameResultsText.text = "Player loses";
                    isPlayerWin = false;
                    break;
            }

            if (isPlayerWin)
            {
                StartCoroutine(ContinueCombat(playerCard, playerAnimator, aiCard, opponentAnimator));
            }
            else
            {
                StartCoroutine(ContinueCombat(aiCard, opponentAnimator, playerCard, playerAnimator));
            }
        }

        private IEnumerator ContinueCombat(CardObject winner, Animator winnerAnimator, CardObject loser, Animator loserAnimator)
        {
            HideRockPaperScissors();
            enemyChoice.SetActive(false);
            
            yield return new WaitForSeconds(1f);
            
            minigameResultsText.text = "";
            
            winnerAnimator.SetBool("isWin", true);
            loserAnimator.SetBool("isLose", true);
            
            yield return new WaitForSeconds(1f);
            
            loser.TakeDamage(winner.Damage);
            winner.DecreaseDamage();
            
            winnerAnimator.SetBool("isWin", false);
            loserAnimator.SetBool("isLose", false);
            
            yield return new WaitForSeconds(2f);
            CheckCombatComplete();
        }

        private void CheckCombatComplete()
        {
            if (isPlayerCardDead)
            {
                Debug.Log("Player died. Draw another card");
                // tell hand manager to do it's thing
                // hand manager does that on its own
                return;
            }
            
            if (isAICardDead)
            {
                Debug.Log("Opponent died. Draw another card");
                // tell ai to draw a card
                StartCoroutine(aiPlayer.PickACard());
                return;
            }
            
            BeginRockPaperScissors();
        }

        private void GetAIChoice()
        {
            aiRockPaperScissors = aiPlayer.PickRockPaperScissors();
            
            Image targetImage = enemyChoice.GetComponent<Image>();
            switch (aiRockPaperScissors)
            {
                case RockPaperScissors.Rock:
                    targetImage.sprite = rockButtonPrefab.GetComponent<Image>().sprite;
                    break;
                case RockPaperScissors.Paper:
                    targetImage.sprite = paperButtonPrefab.GetComponent<Image>().sprite;
                    break;
                case RockPaperScissors.Scissors:
                    targetImage.sprite = scissorsButtonPrefab.GetComponent<Image>().sprite;
                    break;
            }
            
            enemyChoice.GetComponent<Button>().interactable = false;
        }
        
        private void ShowRockPaperScissors()
        {
            SetButtonActive(enemyChoice, false);
            SetButtonActive(rockButton, true);
            SetButtonActive(paperButton, true);
            SetButtonActive(scissorsButton, true);
        }

        private void HideRockPaperScissors()
        {
            SetButtonActive(rockButton, false);
            SetButtonActive(paperButton, false);
            SetButtonActive(scissorsButton, false);
        }

        private void SetButtonActive(GameObject button,  bool active)
        {
            button.SetActive(active);
            button.GetComponent<Button>().interactable = active;
        }
    }
}
