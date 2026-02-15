using UnityEngine;

public class TargetChanger : MonoBehaviour
{
    private Animator animator;
    private bool occupied;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        occupied = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("MoveableBox"))
        {
            Debug.Log("Box entered");
            animator.SetBool("ContainsBox", true);
            occupied = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("MoveableBox"))
        {
            Debug.Log("Box exited");

            animator.SetBool("ContainsBox", false);
            occupied = false;
        }
    }
}
