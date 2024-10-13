using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPvP : NetworkBehaviour
{
    [SerializeField] private ChampSelectPvP selectChampPvP;
    [SerializeField] private TMP_InputField createRoomIF;
    [SerializeField] private TMP_InputField joinRoomIF;
    [SerializeField] private Button createRoomBT;
    [SerializeField] private Button joinRoomBT;

    private static NetworkVariable<FixedString64Bytes> roomID =
        new NetworkVariable<FixedString64Bytes>(default, 
            NetworkVariableReadPermission.Everyone, 
            NetworkVariableWritePermission.Server);

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

        if (string.IsNullOrEmpty(createRoomID))
        {
            Debug.Log("Invalid ID");
            return;
        }

        if (!string.IsNullOrEmpty(roomID.Value.ToString()))
        {
            Debug.Log("A room already exists. Please join the existing room.");
            return;
        }

        // Start room as host
        NetworkManager.Singleton.StartHost();
    }

    public void JoinRoom()
    {
        string joinRoomID = joinRoomIF.text;

        if (string.IsNullOrEmpty(joinRoomID))
        {
            Debug.Log("Invalid Room ID");
            return;
        }

        NetworkManager.Singleton.StartClient();
    }

    public override void OnNetworkSpawn()
    {
        if (IsHost)
        {
            roomID.Value = new FixedString64Bytes(createRoomIF.text);
            selectChampPvP.RoomID = roomID.Value.ToString();
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
        if (roomID.Value.ToString() == joinRoomID && NetworkManager.Singleton.ConnectedClients.Count == 2)
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
            Debug.Log("join sucess");
            selectChampPvP.RoomID = roomID.Value.ToString();
            selectChampPvP.gameObject.SetActive(true);
        }
    }
}
