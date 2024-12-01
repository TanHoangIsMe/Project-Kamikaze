using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Setting : NetworkBehaviour
{
    [SerializeField] private GameObject settingMenu;
    [SerializeField] private Button resumeButton;

    private LoadingScene loadingScene;
    private SetUpTurnList setUpTurnList;

    private void Start()
    {
        // setting loading scene
        loadingScene = FindObjectOfType<LoadingScene>();
        if(loadingScene != null )
            loadingScene.gameObject.SetActive(false);

        setUpTurnList = FindObjectOfType<SetUpTurnList>();
    }

    public void OpenSettingMenu()
    {
        OpenSettingMenuServerRpc(IsHost);
    }

    public void ResumeBattle()
    {
        ResumeBattleServerRpc();
    }

    public void RestartBattle()
    {
        // reload scene
        LoadScene(1);
    }  

    public void BackToMainMenu()
    {
        BackToMainMenuServerRpc(IsHost);
    }

    #region ServerRpc
    [ServerRpc (RequireOwnership = false)]
    private void OpenSettingMenuServerRpc(bool isHost)
    {
        OpenSettingMenuClientRpc(isHost);        
    }

    [ServerRpc(RequireOwnership = false)]
    private void ResumeBattleServerRpc()
    {
        ResumeBattleClientRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void BackToMainMenuServerRpc(bool isHost)
    {
        BackToMainMenuClientRpc(isHost);
    }
    #endregion

    #region ClientRpc
    [ClientRpc]
    private void OpenSettingMenuClientRpc(bool isHost)
    {
        Time.timeScale = 0;
        settingMenu.SetActive(true);

        // DeActive other player resume button
        if ((isHost && !IsHost) || (!isHost && IsHost))
            resumeButton.interactable = false;
    }

    [ClientRpc]
    private void ResumeBattleClientRpc()
    {
        Time.timeScale = 1;
        settingMenu.SetActive(false);
    }

    [ClientRpc]
    private void BackToMainMenuClientRpc(bool isHost)
    {
        settingMenu.SetActive(false);
        NetworkManager.Singleton.Shutdown();

        if ((isHost && IsHost) || (!isHost && !IsHost))
            LoadScene(0);
    }
    #endregion

    #region Loading
    private void LoadScene(int sceneId)
    {
        // open loading scene
        if (loadingScene != null)
        {
            loadingScene.gameObject.SetActive(true);
            loadingScene.LoadScene(sceneId);
        }
    }
    #endregion

    #region Disconnect
    private void OnEnable()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
    }

    private void OnDisable()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
    }

    private void HandleClientDisconnect(ulong clientId)
    {
        if(setUpTurnList != null && IsHost) 
            setUpTurnList.CheckGameOver(false, true);
        else
            setUpTurnList.CheckGameOver(true, false);

        NetworkManager.Singleton.Shutdown();
    }
    #endregion
}
