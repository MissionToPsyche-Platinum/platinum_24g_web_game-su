using UnityEngine;
using UnityEngine.UI;

public class BurnVisual : MonoBehaviour
{
    [SerializeField] private Text targetText;
    [SerializeField] private Text currentText;
    [SerializeField] private Image fill;

    [Header("Range")]
    [SerializeField] private float minSeconds = 0f;
    [SerializeField] private float maxSeconds = 6f;

    public void Bind(Text targetTextRef, Text currentTextRef, Image fillRef, float min, float max)
    {
        targetText = targetTextRef;
        currentText = currentTextRef;
        fill = fillRef;
        minSeconds = min;
        maxSeconds = max;
    }

    public void SetTarget(float seconds)
    {
        if (targetText != null)
        {
            targetText.text = $"Window Target: {seconds:0.00}s";
        }
    }

    public void SetCurrent(float seconds)
    {
        if (currentText != null)
        {
            currentText.text = $"Window Set: {seconds:0.00}s";
        }

        if (fill != null)
        {
            float t = Mathf.InverseLerp(minSeconds, maxSeconds, seconds);
            fill.fillAmount = Mathf.Clamp01(t);
        }
    }
}
