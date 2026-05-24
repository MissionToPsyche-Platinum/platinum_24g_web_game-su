using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ArrowPointerPlayTests
{
    private class TestArrowPointer : ArrowPointer
    {
        public string SceneOverride;
        protected override string GetCurrentScene() => SceneOverride ?? base.GetCurrentScene();
    }

    private static TestArrowPointer CreateArrow(string sceneOverride, out GameObject spriteChild, out SpriteRenderer renderer)
    {
        GameObject arrowRoot = new("ArrowPointer");
        GameObject child = new("ArrowSprite");
        child.transform.SetParent(arrowRoot.transform, false);
        renderer = child.AddComponent<SpriteRenderer>();
        spriteChild = child;

        TestArrowPointer arrow = arrowRoot.AddComponent<TestArrowPointer>();
        arrow.SceneOverride = sceneOverride;
        return arrow;
    }

    // time-sensitive popup showing (timerStarted=false) — arrow hidden
    [UnityTest]
    public IEnumerator ArrowPointer_TimeSensitivePopupShowing_HidesArrow()
    {
        Global.inTimeSensitiveMinigame = true;
        Global.timerStarted = false;
        Global.currentRoom = "LabRoom"; // different from scene so hide-same-room path doesn't trigger

        // player is in CargoRoom, time-sensitive popup showing
        TestArrowPointer arrow = CreateArrow("CargoRoom", out GameObject sprite, out _);
        yield return null; // Start + Update run

        Assert.That(sprite.activeSelf, Is.False);

        Object.Destroy(arrow.gameObject);
        Global.inTimeSensitiveMinigame = false;
        Global.timerStarted = false;
        Global.currentRoom = null;
    }

    // time-sensitive timer running in a room — red arrow to MainHallTrigger
    [UnityTest]
    public IEnumerator ArrowPointer_TimeSensitiveTimerRunning_InRoom_ShowsRedArrowToMainHall()
    {
        Global.inTimeSensitiveMinigame = true;
        Global.timerStarted = true;
        Global.currentRoom = "LabRoom";

        GameObject mainHallTrigger = new("MainHallTrigger");
        mainHallTrigger.tag = "MainHallTrigger";

        TestArrowPointer arrow = CreateArrow("ControlRoom", out GameObject sprite, out SpriteRenderer sr);
        yield return null;

        Assert.That(sprite.activeSelf, Is.True);
        Assert.That(sr.color, Is.EqualTo(Color.red));

        GameObject targetObj = (GameObject)typeof(ArrowPointer)
            .GetField("targetObject", BindingFlags.Instance | BindingFlags.NonPublic)
            .GetValue(arrow);
        Assert.That(targetObj, Is.Not.Null);
        Assert.That(targetObj.name, Is.EqualTo("MainHallTrigger"));

        Object.Destroy(arrow.gameObject);
        Object.Destroy(mainHallTrigger);
        Global.inTimeSensitiveMinigame = false;
        Global.timerStarted = false;
        Global.currentRoom = null;
    }

    // time-sensitive timer running but player is in MainHall — arrow hidden
    [UnityTest]
    public IEnumerator ArrowPointer_TimeSensitiveTimerRunning_InMainHall_HidesArrow()
    {
        Global.inTimeSensitiveMinigame = true;
        Global.timerStarted = true;
        Global.currentRoom = "LabRoom";

        TestArrowPointer arrow = CreateArrow("MainHall", out GameObject sprite, out _);
        yield return null;

        Assert.That(sprite.activeSelf, Is.False);

        Object.Destroy(arrow.gameObject);
        Global.inTimeSensitiveMinigame = false;
        Global.timerStarted = false;
        Global.currentRoom = null;
    }

    // normal state in a room (not time-sensitive) — white arrow to MainHallTrigger
    [UnityTest]
    public IEnumerator ArrowPointer_Normal_InRoom_ShowsWhiteArrow()
    {
        Global.inTimeSensitiveMinigame = false;
        Global.timerStarted = false;
        Global.currentRoom = "LabRoom";

        GameObject mainHallTrigger = new("MainHallTrigger");
        mainHallTrigger.tag = "MainHallTrigger";

        TestArrowPointer arrow = CreateArrow("ControlRoom", out GameObject sprite, out SpriteRenderer sr);
        yield return null;

        Assert.That(sprite.activeSelf, Is.True);
        Assert.That(sr.color, Is.EqualTo(Color.white));

        Object.Destroy(arrow.gameObject);
        Object.Destroy(mainHallTrigger);
        Global.currentRoom = null;
    }
}
