using UnityEngine;

public class WinSceneController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip openSound;

    private void Start()
    {
        if (audioSource != null && openSound != null)
            audioSource.PlayOneShot(openSound, 0.3f);
    }
}
