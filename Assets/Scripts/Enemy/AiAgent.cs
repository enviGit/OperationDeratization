using Random = UnityEngine.Random;
using RatGamesStudios.OperationDeratization.Enemy.State;
using RatGamesStudios.OperationDeratization.RagdollPhysics;
using System;
using System.Collections;
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
        [SerializeField] private AudioSource talkSound;
        [SerializeField] private AudioClip[] searchStateClips = new AudioClip[5];
        [SerializeField] private AudioClip[] attackStateClips = new AudioClip[5];
        private Dictionary<AiStateId, AudioClip[]> stateClipMap = new Dictionary<AiStateId, AudioClip[]>();
        private float lastPlayTime = 0f;

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
            stateMachine.RegisterState(new AiInvestigateSoundState());
            stateMachine.ChangeState(initialState);
            stateClipMap[AiStateId.FindTarget] = searchStateClips;
            stateClipMap[AiStateId.InvestigateSound] = searchStateClips;
            stateClipMap[AiStateId.AttackTarget] = attackStateClips;
        }
        private void Update()
        {
            stateMachine.Update();
            currentState = stateMachine.currentState;
        }
        public void CheckAndPlayRandomClip()
        {
            if (!talkSound.isPlaying && stateClipMap.ContainsKey(currentState)) // Check if the talkSound audio source is not currently playing any sound
            {
                float timeSinceLastPlay = Time.time - lastPlayTime;
                float minTimeBetweenPlays = (currentState == AiStateId.FindTarget || currentState == AiStateId.InvestigateSound) ? 25f : 5f;

                if (timeSinceLastPlay >= minTimeBetweenPlays)
                {
                    StartCoroutine(PlayRandomClipCoroutine(stateClipMap[currentState]));
                    lastPlayTime = Time.time;
                }
            }
        }
        private IEnumerator PlayRandomClipCoroutine(AudioClip[] clipsToUse)
        {
            yield return new WaitForSeconds(Random.Range(0f, 0.5f));

            AudioClip randomClip = clipsToUse[Random.Range(0, clipsToUse.Length)];
            talkSound.clip = randomClip;
            talkSound.Play();
        }
        private void OnDrawGizmos()
        {
            if (locations != null)
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