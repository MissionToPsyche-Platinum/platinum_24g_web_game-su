using UnityEngine;

public class Target : MonoBehaviour
{
    private Animator animator;
    public bool occupied;

    [SerializeField] private AudioClip occupiedSoundClip;
    [SerializeField] private AudioClip unoccupiedSoundClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(GetComponent<Animator>() == null)
        {
            Debug.LogError("Target: Animator component not found");
            return;
        }
        animator = GetComponent<Animator>();
        occupied = false;
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
            if (SoundFXManager.instance != null)
            {
                SoundFXManager.instance.PlaySoundFXClip(occupiedSoundClip, transform, 1f);
            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("MoveableBox"))
        {
            if (collision.gameObject.TryGetComponent(out CargoBox box))
            {
                box.SwitchSprite(false);
            }
            //Debug.Log("Box exited");

            animator.SetBool("ContainsBox", false);
            occupied = false;
            if (SoundFXManager.instance != null && gameObject.scene.isLoaded)
            {
                SoundFXManager.instance.PlaySoundFXClip(unoccupiedSoundClip, transform, 1f);
            }

        }
    }
}
