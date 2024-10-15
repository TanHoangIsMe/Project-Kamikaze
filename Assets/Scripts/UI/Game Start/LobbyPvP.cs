using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LobbyPvP : NetworkBehaviour
{
    [SerializeField] private ChampSelectPvP selectChampPvP;
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
            //selectChampPvP.RoomID = roomID.Value.ToString();
            selectChampPvP.gameObject.SetActive(true);
        }
        else if(IsClient)
        {
            CheckRoomStatusServerRpc(joinRoomIF.text);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void CheckRoomStatusServerRpc(string joinRoomID)
    {
        if (NetworkManager.Singleton.ConnectedClients.Count == 2)
            NotifyClientJoinRoomClientRpc(true);
        else
            NotifyClientJoinRoomClientRpc(false);
    }

    [ClientRpc]
    private void NotifyClientJoinRoomClientRpc(bool canJoin)
    {
        if (!canJoin)
        {
            Debug.Log("Room is full. Disconnecting...");
            NetworkManager.Singleton.Shutdown();
        }
        else
        {
            Debug.Log("join success");
            //selectChampPvP.RoomID = roomID.Value.ToString();
            selectChampPvP.gameObject.SetActive(true);
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
}
