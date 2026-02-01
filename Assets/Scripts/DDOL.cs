using UnityEngine;

public class DDOL : MonoBehaviour
{
    public static DDOL instance;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}
