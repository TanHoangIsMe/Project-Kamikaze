using TMPro;
using UnityEngine;

public class ChampSelectPvP : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roomIDText;

    private string roomID;
    public string RoomID { set { roomID = value; } }

    private void Start()
    {
        roomIDText.text = $"Room ID: {roomID}";
    }

}
