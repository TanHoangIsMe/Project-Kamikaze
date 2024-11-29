using Unity.Netcode;
using UnityEngine;

public class GameplayControllerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject player1Cam;
    [SerializeField] private GameObject player2Cam;
    [SerializeField] private GameObject gameplayControllerPvP;
    [SerializeField] private GameObject skillControllerPvp;

    private void Awake()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            GameObject gameplayController = Instantiate(gameplayControllerPvP);
            NetworkObject gamePlayNetObject = gameplayController.GetComponent<NetworkObject>();
            if (gamePlayNetObject != null) gamePlayNetObject.Spawn();

            GameObject skillController = Instantiate(skillControllerPvp);
            NetworkObject skillNetObject = skillController.GetComponent<NetworkObject>();
            if (skillNetObject != null) skillNetObject.Spawn();

            player1Cam.SetActive(true);
            player2Cam.SetActive(false);
        }
        else
        {
            player1Cam.SetActive(false);
            player2Cam.SetActive(true);
        }
    }
}

