using UnityEngine;
using UnityEngine.UI;

public class PowerCell : MonoBehaviour
{
    public RectTransform indicator; 
    public RectTransform targetZone; 
    public float speed = 400f; 
    
    [HideInInspector]
    public bool isCalibrated = false;

    private float rightLimit = 800f; 
    private float leftLimit = 10f; 
    private bool movingRight = true;

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
        if (indicator.anchoredPosition.x <= 450 && indicator.anchoredPosition.x >= 400) 
        {
            isCalibrated = true;
            targetZone.GetComponent<Image>().color = Color.green; // Feedback!
            Debug.Log(gameObject.name + " Calibrated!");
        }
    }
}