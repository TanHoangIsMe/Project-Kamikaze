using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class ChampSelectPvP : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI roomIDText;
    [SerializeField] private GameObject clientText;

    private void Start()
    {
        // set room id
        string roomID = NetworkManager.Singleton.GetComponent<UnityTransport>()
            .ConnectionData.Port.ToString();

        roomIDText.text = $"Room ID: {roomID}";
    }

    [ServerRpc]
    public void ActiveClientTextServerRpc()
    {
        ActiveClientTextClientRpc();
    }

    [ClientRpc]
    private void ActiveClientTextClientRpc()
    {
        Debug.Log("alo");
        clientText.SetActive(true);
    }
}
