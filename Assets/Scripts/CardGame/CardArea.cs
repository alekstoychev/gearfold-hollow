using UnityEngine;
using UnityEngine.EventSystems;

namespace CardGame
{
    public class CardArea : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private HandManager handManager;

        public void OnPointerEnter(PointerEventData eventData)
        {
            handManager?.OnZoneEnter();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            handManager?.OnZoneExit();
        }
    }
}
