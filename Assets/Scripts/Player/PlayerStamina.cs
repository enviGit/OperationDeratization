using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    [Header("Stamina bar")]
    public float maxStamina = 100f;
    public float sprintStaminaCost = 5f;
    public float jumpStaminaCost = 15f;
    public float attackStaminaCost = 15f;
    public float staminaRegenRate = 25f;
    public float currentStamina = 100f;
    public Image staminaBarFill;
    public bool isStaminaRegenBlocked = false;

    private void Start()
    {
        currentStamina = maxStamina;
        staminaBarFill.transform.parent.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (GetComponent<PlayerMotor>().isRunning && GetComponent<PlayerMotor>().isMoving)
            UseStamina(sprintStaminaCost * Time.deltaTime);
        if (!GetComponent<PlayerMotor>().isRunning && GetComponent<PlayerMotor>().isGrounded)
        {
            if (!isStaminaRegenBlocked)
                currentStamina = Mathf.Clamp(currentStamina + staminaRegenRate * Time.deltaTime, 0, maxStamina);
        }

        UpdateStaminaUI();
    }
    private void UpdateStaminaUI()
    {
        if (currentStamina == maxStamina)
            staminaBarFill.transform.parent.gameObject.SetActive(false);
        else
        {
            staminaBarFill.transform.parent.gameObject.SetActive(true);
            staminaBarFill.fillAmount = currentStamina / maxStamina;
        }
    }
    public bool HasStamina(float amount)
    {
        return currentStamina >= amount;
    }
    public void UseStamina(float amount)
    {
        currentStamina = Mathf.Clamp(currentStamina - amount, 0, maxStamina);
        UpdateStaminaUI();

        if (currentStamina == 0)
        {
            isStaminaRegenBlocked = true;
            Invoke("UnblockStaminaRegen", 4f);
        }
    }
    private void UnblockStaminaRegen()
    {
        isStaminaRegenBlocked = false;
    }
    public void BlockStaminaOnAttack()
    {
        isStaminaRegenBlocked = true;

        if (currentStamina == 0)
            Invoke("UnblockStaminaRegen", 4f);
        else
            Invoke("UnblockStaminaRegen", 2f);
    }
}
