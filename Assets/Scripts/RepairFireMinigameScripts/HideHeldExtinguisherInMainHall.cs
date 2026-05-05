using UnityEngine;
using UnityEngine.SceneManagement;

public class HideHeldExtinguisherInMainHall : MonoBehaviour
{
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "MainHall")
        {
            GameObject held = GameObject.Find("HeldExtinguisher");

            if (held != null)
            {
                SpriteRenderer sr = held.GetComponent<SpriteRenderer>();

                if (sr != null)
                {
                    sr.enabled = false;
                }
            }
        }
    }
}