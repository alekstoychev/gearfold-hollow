using System.Collections;
using Player;
using UnityEngine;

namespace Systems.Managers
{
    public class DocksManager : MonoBehaviour
    {
        [Header("Spawn points")]
        [SerializeField] private Transform boatSpawnPoint;
        [SerializeField] private Transform fromMainStreetSpawnPoint;

        [SerializeField] private GameObject ferrymanTrigger;
        
        [SerializeField] private GameObject teleportTrigger;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            ferrymanTrigger.SetActive(GameManager.Instance.firstTimeDocks);
            
            Transform playerLocation = GameObject.FindGameObjectWithTag("Player").transform;
            switch (GameManager.Instance.docksSpawnPoint)
            {
                case GameManager.DocksSpawnPoint.Boat:
                    playerLocation.position = boatSpawnPoint.position;
                    break;
                case GameManager.DocksSpawnPoint.MainStreet:
                    playerLocation.position = fromMainStreetSpawnPoint.position;
                    
                    StartCoroutine(GameManager.Instance.MovePlayer(-1));
                    break;
            }
            
            StartCoroutine(EnableTeleport());
        }
        
        private IEnumerator EnableTeleport()
        {
            yield return new WaitForSeconds(1f);

            teleportTrigger.SetActive(true);
        }
    }
}
