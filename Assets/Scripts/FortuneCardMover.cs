using System.Collections;
using CardGame;
using UnityEngine;
using UnityEngine.Events;

public class FortuneCardMover : HandManager
{
    public bool shouldShowCards = false;
    [SerializeField] private RectTransform originalTransform;

    public bool ShouldShowCards
    {
        get => shouldShowCards;
        set => shouldShowCards = value;
    }

    public override void AddCard(CardObject newCard)
    {
        newCard.SetManager(this);
        newCard.SetIndex(cards.Count);
            
        cards.Add(newCard);
        moveCoroutines.Add(null);
    }
    
    public override void SetCardHovered(int cardIndex, bool isActive)
    {
        if (isCardBeingPlayed) return;
        if (cardIndex < 0 || cardIndex >= cards.Count) return;

        if (isActive) hoveredCardIndex = cardIndex;
        else if (hoveredCardIndex == cardIndex) hoveredCardIndex = -1;
        
        UpdateAllCards();
    }

    public override void OnCardSelect(int cardIndex) 
    {
        selectedCardIndex = cardIndex;
        hoveredCardIndex = -1;
        isCardBeingPlayed = true;

        if (moveCoroutines[cardIndex] != null)
        {
            StopCoroutine(moveCoroutines[cardIndex]);
        }
        
        DeactivateAllCards();
        
        UpdateAllCards();
    }

    public override void DeactivateAllCards() 
    {
        foreach (CardObject card in cards)
        {
            card.DisableButton();
        }
    }
    
    public override void ActivateAllCards() 
    {
        foreach (CardObject card in cards)
        {
            card.EnableButton();
        }
    }

    public override void UpdateAllCards() 
    {
        int cardsAmount = cards.Count;
        if (cardsAmount <= 0) return;
        
        float halfGaps =  (cardsAmount - 1) / 2f;
        float startAngle = middleAngle - halfGaps * angleSpread;
        float endAngle = middleAngle + halfGaps * angleSpread;
        
        for (int i = 0; i < cardsAmount; i++)
        {
            if (moveCoroutines[i] != null)
            {
                StopCoroutine(moveCoroutines[i]);
            }
            
            float value = cardsAmount > 1 ? (float)i / (cards.Count - 1) : 0.5f;
            
            float angleDeg = Mathf.Lerp(startAngle, endAngle, value);
            float angleRad = angleDeg * Mathf.Deg2Rad;
            
            Vector2 direction = new  Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
            Vector2 basePosition = arcCenter + direction * arcRadius;
            Quaternion baseRotation = Quaternion.FromToRotation(Vector3.up, direction);

            Vector2 targetPosition;
            Quaternion targetRotation = baseRotation; 
            
            
            if (i == hoveredCardIndex)
            {
                targetPosition = basePosition + direction * hoverOffset;
            }
            else if (i ==  selectedCardIndex)
            {
                targetPosition = basePosition + direction * selectOffset;
            }
            else
            {
                targetPosition = basePosition;
            }

            Vector3 targetScale;
            if (shouldShowCards)
            {
                targetScale = new Vector3(-0.5f, -0.5f, 0.5f);
            }
            else
            {
                targetScale = new Vector3(0f,0f,0f);
                targetPosition = originalTransform.anchoredPosition;
            }
            
            moveCoroutines[i] = StartCoroutine(MoveCard(cards[i].GetComponent<RectTransform>(), targetPosition, targetRotation, targetScale, i));
        }
    }

    protected override IEnumerator MoveCard(RectTransform rectTransform, Vector2 targetPosition, Quaternion targetRotation, Vector3 targetScale, int cardIndex, bool isSelected = false)
    {
        Vector2 startPos = rectTransform.anchoredPosition;
        Quaternion startRot = rectTransform.rotation;
        Vector3 startScale = rectTransform.localScale;

        float elapsedTime = 0f;
        float midPoint = moveDuration / 2;
        bool isFlippedCard = false;
        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / moveDuration);
            float curveValue = moveCurve.Evaluate(t);
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPosition, curveValue);
            rectTransform.rotation = Quaternion.Lerp(startRot, targetRotation, curveValue);
            rectTransform.localScale = Vector3.Lerp(startScale, targetScale, curveValue);

            if (elapsedTime >= midPoint && !isFlippedCard && cardIndex == selectedCardIndex)
            {
                cards[selectedCardIndex].UpdateSprite(false);
                isFlippedCard = true;
            }

            yield return null;
        }

        rectTransform.anchoredPosition = targetPosition;
        rectTransform.rotation = targetRotation;
        rectTransform.localScale = targetScale;

        if (cardIndex >= 0 && cardIndex < moveCoroutines.Count)
        {
            moveCoroutines[cardIndex] = null;
        }
    }
}
