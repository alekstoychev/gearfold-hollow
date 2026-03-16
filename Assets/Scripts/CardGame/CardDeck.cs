using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        [SerializeField] private RectTransform defaultPosition;

        [Header("Deck")]
        [SerializeField] private GameObject fakeCardPrefab;
        [SerializeField] private float fakeCardOffset;
        [SerializeField] private float animationTime;
        [SerializeField] private float moveSpeed;
        
        private List<GameObject> cardDeckObjects;
        
        private List<CardObject> availableCards = new List<CardObject>();
        
        public HandManager Hand => hand;
        
        private void Start()
        {
            if (defaultCards.Count == 0) return;
            
            GenerateDeck();

            StartCoroutine(InitialDrawCards());
        }

        private IEnumerator InitialDrawCards()
        {
            for (int i = 0; i < 5; i++)
            {
                DrawCardFromDeck();
                yield return new WaitForSeconds(0.5f);
            }
        }
        
        private void GenerateDeck()
        {
            List<DefaultCard> tempDeck = new List<DefaultCard>(defaultCards);
            cardDeckObjects = new List<GameObject>();
            
            float curYOffest = 0f;
            for (int i = 0; i < defaultCards.Count; i++)
            {
                int idx = Random.Range(0, tempDeck.Count);
                GameObject curCard = Instantiate(tempDeck[idx].prefab, cardHand.transform);
                curCard.GetComponent<RectTransform>().anchoredPosition = defaultPosition.anchoredPosition;
                
                CardObject card = curCard.GetComponent<CardObject>();
                card.CardType = tempDeck[idx].cardType;
                
                availableCards.Add(card);
                tempDeck.RemoveAt(idx);
                
                curCard.SetActive(false);
                
                GameObject fakeCard = Instantiate(fakeCardPrefab, gameObject.transform);
                fakeCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, curYOffest);
                curYOffest += fakeCardOffset;
                
                cardDeckObjects.Add(fakeCard);
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

                if (cardDeckObjects.Count == 0) yield break;
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
