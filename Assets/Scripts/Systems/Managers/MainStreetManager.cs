using System.Collections;
using System.Collections.Generic;
using Interact;
using UnityEngine;

namespace Systems.Managers
{
    public class MainStreetManager : MonoBehaviour
    {
        [Header("Spawn Points")]
        [SerializeField] private Transform fromDocksSpawnLocation;
        [SerializeField] private Transform atMerchantSpawnLocation;
        [SerializeField] private Transform atFortuneTellerSpawnLocation;
        [SerializeField] private Transform fromClocktowerSpawnLocation;
        
        [Header("Merchant")]
        [SerializeField] private GameObject merchantTrigger;
        
        [Header("Fortune Teller")]
        [SerializeField] private GameObject fortuneTeller;
        [SerializeField] private GameObject fortuneTellerDefaultPrefab;
        [SerializeField] private GameObject fortuneTellerSwapPrefab;
        
        [Header("Clocktower triggers")]
        [SerializeField] private GameObject clocktowerTrigger;
        [SerializeField] private GameObject clocktowerTriggerDefaultPrefab;
        [SerializeField] private GameObject clocktowerTriggerSwapPrefab;
        
        [Header("Teleports")]
        [SerializeField] private List<GameObject> teleports;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            merchantTrigger.SetActive(GameManager.Instance.firstTimeMainStreet);
            
            if (GameManager.Instance.playedTutorialCards)
            {
                Merchant merchant = FindFirstObjectByType<Merchant>();
                if (merchant != null)
                {
                    merchant.InformAboutCardGame(GameManager.Instance.winTutorialCards);
                }
            }
                
            fortuneTeller = Instantiate(GameManager.Instance.hasTalkedWithFortuneTeller ? fortuneTellerSwapPrefab : fortuneTellerDefaultPrefab, 
                fortuneTeller.transform.position, Quaternion.identity);

            clocktowerTrigger = Instantiate(GameManager.Instance.hasTalkedWithFortuneTeller ? clocktowerTriggerDefaultPrefab : clocktowerTriggerSwapPrefab,
                clocktowerTrigger.transform.position, Quaternion.identity); 
            
            clocktowerTrigger.SetActive(false);
            teleports.Add(clocktowerTrigger);
            
            Transform playerLocation = GameObject.FindGameObjectWithTag("Player").transform;
            switch (GameManager.Instance.mainStreetSpawnPoint)
            {
                case GameManager.MainStreetSpawnPoint.Docks:
                    playerLocation.position = fromDocksSpawnLocation.position;
                    
                    StartCoroutine(GameManager.Instance.MovePlayer(1));
                    break;
                case GameManager.MainStreetSpawnPoint.MerchantCards:
                    playerLocation.position = atMerchantSpawnLocation.position;
                    break;
                case GameManager.MainStreetSpawnPoint.FortuneTellerHut:
                    playerLocation.position = atFortuneTellerSpawnLocation.position;
                    break;
                case GameManager.MainStreetSpawnPoint.FromClocktower:
                    playerLocation.position = fromClocktowerSpawnLocation.position;
                    
                    StartCoroutine(GameManager.Instance.MovePlayer(-1));
                    break;
            }

            StartCoroutine(EnableTeleports());
        }

        private IEnumerator EnableTeleports()
        {
            yield return new WaitForSeconds(1f);

            foreach (GameObject teleport in teleports)
            {
                teleport?.SetActive(true);
            }
        }
    }
}
