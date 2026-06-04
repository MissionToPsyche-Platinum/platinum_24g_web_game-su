using UnityEngine;

public class ComputerUIController : MonoBehaviour
{
    [SerializeField] private GameObject computerScreenPanel;

    public void OpenComputerScreen()
    {
        computerScreenPanel.SetActive(true);
        Global.anyPanelOpen = true;
    }

    public void CloseComputerScreen()
    {
        computerScreenPanel.SetActive(false);
        Global.anyPanelOpen = false;
    }
}
