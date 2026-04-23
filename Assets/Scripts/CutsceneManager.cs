using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    public Image image1;
    public Image image2;
    public Image image3;
    public Image image4;
    public TextMeshProUGUI cutsceneText;

    private List<Image> images;
    private List<string> script = new List<string>
    {
        "On October 13th, 2023 at 10:13am EDT, NASA launched the Psyche spacecraft from the Kennedy Space Center.",
        "The spacecraft is now enroute to the asteroid Psyche, where it will spend two years orbiting and sending data back to NASA.",
        "A lonely robot is left in charge of maintaining the spacecraft.",
        "Aid the robot and help the mission prevail...."
    };
    private int clickCount = 0;


    //private Animator animator;
    //public string animationName = "Cutscene";
    //public float targetFrame = 5f;
    //public float totalFrames = 15f;

    void Start()
    {
        images = new List<Image> { image1, image2, image3, image4 };
        cutsceneText.text = script[clickCount];
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (clickCount < images.Count - 1)
            {
                images[clickCount].CrossFadeAlpha(0f, 0.7f, false);
                clickCount++;
                cutsceneText.text = script[clickCount];
            }
            else if (clickCount == images.Count - 1)
            {
                images[clickCount].CrossFadeAlpha(0f, 0.7f, false);
                Global.tutorialShown = true;
                SceneManager.LoadScene("MainHall");
            }
        }
    }
}
