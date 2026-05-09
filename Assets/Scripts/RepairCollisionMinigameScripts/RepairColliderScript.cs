using UnityEngine;

public class RepairColliderScript : MonoBehaviour
{
    public GameObject repairColliderController;
    private RepairCollisionController repairCollisionControllerScript;
    private bool activated = false;

    private void Start()
    {
        repairCollisionControllerScript = repairColliderController.GetComponent<RepairCollisionController>();
    }

    public void OnClick()
    {
        if (!activated)
        {
            activated = true;
            repairCollisionControllerScript.ColliderReached();
        }
    }

}
