using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CardGame
{
    [Serializable]
    public struct CardTransforms
    {
        public RectTransform normal;
        public RectTransform hovered;
        public RectTransform selected;
    }

    [Serializable]
    public struct Card
    {
        public CardTransforms transforms;
        public CardObject cardObject;
    }
    
    public class HandManager : MonoBehaviour
    {
        [Header("Layout")]
        [SerializeField] protected Vector2 arcCenter;
        [SerializeField] protected float arcRadius;
        [SerializeField] protected float middleAngle;
        [SerializeField] protected float angleSpread;
        [SerializeField] private float pushOffset;
        [SerializeField] private float endCardMultiplier;
        [SerializeField] private Vector2 disabledDeckPosOffset;
        [SerializeField] private float disabledDeckOffset;

        [Header("Hover offset")] 
        [SerializeField] protected float hoverOffset;
        [SerializeField] protected float selectOffset;

        [Header("Animation")] 
        [SerializeField] protected float moveDuration;
        [SerializeField] protected AnimationCurve moveCurve;
        
        [Header("Playing cards")]
        [SerializeField] private RectTransform playingCardPosition;
        [SerializeField] private CombatManager combatManager;
        [SerializeField] private bool isAI;
        
#if UNITY_EDITOR
        [Header("Debug")] 
        public bool showArcGizmos = true;
        [Range(1, 10)] public int testCardCount;
        public Color arcColor = Color.white;
        public Color cardBaseColor = Color.green;
        public Color hoveredColor = Color.yellow;
        public Color selectedColor = Color.red;
        
        private void OnDrawGizmos()
        {
            if (!showArcGizmos) return;

            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(arcCenter, 5f);

            float halfGaps = (testCardCount - 1) / 2f;
            float startAngle = middleAngle - halfGaps * angleSpread;
            float endAngle = middleAngle + halfGaps * angleSpread;
            
            int segments = 20;
            Vector2 prevPoint = Vector2.zero;
            for (int i = 0; i <= segments; i++)
            {
                float t = (float)i / segments;
                float angleDeg = Mathf.Lerp(startAngle, endAngle, t);
                float angleRad = angleDeg * Mathf.Deg2Rad;
                Vector2 dir = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
                Vector2 point = arcCenter + dir * arcRadius;

                if (i > 0)
                {
                    Gizmos.color = arcColor;
                    Gizmos.DrawLine(prevPoint, point);
                }
                prevPoint = point;
            }

            if (testCardCount > 0)
            {
                for (int i = 0; i < testCardCount; i++)
                {
                    float t = (float)i / (testCardCount - 1);
                    float angleDeg = Mathf.Lerp(startAngle, endAngle, t);
                    float angleRad = angleDeg * Mathf.Deg2Rad;
                    Vector2 dir = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
                    Vector2 basePos = arcCenter + dir * arcRadius;

                    Gizmos.color = cardBaseColor;
                    Gizmos.DrawSphere(basePos, 5f);

                    Gizmos.color = Color.gray;
                    Gizmos.DrawLine(arcCenter, basePos);

                    Vector2 liftedPos = basePos + dir * hoverOffset;
                    Gizmos.color = hoveredColor;
                    Gizmos.DrawSphere(liftedPos, 3f);

                    Vector2 selectedPos = basePos + dir * selectOffset;
                    Gizmos.color = selectedColor;
                    Gizmos.DrawSphere(selectedPos, 3f);
                }
            }
        }
#endif
        
        protected List<CardObject> cards = new List<CardObject>();
        protected List<Coroutine> moveCoroutines = new List<Coroutine>();
        protected int hoveredCardIndex = -1;
        protected int selectedCardIndex = -1;
        protected bool isCardBeingPlayed;
        
        private bool isZoneHovered;

        public Action onLostAllCards;

        public int GetCurrentCardAmount()
        {
            return cards.Count;
        }

        private void CharacterLostCard()
        {
            if (!isCardBeingPlayed) return;

            if (GetCurrentCardAmount() == 0)
            {
                onLostAllCards.Invoke();
                return;
            }
            
            isCardBeingPlayed = false;
            selectedCardIndex = -1;
            ActivateAllCards();
        }

        public virtual void AddCard(CardObject newCard)
        {
            newCard.SetManager(this);
            newCard.SetIndex(cards.Count);

            newCard.onDeath += CharacterLostCard;
            
            cards.Add(newCard);
            moveCoroutines.Add(null);
            
            UpdateHand();
        }

        public void RemoveCard(CardObject card)
        {
            int index = cards.IndexOf(card);
            if (index >= 0)
            {
                if (moveCoroutines[index] != null)
                {
                    StopCoroutine(moveCoroutines[index]);
                }
                
                cards.RemoveAt(index);
                moveCoroutines.RemoveAt(index);
                UpdateHand();
            }
        }

        private void UpdateHand()
        {
            for (int i = 0; i < cards.Count; i++)
            {
                cards[i].SetIndex(i);
            }

            if (hoveredCardIndex >= cards.Count)
            {
                hoveredCardIndex = -1;
            }

            UpdateAllCards();
        }
        
        public void SetZoneHovered(bool isActive)
        {
            if (isCardBeingPlayed) return;

            isZoneHovered = isActive;
            UpdateAllCards();
        }

        public virtual void SetCardHovered(int cardIndex, bool isActive)
        {
            if (isCardBeingPlayed) return;
            if (cardIndex < 0 || cardIndex >= cards.Count) return;

            if (isActive) hoveredCardIndex = cardIndex;
            else if (hoveredCardIndex == cardIndex) hoveredCardIndex = -1;
            
            isZoneHovered = isActive;
            
            UpdateAllCards();
        }

        public virtual void OnCardSelect(int cardIndex)
        {
            selectedCardIndex = cardIndex;
            isZoneHovered = false;
            hoveredCardIndex = -1;
            isCardBeingPlayed = true;

            if (moveCoroutines[cardIndex] != null)
            {
                StopCoroutine(moveCoroutines[cardIndex]);
            }

            Vector2 targetPosition = playingCardPosition.anchoredPosition;
            Quaternion targetRotation = playingCardPosition.rotation;
            Vector3 targetScale = playingCardPosition.localScale;
            
            DeactivateAllCards();
            
            moveCoroutines[cardIndex] = StartCoroutine(MoveCard(cards[cardIndex].GetComponent<RectTransform>(), targetPosition, targetRotation, targetScale, cardIndex,true));

            if (isAI)
            {
                StartCoroutine(combatManager.AISelected(cards[cardIndex]));
            }
            else
            {
                StartCoroutine(combatManager.PlayerSelected(cards[cardIndex]));
            }
        }

        public virtual void DeactivateAllCards()
        {
            foreach (CardObject card in cards)
            {
                card.DisableButton();
            }
        }

        public virtual void ActivateAllCards()
        {
            foreach (CardObject card in cards)
            {
                card.EnableButton();
            }
        }

        public virtual void UpdateAllCards()
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
                
                float finalStartAngle = startAngle;
                float finalEndAngle = endAngle;
                if (hoveredCardIndex != -1)
                {
                    if (hoveredCardIndex == i - 1)
                    {
                        if (i == cards.Count - 1)
                        {            
                            finalEndAngle = middleAngle + halfGaps * (angleSpread + pushOffset * endCardMultiplier);
                        }
                        else
                        {
                            value += pushOffset;
                        }
                    }
                    else if (hoveredCardIndex == i + 1)
                    {
                        if (i == 0)
                        {
                            finalStartAngle = middleAngle - halfGaps * (angleSpread +  pushOffset * endCardMultiplier);
                        }
                        else
                        {
                            value -= pushOffset;
                        }
                    }
                }
                
                float angleDeg = Mathf.Lerp(finalStartAngle, finalEndAngle, value);
                float angleRad = angleDeg * Mathf.Deg2Rad;
                
                Vector2 direction = new  Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
                Vector2 basePosition = arcCenter + direction * arcRadius;
                Quaternion baseRotation = Quaternion.FromToRotation(Vector3.up, direction);

                Vector2 targetPosition;
                Quaternion targetRotation = baseRotation; 
                
                
                if (i == hoveredCardIndex)
                {
                    targetPosition = basePosition + direction * selectOffset;
                }
                else if (isZoneHovered)
                {
                    targetPosition = basePosition + direction * hoverOffset;
                }
                else if (isCardBeingPlayed)
                {
                    basePosition = (arcCenter - disabledDeckPosOffset) + direction * (arcRadius - disabledDeckOffset);
                    targetPosition = basePosition + direction * hoverOffset;
                }
                else
                {
                    targetPosition = basePosition;
                }

                Vector3 targetScale = cards[i].GetComponent<RectTransform>().localScale;
                
                moveCoroutines[i] = StartCoroutine(MoveCard(cards[i].GetComponent<RectTransform>(), targetPosition, targetRotation, targetScale, i));
            }
        }

        protected virtual IEnumerator MoveCard(RectTransform rectTransform, Vector2 targetPosition, Quaternion targetRotation, Vector3 targetScale, int cardIndex, bool isSelected = false)
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

                if (elapsedTime >= midPoint && !isFlippedCard && isSelected)
                {
                    cards[selectedCardIndex].UpdateSprite(true);
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

            if (isSelected)
            {
                rectTransform.SetParent(playingCardPosition);
                rectTransform.anchoredPosition = new Vector2(0,0);
                RemoveCard(cards[selectedCardIndex]);
            }
        }
    }
}