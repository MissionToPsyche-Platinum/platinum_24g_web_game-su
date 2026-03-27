using UnityEngine;

public class WeldHandler : MonoBehaviour
{
    public GameObject maskPrefab;
    private bool isPressed = false;

    private void Update()
    {
        var mousePos = Input.mousePosition;
        mousePos.z = 5;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        if (isPressed)
        {
            GameObject maskSprite = Instantiate(maskPrefab,mousePos,Quaternion.identity);
            maskSprite.transform.parent = gameObject.transform;
            if (Input.GetMouseButtonUp(0))
            {
                isPressed = false;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                isPressed = true;
            }
        }
        
    }

    private void Reveal()
    {
        Destroy(this.gameObject);
    }
}
