using UnityEngine;

public class SpillClean : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        //this will detect the broom after it's attached to the player
        if (other.gameObject.name == "Broom")
        {
            gameObject.SetActive(false);
        }
    }
}
