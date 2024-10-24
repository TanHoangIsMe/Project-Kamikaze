using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ChampSelectPvP : NetworkBehaviour
{
    // Ready Phase
    [SerializeField] private Transform selectChampBG;
    [SerializeField] private TextMeshProUGUI roomIDText;
    [SerializeField] private GameObject clientLabel;
    [SerializeField] private Button startPickButton;
    [SerializeField] private Button startMatchButton;

    private bool isReadyPick = false;

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

    private bool isReadyMatch = false;

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
            SetUpClientUIServerRpc();

            startPickButton.onClick.AddListener(() =>
            {
                ReadyServerRpc(true);
            });
        }
        else
            startPickButton.interactable = false;

            startPickButton.onClick.AddListener(() =>
            {
                // start select champ phase
                StartSelectChampClientRpc();
                
                // set who can click which buttons
                SetButtonOnClickPermissionClientRpc();
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
    public void SetUpClientUIServerRpc()
    {
        // active client player label when client join
        SetUpClientUIClientRpc(); 

        // set client player start button to ready
        UpdateStartButtonTextClientRpc();
    }

    [ServerRpc (RequireOwnership = false)]
    private void ReadyServerRpc(bool isPick)
    {
        ReadyClientRpc(isPick);
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
    private void UpdateStartButtonTextClientRpc()
    {
        if(!IsHost)
            // change start button text to ready
            UpdateStartButtonText("Ready", startPickButton);
    }

    [ClientRpc]
    private void ReadyClientRpc(bool isPick)
    {
        UpdateReadyState(isPick);
    }

    [ClientRpc]
    private void StartSelectChampClientRpc()
    {
        // active all inactive object and vice versa
        if (isReadyPick)
        {
            startPickButton.onClick.RemoveAllListeners();

            foreach (Transform child in selectChampBG)
            {
                if (child.name != "VS")
                    child.gameObject.SetActive(!child.gameObject.activeSelf);
            }

            if (!IsHost)
            {
                UpdateStartButtonText("Ready", startMatchButton);
                startMatchButton.onClick.AddListener(() =>
                {
                    ReadyServerRpc(false);
                });
            }
            else
                startMatchButton.interactable = false;
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
    private void SetButtonOnClickPermissionClientRpc()
    {
        // host
        if(IsHost)
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
    private void UpdateStartButtonText(string text, Button button)
    {
        Transform child = button.gameObject.transform.GetChild(0);
        if (child != null)
        {
            TMP_Text childText = child.gameObject.GetComponent<TMP_Text>();
            if (childText != null)
                childText.text = text;
        }
    }

    // isPick = true -> ready to start pick state
    // isPick = false -> ready to start match state
    private void UpdateReadyState(bool isPick)
    {
        if (isPick)
        {
            isReadyPick = !isReadyPick;

            // active ready icon
            clientLabel.transform.GetChild(0).gameObject.SetActive(isReadyPick);

            if (!IsHost)
            {
                // change start button text to ready
                if (isReadyPick)
                    UpdateStartButtonText("Cancel", startPickButton);
                else
                    UpdateStartButtonText("Ready", startPickButton);
            }

            if (IsHost) // active or inactive host start match button
                if (isReadyPick)
                    startPickButton.interactable = true;
                else
                    startPickButton.interactable = false;
        }
        else
        {
            isReadyMatch = !isReadyMatch;

            if (!IsHost)
            {
                // change start button text to ready
                if (isReadyMatch)
                    UpdateStartButtonText("Cancel", startMatchButton);
                else
                    UpdateStartButtonText("Ready", startMatchButton);
            }
            
            if (IsHost) // active or inactive host start match button
                if(isReadyMatch)
                    startMatchButton.interactable = true;
                else
                    startMatchButton.interactable = false;
        }
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
