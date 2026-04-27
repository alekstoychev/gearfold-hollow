using System;
using System.Collections.Generic;
using CardGame;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FortuneCardSpawner : MonoBehaviour
{
    [SerializeField] private FortuneCardMover cardMover;
    [SerializeField] private RectTransform spawnTarget;
    
    [Serializable]
    private struct DefaultCard
    {
        public GameObject prefab;
        public CardType cardType;
    }
        
    [Header("Cards")]
    [SerializeField] private List<DefaultCard> defaultCards;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        List<DefaultCard> curCards = new List<DefaultCard>(defaultCards);
        
        for (int i = 0; i < 5; i++)
        {
            int randomIndex = Random.Range(0, curCards.Count);
            GameObject spawnedCard = Instantiate(curCards[randomIndex].prefab, spawnTarget.position, spawnTarget.rotation, spawnTarget);
            spawnedCard.transform.localScale = Vector3.zero;
            spawnedCard.GetComponent<CardObject>().CardType = curCards[randomIndex].cardType;
            
            curCards.RemoveAt(randomIndex);
            cardMover.AddCard(spawnedCard.GetComponent<CardObject>());
        }
    }
}
