using UnityEngine;

public class GameOver : MonoBehaviour
{
    private LoadingScene loadingScene;

    private void Awake()
    {
        loadingScene = FindObjectOfType<LoadingScene>();
    }

    public void BackToMainMenu()
    {
        if (loadingScene != null)
        {
            loadingScene.gameObject.SetActive(true);
            loadingScene.LoadScene(0);
        }
    }
}
