using System.Collections;
using UnityEngine;

public class AiAttackTargetState : AiState
{
    public AiStateId GetId()
    {
        return AiStateId.AttackTarget;
    }
    public void Enter(AiAgent agent)
    {
        agent.weapons.ActiveWeapon();
        agent.navMeshAgent.stoppingDistance = agent.config.attackStoppingDistance;
        agent.navMeshAgent.speed = agent.config.attackSpeed;
    }
    public void Update(AiAgent agent)
    {
        if (!agent.targeting.HasTarget)
        {
            agent.stateMachine.ChangeState(AiStateId.FindTarget);

            return;
        }
            
        agent.weapons.SetTarget(agent.targeting.Target.transform);
        agent.navMeshAgent.destination = agent.targeting.TargetPosition;
        UpdateFiring(agent);
        UpdateLowHealth(agent);
        UpdateLowAmmo(agent);
    }
    private void UpdateFiring(AiAgent agent)
    {
        if(agent.targeting.TargetInSight)
            agent.weapons.SetFiring(true);
        else
            agent.weapons.SetFiring(false);
    }
    /*private void ReloadWeapon(AiAgent agent)
    {
        var weapon = agent.weapons.currentWeapon;

        if(weapon && weapon.GetComponent<Weapon>().gun.currentAmmoCount <= 0)
            StartCoroutine(agent.weapons.ReloadCoroutine); 
    }
    private void SelectWeapon(AiAgent agent)
    {
        var bestWeapon = ChooseWeapon(agent);

        if (bestWeapon != agent.weapons.currentWeaponSlot)
            agent.weapons.SwitchWeapon(bestWeapon);
    }
    AiWeapons.WeaponSlot ChooseWeapon(AiAgent agent)
    {
        float distance = agent.targeting.TargetDistance;

        if (distance > agent.config.attackCloseRange)
            return AiWeapons.WeaponSlot.Primary;
        else
            return AiWeapons.WeaponSlotSecondary;
    }*/
    private void UpdateLowHealth(AiAgent agent)
    {
        if (agent.health.IsLowHealth())
            agent.stateMachine.ChangeState(AiStateId.FindFirstAidKit);
    }
    private void UpdateLowAmmo(AiAgent agent)
    {
        if (agent.weapons.IsLowAmmo())
            agent.stateMachine.ChangeState(AiStateId.FindAmmo);
    }
    public void Exit(AiAgent agent)
    {
        agent.weapons.DeactiveWeapon();
        agent.navMeshAgent.stoppingDistance = 0;
    }
}
