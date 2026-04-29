using UnityEngine;
using UnityEngine.UI;

public class PowerCell : MonoBehaviour
{
    public RectTransform indicator; 
    public RectTransform targetZone; 
    public float speed; 
    
    [HideInInspector]
    public bool isCalibrated = false;
    [SerializeField] private AudioClip callibratedSoundClip;
    private float rightLimit = 800f; 
    private float leftLimit = 10f; 
    private bool movingRight = true;

    void Start()
    {
        speed = 400f * Global.round;
    }

    void Update()
    {
        // Stop moving if the player won this bar
        if (isCalibrated) return;

        // Move back and forth
        float move = movingRight ? speed : -speed;
        indicator.anchoredPosition += new Vector2(move * Time.deltaTime, 0);

        // Bounce off the edges of the bar
        if (indicator.anchoredPosition.x >= rightLimit) movingRight = false;
        if (indicator.anchoredPosition.x <= leftLimit) movingRight = true;
    }

    public void AttemptCalibration()
    {
        if (indicator.anchoredPosition.x <= 420 && indicator.anchoredPosition.x >= 380) 
        {
            isCalibrated = true;
            if (SoundFXManager.instance != null)
            {
                SoundFXManager.instance.PlaySoundFXClip(callibratedSoundClip, transform, 1f);
            }
            targetZone.GetComponent<Image>().color = Color.green; // Feedback!
            Debug.Log(gameObject.name + " Calibrated!");
        }
    }
}