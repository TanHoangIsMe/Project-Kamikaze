using Unity.Netcode;
using UnityEngine;

public class GameplayControllerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject player1Cam;
    [SerializeField] private GameObject player2Cam;
    [SerializeField] private GameObject gameplayControllerPvP;
    [SerializeField] private GameObject skillControllerPvP;
    [SerializeField] private GameObject settingCanvasPvP;
    [SerializeField] private GameObject priorityCanvasPvP;

    private void Awake()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            // spawn net object
            SpawnSharedObject(gameplayControllerPvP);
            SpawnSharedObject(skillControllerPvP);
            SpawnSharedObject(settingCanvasPvP);
            SpawnSharedObject(priorityCanvasPvP);

            // setting cam for each player
            player1Cam.SetActive(true);
            player2Cam.SetActive(false);
        }
        else
        {
            player1Cam.SetActive(false);
            player2Cam.SetActive(true);
        }
    }

    private void SpawnSharedObject(GameObject gameObject)
    {
        GameObject spawnObject = Instantiate(gameObject);
        NetworkObject networkObject = spawnObject.GetComponent<NetworkObject>();
        if (networkObject != null) networkObject.Spawn();
    }
}

