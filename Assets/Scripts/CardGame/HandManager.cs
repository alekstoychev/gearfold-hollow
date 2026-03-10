using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CardGame
{
    [Serializable]
    public struct CardPositions
    {
        public Vector2 normal;
        public Vector2 hovered;
        public Vector2 selected;
    }

    [Serializable]
    public struct CardRotations
    {
        public Quaternion normal;
        public Quaternion hovered;
        public Quaternion selected;
    }

    [Serializable]
    public struct Card
    {
        public CardPositions positions;
        public CardRotations rotations;
        public CardObject cardObject;
    }
    
    public class HandManager : MonoBehaviour
    {
        [SerializeField] private Card[] cards;
        [SerializeField] private float moveDuration = 0.3f;
        [SerializeField] private AnimationCurve moveCurve;

        private bool isZoneHovered;
        private int hoveredCardIndex = -1;

        private void Start()
        {
            for (int i = 0; i < cards.Length; i++)
            {
                cards[i].cardObject.SetIndex(i);
                
                Vector2 location = cards[i].positions.normal;
                cards[i].cardObject.GetComponent<RectTransform>().anchoredPosition = location;
                
                Quaternion rotation = cards[i].rotations.normal;
                cards[i].cardObject.GetComponent<RectTransform>().rotation = rotation;
            }
        }

        public void OnZoneEnter()
        {
            isZoneHovered = true;
            UpdateAllCards();
        }

        public void OnZoneExit()
        {
            isZoneHovered = false;
            UpdateAllCards();
        }

        public void OnCardEnter(int cardIndex)
        {
            hoveredCardIndex = cardIndex;
            isZoneHovered = true;
            UpdateAllCards();
        }

        public void OnCardExit(int cardIndex)
        {
            if (hoveredCardIndex == cardIndex)
            {
                hoveredCardIndex = -1;
                isZoneHovered = false;
                UpdateAllCards();
            }
        }

        private void UpdateAllCards()
        {
            for (int i = 0; i < cards.Length; i++)
            {
                Vector2 targetPosition;
                Quaternion targetRotation;
                if (i == hoveredCardIndex)
                {
                    targetPosition = cards[i].positions.selected;
                    targetRotation = cards[i].rotations.selected;
                }
                else if (isZoneHovered)
                {
                    targetPosition = cards[i].positions.hovered;
                    targetRotation = cards[i].rotations.hovered;
                }
                else
                {
                    targetPosition = cards[i].positions.normal;
                    targetRotation = cards[i].rotations.normal;
                }
                
                StartCoroutine(MoveCard(cards[i].cardObject.GetComponent<RectTransform>(), targetPosition, targetRotation));
            }
        }

        private IEnumerator MoveCard(RectTransform rectTransform, Vector2 targetPosition, Quaternion targetRotation)
        {
            Vector2 startPos = rectTransform.anchoredPosition;
            Quaternion startRot = rectTransform.rotation;
            
            float elapsedTime = 0f;
            while (elapsedTime < moveDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / moveDuration);
                float curveValue = moveCurve.Evaluate(t);
                rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPosition, curveValue);
                rectTransform.rotation = Quaternion.Lerp(startRot, targetRotation, curveValue);

                yield return null;
            }
            
            rectTransform.anchoredPosition = targetPosition;
            rectTransform.rotation = targetRotation;
        }
    }
}