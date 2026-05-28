using NUnit.Framework;
using System.Collections;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

public class SpillManagerPlayTests
{
    private GameObject managerObj;
    private GameObject panelObj;
    private GameObject textObj;
    private SpillManager manager;

    private FieldInfo GetPrivateField(string fieldName)
    {
        return typeof(SpillManager).GetField(
            fieldName,
            BindingFlags.NonPublic | BindingFlags.Instance
        );
    }

    [SetUp]
    public void Setup()
    {
        SpillManager.Instance = null;
        Global.currentRoomCompleted = false;
        Global.totalScore = 0;

        managerObj = new GameObject("SpillManager");
        manager = managerObj.AddComponent<SpillManager>();

        panelObj = new GameObject("GameOverPanel");
        panelObj.SetActive(false);
        manager.gameOverPanel = panelObj;

        textObj = new GameObject("ScoreText");
        manager.scoreText = textObj.AddComponent<TextMeshProUGUI>() as TMP_Text;
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(managerObj);
        Object.Destroy(panelObj);
        Object.Destroy(textObj);

        SpillManager.Instance = null;
    }

    [UnityTest]
    public IEnumerator CoverageProof_SpillManagerAwakeRuns()
    {
        yield return null;

        Assert.AreEqual(manager, SpillManager.Instance);
    }

    [UnityTest]
    public IEnumerator FinalSpillTriggersGameOver()
    {
        yield return null;

        GetPrivateField("spillsRemaining").SetValue(manager, 1);
        GetPrivateField("awarded").SetValue(manager, false);

        Debug.Log("SPILL MANAGER TEST RAN");

        manager.OnSpillCleaned();

        Debug.Log("ON SPILL CLEANED CALLED");

        Assert.IsTrue(manager.gameOverPanel.activeSelf);
        Assert.IsTrue(Global.currentRoomCompleted);
        Assert.AreEqual(
            "Total score: " + Global.totalScore,
            manager.scoreText.text
        );
    }

    [UnityTest]
    public IEnumerator SpillCleanDoesNothingAfterAwarded()
    {
        yield return null;

        GetPrivateField("spillsRemaining").SetValue(manager, 1);
        GetPrivateField("awarded").SetValue(manager, true);

        manager.OnSpillCleaned();

        Assert.IsFalse(manager.gameOverPanel.activeSelf);
    }
}