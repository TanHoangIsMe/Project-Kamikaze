using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LobbyPvP : NetworkBehaviour
{
    [SerializeField] private GameObject selectChampPvP;
    [SerializeField] private TMP_InputField createRoomIF;
    [SerializeField] private TMP_InputField joinRoomIF;
    [SerializeField] private Button createRoomBT;
    [SerializeField] private Button joinRoomBT;
    [SerializeField] private GameObject alertCanvas;
    [SerializeField] private TextMeshProUGUI errorTittle;
    [SerializeField] private TextMeshProUGUI errorMessage;

    private void Awake()
    {
        // player create room as host 
        createRoomBT.onClick.AddListener(() => {
            CreateRoom();
        });

        // player join room as client
        joinRoomBT.onClick.AddListener(() => {
            JoinRoom();
        });
    }

    private void CreateRoom()
    {
        string createRoomID = createRoomIF.text;

        createRoomIF.text = ""; // reset input text

        // check input text 
        if (string.IsNullOrEmpty(createRoomID) || createRoomID.Length != 4 || !IsNumeric(createRoomID))
        {
            ShowAlert(
                "Invalid ID",
                "Please enter a ID that is numeric and 4 characters long.");
            return;
        }

        // check room id in use
        if (IsPortInUse(int.Parse(createRoomID)))
        {
            ShowAlert(
                "Already Existed",
                "The room ID is in use, please try another ID");
        }
        else
        {
            // Update Port
            NetworkManager.Singleton.GetComponent<UnityTransport>()
                .ConnectionData.Port = ushort.Parse(createRoomID);

            // Start room as host
            NetworkManager.Singleton.StartHost();
        }
    }

    public void JoinRoom()
    {
        string joinRoomID = joinRoomIF.text;

        joinRoomIF.text = ""; // reset input text

        // check input text
        if (string.IsNullOrEmpty(joinRoomID) || joinRoomID.Length != 4 || !IsNumeric(joinRoomID))
        {
            ShowAlert(
                "Invalid ID",
                "Please enter a ID that is numeric and 4 characters long.");
            return;
        }

        // check room id in use
        if (IsPortInUse(int.Parse(joinRoomID)))
        {
            // Update Port
            NetworkManager.Singleton.GetComponent<UnityTransport>()
            .ConnectionData.Port = ushort.Parse(joinRoomID);

            // join room as client
            NetworkManager.Singleton.StartClient();           
        }
        else
        {
            ShowAlert(
                "Invalid Room",
                "The room does not exist, please join another room.");
        }          
    }

    public override void OnNetworkSpawn()
    {  
        if(IsHost)
            MoveToSelectChampPhase();
        else if(IsClient)
            CheckRoomStatusServerRpc(NetworkManager.Singleton.LocalClientId);         
    }

    private void SpawnSelectChampCanvas()
    {
        GameObject selectChampCanvas = 
            GameObject.FindGameObjectWithTag("SelectChampPvP");

        if (selectChampCanvas == null)
        {
            selectChampCanvas = Instantiate(selectChampPvP);
            selectChampCanvas.GetComponent<NetworkObject>().Spawn();
        }
        
        SendLobbyInfoToSelectChamp(selectChampCanvas);
    }

    private void SendLobbyInfoToSelectChamp(GameObject selectChampCanvas)
    {
        ChampSelectPvP champSelectPvP = selectChampCanvas.GetComponent<ChampSelectPvP>();
        if (champSelectPvP != null)
            champSelectPvP.LobbyPvp = gameObject;
    }

    [ServerRpc(RequireOwnership = false)]
    private void CheckRoomStatusServerRpc(ulong clientId)
    {
       // check if room has 2 player
       bool canJoin = NetworkManager.Singleton.ConnectedClients.Count == 2;
       NotifyClientJoinRoomClientRpc(canJoin, clientId);
    }

    [ClientRpc]
    private void NotifyClientJoinRoomClientRpc(bool canJoin, ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId != clientId)
            return;

        if (!canJoin)
        {
            ShowAlert(
                "Maximum Reach",
                "The room is full, please join another room.");

            NetworkManager.Singleton.Shutdown();
        }
        else
        {
            MoveToSelectChampPhase();
        }
    }

    private bool IsPortInUse(int port)
    {
        return System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties()
            .GetActiveUdpListeners()
            .Any(p => p.Port == port);
    }

    // check if string contain all number
    private bool IsNumeric(string str)
    {
        foreach (char c in str)
        {
            if (!char.IsDigit(c))
            {
                return false;
            }
        }
        return true;
    }

    private void MoveToSelectChampPhase()
    {
        // spawn shared object to select champ
        SpawnSelectChampCanvas();

        // reset input field
        ResetScene();

        // turn off join room UI
        gameObject.SetActive(false);
    }

    public void ResetScene()
    {
        // reset input field
        createRoomIF.text = "";
        joinRoomIF.text = "";
    }

    private void ShowAlert(string tittle, string message)
    {
        alertCanvas.SetActive(true);
        errorTittle.text = tittle;
        errorMessage.text = message;
    }

    public void ConfirmAlert()
    {
        alertCanvas.SetActive(false);
    }
}
