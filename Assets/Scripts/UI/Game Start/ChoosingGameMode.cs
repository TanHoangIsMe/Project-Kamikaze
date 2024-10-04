using UnityEngine;
using UnityEngine.Video;

public class ChoosingGameMode : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject champSelectPvE;
    [SerializeField] VideoPlayer videoPlayer;

    public void MoveToMainMenu()
    {
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
}
