using UnityEngine;

public class SpillClean : MonoBehaviour
{
    private bool cleaned = false; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (cleaned) return;

        
        if (!other.CompareTag("Broom"))
            return;

        
        BroomPickup broomPickup = other.GetComponentInParent<BroomPickup>();
        if (broomPickup == null)
            return;

        
        if (!broomPickup.isHoldingBroom)
            return;

        cleaned = true;

        
        if (SpillManager.Instance != null)
            SpillManager.Instance.OnSpillCleaned();

        Destroy(gameObject);
    }
}