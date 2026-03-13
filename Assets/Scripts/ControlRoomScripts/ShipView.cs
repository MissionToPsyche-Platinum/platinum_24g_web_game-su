using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShipView : MonoBehaviour
{
    [SerializeField] private RectTransform shipRoot;
    [SerializeField] private Image glow;
    [SerializeField] private RectTransform targetTrajectoryLine;
    [SerializeField] private RectTransform dottedTrajectoryRoot;

    [Header("Trajectory")]
    [SerializeField] private float travelDistancePixels = 380f;
    [SerializeField] private float targetAngleMinFromVertical = -30f;
    [SerializeField] private float targetAngleMaxFromVertical = 30f;
    [SerializeField] private float maxMissAngle = 22f;
    [SerializeField] private float targetLineThickness = 6f;
    [SerializeField] private float dotSpacing = 16f;
    [SerializeField] private float dotSize = 6f;
    [SerializeField] private Color dotColor = new Color(1f, 0.85f, 0.2f, 0.9f);

    [Header("Animation")]
    [SerializeField] private float rotateDuration = 0.25f;
    [SerializeField] private float travelDuration = 1.2f;
    [SerializeField] private float glowPeak = 0.8f;
    [SerializeField] private float holdAtEndSeconds = 0.2f;
    [SerializeField] private float minScore = 0f;

    private Vector2 startPos;
    private float startRotationZ;
    private bool playing;
    private int missDirection = 1;
    private float targetAngleFromVertical;

    private void Awake()
    {
        if (shipRoot != null)
        {
            startPos = shipRoot.anchoredPosition;
            startRotationZ = shipRoot.localEulerAngles.z;
        }

        if (glow != null)
        {
            Color c = glow.color;
            glow.color = new Color(c.r, c.g, c.b, 0f);
        }

        GenerateTargetAngle();
        RefreshTargetLine();
        ClearDottedTrajectory();
    }

    public void Bind(RectTransform shipRootRef, Image glowRef, RectTransform targetTrajectoryLineRef, RectTransform dottedTrajectoryRootRef)
    {
        shipRoot = shipRootRef;
        glow = glowRef;
        targetTrajectoryLine = targetTrajectoryLineRef;
        dottedTrajectoryRoot = dottedTrajectoryRootRef;

        if (shipRoot != null)
        {
            startPos = shipRoot.anchoredPosition;
            startRotationZ = shipRoot.localEulerAngles.z;
        }

        GenerateTargetAngle();
        RefreshTargetLine();
        ClearDottedTrajectory();
    }

    public void PlayCourseCorrection(int score)
    {
        if (playing || shipRoot == null)
        {
            return;
        }

        float accuracy01 = Mathf.InverseLerp(minScore, 100f, Mathf.Clamp(score, minScore, 100f));
        float missAngle = Mathf.Lerp(maxMissAngle, 0f, accuracy01) * missDirection;
        missDirection *= -1;

        float finalAngleFromVertical = targetAngleFromVertical + missAngle;
        Vector2 finalPoint = startPos + DirectionFromVertical(finalAngleFromVertical) * travelDistancePixels;

        RefreshTargetLine();
        BuildDottedTrajectory(startPos, finalPoint);
        StartCoroutine(PlayCourseCorrectionRoutine(finalPoint));
    }

    private IEnumerator PlayCourseCorrectionRoutine(Vector2 finalPoint)
    {
        playing = true;
        shipRoot.anchoredPosition = startPos;
        shipRoot.localEulerAngles = new Vector3(0f, 0f, startRotationZ);

        float targetRotationZ = RotationForDirection(finalPoint - startPos);
        float elapsed = 0f;
        while (elapsed < rotateDuration)
        {
            float t = Mathf.Clamp01(elapsed / Mathf.Max(rotateDuration, 0.01f));
            float eased = Mathf.SmoothStep(0f, 1f, t);
            float z = Mathf.LerpAngle(startRotationZ, targetRotationZ, eased);
            shipRoot.localEulerAngles = new Vector3(0f, 0f, z);
            ApplyGlow(Mathf.Lerp(0.15f, 0.5f, eased));
            elapsed += Time.deltaTime;
            yield return null;
        }

        shipRoot.localEulerAngles = new Vector3(0f, 0f, targetRotationZ);
        elapsed = 0f;
        while (elapsed < travelDuration)
        {
            float t = Mathf.Clamp01(elapsed / Mathf.Max(travelDuration, 0.01f));
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            shipRoot.anchoredPosition = Vector2.Lerp(startPos, finalPoint, eased);
            ApplyGlow(Mathf.Lerp(0.35f, glowPeak, eased));
            elapsed += Time.deltaTime;
            yield return null;
        }

        shipRoot.anchoredPosition = finalPoint;
        yield return new WaitForSeconds(holdAtEndSeconds);
        ApplyGlow(0f);
        playing = false;
    }

    private void GenerateTargetAngle()
    {
        targetAngleFromVertical = Random.Range(targetAngleMinFromVertical, targetAngleMaxFromVertical);
    }

    private void RefreshTargetLine()
    {
        if (targetTrajectoryLine == null)
        {
            return;
        }

        targetTrajectoryLine.anchoredPosition = startPos;
        targetTrajectoryLine.localEulerAngles = new Vector3(0f, 0f, 90f - targetAngleFromVertical);
        targetTrajectoryLine.sizeDelta = new Vector2(travelDistancePixels, targetLineThickness);
    }

    private void BuildDottedTrajectory(Vector2 from, Vector2 to)
    {
        if (dottedTrajectoryRoot == null)
        {
            return;
        }

        ClearDottedTrajectory();

        float distance = Vector2.Distance(from, to);
        int dotCount = Mathf.Clamp(Mathf.CeilToInt(distance / Mathf.Max(dotSpacing, 1f)), 6, 36);
        for (int i = 0; i <= dotCount; i++)
        {
            float t = dotCount > 0 ? (float)i / dotCount : 0f;
            Vector2 pos = Vector2.Lerp(from, to, t);

            GameObject dotObj = new GameObject("TrajectoryDot", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            dotObj.transform.SetParent(dottedTrajectoryRoot, false);

            RectTransform rect = dotObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(dotSize, dotSize);
            rect.anchoredPosition = pos;

            Image dot = dotObj.GetComponent<Image>();
            dot.color = dotColor;
        }
    }

    private void ClearDottedTrajectory()
    {
        if (dottedTrajectoryRoot == null)
        {
            return;
        }

        for (int i = dottedTrajectoryRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(dottedTrajectoryRoot.GetChild(i).gameObject);
        }
    }

    private static Vector2 DirectionFromVertical(float angleFromVertical)
    {
        float radians = angleFromVertical * Mathf.Deg2Rad;
        return new Vector2(Mathf.Sin(radians), Mathf.Cos(radians)).normalized;
    }

    private float RotationForDirection(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.0001f)
        {
            return startRotationZ;
        }

        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
    }

    private void ApplyGlow(float alpha)
    {
        if (glow == null)
        {
            return;
        }

        Color c = glow.color;
        glow.color = new Color(c.r, c.g, c.b, Mathf.Clamp01(alpha));
    }
}
