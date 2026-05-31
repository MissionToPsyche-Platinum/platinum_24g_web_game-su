using UnityEngine;

public class WeldGunScript : MonoBehaviour
{
    public GameObject weldSpark;
    public GameObject helpPanel;

    // torch audio
    public AudioSource torchAudioSource;
    public AudioClip torchLoopSound;

    // Update is called once per frame
    void Update()
    {
        FollowCursor();

        if (Input.GetMouseButtonDown(0))
        {
            TriggerSpark();

            //play torch audio while using it
            if (torchAudioSource != null && torchLoopSound != null)
            {
                torchAudioSource.clip = torchLoopSound;
                torchAudioSource.loop = true;
                torchAudioSource.Play();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            TriggerSpark();

            if (torchAudioSource != null)
            {
                torchAudioSource.Stop();
            }
        }
    }

    private void FollowCursor()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        mousePosition.z = transform.position.z;

        transform.position = mousePosition - new Vector3(-1.5f, 1.5f);
    }

    public void TriggerSpark()
    {
        weldSpark.SetActive(!weldSpark.activeSelf);
        helpPanel.SetActive(false);
    }
}