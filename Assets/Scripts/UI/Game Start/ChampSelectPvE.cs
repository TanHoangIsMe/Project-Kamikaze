using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChampSelectPvE : MonoBehaviour
{
    [SerializeField] GameObject selectChampionList;
    [SerializeField] GameObject playerTeam;
    [SerializeField] GameObject enemyTeam;

    private string selectedChamp;

    private Button[] champSelectButtons;
    private Button[] playerSlotButtons;
    private Button[] enemySlotButtons;

    private Dictionary<int, string> championList;

    private void Awake()
    {
        championList = new Dictionary<int, string>();
    }

    private void Start()
    {
        AddOnClickListener(playerSlotButtons, playerTeam, true);
        AddOnClickListener(enemySlotButtons, enemyTeam, true);
        AddOnClickListener(champSelectButtons, selectChampionList, false);
    }

    public void SendChampionListToPvEMode()
    {
        if (championList.Count > 0)
        {
            bool isPlayerTeamNumberGreaterThan0 = false;
            bool isEnemyTeamNumberGreaterThan0 = false;

            foreach(KeyValuePair<int,string> champ in championList)
                if (new[] { 0, 1, 2, 3, 4 }.Contains(champ.Key))
                    isEnemyTeamNumberGreaterThan0 = true;
                else if (new[] { 6, 7, 8, 9, 10 }.Contains(champ.Key))
                    isPlayerTeamNumberGreaterThan0 = true;

            // send champion list to pve mode
            if ( isEnemyTeamNumberGreaterThan0 && isPlayerTeamNumberGreaterThan0)
            {
                DataManager.Instance.championList = championList;
                SceneManager.LoadSceneAsync(1);
            }
        }
    }

    private void SelectSlotButtonClicked(Button clickedButton)
    {
        if(selectedChamp != null)
        {
            Image image = clickedButton.gameObject.GetComponent<Image>();

            // set up champion value
            int champPosition = int.Parse(clickedButton.gameObject.name);
            string champName = selectedChamp.Replace(" ", "");

            // remove champion slot
            if (selectedChamp == "None")
            {
                image.sprite = Resources.Load<Sprite>("Art/UI/GameStart/Add Champion Image");

                foreach (KeyValuePair<int, string> champ in championList)
                    if (champ.Key == champPosition)
                    { 
                        championList.Remove(champ.Key);
                        break;
                    }
            }
            else // add champion to slot
            {
                image.sprite = Resources.Load<Sprite>($"Art/UI/Character Avatars/{selectedChamp} Avatar");

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

    private void SelectChampButtonClicked(Button clickedButton)
    {
        foreach (Button button in champSelectButtons)
            SelectBorderHandler(false, button);

        selectedChamp = clickedButton.gameObject.name;
        SelectBorderHandler(true, clickedButton);
    }

    // add listener to button
    private void AddOnClickListener(Button[] buttons, GameObject gameObject, bool isSlot)
    {
        buttons = gameObject.GetComponentsInChildren<Button>();

        if (!isSlot)
            champSelectButtons = buttons;

        foreach (Button button in buttons)
            if(isSlot)
                button.onClick.AddListener(() => SelectSlotButtonClicked(button));
            else
                button.onClick.AddListener(() => SelectChampButtonClicked(button));
    }

    // turn on-off select border 
    private void SelectBorderHandler(bool isActive, Button button)
    {
        Transform selectBorder = button.gameObject.transform.GetChild(0);
        if (selectBorder != null)
            selectBorder.gameObject.SetActive(isActive);
    }
}
