using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace Interact
{
    public class EnterablePlace : Interactable
    {
        public UnityEvent onEnter;
        
        [Header("Fortune Teller")]
        public bool isFortuneTeller;
        public string cutsceneTag;
        
        [Header("To Clocktower")]
        public bool isClocktower;
        public string clocktowerTag;

        public override void Interact()
        {
            if (isFortuneTeller)
            {
                PlayableDirector director = GameObject.FindGameObjectWithTag(cutsceneTag).GetComponent<PlayableDirector>();
                director.Play();
                
                return;
            }

            if (isClocktower)
            {
                PlayableDirector director = GameObject.FindGameObjectWithTag(clocktowerTag).GetComponent<PlayableDirector>();
                director.Play();

                return;
            }
            
            onEnter?.Invoke();
        }
    }
}
