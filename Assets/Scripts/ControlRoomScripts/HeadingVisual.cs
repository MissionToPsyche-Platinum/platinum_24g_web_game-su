using UnityEngine;
using UnityEngine.UI;

public class HeadingVisual : MonoBehaviour
{
    [SerializeField] private RectTransform rotatingLine;
    [SerializeField] private RectTransform shipMarker;
    [SerializeField] private Image targetBand;

    [Header("Line")]
    [SerializeField] private float lineLength = 140f;

    public void Bind(RectTransform line, RectTransform marker, Image band)
    {
        rotatingLine = line;
        shipMarker = marker;
        targetBand = band;
        UpdateMarkerPosition();
    }

    public void SetAngle(float degrees)
    {
        if (rotatingLine != null)
        {
            rotatingLine.localEulerAngles = new Vector3(0f, 0f, degrees);
        }
    }

    public void SetTargetBand(float centerDegrees, float halfWidthDegrees)
    {
        if (targetBand == null)
        {
            return;
        }

        targetBand.transform.localEulerAngles = new Vector3(0f, 0f, centerDegrees);
        targetBand.fillAmount = Mathf.Clamp01(halfWidthDegrees / 180f);
    }

    private void UpdateMarkerPosition()
    {
        if (shipMarker == null)
        {
            return;
        }

        shipMarker.anchoredPosition = new Vector2(lineLength, 0f);
    }
}
