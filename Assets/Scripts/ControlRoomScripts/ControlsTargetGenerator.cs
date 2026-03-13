using UnityEngine;

public class ControlsTargetGenerator : MonoBehaviour
{
    [Header("Target Ranges")]
    [SerializeField] private float minHeadingDegrees = -30f;
    [SerializeField] private float maxHeadingDegrees = 30f;
    [SerializeField] private float minThrust = 20f;
    [SerializeField] private float maxThrust = 80f;
    [SerializeField] private float minCorrectionWindow = 1f;
    [SerializeField] private float maxCorrectionWindow = 5f;

    public float TargetHeading { get; private set; }
    public float TargetThrust { get; private set; }
    public float TargetCorrectionWindow { get; private set; }

    private void Awake()
    {
        GenerateTargets();
    }

    public void GenerateTargets()
    {
        TargetHeading = Random.Range(minHeadingDegrees, maxHeadingDegrees);
        TargetThrust = Random.Range(minThrust, maxThrust);
        TargetCorrectionWindow = Random.Range(minCorrectionWindow, maxCorrectionWindow);
    }
}
