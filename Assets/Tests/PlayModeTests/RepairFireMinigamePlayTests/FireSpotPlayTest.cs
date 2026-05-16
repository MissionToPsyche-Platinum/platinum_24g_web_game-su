using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

public class FireSpotPlayTest
{
    [UnityTest]
    public IEnumerator FireSpot_Disappears_WhenSmokeTouchesIt()
    {
        GameObject fireObject = new GameObject("FireSpot");
        fireObject.AddComponent<FireSpot>();

        BoxCollider2D fireCollider = fireObject.AddComponent<BoxCollider2D>();
        fireCollider.isTrigger = true;

        GameObject smokeObject = new GameObject("Smoke");
        smokeObject.tag = "Smoke";

        BoxCollider2D smokeCollider = smokeObject.AddComponent<BoxCollider2D>();
        smokeCollider.isTrigger = true;

        Rigidbody2D smokeRb = smokeObject.AddComponent<Rigidbody2D>();
        smokeRb.gravityScale = 0;

        fireObject.transform.position = Vector2.zero;
        smokeObject.transform.position = Vector2.zero;

        yield return new WaitForFixedUpdate();

        Assert.IsFalse(fireObject.activeSelf);
    }
}