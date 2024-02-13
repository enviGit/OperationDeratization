using RatGamesStudios.OperationDeratization.Enemy.State;
using RatGamesStudios.OperationDeratization.RagdollPhysics;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RatGamesStudios.OperationDeratization.Enemy
{
    [Serializable]
    public class CriticalLocations
    {
        public string locationName;
        public Transform location;
        public float radius;
    }
    public class AiAgent : MonoBehaviour
    {
        [HideInInspector] public AiStateMachine stateMachine;
        public AiStateId initialState;
        public AiStateId currentState;
        [HideInInspector] public NavMeshAgent navMeshAgent;
        public AiAgentConfig config;
        [HideInInspector] public Ragdoll ragdoll;
        [HideInInspector] public EnemyHealth healthBar;
        [HideInInspector] public AiWeapons weapons;
        [HideInInspector] public AiSightSensor sightSensor;
        [HideInInspector] public AiTargetingSystem targeting;
        [HideInInspector] public EnemyHealth health;
        [HideInInspector] public AiAudioSensor audioSensor;
        public List<CriticalLocations> locations = new List<CriticalLocations>();

        private void Start()
        {
            ragdoll = GetComponent<Ragdoll>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            weapons = GetComponent<AiWeapons>();
            healthBar = GetComponent<EnemyHealth>();
            sightSensor = GetComponent<AiSightSensor>();
            targeting = GetComponent<AiTargetingSystem>();
            health = GetComponent<EnemyHealth>();
            audioSensor = GetComponent<AiAudioSensor>();
            stateMachine = new AiStateMachine(this);
            stateMachine.RegisterState(new AiDeathState());
            stateMachine.RegisterState(new AiIdleState());
            stateMachine.RegisterState(new AiFindWeaponState());
            stateMachine.RegisterState(new AiAttackTargetState());
            stateMachine.RegisterState(new AiFindTargetState());
            stateMachine.RegisterState(new AiFindFirstAidKitState());
            stateMachine.RegisterState(new AiFindAmmoState());
            stateMachine.RegisterState(new AiPatrolState());
            stateMachine.ChangeState(initialState);
        }
        private void Update()
        {
            stateMachine.Update();
            currentState = stateMachine.currentState;
        }
        private void OnDrawGizmos()
        {
            if(locations != null)
            {
                foreach (var location in locations)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireSphere(location.location.position, location.radius);
                }
            }
        }
    }
}