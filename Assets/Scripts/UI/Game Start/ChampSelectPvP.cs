using System.Collections.Generic;
using TMPro;
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

    private Button[] champListButtons;
    private Button[] hostSlotButtons;
    private Button[] clientSlotButtons;

    private string hostSelectedChamp;
    private string clientSelectedChamp;

    private Dictionary<int, string> championList;

    private void Start()
    {
        // set room id
        string roomID = NetworkManager.Singleton.GetComponent<UnityTransport>()
            .ConnectionData.Port.ToString();

        roomIDText.text = $"Room ID: {roomID}";

        championList = new Dictionary<int, string>();

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
                // start select champ phase
                StartSelectChampClientRpc();
                
                // set who can click which buttons
                SetButtonOnClickPermissionClientRpc(NetworkManager.Singleton.LocalClientId);
            });
    }

    private void SelectChampButtonClicked(Button clickedButton)
    {
        // turn off all select border ui
        foreach (Button button in champListButtons)
            SelectBorderHandler(false, button);

        // store host or client select champ
        if (IsHost)
            //hostSelectedChamp.Value = new List<byte>(Encoding.UTF8.GetBytes(clickedButton.name));
            UpdateHostSelectChampClientRpc(clickedButton.name);
        else
            // request server update client champ select
            UpdateClientSelectChampServerRpc(clickedButton.name);

        // turn on champ select border ui
        SelectBorderHandler(true, clickedButton);
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

    [ServerRpc (RequireOwnership = false)]
    private void UpdateClientSelectChampServerRpc(string champName)
    {
        UpdateClientSelectChampClientRpc(champName);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetUpClientChampSelectServerRpc(int buttonIndex, string selectChamp)
    {
        SetUpClientChampSelectClientRpc(buttonIndex, selectChamp);
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
    private void UpdateClientSelectChampClientRpc(string champName)
    {
        clientSelectedChamp = champName;
    }

    [ClientRpc]
    private void UpdateHostSelectChampClientRpc(string champName)
    {
        hostSelectedChamp = champName;
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

    [ClientRpc]
    private void SetUpHostChampSelectClientRpc(int buttonIndex, string selectChamp)
    {
        Image image;
        
        if (selectChamp != null)
        {
            image = hostSlotButtons[buttonIndex].gameObject.GetComponent<Image>();

            // set up champion value
            int champPosition = int.Parse(hostSlotButtons[buttonIndex].gameObject.name);
            string champName = selectChamp.Replace(" ", "");

            // remove champion slot
            if (selectChamp == "None")
            {
                image.sprite = Resources.Load<Sprite>("Art/UI/Game Start/Others/Add Champion Image");

                foreach (KeyValuePair<int, string> champ in championList)
                    if (champ.Key == champPosition)
                    {
                        championList.Remove(champ.Key);
                        break;
                    }
            }
            else // add champion to slot
            {
                image.sprite = Resources.Load<Sprite>($"Art/UI/Character Avatars/{selectChamp} Avatar");

                bool isExisted = false; // flag to know a slot already have champ

                // if have just replace champ name
                foreach (KeyValuePair<int, string> champ in championList)
                    if (champ.Key == champPosition)
                    {
                        championList[champ.Key] = champName;
                        isExisted = true;
                        break;
                    }

                if (!isExisted) // else create new champ slot
                    championList.Add(champPosition, champName);
            }
        }
    }

    [ClientRpc]
    private void SetUpClientChampSelectClientRpc(int buttonIndex, string selectChamp)
    {
        Image image;

        if (selectChamp != null)
        {
            image = clientSlotButtons[buttonIndex].gameObject.GetComponent<Image>();

            // set up champion value
            int champPosition = int.Parse(clientSlotButtons[buttonIndex].gameObject.name);
            string champName = selectChamp.Replace(" ", "");

            // remove champion slot
            if (selectChamp == "None")
            {
                image.sprite = Resources.Load<Sprite>("Art/UI/Game Start/Others/Add Champion Image");

                foreach (KeyValuePair<int, string> champ in championList)
                    if (champ.Key == champPosition)
                    {
                        championList.Remove(champ.Key);
                        break;
                    }
            }
            else // add champion to slot
            {
                image.sprite = Resources.Load<Sprite>($"Art/UI/Character Avatars/{selectChamp} Avatar");

                bool isExisted = false; // flag to know a slot already have champ

                // if have just replace champ name
                foreach (KeyValuePair<int, string> champ in championList)
                    if (champ.Key == champPosition)
                    {
                        championList[champ.Key] = champName;
                        isExisted = true;
                        break;
                    }

                if (!isExisted) // else create new champ slot
                    championList.Add(champPosition, champName);
            }
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
                        SetUpHostChampSelectClientRpc(index, hostSelectedChamp);
                    });
                else if (whichType == 2) // client button
                    slotButtons[index].onClick.AddListener(() =>
                    {
                        SetUpClientChampSelectServerRpc(index, clientSelectedChamp);
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

    // turn on-off select border 
    private void SelectBorderHandler(bool isActive, Button button)
    {
        Transform selectBorder = button.gameObject.transform.GetChild(0);
        if (selectBorder != null)
            selectBorder.gameObject.SetActive(isActive);
    }
    #endregion   
}
