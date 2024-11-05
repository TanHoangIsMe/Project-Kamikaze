using Unity.Netcode;
using UnityEngine;

public class GameplayControllerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject gameplayControllerPvP;

    private void Awake()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            GameObject gameplayController = Instantiate(gameplayControllerPvP);
            NetworkObject networkObject = gameplayController.GetComponent<NetworkObject>();
            if (networkObject != null) networkObject.Spawn();
        }
    }
}

