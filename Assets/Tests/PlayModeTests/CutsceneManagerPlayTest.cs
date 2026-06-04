using NUnit.Framework;
using System.Collections;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class CutsceneManagerPlayTests : InputTestFixture
{
    private GameObject _go;
    private CutsceneManager _cm;
    private Image _img1, _img2, _img3, _img4;
    private TextMeshProUGUI _text;
    private Mouse _mouse;

    public override void Setup()
    {
        base.Setup();

        // Create GameObject and add component
        _go = new GameObject("CutsceneManagerGO");
        _cm = _go.AddComponent<CutsceneManager>();

        // Create UI Images (no Canvas required for these tests)
        var imgObj1 = new GameObject("Image1");
        _img1 = imgObj1.AddComponent<Image>();

        var imgObj2 = new GameObject("Image2");
        _img2 = imgObj2.AddComponent<Image>();

        var imgObj3 = new GameObject("Image3");
        _img3 = imgObj3.AddComponent<Image>();

        var imgObj4 = new GameObject("Image4");
        _img4 = imgObj4.AddComponent<Image>();

        // Create a TextMeshProUGUI. It will add a RectTransform automatically.
        var textObj = new GameObject("CutsceneText");
        _text = textObj.AddComponent<TextMeshProUGUI>();

        // Assign public fields
        _cm.image1 = _img1;
        _cm.image2 = _img2;
        _cm.image3 = _img3;
        _cm.image4 = _img4;
        _cm.cutsceneText = _text;

        // Optionally add a test mouse device (some tests may use it)
        _mouse = InputSystem.AddDevice<Mouse>();
    }

    public override void TearDown()
    {
        if (_go != null) Object.DestroyImmediate(_go);
        // Clean up created UI objects
        var objs = GameObject.FindObjectsOfType<GameObject>();
        foreach (var o in objs)
        {
            if (o.name.StartsWith("Image") || o.name == "CutsceneText")
                Object.DestroyImmediate(o);
        }

        base.TearDown();
    }

    FieldInfo GetPrivateField(string name)
    {
        return typeof(CutsceneManager).GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
    }

    MethodInfo GetPrivateMethod(string name)
    {
        return typeof(CutsceneManager).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance);
    }

    [UnityTest]
    public IEnumerator Start_InitializesImagesList_AndSetsInitialText()
    {
        // Arrange & Act
        var start = GetPrivateMethod("Start");
        start.Invoke(_cm, null);

        yield return null;

        // Assert: private 'images' list contains the 4 image references
        var imagesField = GetPrivateField("images");
        Assert.IsNotNull(imagesField, "private field 'images' should exist");
        var images = imagesField.GetValue(_cm) as System.Collections.IList;
        Assert.IsNotNull(images, "'images' should be a list after Start");
        Assert.AreEqual(4, images.Count, "images list should contain 4 entries");
        Assert.AreSame(_img1, images[0], "image1 should be first element");
        Assert.AreSame(_img2, images[1], "image2 should be second element");
        Assert.AreSame(_img3, images[2], "image3 should be third element");
        Assert.AreSame(_img4, images[3], "image4 should be fourth element");

        // Assert: cutsceneText is set to the first script entry
        var scriptField = GetPrivateField("script");
        Assert.IsNotNull(scriptField, "private field 'script' should exist");
        var script = scriptField.GetValue(_cm) as System.Collections.IList;
        Assert.IsNotNull(script, "'script' should be a list");
        Assert.IsTrue(script.Count > 0, "script should contain entries");

        Assert.AreEqual((string)script[0], _cm.cutsceneText.text, "cutsceneText should be initialized to the first script entry");
    }

    [UnityTest]
    public IEnumerator Update_NoClick_DoesNotAdvanceClickCount()
    {
        // Arrange: call Start to initialize state
        var start = GetPrivateMethod("Start");
        start.Invoke(_cm, null);
        yield return null;

        var clickCountField = GetPrivateField("clickCount");
        var before = (int)clickCountField.GetValue(_cm);

        // Act: invoke the private Update method directly (no mouse click simulated)
        var update = GetPrivateMethod("Update");
        update.Invoke(_cm, null);
        yield return null;

        // Assert: clickCount remains the same when no click is present
        var after = (int)clickCountField.GetValue(_cm);
        Assert.AreEqual(before, after, "clickCount should not change when no mouse click occurs in Update()");
    }

    [UnityTest]
    public IEnumerator AdvanceCutscene_WhenImagesNull_InitializesImages_AndAdvances()
    {
        // Arrange: ensure images is null to exercise defensive initialization
        var imagesField = GetPrivateField("images");
        imagesField.SetValue(_cm, null);

        var scriptField = GetPrivateField("script");
        var script = scriptField.GetValue(_cm) as System.Collections.IList;

        var clickCountField = GetPrivateField("clickCount");
        clickCountField.SetValue(_cm, 0);

        // Act: call AdvanceCutscene which should initialize images and advance once
        _cm.AdvanceCutscene();
        yield return null;

        // Assert: images were initialized
        var images = imagesField.GetValue(_cm) as System.Collections.IList;
        Assert.IsNotNull(images, "images should be initialized when null");
        Assert.AreEqual(4, images.Count, "images list should contain 4 entries after defensive init");

        // Assert: clickCount incremented and text updated
        var afterClick = (int)clickCountField.GetValue(_cm);
        Assert.AreEqual(1, afterClick, "clickCount should increment by 1 after AdvanceCutscene() when images were null");
        Assert.AreEqual((string)script[1], _cm.cutsceneText.text, "cutsceneText should update to the next script entry after AdvanceCutscene()");
    }
    [UnityTest]
    public IEnumerator AdvanceCutscene_ClickCountEqualImageCount_SetsGlobalsAndChangesScene()
    {
        // Arrange: set clickCount to last image index
        var clickCountField = GetPrivateField("clickCount");
        clickCountField.SetValue(_cm, 3);
        LogAssert.Expect(LogType.Exception, "IndexOutOfRangeException: Index was outside the bounds of the array.");

        // Act
        _cm.AdvanceCutscene();
        yield return null;

        // Assert
        Assert.IsTrue(Global.tutorialShown, "Global.tutorialShown should be set to true after advancing past last image");
        Assert.AreEqual("MainHall", Global.playerRoomTracker, "Global.playerRoomTracker should be set to 'MainHall' after advancing past last image");
        Assert.AreNotEqual("CutsceneManagerPlayTests", UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, "Scene should change after advancing past last image");
    }
}