using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUpdater : MonoBehaviour
{

    public Slider progressSlider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        progressSlider.value = (float) Global.totalScore / Global.maxScore;
    }
}
