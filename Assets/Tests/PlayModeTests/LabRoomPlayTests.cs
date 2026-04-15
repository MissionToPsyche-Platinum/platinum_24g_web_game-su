using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class LabRoomPlayTests
{
    [UnityTest]
    public IEnumerator SpillDisappears_WhenTouchedByBroom()
    {
        //creat the spill manager object
        GameObject managerObject = new GameObject("SpillManager");
        managerObject.AddComponent<SpillManager>();
        //making a spill:
        GameObject spillObject = new GameObject("Spill");
        spillObject.AddComponent<BoxCollider2D>().isTrigger = true;
        SpillClean spillClean = spillObject.AddComponent<SpillClean>();
        GameObject broomParent = new GameObject("BroomParent");  //have the broom get picked up
        BroomPickup broomPickup = broomParent.AddComponent<BroomPickup>();
        broomPickup.isHoldingBroom = true;
        GameObject broomObject = new GameObject("Broom");//collide
        broomObject.tag = "Broom";
        broomObject.transform.SetParent(broomParent.transform);
        BoxCollider2D broomCollider = broomObject.AddComponent<BoxCollider2D>();
        spillClean.SendMessage("OnTriggerEnter2D", broomCollider); //calling the trigger
        yield return null; //wait for destroy to be called (need this to wait for a second)
        //the spill SHOULD be destroyed
        Assert.IsTrue(spillObject == null);
    }
}