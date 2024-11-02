using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameplayControllerPvP : NetworkBehaviour
{

    private void Start()
    {
        if (DataManager.Instance.championList != null)
            SpawnChamp();
    }

    private void SpawnChamp()
    {
        foreach (KeyValuePair<int, string> champ in DataManager.Instance.championList)
        {
            string prefabPath = $"Prefabs/Characters/{champ.Value}";

            GameObject prefab = Resources.Load<GameObject>(prefabPath);

            Instantiate(prefab, new Vector3(Random.Range(-2, 2), 0, Random.Range(-2, 2)), Quaternion.identity);
        }
    }
}
