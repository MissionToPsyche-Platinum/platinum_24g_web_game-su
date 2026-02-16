using UnityEngine;

public class SpillClean : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("SPILL HIT by: " + other.name);

        //only allow the broom hitbox to clean
        if (other.CompareTag("Broom") == false)
        {
            Debug.Log("Not broom tag");
            return;
        }


        BroomPickup broomPickup = other.GetComponentInParent<BroomPickup>();
        if (broomPickup == null)
        {
            Debug.Log("No BroomPickup found on parent");
            return;
        }

        Debug.Log("BroomPickup found. isHoldingBroom = " + broomPickup.isHoldingBroom);

        if (broomPickup.isHoldingBroom == true)
        {
            Debug.Log("Cleaning spill!");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Broom not active yet");
        }
    }
}
