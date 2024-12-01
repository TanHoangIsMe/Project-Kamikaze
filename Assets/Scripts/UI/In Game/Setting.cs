using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Setting : NetworkBehaviour
{
    [SerializeField] private GameObject settingMenu;
    [SerializeField] private Button resumeButton;

    private LoadingScene loadingScene;

    private void Start()
    {
        // setting loading scene
        loadingScene = FindObjectOfType<LoadingScene>();
        if(loadingScene != null )
            loadingScene.gameObject.SetActive(false);
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
        BackToMainMenuServerRpc();
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
    private void BackToMainMenuServerRpc()
    {
        BackToMainMenuClientRpc();
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
    private void BackToMainMenuClientRpc()
    {
        if (IsHost) NetworkManager.Singleton.Shutdown();

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
            settingMenu.SetActive(false);
            Time.timeScale = 1;
        }

        // load scene
        SceneManager.LoadSceneAsync(sceneId);
    }
    #endregion
}
