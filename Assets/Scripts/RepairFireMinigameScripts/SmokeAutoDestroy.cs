using UnityEngine;

public class SmokeAutoDestroy : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 0.5f);
    }
}