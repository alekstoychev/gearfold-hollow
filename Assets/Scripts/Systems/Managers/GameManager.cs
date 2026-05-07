using System;
using System.Collections;
using Player;
using UnityEngine;

namespace Systems.Managers
{
    public class GameManager : MonoBehaviour
    {
        [Serializable]
        public enum MainStreetSpawnPoint
        {
            Docks,
            MerchantCards,
            FortuneTellerHut,
            FromClocktower
        }

        [Serializable]
        public enum DocksSpawnPoint
        {
            Boat,
            MainStreet
        }

        public static GameManager Instance { get; private set; }

        #region Main street variables
        [Header("Main street variables")]
        public bool firstTimeMainStreet = true;
        public bool playedTutorialCards = false;
        public bool winTutorialCards = false;
        public bool hasTalkedWithFortuneTeller = false;

        public MainStreetSpawnPoint mainStreetSpawnPoint = MainStreetSpawnPoint.Docks;
        #endregion
        
        #region Docks variables
        [Header("Docks variables")]
        public bool firstTimeDocks = true;
        public DocksSpawnPoint docksSpawnPoint = DocksSpawnPoint.Boat;
        
        #endregion

        public bool HasTalkedWithFortuneTeller
        {
            get => hasTalkedWithFortuneTeller;
            set => hasTalkedWithFortuneTeller = value;
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SetMainStreetSpawnPoint(MainStreetSpawnPoint spawnPoint)
        {
            mainStreetSpawnPoint = spawnPoint;
        }

        public void SetMainStreetSpawnPoint(int spawnPoint)
        {
            mainStreetSpawnPoint = (MainStreetSpawnPoint)spawnPoint;
        }

        public void SetDocksSpawnPoint(DocksSpawnPoint spawnPoint)
        {
            docksSpawnPoint = spawnPoint;
        }

        public void SetDocksSpawnPoint(int spawnPoint)
        {
            docksSpawnPoint = (DocksSpawnPoint)spawnPoint;
        }

        public void PlayerWinTutorialCards()
        {
            firstTimeMainStreet = false;
            playedTutorialCards = true;
            winTutorialCards = true;
        }

        public void PlayerLoseTutorialCards()
        {
            firstTimeMainStreet = false;
            playedTutorialCards = true;
            winTutorialCards = false;
        }
        
        public IEnumerator MovePlayer(float moveDirection)
        {
            yield return new WaitForSeconds(0.5f);
            
            PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            player.CanMove = false;
            player.MoveToAnotherArea(moveDirection);

            yield return new WaitForSeconds(0.5f);
            
            player.StopMoveToAnotherArea();
            player.CanMove = true;
        }
    }
}
