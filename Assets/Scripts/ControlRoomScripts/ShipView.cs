using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShipView : MonoBehaviour
{
    [SerializeField] private RectTransform shipRoot;
    [SerializeField] private Image glow;

    [Header("Takeoff")]
    [SerializeField] private float riseDistance = 260f;
    [SerializeField] private float riseDuration = 1.4f;
    [SerializeField] private float glowPeak = 0.8f;
    [SerializeField] private float minScore = 25f;

    private Vector2 startPos;
    private bool playing;

    private void Awake()
    {
        if (shipRoot != null)
        {
            startPos = shipRoot.anchoredPosition;
        }

        if (glow != null)
        {
            Color c = glow.color;
            glow.color = new Color(c.r, c.g, c.b, 0f);
        }
    }

    public void Bind(RectTransform shipRootRef, Image glowRef)
    {
        shipRoot = shipRootRef;
        glow = glowRef;
        if (shipRoot != null)
        {
            startPos = shipRoot.anchoredPosition;
        }
    }

    public void SetMaxRiseToWorldY(float targetWorldY)
    {
        if (shipRoot == null)
        {
            return;
        }

        float delta = targetWorldY - shipRoot.position.y;
        riseDistance = Mathf.Max(0f, delta);
    }

    public void PlayTakeoff(int score)
    {
        if (playing || shipRoot == null)
        {
            return;
        }

        float clamped = Mathf.Clamp(score, minScore, 100f);
        float t = Mathf.InverseLerp(minScore, 100f, clamped);
        float rise = Mathf.Lerp(0f, riseDistance, t);
        StartCoroutine(TakeoffRoutine(rise));
    }

    private IEnumerator TakeoffRoutine(float rise)
    {
        playing = true;
        Vector2 endPos = startPos + new Vector2(0f, rise);

        float elapsed = 0f;
        while (elapsed < riseDuration)
        {
            float t = Mathf.Clamp01(elapsed / riseDuration);
            shipRoot.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

            if (glow != null)
            {
                Color c = glow.color;
                float alpha = Mathf.Lerp(0f, glowPeak, t);
                glow.color = new Color(c.r, c.g, c.b, alpha);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        shipRoot.anchoredPosition = endPos;
        if (glow != null)
        {
            Color c = glow.color;
            glow.color = new Color(c.r, c.g, c.b, 0f);
        }

        playing = false;
    }
}
