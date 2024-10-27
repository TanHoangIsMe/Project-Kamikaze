using UnityEngine;
using UnityEngine.Video;

public class ChoosingGameMode : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject champSelectPvE;
    [SerializeField] GameObject lobbyPvP;
    [SerializeField] VideoPlayer videoPlayer;

    public void MoveToMainMenuFromSelectChampPvE()
    {
        BackToMainMenu(true);
    }

    public void MoveToMainMenuFromLobbyPvP()
    {
        BackToMainMenu(false);
    }

    public void MoveToChampSelectPvE()
    {
        if(videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
            videoPlayer.frame = 0; // reset video
        }

        mainMenu.SetActive(false);
        champSelectPvE.SetActive(true);
    }

    public void MoveToLobbyPvP()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
            videoPlayer.frame = 0; // reset video
        }

        lobbyPvP.SetActive(true);
        mainMenu.SetActive(false);
    }

    // isPve -> from select champ pve to home
    // !isPvE -> from lobby pvp to home
    private void BackToMainMenu(bool isPvE)
    {
        // reset select champ pve scene
        if (isPvE)
        {
            ChampSelectPvE champSelectScene = champSelectPvE.GetComponent<ChampSelectPvE>();
            if (champSelectScene != null)
                champSelectScene.ResetScene();

            champSelectPvE.SetActive(false);
        }
        else // reset lobby scene
        {
            LobbyPvP lobbyScene = lobbyPvP.GetComponent<LobbyPvP>();
            if (lobbyScene != null)
                lobbyScene.ResetScene();

            lobbyPvP.SetActive(false);
        }
       
        mainMenu.SetActive(true);

        // stop intro video
        if (!videoPlayer.isPlaying)
            videoPlayer.Play();
    }
}
