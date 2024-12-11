using Unity.Netcode;
using UnityEngine;

public class GameplayControllerPvE : NetworkBehaviour
{
    private SpawnChampion spawnChampion;
    private SetUpTurnList setUpTurnList;

    private void Awake()
    {
        spawnChampion = GetComponent<SpawnChampion>();
        setUpTurnList = GetComponent<SetUpTurnList>();
    }

    private void Start()
    {
        // spawn champions
        if (spawnChampion != null) 
            spawnChampion.SpawnEnemiesAndHeroes(false);

        // await a little bit to start combat 
        Time.timeScale = 1;
        Invoke("StartNewPhase", 3f);
    }

    private void StartNewPhase()
    {
        if (setUpTurnList != null)
            setUpTurnList.StartNewPhase();
    }
}
