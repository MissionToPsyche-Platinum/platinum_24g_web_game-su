using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShipView : MonoBehaviour
{
    [SerializeField] private RectTransform shipRoot;
    [SerializeField] private RectTransform targetTrajectoryDotsRoot;
    [SerializeField] private RectTransform dottedTrajectoryRoot;

    [Header("Trajectory")]
    [SerializeField] private float travelDistancePixels = 410f;
    [SerializeField] private float targetAngleMinFromVertical = -30f;
    [SerializeField] private float targetAngleMaxFromVertical = 30f;
    [SerializeField] private float maxMissAngle = 22f;
    [SerializeField] private float targetDotSpacing = 16f;
    [SerializeField] private float targetDotSize = 6f;
    [SerializeField] private Color targetDotColor = new Color(0.25f, 0.9f, 1f, 0.85f);
    [SerializeField] private float dotSpacing = 16f;
    [SerializeField] private float dotSize = 6f;
    [SerializeField] private Color dotColor = new Color(1f, 0.85f, 0.2f, 0.9f);

    [Header("Animation")]
    [SerializeField] private float rotateDuration = 0.25f;
    [SerializeField] private float travelDuration = 1.2f;
    [SerializeField] private float holdAtEndSeconds = 0.2f;
    [SerializeField] private float minScore = 0f;

    private Vector2 startPos;
    private float startRotationZ;
    private bool playing;
    private float targetAngleFromVertical;
    private Vector2 stabilityJitter;

    private void Awake()
    {
        if (shipRoot != null)
        {
            startPos = shipRoot.anchoredPosition;
            startRotationZ = shipRoot.localEulerAngles.z;
        }

        GenerateTargetAngle();
        RefreshTargetLine();
        ClearDots(dottedTrajectoryRoot);
    }

    public void Bind(RectTransform shipRootRef, RectTransform targetTrajectoryDotsRootRef, RectTransform dottedTrajectoryRootRef)
    {
        shipRoot = shipRootRef;
        targetTrajectoryDotsRoot = targetTrajectoryDotsRootRef;
        dottedTrajectoryRoot = dottedTrajectoryRootRef;

        if (shipRoot != null)
        {
            startPos = shipRoot.anchoredPosition;
            startRotationZ = shipRoot.localEulerAngles.z;
        }

        GenerateTargetAngle();
        RefreshTargetLine();
        ClearDots(dottedTrajectoryRoot);
    }

    public void PlayCourseCorrection(int score)
    {
        if (playing || shipRoot == null)
        {
            return;
        }

        float accuracy01 = Mathf.InverseLerp(minScore, 100f, Mathf.Clamp(score, minScore, 100f));
        float missMagnitude = Mathf.Lerp(maxMissAngle, 0f, accuracy01);
        float missAngle = Random.value < 0.5f ? -missMagnitude : missMagnitude;

        float finalAngleFromVertical = targetAngleFromVertical + missAngle;
        Vector2 finalPoint = startPos + DirectionFromVertical(finalAngleFromVertical) * travelDistancePixels;

        RefreshTargetLine();
        Vector2 localStart = GetLocalPathPoint(dottedTrajectoryRoot, startPos);
        Vector2 localFinalPoint = GetLocalPathPoint(dottedTrajectoryRoot, finalPoint);
        BuildDottedPath(dottedTrajectoryRoot, localStart, localFinalPoint, dotSpacing, dotSize, dotColor, "TrajectoryDot");
        StartCoroutine(PlayCourseCorrectionRoutine(finalPoint));
    }

    public void SetInstability(float intensity01)
    {
        if (shipRoot == null)
        {
            return;
        }

        if (playing)
        {
            return;
        }

        float clamped = Mathf.Clamp01(intensity01);
        Vector2 nextJitter = Random.insideUnitCircle * (6f * clamped);
        stabilityJitter = Vector2.Lerp(stabilityJitter, nextJitter, 0.35f);
        shipRoot.anchoredPosition = startPos + stabilityJitter;

        if (!playing)
        {
            float jitterRotation = Mathf.Lerp(0f, 4f, clamped) * Mathf.Sin(Time.time * 18f);
            shipRoot.localEulerAngles = new Vector3(0f, 0f, startRotationZ + jitterRotation);
        }
    }

    private IEnumerator PlayCourseCorrectionRoutine(Vector2 finalPoint)
    {
        playing = true;
        stabilityJitter = Vector2.zero;
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
            elapsed += Time.deltaTime;
            yield return null;
        }

        shipRoot.anchoredPosition = finalPoint;
        yield return new WaitForSeconds(holdAtEndSeconds);
        playing = false;
    }

    private void GenerateTargetAngle()
    {
        targetAngleFromVertical = Random.Range(targetAngleMinFromVertical, targetAngleMaxFromVertical);
    }

    private void RefreshTargetLine()
    {
        if (targetTrajectoryDotsRoot == null)
        {
            return;
        }

        Vector2 localStart = GetLocalPathPoint(targetTrajectoryDotsRoot, startPos);
        Vector2 localTargetEnd = localStart + DirectionFromVertical(targetAngleFromVertical) * travelDistancePixels;
        BuildDottedPath(targetTrajectoryDotsRoot, localStart, localTargetEnd, targetDotSpacing, targetDotSize, targetDotColor, "TargetDot");
    }

    private void BuildDottedPath(RectTransform root, Vector2 from, Vector2 to, float spacing, float size, Color color, string dotName)
    {
        if (root == null)
        {
            return;
        }

        ClearDots(root);

        float distance = Vector2.Distance(from, to);
        int dotCount = Mathf.Clamp(Mathf.CeilToInt(distance / Mathf.Max(spacing, 1f)), 6, 36);
        for (int i = 0; i <= dotCount; i++)
        {
            float t = dotCount > 0 ? (float)i / dotCount : 0f;
            Vector2 pos = Vector2.Lerp(from, to, t);

            GameObject dotObj = new GameObject(dotName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            dotObj.transform.SetParent(root, false);

            RectTransform rect = dotObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(size, size);
            rect.anchoredPosition = pos;

            Image dot = dotObj.GetComponent<Image>();
            dot.color = color;
        }
    }

    private void ClearDots(RectTransform root)
    {
        if (root == null)
        {
            return;
        }

        for (int i = root.childCount - 1; i >= 0; i--)
        {
            Destroy(root.GetChild(i).gameObject);
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

    private static Vector2 GetLocalPathPoint(RectTransform root, Vector2 pointInParentSpace)
    {
        return root != null ? pointInParentSpace - root.anchoredPosition : pointInParentSpace;
    }

}
