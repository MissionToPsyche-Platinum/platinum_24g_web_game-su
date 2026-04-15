using System;
using System.Linq;
using NUnit.Framework;

public class ControlRoomEditTests
{
    [TestCase("ControlsTrigger")]
    [TestCase("ControlsMinigameBootstrap")]
    [TestCase("ControlsMinigameController")]
    [TestCase("ShipView")]
    public void ControlRoomScripts_Exist(string typeName)
    {
        Type type = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .FirstOrDefault(t => t.Name == typeName);

        Assert.That(type, Is.Not.Null, $"{typeName} should exist in the loaded assemblies.");
    }

    [Test]
    public void ControlRoomScriptCount_IsExpected()
    {
        string[] expectedTypes =
        {
            "ControlsTrigger",
            "ControlsMinigameBootstrap",
            "ControlsMinigameController",
            "ShipView"
        };

        int found = expectedTypes.Count(typeName =>
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Any(t => t.Name == typeName));

        Assert.That(found, Is.EqualTo(expectedTypes.Length));
    }
}
