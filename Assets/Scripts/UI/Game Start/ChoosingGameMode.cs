using Unity.Netcode;
using UnityEngine;
using UnityEngine.Video;

public class ChoosingGameMode : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject champSelectPvE;
    [SerializeField] GameObject lobbyPvP;
    [SerializeField] VideoPlayer videoPlayer;

    public void MoveToMainMenu()
    {
        // reset select champ scene
        ChampSelectPvE champSelectScene = FindObjectOfType<ChampSelectPvE>();
        if (champSelectScene != null )
            champSelectScene.ResetScene();

        champSelectPvE.SetActive(false);
        mainMenu.SetActive(true);

        if (!videoPlayer.isPlaying)
            videoPlayer.Play();
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
}
