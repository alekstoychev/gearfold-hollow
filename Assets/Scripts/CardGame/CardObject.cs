using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CardGame
{
    [Serializable]
    public enum CardType
    {
        Type1,
        Type2,
        Type3,
        Type4,
        Type5
    }
    
    public class CardObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("References")]
        [SerializeField] private HandManager handManager;
        [SerializeField] private TMPro.TextMeshProUGUI healthText;
        [SerializeField] private TMPro.TextMeshProUGUI damageText;
        
        [Header("Card Stats")]
        [SerializeField] private float damage;
        [SerializeField] private float health;
        [SerializeField] private float damageDecreaseRate;
        [SerializeField] private CardType cardType;

        private int cardIndex;

        public Action onDeath;
        
        public float Damage { get => damage; }
        public float Health { get => health; }
        public CardType CardType { get => cardType; set => cardType = value; }

        public void TakeDamage(float damageToApply)
        {
            health -= damageToApply;
            if (health <= 0)
            {
                onDeath?.Invoke(); 
            }
        }

        public void DecreaseDamage()
        {
            damage -= damageDecreaseRate;
            if (damage <= 0)
            {
                onDeath?.Invoke();
            }
        }

        public void SetManager(HandManager handManagerToSet)
        {
            handManager = handManagerToSet;
        }
        
        public void SetIndex(int index)
        {
            cardIndex = index;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            handManager?.SetCardHovered(cardIndex, true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            handManager?.SetCardHovered(cardIndex, false);
        }

        public void OnCardSelect()
        {
            handManager?.OnCardSelect(cardIndex);
        }

        public void DisableButton()
        {
            GetComponent<Button>().enabled = false;
        }
    }
}
