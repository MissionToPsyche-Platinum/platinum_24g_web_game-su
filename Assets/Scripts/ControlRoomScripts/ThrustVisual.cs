using UnityEngine;
using UnityEngine.UI;

public class ThrustVisual : MonoBehaviour
{
    [SerializeField] private RectTransform needle;
    [SerializeField] private RectTransform bar;
    [SerializeField] private Image targetTick;

    [Header("Range")]
    [SerializeField] private float minValue = 0f;
    [SerializeField] private float maxValue = 100f;

    public void Bind(RectTransform barRef, RectTransform needleRef, Image targetTickRef, float min, float max)
    {
        bar = barRef;
        needle = needleRef;
        targetTick = targetTickRef;
        minValue = min;
        maxValue = max;
    }

    public void SetCurrent(float value)
    {
        if (bar == null || needle == null)
        {
            return;
        }

        float t = Mathf.InverseLerp(minValue, maxValue, value);
        float x = Mathf.Lerp(-bar.rect.width * 0.5f, bar.rect.width * 0.5f, t);
        needle.anchoredPosition = new Vector2(x, needle.anchoredPosition.y);
    }

    public void SetTarget(float value)
    {
        if (bar == null || targetTick == null)
        {
            return;
        }

        float t = Mathf.InverseLerp(minValue, maxValue, value);
        float x = Mathf.Lerp(-bar.rect.width * 0.5f, bar.rect.width * 0.5f, t);
        RectTransform rect = targetTick.rectTransform;
        rect.anchoredPosition = new Vector2(x, rect.anchoredPosition.y);
    }
}
