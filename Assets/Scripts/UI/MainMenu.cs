using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayPvE()
    {
        SceneManager.LoadSceneAsync("PvEScene");
    }
}
