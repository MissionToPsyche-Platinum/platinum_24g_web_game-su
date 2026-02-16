using UnityEngine;

public class CargoRoomController : MonoBehaviour
{
    private GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerMovement2D>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
