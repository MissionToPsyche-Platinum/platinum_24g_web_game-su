using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnPointFinder : MonoBehaviour
{
    [SerializeField] private Transform player;
    private GameObject spawnPoint;
    private string currScene;

    private void OnEnable()
    {
        currScene = "MainHall";
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneLoaded;   
    }


    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
        if(scene.name == "CargoRoom"|| scene.name == "PowerRoom")
        {
            spawnPoint = GameObject.Find("Spawnpoint");
            player.SetPositionAndRotation(spawnPoint.transform.position, player.transform.rotation);
            Physics.SyncTransforms();
        }

        if(scene.name == "MainHall")
        {
            if(currScene == "CargoRoom")
            {
                spawnPoint = GameObject.Find("SpawnFromCargo");
                player.SetPositionAndRotation(spawnPoint.transform.position, player.transform.rotation);
                Physics.SyncTransforms();
            }
            if(currScene == "PowerRoom")
            {
                spawnPoint = GameObject.Find("SpawnFromPower");
                player.SetPositionAndRotation(spawnPoint.transform.position, player.transform.rotation);
                Physics.SyncTransforms();   
            }
        }
        currScene = scene.name;
    }
}
