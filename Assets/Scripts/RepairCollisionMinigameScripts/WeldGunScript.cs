using UnityEngine;

public class WeldGunScript : MonoBehaviour
{
    public GameObject weldSpark;
    public GameObject helpPanel;

    // Update is called once per frame
    void Update()
    {
        FollowCursor();
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
        {
            TriggerSpark();
        }
    }

    private void FollowCursor()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        mousePosition.z = transform.position.z;
        transform.position = mousePosition - new Vector3(-1.5f, 1.5f);
    }

    public void TriggerSpark()
    {
        weldSpark.SetActive(!weldSpark.activeSelf);
        helpPanel.SetActive(false);
    }
}
