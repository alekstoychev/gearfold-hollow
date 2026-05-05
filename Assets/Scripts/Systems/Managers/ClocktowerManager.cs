using System.Collections;
using UnityEngine;

namespace Systems.Managers
{
    public class ClocktowerManager : MonoBehaviour
    {
        public GameObject spawnPoint;
        
        public GameObject clocktowerTrigger;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Transform playerLocation = GameObject.FindGameObjectWithTag("Player").transform;
            playerLocation.position = spawnPoint.transform.position;

            StartCoroutine(GameManager.Instance.MovePlayer(1));

            StartCoroutine(EnableTeleport());
        }
        
        private IEnumerator EnableTeleport()
        {
            yield return new WaitForSeconds(1f);

            clocktowerTrigger.SetActive(true);
        }
    }
}
