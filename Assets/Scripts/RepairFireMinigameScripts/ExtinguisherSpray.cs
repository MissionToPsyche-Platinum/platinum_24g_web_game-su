using UnityEngine;

public class ExtinguisherSpray : MonoBehaviour
{
    public GameObject smokePrefab;
    public Transform sprayPoint;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (smokePrefab != null && sprayPoint != null)
            {
                Vector3 direction = transform.localScale.x > 0 ? Vector3.right : Vector3.left;

                Quaternion rotation = direction == Vector3.right 
                    ? Quaternion.identity 
                    : Quaternion.Euler(0, 180, 0);

                Instantiate(smokePrefab, sprayPoint.position, rotation);
            }
        }
    }
}