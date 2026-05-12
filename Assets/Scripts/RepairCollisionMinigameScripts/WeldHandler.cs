using UnityEngine;
using UnityEngine.SceneManagement;

public class WeldHandler : MonoBehaviour
{
    public GameObject maskPrefab;
    private bool isPressed = false;
    private bool isColliding = false;
    private Vector3 previousMousePosition;
    private Vector3 currentMousePosition;

    private void Update()
    {
        previousMousePosition = currentMousePosition;
        currentMousePosition = Input.mousePosition;
        currentMousePosition.z = 5;
        currentMousePosition = Camera.main.ScreenToWorldPoint(currentMousePosition);

        if (isPressed && isColliding)
        {
            if (previousMousePosition != currentMousePosition)
            {
                GameObject maskSprite = Instantiate(maskPrefab, currentMousePosition, Quaternion.identity);
                maskSprite.transform.parent = gameObject.transform;
            }
            Debug.Log("Mouse is pressed and colliding. Current mouse position: " + currentMousePosition);

            if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("Mouse released, setting isPressed to false");
                isPressed = false;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                isPressed = true;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isPressed = false;
            }
        }

    }

    private void OnMouseEnter()
    {
        isColliding = true;
    }
    private void OnMouseExit()
    {
        isColliding = false;
    }

    public void Reveal()
    {
        Destroy(this.gameObject);
    }

}
