using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractableAssetControl : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if(Global.currentRoom == SceneManager.GetActiveScene().name)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
