using Random = System.Random;
using RatGamesStudios.OperationDeratization.UI.InGame;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RatGamesStudios.OperationDeratization
{
    public class SpawnPosition : MonoBehaviour
    {
        [Serializable]
        public class LocationTeleports
        {
            public string locationName;
            public Transform[] teleportPoints;
        }
        [SerializeField] private LocationTeleports[] locations;
        [SerializeField] private Tracker tracker;
        private GameObject player;
        private GameObject[] bots;
        private NavMeshAgent[] botNavMeshAgents;

        private void Start()
        {
            player = tracker.player.gameObject;
            Random random = new Random();
            bots = new GameObject[tracker.opponents.Count];
            botNavMeshAgents = new NavMeshAgent[bots.Length];

            for (int i = 0; i < tracker.opponents.Count; i++)
            {
                bots[i] = tracker.opponents[i];
                botNavMeshAgents[i] = bots[i].GetComponent<NavMeshAgent>();
            }

            List<LocationTeleports> availableLocations = new List<LocationTeleports>(locations);
            SetPlayerStartPosition(random, availableLocations);

            for (int i = 0; i < bots.Length; i++)
            {
                if (availableLocations.Count == 0)
                {
                    Debug.LogWarning("No more available locations for bots.");

                    break;
                }

                int randomLocationIndex = random.Next(0, availableLocations.Count);
                LocationTeleports selectedLocation = availableLocations[randomLocationIndex];

                if (selectedLocation.teleportPoints.Length == 0)
                {
                    Debug.LogWarning("No more available teleport points in location: " + selectedLocation.locationName);
                    availableLocations.RemoveAt(randomLocationIndex);
                    i--;

                    continue;
                }

                int botTeleportIndex = random.Next(0, selectedLocation.teleportPoints.Length);
                SetStartPosition(bots[i], selectedLocation.teleportPoints[botTeleportIndex]);
                UpdateNavMeshAgent(bots[i], selectedLocation.teleportPoints[botTeleportIndex]);
                //Debug.Log("Bot " + bots[i].gameObject.name + " spawned at Location: " + selectedLocation.locationName + " on Teleporter: " + botTeleportIndex);
                selectedLocation.teleportPoints = RemoveAtIndex(selectedLocation.teleportPoints, botTeleportIndex);

                if (selectedLocation.teleportPoints.Length == 0)
                    availableLocations.RemoveAt(randomLocationIndex);
            }
        }

        private void SetPlayerStartPosition(Random random, List<LocationTeleports> availableLocations)
        {
            if (availableLocations.Count > 0)
            {
                int randomLocationIndex = random.Next(0, availableLocations.Count);
                LocationTeleports selectedLocation = availableLocations[randomLocationIndex];

                if (selectedLocation.teleportPoints.Length > 0)
                {
                    int playerTeleportIndex = random.Next(0, selectedLocation.teleportPoints.Length);
                    SetStartPosition(player, selectedLocation.teleportPoints[playerTeleportIndex]);
                    //Debug.Log("Player spawned at Location: " + selectedLocation.locationName + " on Teleporter: " + playerTeleportIndex);
                    selectedLocation.teleportPoints = RemoveAtIndex(selectedLocation.teleportPoints, playerTeleportIndex);

                    if (selectedLocation.teleportPoints.Length == 0)
                        availableLocations.RemoveAt(randomLocationIndex);
                }
            }
        }
        private void SetStartPosition(GameObject character, Transform teleportPoint)
        {
            Vector3 newPosition = teleportPoint.position + new Vector3(0f, 1.5f, 0f);
            character.transform.position = newPosition;
        }
        private void UpdateNavMeshAgent(GameObject bot, Transform teleportPoint)
        {
            if (bot == null)
                return;

            NavMeshAgent navMeshAgent = bot.GetComponent<NavMeshAgent>();

            if (navMeshAgent != null)
            {
                Vector3 newPosition = teleportPoint.position + new Vector3(0f, 1.5f, 0f);
                navMeshAgent.Warp(newPosition);
            }
        }
        private Transform[] RemoveAtIndex(Transform[] array, int index)
        {
            List<Transform> list = new List<Transform>(array);
            list.RemoveAt(index);

            return list.ToArray();
        }
    }
}
