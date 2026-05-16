using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RestarterScript : MonoBehaviour
{

    public GameObject helpPanel;
    public GameObject hint;
    private GameObject player;
    private bool canInteract;

    [SerializeField] private AudioClip blipSoundClip;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if(player == null)
        {
            Debug.LogError("RestarterScript: Player GameObject with tag 'Player' not found in the scene.");
            return;
        }
        helpPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !helpPanel.activeSelf && canInteract)
        {
            if (SoundFXManager.instance != null)
            {
                SoundFXManager.instance.PlaySoundFXClip(blipSoundClip, transform, 1f);
            }
            helpPanel.SetActive(true);
            player.GetComponent<PlayerMovement2D>().enabled = false;
            player.GetComponent<Animator>().SetBool("IsMoving", false);
            ToggleHint(false);
        }

        if (Input.GetKeyDown(KeyCode.Escape) && helpPanel.activeSelf)
        {
            ExitPanel();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hint == null)
        {
            Debug.LogError("RestarterScript: hint is NOT assigned in Inspector.");
            return; ;
        }
        canInteract = true;

        ToggleHint(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        canInteract = false;
        ToggleHint(false);
    }

    private void ToggleHint(bool isVisible)
    {
        if (hint != null)
        {
            hint.SetActive(isVisible);
        }
    }

    public void RestartMinigame()
    {
        player.GetComponent<PlayerMovement2D>().enabled = true;
        string sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }

    public void ExitPanel()
    {
        if (SoundFXManager.instance != null)
        {
            SoundFXManager.instance.PlaySoundFXClip(blipSoundClip, transform, 1f);
        }
        helpPanel.SetActive(false);
        player.GetComponent<PlayerMovement2D>().enabled = true;

        ToggleHint(true);
    }
}
