using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class WinScenePlayTests
{
    [UnityTest]
    public IEnumerator WinSceneController_Awake_SetsTimeScaleToOne()
    {
        Time.timeScale = 0f;

        GameObject go = new("WinSceneController");
        go.AddComponent<WinSceneController>();
        yield return null;

        Assert.That(Time.timeScale, Is.EqualTo(1f));

        Time.timeScale = 1f;
        UnityEngine.Object.Destroy(go);
    }

    [UnityTest]
    public IEnumerator WinSceneController_Awake_CreatesCanvasWhenNonePresent()
    {
        GameObject go = new("WinSceneController");
        go.AddComponent<WinSceneController>();
        yield return null;

        Canvas canvas = UnityEngine.Object.FindFirstObjectByType<Canvas>();
        Assert.That(canvas, Is.Not.Null);

        UnityEngine.Object.Destroy(go);
        UnityEngine.Object.Destroy(canvas.gameObject);
    }

    [UnityTest]
    public IEnumerator WinSceneController_Awake_CreatesWinScenePanel()
    {
        GameObject go = new("WinSceneController");
        go.AddComponent<WinSceneController>();
        yield return null;

        GameObject panel = GameObject.Find("WinScenePanel");
        Assert.That(panel, Is.Not.Null);

        UnityEngine.Object.Destroy(go);
        UnityEngine.Object.Destroy(panel.transform.root.gameObject);
    }

    [UnityTest]
    public IEnumerator WinSceneController_Awake_PanelHasTitleBodyAndReturnButton()
    {
        GameObject go = new("WinSceneController");
        go.AddComponent<WinSceneController>();
        yield return null;

        GameObject panel = GameObject.Find("WinScenePanel");
        Assert.That(panel, Is.Not.Null);

        Text title = panel.transform.Find("Title")?.GetComponent<Text>();
        Text body = panel.transform.Find("Body")?.GetComponent<Text>();
        Button returnButton = panel.transform.Find("ReturnButton")?.GetComponent<Button>();

        Assert.That(title, Is.Not.Null);
        Assert.That(body, Is.Not.Null);
        Assert.That(returnButton, Is.Not.Null);
        Assert.That(title.text, Is.EqualTo("You Won!"));
        Assert.That(body.text, Is.EqualTo("Mission complete. Great work."));

        GameObject root = panel.transform.root.gameObject;
        UnityEngine.Object.Destroy(go);
        UnityEngine.Object.Destroy(root);
    }

    [UnityTest]
    public IEnumerator WinSceneController_Awake_UsesExistingCanvasIfPresent()
    {
        foreach (Canvas c in UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None))
            UnityEngine.Object.Destroy(c.gameObject);
        yield return null;

        GameObject canvasObject = new("ExistingCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas existingCanvas = canvasObject.GetComponent<Canvas>();
        existingCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

        GameObject go = new("WinSceneController");
        go.AddComponent<WinSceneController>();
        yield return null;

        Canvas[] allCanvases = UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        Assert.That(allCanvases.Length, Is.EqualTo(1));

        GameObject panel = GameObject.Find("WinScenePanel");
        Assert.That(panel.transform.parent, Is.EqualTo(canvasObject.transform));

        UnityEngine.Object.Destroy(go);
        UnityEngine.Object.Destroy(canvasObject);
    }

    [UnityTest]
    public IEnumerator WinSceneController_Awake_DoesNotDuplicatePanelOnSecondAwake()
    {
        GameObject canvasObject = new("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        GameObject go = new("WinSceneController");
        WinSceneController controller = go.AddComponent<WinSceneController>();
        yield return null;

        int countBefore = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None)
            .CountMatches(o => o.name == "WinScenePanel");

        InvokePrivate(controller, "EnsureUI");
        yield return null;

        int countAfter = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None)
            .CountMatches(o => o.name == "WinScenePanel");

        Assert.That(countAfter, Is.EqualTo(countBefore));

        UnityEngine.Object.Destroy(go);
        UnityEngine.Object.Destroy(canvasObject);
    }

    [UnityTest]
    public IEnumerator WinSceneController_ReturnToStartMenu_ResetsGameState()
    {
        Global.totalScore = 50;
        Global.hasWon = true;
        Global.lastAwardedFactText = "some fact";

        GameObject canvasObject = new("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        GameObject go = new("WinSceneController");
        WinSceneController controller = go.AddComponent<WinSceneController>();
        yield return null;

        InvokePrivate(controller, "ReturnToStartMenu");
        yield return null;

        Assert.That(Global.totalScore, Is.EqualTo(0));
        Assert.That(Global.hasWon, Is.False);
        Assert.That(Global.lastAwardedFactText, Is.EqualTo(""));

        UnityEngine.Object.Destroy(go);
        UnityEngine.Object.Destroy(canvasObject);
    }

    private static void InvokePrivate(object instance, string methodName, params object[] args)
    {
        MethodInfo method = instance.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, $"{methodName} should exist.");
        method.Invoke(instance, args);
    }
}

internal static class GameObjectCountExtensions
{
    public static int CountMatches(this GameObject[] objects, System.Func<GameObject, bool> predicate)
    {
        int count = 0;
        foreach (GameObject o in objects)
            if (predicate(o)) count++;
        return count;
    }
}
