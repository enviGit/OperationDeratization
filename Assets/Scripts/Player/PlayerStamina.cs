using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    public float maxStamina = 100f;
    public float sprintStaminaCost = 10f;
    public float jumpStaminaCost = 30f;
    //public float attackStaminaCost = 30f;
    public float staminaRegenRate = 20f;
    public Image staminaBarFill;
    public float currentStamina;
    private bool isStaminaRegenBlocked = false;
    //private float attackTimer = 0f;

    private void Start()
    {
        currentStamina = maxStamina;
        staminaBarFill.transform.parent.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (GetComponent<PlayerMotor>().isRunning)
            UseStamina(sprintStaminaCost * Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.Space) && GetComponent<PlayerMotor>().isGrounded && HasStamina(jumpStaminaCost / 2))
            UseStamina(jumpStaminaCost);
        /*if (Input.GetMouseButton(0) && Time.time > attackTimer && GetComponent<PlayerInventory>().CurrentWeapon.gunStyle == GunStyle.Melee && HasStamina(attackStaminaCost / 2))
        {
            attackTimer = Time.time + GetComponent<PlayerInventory>().CurrentWeapon.timeBetweenShots;
            UseStamina(attackStaminaCost);
        }*/
        if (!GetComponent<PlayerMotor>().isRunning && GetComponent<PlayerMotor>().isGrounded && !Input.GetKeyDown(KeyCode.Space))
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
}
