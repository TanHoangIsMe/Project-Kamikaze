using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Networking.Transport.Relay;

public class LobbyPvP : NetworkBehaviour
{
    [SerializeField] private GameObject selectChampPvP;
    [SerializeField] private GameObject loadingScene;
    [SerializeField] private TMP_InputField createRoomIF;
    [SerializeField] private TMP_InputField joinRoomIF;
    [SerializeField] private Button createRoomBT;
    [SerializeField] private Button joinRoomBT;
    [SerializeField] private GameObject alertCanvas;
    [SerializeField] private TextMeshProUGUI errorTittle;
    [SerializeField] private TextMeshProUGUI errorMessage;

    private string joinCode;

    private void Awake()
    {
        // player create room as host 
        createRoomBT.onClick.AddListener(() => {
            //CreateRoom();
            CreateRelay();
        });

        // player join room as client
        joinRoomBT.onClick.AddListener(() => {
            //JoinRoom();
            JoinRelay();
        });
    }

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed In" + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private async void CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            ShowAlert(
                "Create Room Failed",
                "Something went wrong while try to create room. Please try again.");
        }
    }

    private async void JoinRelay()
    {
        string joinRoomID = joinRoomIF.text;

        joinRoomIF.text = ""; // reset input text

        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinRoomID);
            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e) {
            Debug.Log(e);
            ShowAlert(
                "Join Room Failed",
                "Something went wrong while try to join room. Please try again.");
        }
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
            MoveToSelectChampPhase(true);
        else if(IsClient)
            CheckRoomStatusServerRpc(NetworkManager.Singleton.LocalClientId);         
    }

    private void SpawnSelectChampCanvas()
    {
        // spawn shared object
        GameObject selectChampCanvas = Instantiate(selectChampPvP);
        selectChampCanvas.GetComponent<NetworkObject>().Spawn();
    }

    private void SendInfoToSelectChamp()
    {
        GameObject selectChampCanvas = 
            GameObject.Find("PvP Choosing Champ Canvas(Clone)");

        if (selectChampCanvas != null && loadingScene != null)
        {
            ChampSelectPvP selectChampPvP = selectChampCanvas.GetComponent<ChampSelectPvP>();
            
            selectChampPvP.LobbyPvP = gameObject;
            selectChampPvP.LoadingCanvas = loadingScene;
            selectChampPvP.RoomID = joinCode;
        }
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
            MoveToSelectChampPhase(false);
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

    private void MoveToSelectChampPhase(bool isHost)
    {
        if(isHost)
            // spawn shared object to select champ
            SpawnSelectChampCanvas();

        SendInfoToSelectChamp();

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
        joinCode = "";
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
