using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUpdater : MonoBehaviour
{

    public Slider progressSlider;
    private float progress;


    // Update is called once per frame
    void Update()
    {
        progress = (float)Global.totalScore / Global.maxScore;
        progressSlider.value = progress >= 0 ? progress : 0;
    }
}
