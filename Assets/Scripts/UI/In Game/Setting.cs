using UnityEngine;
using UnityEngine.SceneManagement;

public class Setting : MonoBehaviour
{
    [SerializeField] private GameObject settingMenu;
    [SerializeField] private GameObject loadingCanvas;

    private LoadingScene loadingScene;

    private void Awake()
    {
        loadingScene = loadingCanvas.GetComponent<LoadingScene>();
    }

    public void OpenSettingMenu()
    {
        Time.timeScale = 0;
        settingMenu.SetActive(true);
    }

    public void ResumeBattle()
    {
        Time.timeScale = 1;
        settingMenu.SetActive(false);
    }

    public void RestartBattle()
    {
        // reload scene
        LoadScene(1);
    }  

    public void BackToMainMenu()
    {
        // load main menu scene
        LoadScene(0);
    }

    private void LoadScene(int sceneId)
    {
        // open loading scene
        if (loadingScene != null)
        {
            loadingCanvas.SetActive(true);
            loadingScene.LoadScene(sceneId);
            settingMenu.SetActive(false);
            Time.timeScale = 1;
        }

        // load scene
        SceneManager.LoadSceneAsync(sceneId);
    }
}
