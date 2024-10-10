using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPvP : MonoBehaviour
{
    [SerializeField] private GameObject loadingCanvas;
    //[SerializeField] private InputField createRoomIF;
    //[SerializeField] private InputField joinRoomIF;
    [SerializeField] private Button createRoomBT;
    [SerializeField] private Button joinRoomBT;

    //private string currentRoomID;
    private LoadingScene loadingScene;

    private void Awake()
    {
        loadingScene = loadingCanvas.GetComponent<LoadingScene>();

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
        //currentRoomID = createRoomIF.text;

        //if (string.IsNullOrEmpty(currentRoomID))
        //{
        //    Debug.Log("Invalid ID");
        //    return;
        //}

        // start room as host
        NetworkManager.Singleton.StartHost();

        // open loading scene
        if (loadingScene != null)
        {
            loadingCanvas.SetActive(true);
            loadingScene.LoadScene(2);
            gameObject.SetActive(false);
        }
        //Debug.Log($"Host started with room ID: {currentRoomID}");
    }

    public void JoinRoom()
    {
        //string roomIDToJoin = joinRoomIF.text;

        //if (IsRoomAvailable(roomIDToJoin))
        //{
            // join room
            NetworkManager.Singleton.StartClient();

            // open loading scene
            if (loadingScene != null)
            {
                loadingCanvas.SetActive(true);
                loadingScene.LoadScene(2);
                gameObject.SetActive(false);
            }
        //    Debug.Log($"Joining room ID: {roomIDToJoin}");
        //}
        //else
        //{
        //    Debug.Log("Invalid");
        //}
    }

    //// check join room condition
    //private bool IsRoomAvailable(string roomID)
    //{
    //    return roomID == currentRoomID && IsRoomFull() == false;
    //}

    //private bool IsRoomFull()
    //{
    //    // 2 player limit
    //    return NetworkManager.Singleton.ConnectedClients.Count >= 2; 
    //}
}
