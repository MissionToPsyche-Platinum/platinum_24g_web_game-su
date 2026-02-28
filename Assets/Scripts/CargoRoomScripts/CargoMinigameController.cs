using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CargoMinigameController : MonoBehaviour
{

    private GameObject[] targets;
    private bool gameComplete;
    private GameObject player;
    private new RectTransform transform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targets = GameObject.FindGameObjectsWithTag("CargoroomTarget");
        Debug.Log(targets.Length);
        player = GameObject.FindGameObjectWithTag("Player");
        transform = GameObject.Find("CompletedPanel").GetComponent<RectTransform>();
        transform.anchoredPosition = new Vector2(1000f, 1000f);
        gameComplete = false;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        foreach (GameObject target in targets)
        {   
            if (!target.GetComponent<Target>().occupied)
            {
                gameComplete = false;
            }
            else
            {
                gameComplete = true;
            }
        }*/
        gameComplete = targets.All(target => target.GetComponent<Target>().occupied);

        if (gameComplete)
        {
            Invoke("EndMinigame", 1.0f);
        }
        

    }

    private void EndMinigame()
    {

        transform.anchoredPosition = new Vector2(0f, 0f);
        player.GetComponent<PlayerMovement2D>().enabled = false;
        player.GetComponent<Animator>().SetBool("IsMoving", false);

    }
}
