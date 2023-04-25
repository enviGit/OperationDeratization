using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    public float maxStamina = 100f;
    public float sprintStaminaCost = 10f;
    public float jumpStaminaCost = 30f;
    public float staminaRegenRate = 10f;
    public Image staminaBarFill;
    public float currentStamina;

    private void Start()
    {
        currentStamina = maxStamina;
        UpdateStaminaUI();
    }

    private void Update()
    {
        if (GetComponent<PlayerMotor>().isRunning && HasStamina(sprintStaminaCost))
            currentStamina = Mathf.Clamp(currentStamina - sprintStaminaCost * Time.deltaTime, 0f, maxStamina);
        else if(Input.GetKeyDown(KeyCode.Space) && GetComponent<PlayerMotor>().isGrounded && HasStamina(jumpStaminaCost))
            currentStamina = Mathf.Clamp(currentStamina - jumpStaminaCost, 0f, maxStamina);
        else
            currentStamina = Mathf.Clamp(currentStamina + staminaRegenRate * Time.deltaTime, 0f, maxStamina);

        UpdateStaminaUI();
    }
    private void UpdateStaminaUI()
    {
        staminaBarFill.fillAmount = currentStamina / maxStamina;
    }
    public bool HasStamina(float amount)
    {
        if (currentStamina >= amount)
            return true;
        else
            return false;
    }
    public void UseStamina(float amount)
    {
        currentStamina = Mathf.Clamp(currentStamina - amount, 0f, maxStamina);
        UpdateStaminaUI();
    }

}
