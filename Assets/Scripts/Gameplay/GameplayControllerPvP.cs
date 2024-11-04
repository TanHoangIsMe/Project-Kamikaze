using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameplayControllerPvP : NetworkBehaviour
{
    private void Start()
    {
        // spawn champion from champs selected list
        SpawnChampion spawnChampion = GetComponent<SpawnChampion>();
        if (spawnChampion != null)
            spawnChampion.SpawnEnemiesAndHeroes(true);

        // await a little bit to start combat 
        //Invoke("StartNewPhase", 2f);
        SetUpTurnList setUpTurnList = GetComponent<SetUpTurnList>();
        if (NetworkManager.Singleton.IsHost)
        {
            if (setUpTurnList != null)
                setUpTurnList.StartNewPhase();
        }
        Debug.Log(setUpTurnList.TurnList.Count);
    }

    //private void StartNewPhase()
    //{
    //    SetUpTurnList setUpTurnList = GetComponent<SetUpTurnList>();
    //    if (setUpTurnList != null)
    //        setUpTurnList.StartNewPhase();
    //}
}
