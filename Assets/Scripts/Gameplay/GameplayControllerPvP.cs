using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class GameplayControllerPvP : NetworkBehaviour
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
        if (spawnChampion != null)
        {
            if (IsHost) // host spawn champ
                spawnChampion.SpawnEnemiesAndHeroes(true);
            else // client update his champ layer
                spawnChampion.UpdateClientChampLayer();
        }

        // await a little bit to start combat 
        Time.timeScale = 1;
        Invoke("StartNewPhase", 5f);
    }

    private void StartNewPhase()
    {
        if (IsHost && setUpTurnList != null)
            setUpTurnList.StartNewPhaseClientRpc();
    }
}
