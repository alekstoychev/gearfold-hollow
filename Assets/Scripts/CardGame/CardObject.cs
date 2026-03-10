using UnityEngine;
using UnityEngine.EventSystems;

namespace CardGame
{
    public class CardObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private HandManager handManager;
        private int cardIndex;
        
        public void SetIndex(int index)
        {
            cardIndex = index;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            handManager?.OnCardEnter(cardIndex);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            handManager?.OnCardExit(cardIndex);
        }
    }
}
