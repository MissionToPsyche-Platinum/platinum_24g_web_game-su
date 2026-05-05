using UnityEngine;

public class FireSpot : MonoBehaviour
{
    private bool extinguished = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (extinguished)
        {
            return;
        }

        if (other.CompareTag("Smoke"))
        {
            extinguished = true;

            Debug.Log("Fire extinguished!");

            FireMinigameController controller = FindFirstObjectByType<FireMinigameController>();

            if (controller != null)
            {
                controller.FirePutOut();
            }

            gameObject.SetActive(false);
        }
    }
}