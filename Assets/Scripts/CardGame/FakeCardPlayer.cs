using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CardGame
{
    public class FakeCardPlayer : MonoBehaviour
    {
        [SerializeField] private CardDeck cardDeck;
        [SerializeField] private HandManager fakeHandManager;
        

        public IEnumerator PickACard()
        {
            int lastSelected = -1;
            for (int i = 0; i < 3; i++)
            {
                int randIdx = Random.Range(0, fakeHandManager.GetCurrentCardAmount());
                if (fakeHandManager.GetCurrentCardAmount() > 1)
                {
                    while (randIdx == lastSelected)
                    {
                        randIdx = Random.Range(0, fakeHandManager.GetCurrentCardAmount());
                    }
                }
                
                lastSelected = randIdx;
                
                fakeHandManager.SetCardHovered(randIdx, true);
                fakeHandManager.SetZoneHovered(false);
                
                yield return new WaitForSeconds(0.5f);
                
                if (i == 2 || fakeHandManager.GetCurrentCardAmount() == 1)
                {
                    fakeHandManager.OnCardSelect(randIdx);
                    break;
                }
                
                fakeHandManager.SetCardHovered(randIdx, false);
                
                yield return new WaitForSeconds(0.3f);
            }
        }

        public RockPaperScissors PickRockPaperScissors()
        {
            int rand = Random.Range(1, 4);
            Debug.Log($"AI: I have picked: {rand}");
            return (RockPaperScissors)rand;
        }
    }
}
