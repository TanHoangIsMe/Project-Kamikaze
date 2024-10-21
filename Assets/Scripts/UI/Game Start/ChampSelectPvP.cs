using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class ChampSelectPvP : NetworkBehaviour
{
    // Ready Phase
    [SerializeField] private Transform selectChampBG;
    [SerializeField] private TextMeshProUGUI roomIDText;
    [SerializeField] private GameObject clientLabel;
    [SerializeField] private Button startButton;

    private NetworkVariable<bool> isClientReady = 
        new NetworkVariable<bool>(false, 
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

    // Select Champ Phase
    [SerializeField] private GameObject hostTeam;
    [SerializeField] private GameObject clientTeam;
    [SerializeField] private GameObject champList;

    private string selectedChamp;
    private Button[] champListButtons;
    private Button[] hostSlotButtons;
    private Button[] clientSlotButtons;

    private NetworkVariable<string> hostSelectedChamp = new NetworkVariable<string>(null);
    private NetworkVariable<string> clientSelectedChamp = new NetworkVariable<string>(null);

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
                StartSelectChampClientRpc(); // start select champ phase
                // set who can click which buttons
                SetButtonOnClickPermissionClientRpc(NetworkManager.Singleton.LocalClientId);
            });
    }

    #region Server Rpc
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
    #endregion

    #region Client Rpc
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
        {
            startButton.onClick.RemoveAllListeners();

            foreach (Transform child in selectChampBG)
                child.gameObject.SetActive(!child.gameObject.activeSelf);
        }
    }

    [ClientRpc]
    private void SetButtonOnClickPermissionClientRpc(ulong clientId)
    {
        // host
        if(NetworkManager.Singleton.LocalClientId == clientId)
        {
            AddListener(hostTeam, 1, true);
            AddListener(clientTeam, 2, false);
            AddListener(champList, 3, true);
        }
        else // client
        {
            AddListener(clientTeam, 2, true);
            AddListener(hostTeam, 1, false);
            AddListener(champList, 3, true);
        }
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

    // which type = 1 -> host
    // which type = 2 -> client
    // which type = 3 -> select champ 
    private void AddListener(GameObject team, int whichType, bool canClick)
    {
        // get all slot button to set if it interactable 
        Button[] slotButtons = team.GetComponentsInChildren<Button>();

        // store button list
        if(whichType == 1) 
            hostSlotButtons = slotButtons;
        else if (whichType == 2)
            clientSlotButtons = slotButtons;
        else
            champListButtons = slotButtons;
        
        for(int i = 0; i < slotButtons.Length; i++) 
        {
            int index = i; // store index to prevent out of index bug

            if (canClick) // can click
            {
                if (whichType == 1) // host button
                    slotButtons[index].onClick.AddListener(() =>
                    {
                        SetUpChampSelectServerRpc(index, true);
                    });
                else if (whichType == 2) // client button
                    slotButtons[index].onClick.AddListener(() =>
                    {
                        SetUpChampSelectServerRpc(index, false);
                    });
                else // champ list button
                {
                    slotButtons[index].onClick.AddListener(() =>
                    {
                        SelectChampButtonClicked(slotButtons[index]);
                    });
                }
            }
            else // can not click
                slotButtons[i].interactable = false;
        }
    }
    #endregion
    private void SelectChampButtonClicked(Button clickedButton)
    {
        foreach (Button button in champListButtons)
            SelectBorderHandler(false, button);

        selectedChamp = clickedButton.gameObject.name;
        SelectBorderHandler(true, clickedButton);
    }

    // turn on-off select border 
    private void SelectBorderHandler(bool isActive, Button button)
    {
        Transform selectBorder = button.gameObject.transform.GetChild(0);
        if (selectBorder != null)
            selectBorder.gameObject.SetActive(isActive);
    }

    [ServerRpc (RequireOwnership = false)]
    private void SetUpChampSelectServerRpc(int buttonIndex, bool isHost)
    {
        SetUpChampSelectClientRpc(buttonIndex, isHost);
    }

    [ClientRpc]
    private void SetUpChampSelectClientRpc(int buttonIndex, bool isHost)
    {
        if (selectedChamp != null)
        {
            Image image;
            if (isHost) 
                image = hostSlotButtons[buttonIndex].gameObject.GetComponent<Image>();
            else
                image = clientSlotButtons[buttonIndex].gameObject.GetComponent<Image>();

            // set up champion value
            //int champPosition = int.Parse(champListButtons[buttonIndex].gameObject.name);
            //string champName = selectedChamp.Replace(" ", "");

            // remove champion slot
            if (selectedChamp == "None")
            {
                image.sprite = Resources.Load<Sprite>("Art/UI/Game Start/Others/Add Champion Image");

                //    //foreach (KeyValuePair<int, string> champ in championList)
                //    //    if (champ.Key == champPosition)
                //    //    {
                //    //        championList.Remove(champ.Key);
                //    //        break;
                //    //    }
            }
            else // add champion to slot
            {
                image.sprite = Resources.Load<Sprite>($"Art/UI/Character Avatars/{selectedChamp} Avatar");
            }
        }
    }
}
