using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Random = UnityEngine.Random;

namespace CardGame
{
    public class CardDeck : MonoBehaviour
    {
        [Serializable]
        private struct DefaultCard
        {
            public GameObject prefab;
            public CardType cardType;
        }
        
        [Header("Cards")]
        [SerializeField] private List<DefaultCard> defaultCards;
        [SerializeField] private GameObject cardHand;
        [SerializeField] private int  defaultCardCount;
        [SerializeField] private HandManager hand;

        [Header("Deck")]
        [SerializeField] private List<GameObject> cardDeckObjects;
        [SerializeField] private float animationTime;
        [SerializeField] private float moveSpeed;
        
        private List<CardObject> availableCards = new List<CardObject>();
        
        private void Start()
        {
            if (defaultCards.Count == 0) return;
            
            GenerateDeck(availableCards);
        }
        
        private void GenerateDeck(List<CardObject> deck)
        {
            List<DefaultCard> tempDeck = new List<DefaultCard>(defaultCards);
            
            for (int i = 0; i < defaultCards.Count; i++)
            {
                int idx = Random.Range(0, tempDeck.Count);
                GameObject curCard = Instantiate(tempDeck[idx].prefab, cardHand.transform);
                CardObject card = curCard.GetComponent<CardObject>();
                card.CardType = tempDeck[idx].cardType;
                
                deck.Add(card);
                tempDeck.RemoveAt(idx);
                
                curCard.SetActive(false);
            }
        }

        public void DrawCardFromDeck()
        {
            if (cardDeckObjects.Count == 0) return;
            
            StartCoroutine(DrawCardAnimation());
        }

        private IEnumerator DrawCardAnimation()
        {
            float elapsedTime = 0f;
            
            while (elapsedTime < animationTime)
            {
                elapsedTime += Time.deltaTime;

                Vector2 curPos = cardDeckObjects[0].GetComponent<RectTransform>().anchoredPosition;
                curPos.y -= moveSpeed;
                cardDeckObjects[0].GetComponent<RectTransform>().anchoredPosition = curPos;

                yield return null;
            }

            Destroy(cardDeckObjects[0]);
            cardDeckObjects.RemoveAt(0);
            DrawCard();
        }

        private void DrawCard()
        {
            if (availableCards.Count == 0) return;

            CardObject drawCard = availableCards[0];
            availableCards.RemoveAt(0);
            drawCard.gameObject.SetActive(true);
            
            hand.AddCard(drawCard);
        }
    }
}
