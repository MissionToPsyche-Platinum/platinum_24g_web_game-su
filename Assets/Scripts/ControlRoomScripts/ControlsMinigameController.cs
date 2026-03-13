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
        Burn,
        Completed
    }

    private enum ControlMode
    {
        Heading,
        Thrust,
        BurnDuration
    }

    [Header("Target")]
    [SerializeField] private ControlsTargetGenerator targetGenerator;
    [SerializeField] private HeadingVisual headingVisual;
    [SerializeField] private ThrustVisual thrustVisual;
    [SerializeField] private BurnVisual burnVisual;

    [Header("UI")]
    [SerializeField] private Text statusText;
    [SerializeField] private Text modeText;
    [SerializeField] private GameObject burnOverlay;
    [SerializeField] private MiniGameResultsPopup factCardPopup;
    [SerializeField] private ShipView shipView;

    [Header("Burn")]
    [SerializeField] private float burnDurationSeconds = 1.5f;

    [Header("Ranges")]
    [SerializeField] private float headingMin = -70f;
    [SerializeField] private float headingMax = 70;
    [SerializeField] private float thrustMin = 0f;
    [SerializeField] private float thrustMax = 100f;
    [SerializeField] private float burnMin = 1f;
    [SerializeField] private float burnMax = 4f;

    private GameState state = GameState.Aligning;
    private float timeInsideTolerance;
    private ControlMode mode = ControlMode.Heading;

    private float headingAngle;
    private float thrustValue;
    private float burnTimer;
    private bool burnTiming;
    private float headingPauseUntil;
    private float thrustPauseUntil;
    private bool waitingForRelease;

    [Header("Oscillation")]
    [SerializeField] private float headingSpeed = 0.6f;
    [SerializeField] private float thrustSpeed = 0.5f;
    [SerializeField] private float missPauseSeconds = 0.2f;

    [Header("Scoring Weights")]
    [SerializeField] private float headingWeight = 0.35f;
    [SerializeField] private float thrustWeight = 0.35f;
    [SerializeField] private float burnWeight = 0.3f;

    [Header("Scoring Ranges")]
    [SerializeField] private float headingScoreRange = 30f;
    [SerializeField] private float thrustScoreRange = 40f;
    [SerializeField] private float burnScoreRange = 1.5f;

    private float currentHeading;
    private float currentThrust;
    private float currentBurnDuration;

    public void Bind(
        ControlsTargetGenerator targetGeneratorRef,
        HeadingVisual headingVisualRef,
        ThrustVisual thrustVisualRef,
        BurnVisual burnVisualRef,
        Text statusTextRef,
        Text modeTextRef,
        GameObject burnOverlayRef,
        MiniGameResultsPopup factCardPopupRef,
        ShipView shipViewRef)
    {
        targetGenerator = targetGeneratorRef;
        headingVisual = headingVisualRef;
        thrustVisual = thrustVisualRef;
        burnVisual = burnVisualRef;
        statusText = statusTextRef;
        modeText = modeTextRef;
        burnOverlay = burnOverlayRef;
        factCardPopup = factCardPopupRef;
        shipView = shipViewRef;
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

        if (burnVisual == null)
        {
            burnVisual = FindFirstObjectByType<BurnVisual>();
        }

        if (factCardPopup == null)
        {
            factCardPopup = FindFirstObjectByType<MiniGameResultsPopup>();
        }

        currentHeading = Mathf.Lerp(headingMin, headingMax, 0.5f);
        currentThrust = Mathf.Lerp(thrustMin, thrustMax, 0.5f);
        currentBurnDuration = Mathf.Lerp(burnMin, burnMax, 0.5f);
        headingAngle = currentHeading;
        thrustValue = currentThrust;

        if (burnOverlay != null)
        {
            burnOverlay.SetActive(false);
        }

        if (statusText != null)
        {
            statusText.text = "Align heading and thrust to planned trajectory.";
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
            if (burnVisual != null)
            {
                burnVisual.SetTarget(targetGenerator.TargetBurnDuration);
            }
        }

        UpdateModeText();
    }

    private void Update()
    {
        if (state == GameState.Completed)
        {
            return;
        }

        HandleModeSwitch();
        HandleInput();

        if (headingVisual != null)
        {
            headingVisual.SetAngle(headingAngle);
        }

        if (thrustVisual != null)
        {
            thrustVisual.SetCurrent(thrustValue);
        }

        if (burnVisual != null)
        {
            burnVisual.SetCurrent(burnTimer);
        }

        if (state == GameState.Burn)
        {
            return;
        }
    }

    private void HandleModeSwitch()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            mode = ControlMode.Heading;
            UpdateModeText();
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            mode = ControlMode.Thrust;
            UpdateModeText();
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            mode = ControlMode.BurnDuration;
            UpdateModeText();
        }
    }

    private void HandleInput()
    {
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
                    headingAngle = Mathf.Lerp(headingMin, headingMax, Mathf.PingPong(Time.time * headingSpeed, 1f));
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
                    thrustValue = Mathf.Lerp(thrustMin, thrustMax, Mathf.PingPong(Time.time * thrustSpeed, 1f));
                }
                if (thrustHeld)
                {
                    thrustPauseUntil = Time.time + missPauseSeconds;
                    currentThrust = thrustValue;
                    mode = ControlMode.BurnDuration;
                    UpdateModeText();
                    waitingForRelease = true;
                }
                break;
            case ControlMode.BurnDuration:
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
                    burnTiming = true;
                    burnTimer = 0f;
                }

                if (burnTiming)
                {
                    burnTimer += Time.deltaTime;
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        burnTiming = false;
                        currentBurnDuration = burnTimer;
                        StartCoroutine(BurnSequence());
                        waitingForRelease = true;
                    }
                }
                break;
        }
    }

    private IEnumerator BurnSequence()
    {
        state = GameState.Burn;

        if (statusText != null)
        {
            statusText.text = "Course correction in progress...";
        }

        ScoreResult result = ComputeScore();
        if (shipView != null)
        {
            shipView.PlayCourseCorrection(result.distance);
        }

        if (burnOverlay != null)
        {
            burnOverlay.SetActive(true);
        }

        yield return new WaitForSeconds(burnDurationSeconds);

        if (burnOverlay != null)
        {
            burnOverlay.SetActive(false);
        }

        state = GameState.Completed;

        if (statusText != null)
        {
            statusText.text = "Course correction complete.";
        }

        if (factCardPopup != null)
        {
            factCardPopup.ShowResults(result.distance, result.score, result.stars);
        }
        Global.MinigameScore(result.score);
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
        float burnError = Mathf.Abs(currentBurnDuration - targetGenerator.TargetBurnDuration);

        float headingNorm = Mathf.Clamp01(headingError / Mathf.Max(headingScoreRange, 0.01f));
        float thrustNorm = Mathf.Clamp01(thrustError / Mathf.Max(thrustScoreRange, 0.01f));
        float burnNorm = Mathf.Clamp01(burnError / Mathf.Max(burnScoreRange, 0.01f));

        float weighted = Mathf.Clamp01(headingWeight * headingNorm + thrustWeight * thrustNorm + burnWeight * burnNorm);
        float scoreFloat = 100f * (1f - weighted);
        int distance = Mathf.Clamp(Mathf.RoundToInt(scoreFloat), 0, 100);
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
            ControlMode.Heading => "Heading: Press Space to lock angle",
            ControlMode.Thrust => "Thrust: Press Space to lock power",
            ControlMode.BurnDuration => "Correction Window: Hold Space to set duration",
            _ => "Heading: Press Space to lock angle"
        };

        modeText.text = label;

        if (statusText != null)
        {
            statusText.text = label;
        }
    }
}
