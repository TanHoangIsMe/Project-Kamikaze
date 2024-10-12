using TMPro;
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

    private NetworkVariable<string> roomID;

    private void Awake()
    {
        roomID = new NetworkVariable<string>(string.Empty);

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

        // start room as host
        NetworkManager.Singleton.StartHost();

        roomID.Value = createRoomID;        
        selectChampPvP.gameObject.SetActive(true);
        selectChampPvP.RoomID = roomID.Value;
    }

    public void JoinRoom()
    {
        string joinRoomID = joinRoomIF.text;

        if (IsRoomAvailable(joinRoomID))
        {
            // join room
            NetworkManager.Singleton.StartClient();
            selectChampPvP.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("Invalid");
        }
    }

    // check join room condition
    private bool IsRoomAvailable(string roomID)
    {
        return roomID == this.roomID.Value && IsRoomFull() == false;
    }

    private bool IsRoomFull()
    {
        // 2 player limit
        return NetworkManager.Singleton.ConnectedClients.Count >= 2;
    }
}
