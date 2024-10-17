using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class ChampSelectPvP : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI roomIDText;
    [SerializeField] private GameObject clientLabel;
    [SerializeField] private Button startButton;

    private void Start()
    {
        // set room id
        string roomID = NetworkManager.Singleton.GetComponent<UnityTransport>()
            .ConnectionData.Port.ToString();

        roomIDText.text = $"Room ID: {roomID}";

        // set up ui depend on server or client
        if (!IsServer)
        {
            SetUpClientUIServerRpc(NetworkManager.Singleton.LocalClientId);
            startButton.onClick.AddListener(() =>
            {

            });
        }
        else
            startButton.onClick.AddListener(() =>
            {

            });
    }

    [ServerRpc (RequireOwnership = false)]
    public void SetUpClientUIServerRpc(ulong clientId)
    {
        // active client player label when client join
        SetUpClientUIClientRpc(); 

        // set client player start button to ready
        UpdateStartButtonTextClientRpc(clientId);
    }

    [ClientRpc]
    private void SetUpClientUIClientRpc()
    {
        clientLabel.SetActive(true);      
    }

    [ClientRpc]
    private void UpdateStartButtonTextClientRpc(ulong clientId)
    {
        // just update if this client 
        if (NetworkManager.Singleton.LocalClientId != clientId)
            return;

        // change start button text to ready
        Transform child = startButton.gameObject.transform.GetChild(0);
        TMP_Text childText = child.gameObject.GetComponent<TMP_Text>();
    }
}
