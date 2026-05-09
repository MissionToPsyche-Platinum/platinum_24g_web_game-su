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
        "On October 13th, 2023 at 10:13am EDT, NASA launched the Psyche spacecraft from the Kennedy Space Center to study the asteroid Psyche.",
        "One hundred years later, NASA has sent the Psyche II spacecraft to return to Psyche in order to learn more about its metal-rich properties.",
        "A lonely robot is left in charge of maintaining the spacecraft.",
        "Aid the robot and help the mission prevail...."
    };
    private int clickCount = 0;

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
