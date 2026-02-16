using UnityEngine;
using UnityEngine.SceneManagement;

public class SpillManager : MonoBehaviour
{
    public static SpillManager Instance;

    public GameObject gameOverPanel;
    public string labRoomSceneName = "LabRoom";

    private int spillsRemaining;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        spillsRemaining = FindObjectsByType<SpillClean>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        ).Length;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
    }
    }


    public void OnSpillCleaned()
    {
        spillsRemaining -= 1;

        if (spillsRemaining <= 0)
        {
            gameOverPanel.SetActive(true);
        }
    }


    public void ReturnToLabRoom()
    {
        SceneManager.LoadScene(labRoomSceneName);
    }
}
