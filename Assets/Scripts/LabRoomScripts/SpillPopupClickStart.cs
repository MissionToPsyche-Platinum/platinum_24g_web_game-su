using UnityEngine;
using UnityEngine.SceneManagement;

public class SpillPopupClickStart : MonoBehaviour
{
    private bool readyToClick;

    private void OnEnable()
    {
        // Small delay so the click that opens the popup
        // doesn’t instantly start the minigame
        readyToClick = false;
        Invoke(nameof(EnableClick), 0.2f);
    }

    private void EnableClick()
    {
        readyToClick = true;
    }

    private void Update()
    {
        if (!readyToClick) return;

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Spill popup clicked → loading LabMinigame");
            SceneManager.LoadScene("LabMinigame");
        }
    }
}
