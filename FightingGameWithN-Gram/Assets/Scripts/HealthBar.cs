using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private GameObject[] hearts; // Assign 3 heart GameObjects in Inspector

    public void SetHealth(int currentHealth)
    {
        // Validate input
        currentHealth = Mathf.Clamp(currentHealth, 0, hearts.Length);

        // Update hearts visibility
        for (int i = 0; i < hearts.Length; i++)
        {
            // Activate heart if its index is less than current health
            hearts[i].SetActive(i < currentHealth);
        }
    }
}
