using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class ChampSelectPvP : NetworkBehaviour
{
    [SerializeField] private Transform selectChampBG;
    [SerializeField] private TextMeshProUGUI roomIDText;
    [SerializeField] private GameObject clientLabel;
    [SerializeField] private Button startButton;

    private NetworkVariable<bool> isClientReady = 
        new NetworkVariable<bool>(false, 
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

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
                ClientReadyServerRpc(NetworkManager.Singleton.LocalClientId);
            });
        }
        else
            startButton.onClick.AddListener(() =>
            {
                StartSelectChampClientRpc();
            });
    }

    #region Rpc
    [ServerRpc (RequireOwnership = false)]
    public void SetUpClientUIServerRpc(ulong clientId)
    {
        // active client player label when client join
        SetUpClientUIClientRpc(); 

        // set client player start button to ready
        UpdateStartButtonTextClientRpc(clientId);
    }

    [ServerRpc (RequireOwnership = false)]
    private void ClientReadyServerRpc(ulong clientId)
    {
        // update client state
        isClientReady.Value = !isClientReady.Value;

        ClientReadyClientRpc(clientId, isClientReady.Value);
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
        UpdateStartButtonText("Ready");
    }

    [ClientRpc]
    private void ClientReadyClientRpc(ulong clientId, bool isReady)
    {
        if (isReady)
        {
            // client player ready
            UpdateClientState(clientId, true);
        }
        else
        {
            UpdateClientState(clientId, false);
        }
    }

    [ClientRpc]
    private void StartSelectChampClientRpc()
    {
        // active all inactive object and vice versa
        if (isClientReady.Value)
            foreach (Transform child in selectChampBG)
                child.gameObject.SetActive(!child.gameObject.activeSelf);
    }
    #endregion

    #region Reuse Code
    private void UpdateStartButtonText(string text)
    {
        Transform child = startButton.gameObject.transform.GetChild(0);
        if (child != null)
        {
            TMP_Text childText = child.gameObject.GetComponent<TMP_Text>();
            if (childText != null)
                childText.text = text;
        }
    }

    private void UpdateClientState(ulong clientId, bool isReady)
    {
        // active ready icon
        clientLabel.transform.GetChild(0).gameObject.SetActive(isReady);

        // just update if this client 
        if (NetworkManager.Singleton.LocalClientId != clientId)
            return;

        // change start button text to ready
        if (isReady)           
            UpdateStartButtonText("Cancel");
        else
            UpdateStartButtonText("Ready");
    }
    #endregion
}
