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
            Debug.Log("Invalid Room ID");
            return;
        }

        // check room id in use
        if (IsPortInUse(int.Parse(createRoomID)))
        {
            Debug.Log($"Port {createRoomID} is in use.");
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
            Debug.Log("Invalid Room ID");
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
            Debug.Log($"Port {joinRoomID} is not exist.");
        }          
    }

    public override void OnNetworkSpawn()
    {
        if (IsHost)
        {
            MoveToSelectChampPhase();
        }
        else if(IsClient)
        {
            CheckRoomStatusServerRpc(NetworkManager.Singleton.LocalClientId);
        }
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
        // just send notify to local client
        if (NetworkManager.Singleton.LocalClientId != clientId)
            return;

        if (!canJoin)
        {
            Debug.Log("Room is full. Disconnecting...");
            NetworkManager.Singleton.Shutdown();
        }
        else
        {
            Debug.Log("join success");
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
        createRoomIF.text = "";
        joinRoomIF.text = "";
    }
}
