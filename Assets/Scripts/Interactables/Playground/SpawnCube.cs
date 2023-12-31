using System.Collections.Generic;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Interactables.Playground
{
    public class SpawnCube : Interactable
    {
        [Header("References")]
        [SerializeField] private GameObject botPrefab;

        [Header("Bots")]
        [SerializeField] private List<Vector3> botSpawnPositions;

        private void Start()
        {
            botSpawnPositions.Add(new Vector3(1.98099995f, 0.347000003f, 9.3579998f));
            botSpawnPositions.Add(new Vector3(3.32800007f, 0.349999428f, 9.34500027f));
            botSpawnPositions.Add(new Vector3(4.70699978f, 0.349999428f, 9.5340004f));
            botSpawnPositions.Add(new Vector3(6.08099985f, 0.349999428f, 9.72000027f));
            botSpawnPositions.Add(new Vector3(2.10599995f, 0.347000003f, 8.43899918f));
            botSpawnPositions.Add(new Vector3(3.45300007f, 0.349999428f, 8.42599964f));
            botSpawnPositions.Add(new Vector3(4.83199978f, 0.349999428f, 8.61499977f));
            botSpawnPositions.Add(new Vector3(6.20599985f, 0.349999428f, 8.80099964f));
            botSpawnPositions.Add(new Vector3(1.99499989f, 0.347000003f, 7.42399931f));
            botSpawnPositions.Add(new Vector3(3.34200001f, 0.349999428f, 7.41099977f));
            botSpawnPositions.Add(new Vector3(4.72099972f, 0.349999428f, 7.5999999f));
            botSpawnPositions.Add(new Vector3(6.09499979f, 0.349999428f, 7.78599977f));
        }

        protected override void Interact()
        {
            foreach (Vector3 spawnPos in botSpawnPositions)
            {
                GameObject bot = Instantiate(botPrefab, spawnPos, Quaternion.identity);
                bot.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }
        }
    }
}