using Unity.Netcode;
using UnityEngine;

public class PreventNetManagerSpawn : MonoBehaviour
{
    private static NetworkManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = gameObject.GetComponent<NetworkManager>();
            DontDestroyOnLoad(gameObject); // Not destroy object
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
