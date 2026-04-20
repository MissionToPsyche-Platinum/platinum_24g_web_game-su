using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ControlsMinigameController : MonoBehaviour
{
    private struct ScoreResult
    {
        public int distance;
        public int score;
        public int stars;
    }

    private enum GameState
    {
        Aligning,
        CourseCorrection,
        Completed
    }

    private enum ControlMode
    {
        Heading,
        Thrust,
        CorrectionWindow
    }

    [Header("Target")]
    [SerializeField] private ControlsTargetGenerator targetGenerator;
    [SerializeField] private HeadingVisual headingVisual;
    [SerializeField] private ThrustVisual thrustVisual;
    [SerializeField] private CorrectionWindowVisual correctionWindowVisual;

    [Header("UI")]
    [SerializeField] private Text modeText;
    [SerializeField] private Text stabilityHintText;
    [SerializeField] private Text stabilityValueText;
    [SerializeField] private Image stabilityFill;
    [SerializeField] private Image stabilityPanel;
    [SerializeField] private MiniGameResultsPopup factCardPopup;
    [SerializeField] private ShipView shipView;

    [Header("Course Correction")]
    [SerializeField] private float courseCorrectionDelaySeconds = 1.5f;

    [Header("Ranges")]
    [SerializeField] private float headingMin = -70f;
    [SerializeField] private float headingMax = 70;
    [SerializeField] private float thrustMin = 0f;
    [SerializeField] private float thrustMax = 100f;
    [SerializeField] private float correctionWindowMin = 1f;
    [SerializeField] private float correctionWindowMax = 4f;

    private GameState state = GameState.Aligning;
    private ControlMode mode = ControlMode.Heading;

    private float headingAngle;
    private float thrustValue;
    private float headingPhase;
    private float thrustPhase;
    private float correctionWindowTimer;
    private bool correctionWindowTiming;
    private float headingPauseUntil;
    private float thrustPauseUntil;
    private bool waitingForRelease;

    [Header("Oscillation")]
    [SerializeField] private float headingSpeed = 0.6f;
    [SerializeField] private float thrustSpeed = 0.5f;
    [SerializeField] private float missPauseSeconds = 0.2f;

    [Header("Stability")]
    [SerializeField] private float stabilityMax = 100f;
    [SerializeField] private float startingStability = 100f;
    [SerializeField] private float stabilityDrainPerSecond = 14f;
    [SerializeField] private float stabilityRecoverPerTap = 10f;
    [SerializeField] private float tapCooldown = 0.2f;
    //[SerializeField] private float lowStabilityThreshold = 35f;
    [SerializeField] private float criticalStabilityThreshold = 15f;
    [SerializeField] private float maxOscillationMultiplier = 3.5f;
    [SerializeField] private float stabilityScorePenaltyWeight = 0.18f;
    [SerializeField] private float stabilityWarningThreshold = 80f;
    [SerializeField] private float stabilityCautionThreshold = 50f;
    [SerializeField] private float stabilityDangerThreshold = 20f;
    [SerializeField] private Color stabilityHealthyColor = new Color(0.2f, 0.78f, 0.95f, 1f);
    [SerializeField] private Color stabilityWarningColor = new Color(1f, 0.9f, 0.2f, 1f);
    [SerializeField] private Color stabilityLowColor = new Color(1f, 0.58f, 0.18f, 1f);
    [SerializeField] private Color stabilityCriticalColor = new Color(1f, 0.35f, 0.32f, 1f);
    [SerializeField] private Color stabilityPanelBaseColor = new Color(0.1f, 0.12f, 0.16f, 0.95f);
    [SerializeField] private Color stabilityPanelCriticalColor = new Color(0.3f, 0.08f, 0.08f, 0.98f);

    [Header("Scoring Weights")]
    [SerializeField] private float headingWeight = 0.35f;
    [SerializeField] private float thrustWeight = 0.35f;
    [SerializeField] private float correctionWindowWeight = 0.3f;

    [Header("Scoring Ranges")]
    [SerializeField] private float headingScoreRange = 30f;
    [SerializeField] private float thrustScoreRange = 40f;
    [SerializeField] private float correctionWindowScoreRange = 1.5f;

    private float currentHeading;
    private float currentThrust;
    private float currentCorrectionWindow;
    private float currentStability;
    private float lastStabilityTapTime = float.NegativeInfinity;
    private float stabilitySampleDuration;
    private float cumulativeStabilityNormalized;
    private float lowestStabilityNormalized = 1f;
    private float stabilityFillMaxHeight;
    private bool stabilityEnabled;

    public void Bind(
        ControlsTargetGenerator targetGeneratorRef,
        HeadingVisual headingVisualRef,
        ThrustVisual thrustVisualRef,
        CorrectionWindowVisual correctionWindowVisualRef,
        Text modeTextRef,
        Text stabilityHintTextRef,
        Text stabilityValueTextRef,
        Image stabilityFillRef,
        Image stabilityPanelRef,
        MiniGameResultsPopup factCardPopupRef,
        ShipView shipViewRef,
        bool stabilityEnabledRef)
    {
        targetGenerator = targetGeneratorRef;
        headingVisual = headingVisualRef;
        thrustVisual = thrustVisualRef;
        correctionWindowVisual = correctionWindowVisualRef;
        modeText = modeTextRef;
        stabilityHintText = stabilityHintTextRef;
        stabilityValueText = stabilityValueTextRef;
        stabilityFill = stabilityFillRef;
        stabilityPanel = stabilityPanelRef;
        factCardPopup = factCardPopupRef;
        shipView = shipViewRef;
        stabilityEnabled = stabilityEnabledRef;
    }

    private void Start()
    {
        if (targetGenerator == null)
        {
            targetGenerator = FindFirstObjectByType<ControlsTargetGenerator>();
        }

        if (headingVisual == null)
        {
            headingVisual = FindFirstObjectByType<HeadingVisual>();
        }

        if (thrustVisual == null)
        {
            thrustVisual = FindFirstObjectByType<ThrustVisual>();
        }

        if (correctionWindowVisual == null)
        {
            correctionWindowVisual = FindFirstObjectByType<CorrectionWindowVisual>();
        }

        if (factCardPopup == null)
        {
            factCardPopup = FindFirstObjectByType<MiniGameResultsPopup>();
        }

        currentHeading = Mathf.Lerp(headingMin, headingMax, 0.5f);
        currentThrust = Mathf.Lerp(thrustMin, thrustMax, 0.5f);
        currentCorrectionWindow = Mathf.Lerp(correctionWindowMin, correctionWindowMax, 0.5f);
        currentStability = Mathf.Clamp(startingStability, 0f, stabilityMax);
        headingAngle = currentHeading;
        thrustValue = currentThrust;
        headingPhase = 0.5f;
        thrustPhase = 0.5f;
        if (stabilityEnabled && stabilityFill != null)
        {
            stabilityFillMaxHeight = stabilityFill.rectTransform.sizeDelta.y;
        }

        if (targetGenerator != null)
        {
            if (headingVisual != null)
            {
                headingVisual.SetTargetBand(targetGenerator.TargetHeading, 0f);
            }
            if (thrustVisual != null)
            {
                thrustVisual.SetTarget(targetGenerator.TargetThrust);
            }
            if (correctionWindowVisual != null)
            {
                correctionWindowVisual.SetTarget(targetGenerator.TargetCorrectionWindow);
            }
        }

        UpdateModeText();
        UpdateStabilityVisuals();
    }

    private void Update()
    {
        if (state == GameState.Completed)
        {
            return;
        }

        if (stabilityEnabled)
        {
            UpdateStability();
        }
        HandleInput();

        if (headingVisual != null)
        {
            headingVisual.SetAngle(headingAngle);
        }

        if (thrustVisual != null)
        {
            thrustVisual.SetCurrent(thrustValue);
        }

        if (correctionWindowVisual != null)
        {
            correctionWindowVisual.SetCurrent(correctionWindowTimer);
        }

        if (stabilityEnabled)
        {
            UpdateStabilityVisuals();
        }

        if (state == GameState.CourseCorrection)
        {
            return;
        }
    }

    private void HandleInput()
    {
        if (stabilityEnabled)
        {
            HandleStabilityInput();
        }

        switch (mode)
        {
            case ControlMode.Heading:
                if (state != GameState.Aligning)
                {
                    break;
                }

                bool headingHeld = Input.GetKey(KeyCode.Space);
                if (waitingForRelease)
                {
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        waitingForRelease = false;
                    }
                    break;
                }
                if (!headingHeld && Time.time >= headingPauseUntil)
                {
                    headingPhase += GetCurrentHeadingSpeed() * Time.deltaTime;
                    headingAngle = Mathf.Lerp(headingMin, headingMax, Mathf.PingPong(headingPhase, 1f));
                }
                if (headingHeld)
                {
                    headingPauseUntil = Time.time + missPauseSeconds;
                    currentHeading = headingAngle;
                    mode = ControlMode.Thrust;
                    UpdateModeText();
                    waitingForRelease = true;
                }
                break;
            case ControlMode.Thrust:
                if (state != GameState.Aligning)
                {
                    break;
                }

                bool thrustHeld = Input.GetKey(KeyCode.Space);
                if (waitingForRelease)
                {
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        waitingForRelease = false;
                    }
                    break;
                }
                if (!thrustHeld && Time.time >= thrustPauseUntil)
                {
                    thrustPhase += GetCurrentThrustSpeed() * Time.deltaTime;
                    thrustValue = Mathf.Lerp(thrustMin, thrustMax, Mathf.PingPong(thrustPhase, 1f));
                }
                if (thrustHeld)
                {
                    thrustPauseUntil = Time.time + missPauseSeconds;
                    currentThrust = thrustValue;
                    mode = ControlMode.CorrectionWindow;
                    UpdateModeText();
                    waitingForRelease = true;
                }
                break;
            case ControlMode.CorrectionWindow:
                if (waitingForRelease)
                {
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        waitingForRelease = false;
                    }
                    break;
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    correctionWindowTiming = true;
                    correctionWindowTimer = 0f;
                }

                if (correctionWindowTiming)
                {
                    correctionWindowTimer += Time.deltaTime;
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        correctionWindowTiming = false;
                        currentCorrectionWindow = correctionWindowTimer;
                        StartCoroutine(CourseCorrectionSequence());
                        waitingForRelease = true;
                    }
                }
                break;
        }
    }

    private void HandleStabilityInput()
    {
        if (!Input.GetKeyDown(KeyCode.C))
        {
            return;
        }

        if (Time.time < lastStabilityTapTime + tapCooldown)
        {
            return;
        }

        lastStabilityTapTime = Time.time;
        currentStability = Mathf.Clamp(currentStability + stabilityRecoverPerTap, 0f, stabilityMax);
    }

    private void UpdateStability()
    {
        currentStability = Mathf.Clamp(currentStability - stabilityDrainPerSecond * Time.deltaTime, 0f, stabilityMax);

        float stabilityNormalized = GetStabilityNormalized();
        cumulativeStabilityNormalized += stabilityNormalized * Time.deltaTime;
        stabilitySampleDuration += Time.deltaTime;
        lowestStabilityNormalized = Mathf.Min(lowestStabilityNormalized, stabilityNormalized);

        if (shipView != null)
        {
            shipView.SetInstability(GetCriticalInstability01());
        }
    }

    private void UpdateStabilityVisuals()
    {
        if (!stabilityEnabled)
        {
            return;
        }

        float stabilityNormalized = GetStabilityNormalized();
        Color uiColor = GetStabilityColor();

        if (stabilityFill != null)
        {
            stabilityFill.color = uiColor;
            RectTransform fillRect = stabilityFill.rectTransform;
            Vector2 size = fillRect.sizeDelta;
            size.y = stabilityFillMaxHeight * stabilityNormalized;
            fillRect.sizeDelta = size;
        }

        if (stabilityValueText != null)
        {
            stabilityValueText.text = $"{Mathf.RoundToInt(currentStability)}%";
            stabilityValueText.color = uiColor;
        }

        if (stabilityHintText != null)
        {
            stabilityHintText.text = $"Tap C to stabilize\nOscillation x{GetCurrentOscillationMultiplier():0.00}";
            stabilityHintText.color = Color.Lerp(Color.white, uiColor, 0.65f);
        }

        if (stabilityPanel != null)
        {
            float pulse = 0.5f + 0.5f * Mathf.Sin(Time.time * 8f);
            Color targetPanelColor = Color.Lerp(stabilityPanelBaseColor, stabilityPanelCriticalColor, GetCriticalInstability01() * Mathf.Lerp(0.35f, 1f, pulse));
            stabilityPanel.color = targetPanelColor;
        }
    }

    private float GetStabilityNormalized()
    {
        return Mathf.Clamp01(currentStability / Mathf.Max(stabilityMax, 0.01f));
    }

    private float GetCurrentHeadingSpeed()
    {
        if (!stabilityEnabled)
        {
            return headingSpeed;
        }

        return headingSpeed * GetCurrentOscillationMultiplier();
    }

    private float GetCurrentThrustSpeed()
    {
        if (!stabilityEnabled)
        {
            return thrustSpeed;
        }

        return thrustSpeed * GetCurrentOscillationMultiplier();
    }

    private float GetCurrentOscillationMultiplier()
    {
        float stabilityNorm = Mathf.Clamp01(currentStability / Mathf.Max(startingStability, 0.01f));
        return Mathf.Lerp(Mathf.Max(1f, maxOscillationMultiplier), 1f, stabilityNorm);
    }

    private float GetCriticalInstability01()
    {
        if (!stabilityEnabled)
        {
            return 0f;
        }

        if (criticalStabilityThreshold <= 0f)
        {
            return 0f;
        }

        return 1f - Mathf.Clamp01(currentStability / criticalStabilityThreshold);
    }

    private Color GetStabilityColor()
    {
        if (currentStability <= stabilityDangerThreshold)
        {
            return stabilityCriticalColor;
        }

        if (currentStability <= stabilityCautionThreshold)
        {
            return stabilityLowColor;
        }

        if (currentStability <= stabilityWarningThreshold)
        {
            return stabilityWarningColor;
        }

        return stabilityHealthyColor;
    }

    private IEnumerator CourseCorrectionSequence()
    {
        state = GameState.CourseCorrection;

        ScoreResult result = ComputeScore();
        if (shipView != null)
        {
            shipView.PlayCourseCorrection(result.distance);
        }

        yield return new WaitForSeconds(courseCorrectionDelaySeconds);

        state = GameState.Completed;

        bool allowReplay = result.stars < 3;
        bool allowFact = result.stars >= 3;

        if (allowFact)
        {
            Global.MinigameScore(result.score);
        }
        else
        {
            Global.MinigameScoreNoFact(result.score);
        }

        Global.currentRoomCompleted = true;

        if (factCardPopup != null)
        {
            factCardPopup.ShowResults(result.distance, result.score, result.stars, allowReplay, allowFact);
        }
    }

    private int ComputeStars(int score)
    {
        if (score >= 80)
        {
            return 3;
        }

        if (score >= 50)
        {
            return 2;
        }

        if (score >= 20)
        {
            return 1;
        }

        return 0;
    }

    private int ComputeScoreFromStars(int stars)
    {
        return stars switch
        {
            3 => 30,
            2 => 20,
            1 => 10,
            _ => 5
        };
    }

    private ScoreResult ComputeScore()
    {
        if (targetGenerator == null)
        {
            return new ScoreResult { distance = 0, score = 0, stars = 0 };
        }

        float headingError = Mathf.Abs(currentHeading - targetGenerator.TargetHeading);
        float thrustError = Mathf.Abs(currentThrust - targetGenerator.TargetThrust);
        float correctionWindowError = Mathf.Abs(currentCorrectionWindow - targetGenerator.TargetCorrectionWindow);

        float headingNorm = Mathf.Clamp01(headingError / Mathf.Max(headingScoreRange, 0.01f));
        float thrustNorm = Mathf.Clamp01(thrustError / Mathf.Max(thrustScoreRange, 0.01f));
        float correctionWindowNorm = Mathf.Clamp01(correctionWindowError / Mathf.Max(correctionWindowScoreRange, 0.01f));

        float weighted = Mathf.Clamp01(headingWeight * headingNorm + thrustWeight * thrustNorm + correctionWindowWeight * correctionWindowNorm);
        float baseAccuracy = 100f * (1f - weighted);
        float stabilityPenalty = 0f;
        if (stabilityEnabled)
        {
            float averageStabilityNormalized = stabilitySampleDuration > 0f
                ? cumulativeStabilityNormalized / stabilitySampleDuration
                : GetStabilityNormalized();
            float stabilityQuality = Mathf.Clamp01((averageStabilityNormalized + lowestStabilityNormalized) * 0.5f);
            stabilityPenalty = (1f - stabilityQuality) * 100f * Mathf.Max(stabilityScorePenaltyWeight, 0f);
        }
        int distance = Mathf.Clamp(Mathf.RoundToInt(baseAccuracy - stabilityPenalty), 0, 100);
        int stars = ComputeStars(distance);
        int score = ComputeScoreFromStars(stars);
        return new ScoreResult { distance = distance, score = score, stars = stars };
    }

    private void UpdateModeText()
    {
        if (modeText == null)
        {
            return;
        }

        string label = mode switch
        {
            ControlMode.Heading => stabilityEnabled ? "Heading: Press Space to lock angle | Then thrust unlocks | Tap C to stabilize" : "Heading: Press Space to lock angle | Then thrust unlocks",
            ControlMode.Thrust => stabilityEnabled ? "Thrust: Press Space to lock power | Then burn window unlocks | Tap C to stabilize" : "Thrust: Press Space to lock power | Then burn window unlocks",
            ControlMode.CorrectionWindow => stabilityEnabled ? "Burn Window: Hold Space to set duration | Tap C to stabilize" : "Burn Window: Hold Space to set duration",
            _ => stabilityEnabled ? "Heading: Press Space to lock angle | Then thrust unlocks | Tap C to stabilize" : "Heading: Press Space to lock angle | Then thrust unlocks"
        };

        modeText.text = label;
    }
}
