using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Enemy
{
    [ExecuteInEditMode]
    public class AiTargetingSystem : MonoBehaviour
    {
        public float memorySpan = 3f;
        public float distanceWeight = 2f;
        public float angleWeight = 1f;
        public float ageWeight = 1f;
        public bool HasTarget
        {
            get
            {
                return bestMemory != null;
            }
        }
        public GameObject Target
        {
            get
            {
                return bestMemory.gameObject;
            }
        }
        public Vector3 TargetPosition
        {
            get
            {
                return bestMemory.gameObject.transform.position;
            }
        }
        public bool TargetInSight
        {
            get
            {
                return bestMemory.Age < 0.5f;
            }
        }
        public float TargetDistance
        {
            get
            {
                return bestMemory.distance;
            }
        }
        private AiSensoryMemory memory = new AiSensoryMemory(10);
        private AiSightSensor sensor;
        private AiMemory bestMemory;

        private void Start()
        {
            sensor = GetComponent<AiSightSensor>();
        }
        private void Update()
        {
            memory.UpdateSenses(sensor);
            memory.ForgetMemories(memorySpan);
            EvaluateScores();
        }
        private void EvaluateScores()
        {
            bestMemory = null;

            foreach (var memory in memory.memories)
            {
                memory.score = CalculateScore(memory);

                if (bestMemory == null || memory.score > bestMemory.score)
                    bestMemory = memory;
            }
        }
        private float Normalize(float value, float maxValue)
        {
            return 1f - (value / maxValue);
        }
        private float CalculateScore(AiMemory memory)
        {
            float distanceScore = Normalize(memory.distance, sensor.distance) * distanceWeight;
            float angleScore = Normalize(memory.angle, sensor.angle) * angleWeight;
            float ageScore = Normalize(memory.Age, memorySpan) * ageWeight;

            return distanceScore + angleScore + ageScore;
        }
        private void OnDrawGizmos()
        {
            float maxScore = float.MinValue;

            foreach (var memory in memory.memories)
                maxScore = Mathf.Max(maxScore, memory.score);
            foreach (var memory in memory.memories)
            {
                Color color = Color.red;

                if (memory == bestMemory)
                    color = Color.yellow;

                color.a = memory.score / maxScore;
                Gizmos.color = color;
                Gizmos.DrawSphere(memory.position, 0.3f);
            }
        }
    }
}