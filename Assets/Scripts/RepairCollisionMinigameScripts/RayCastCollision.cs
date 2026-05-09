using UnityEngine;

public class RayCastCollision : MonoBehaviour
{
    private Collider2D collision;

    void Update()
    {
        collision = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (collision != null)
        {
            GameObject gameObj = collision.gameObject;
            if (gameObj != null)
            {
                RepairColliderScript repairColliderScript = gameObj.GetComponent<RepairColliderScript>();
                if (repairColliderScript != null && (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)))
                {
                    repairColliderScript.OnClick();
                }
            }
            
        }
    }
}
