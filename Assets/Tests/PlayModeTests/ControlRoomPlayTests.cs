using System;
using System.Linq;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ControlRoomPlayTests
{
    [UnityTest]
    public IEnumerator ControlRoomComponents_CanBeAddedToGameObjects()
    {
        string[] componentNames =
        {
            "ControlsTrigger",
            "ControlsMinigameBootstrap",
            "ControlsMinigameController",
            "ShipView"
        };

        foreach (string componentName in componentNames)
        {
            Type type = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .FirstOrDefault(t => t.Name == componentName);

            Assert.That(type, Is.Not.Null, $"{componentName} type was not found.");

            GameObject go = new GameObject(componentName);
            Component component = go.AddComponent(type);

            Assert.That(component, Is.Not.Null, $"{componentName} component should be addable.");

            UnityEngine.Object.Destroy(go);
        }

        yield return null;
    }
}
