using Unity.Mathematics.Geometry;
using UnityEngine;

public class CargoBox : MonoBehaviour
{
    private Rigidbody2D rb;
    public GameObject warning;
    public GameObject correction;
    
    [SerializeField] private AudioClip[] pushSoundClips;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        correction.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(rb.linearVelocity.y != 0 || rb.linearVelocity.x != 0)
        {
            
            Vector2 movementVelocity = rb.linearVelocity *= -0.99f;
            rb.linearVelocity = Vector2.Max(movementVelocity, Vector2.zero);
            //SoundFXManager.instance.PlayRandomSoundFXClip(pushSoundClips, transform, 1f);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {

        if (hit.transform.CompareTag("Player"))
        {
            Vector2 pushDir = new Vector2(hit.moveDirection.x, hit.moveDirection.y);

            rb.linearVelocity = pushDir;
        }
    }

    public void SwitchSprite(bool finished)
    {
        correction.SetActive(finished);
        warning.SetActive(!finished);
    }

}
