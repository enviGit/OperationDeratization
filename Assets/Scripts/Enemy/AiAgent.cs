using UnityEngine;
using UnityEngine.AI;

public class AiAgent : MonoBehaviour
{
    [HideInInspector] public AiStateMachine stateMachine;
    public AiStateId initialState;
    [HideInInspector] public NavMeshAgent navMeshAgent;
    public AiAgentConfig config;
    [HideInInspector] public Ragdoll ragdoll;
    [HideInInspector] public EnemyHealth healthBar;
    public Transform playerTransform;
    [HideInInspector] public AiWeapons weapons;
    [HideInInspector] public AiSightSensor sightSensor;
    [HideInInspector] public AiTargetingSystem targeting;

    private void Start()
    {
        ragdoll = GetComponent<Ragdoll>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        weapons = GetComponent<AiWeapons>();
        healthBar = GetComponent<EnemyHealth>();
        sightSensor = GetComponent<AiSightSensor>();
        targeting = GetComponent<AiTargetingSystem>();
        stateMachine = new AiStateMachine(this);
        stateMachine.RegisterState(new AiChasePlayerState());
        stateMachine.RegisterState(new AiDeathState());
        stateMachine.RegisterState(new AiIdleState());
        stateMachine.RegisterState(new AiFindWeaponState());
        stateMachine.RegisterState(new AiAttackTargetState());
        stateMachine.RegisterState(new AiFindTargetState());
        stateMachine.ChangeState(initialState);
    }
    private void Update()
    {
        stateMachine.Update();
    }
}
