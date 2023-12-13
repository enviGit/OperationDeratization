using UnityEngine;
using UnityEngine.AI;

public class AiAgent : MonoBehaviour
{
    public AiStateMachine stateMachine;
    public AiStateId initialState;
    [HideInInspector] public NavMeshAgent navMeshAgent;
    public AiAgentConfig config;
    [HideInInspector] public Ragdoll ragdoll;
    [HideInInspector] public EnemyHealth healthBar;
    public Transform playerTransform;
    [HideInInspector] public AiWeapons weapons;
    [HideInInspector] public AiSightSensor sightSensor;

    private void Start()
    {
        ragdoll = GetComponent<Ragdoll>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        weapons = GetComponent<AiWeapons>();
        healthBar = GetComponent<EnemyHealth>();
        sightSensor = GetComponent<AiSightSensor>();
        stateMachine = new AiStateMachine(this);
        stateMachine.RegisterState(new AiChasePlayerState());
        stateMachine.RegisterState(new AiDeathState());
        stateMachine.RegisterState(new AiIdleState());
        stateMachine.RegisterState(new AiFindWeaponState());
        stateMachine.RegisterState(new AiAttackPlayerState());
        stateMachine.ChangeState(initialState);
    }
    private void Update()
    {
        stateMachine.Update();
    }
}
