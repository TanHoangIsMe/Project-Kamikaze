using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ChampSelectPvE : MonoBehaviour
{
    [SerializeField] GameObject loadingCanvas;
    [SerializeField] GameObject selectChampionList;
    [SerializeField] GameObject playerTeam;
    [SerializeField] GameObject enemyTeam;

    private string selectedChamp;

    private Button[] champSelectButtons;
    private Button[] playerSlotButtons;
    private Button[] enemySlotButtons;

    private Dictionary<int, string> championList;

    private void Start()
    {
        championList = new Dictionary<int, string>();

        AddOnClickListener(champSelectButtons, selectChampionList, 1);
        AddOnClickListener(playerSlotButtons, playerTeam, 2);
        AddOnClickListener(enemySlotButtons, enemyTeam, 3);
    }

    public void ResetScene()
    {
        selectedChamp = null; // reset select champ flag 

        // reset player squad
        foreach (Button button in playerSlotButtons)
        {
            Image image = button.gameObject.GetComponent<Image>();
            if (image != null)
                image.sprite = Resources.Load<Sprite>("Art/UI/Game Start/Add Champion Image");
        }

        // reset enemy squad
        foreach (Button button in enemySlotButtons)
        {
            Image image = button.gameObject.GetComponent<Image>();
            if (image != null)
                image.sprite = Resources.Load<Sprite>("Art/UI/Game Start/Add Champion Image");
        }

        // turn off all select champ border
        foreach (Button button in champSelectButtons)
            SelectBorderHandler(false, button);

        // reset champion list
        championList.Clear();
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
                // clone champion for send data cause champ list data will be reset
                Dictionary<int,string> cloneChampList = new Dictionary<int,string>(championList);
                ResetScene(); // reset this values
                DataManager.Instance.championList = cloneChampList;

                // open loading scene
                LoadingScene loadingScene = loadingCanvas.GetComponent<LoadingScene>();

                if (loadingScene != null)
                {
                    loadingCanvas.SetActive(true);
                    loadingScene.LoadScene(1);
                    gameObject.SetActive(false);
                }
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
    // whichButtons = 1 -> champSelectButtons
    // whichButtons = 2 -> playerSlotButtons
    // whichButtons = 3 -> enemySelectButtons
    private void AddOnClickListener(Button[] buttons, GameObject gameObject, int whichButtons)
    {
        buttons = gameObject.GetComponentsInChildren<Button>();

        if (whichButtons == 1)
            champSelectButtons = buttons;
        else if (whichButtons == 2)
            playerSlotButtons = buttons;
        else
            enemySlotButtons = buttons;

        // add listener
        foreach (Button button in buttons)
            if(whichButtons == 1)
                button.onClick.AddListener(() => SelectChampButtonClicked(button));
            else
                button.onClick.AddListener(() => SelectSlotButtonClicked(button));
    }

    // turn on-off select border 
    private void SelectBorderHandler(bool isActive, Button button)
    {
        Transform selectBorder = button.gameObject.transform.GetChild(0);
        if (selectBorder != null)
            selectBorder.gameObject.SetActive(isActive);
    }
}
