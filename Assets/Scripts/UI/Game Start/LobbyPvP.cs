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
            CreateRoom();
        });

        // player join room as client
        joinRoomBT.onClick.AddListener(() => {
            JoinRoom();
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

    private async void CreateRoom()
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

    private async void JoinRoom()
    {
        string joinRoomID = joinRoomIF.text;

        joinRoomIF.text = ""; // reset input text

        // check input text empty
        if (string.IsNullOrEmpty(joinRoomID))
        {
            ShowAlert(
                "Input Required",
                "Please enter join code before submitting.");
            return;
        }

        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinRoomID);
            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            ShowAlert(
                "Join Room Failed",
                "Something went wrong while try to join room. Please try again.");
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
