using UnityEngine;

public class Target : MonoBehaviour
{
    private Animator animator;
    public bool occupied;
    private CargoBox box;


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
            if (collision.gameObject.TryGetComponent(out CargoBox box))
            {
                box.SwitchSprite(true);
            }
               
            //Debug.Log("Box entered");
            animator.SetBool("ContainsBox", true);
            occupied = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("MoveableBox"))
        {
            if (collision.gameObject.TryGetComponent(out CargoBox box))
            {
                box.SwitchSprite(true);
            }
            //Debug.Log("Box exited");

            animator.SetBool("ContainsBox", false);
            occupied = false;
        }
    }
}
