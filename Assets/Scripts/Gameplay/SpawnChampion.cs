using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class SpawnChampion : NetworkBehaviour
{
    // isNetwork -> spawn champ for pvp
    // !isNetwork -> spawn champ for pve
    public void SpawnEnemiesAndHeroes(bool isNetwork)
    {
        // get champion list data
        Dictionary<int, string> championList = DataManager.Instance.championList;

        // spawn champions in list by host
        if (championList != null)
            foreach (KeyValuePair<int, string> champ in championList)
                CreateCharacter(champ, isNetwork);
    }

    private void CreateCharacter(KeyValuePair<int, string> champ, bool isNetwork)
    {
        string prefabPath = $"Prefabs/Characters/{champ.Value}";

        GameObject prefab = Resources.Load<GameObject>(prefabPath);

        if (prefab != null)
        {
            // create champion
            GameObject champion = Instantiate(prefab, GetPosition(champ.Key), Quaternion.identity);

            if (isNetwork) // shared network object 
                champion.GetComponent<NetworkObject>().Spawn();

            // set on field character position
            champion.GetComponent<OnFieldCharacter>().Position = champ.Key;

            // set up champion layer
            if (new[] { 0, 1, 2, 3, 4 }.Contains(champ.Key))
                champion.layer = 7;
            else
                champion.layer = 6;

            if (champion.layer == 7)
            {
                champion.transform.eulerAngles = new Vector3(
                    transform.eulerAngles.x,
                    180f,
                    transform.eulerAngles.z);

                GameObject overheadBars = champion.transform.Find("Health Bar Canvas").gameObject;
                if (overheadBars != null)
                {
                    RectTransform overheadBarsTransform = overheadBars.GetComponent<RectTransform>();
                    if (overheadBarsTransform != null)
                        overheadBarsTransform.localScale *= 1.5f;
                }
            }

            // disable selected ring
            foreach (Transform child in champion.transform)
            {
                if (child.name == "Selected Rings")
                {
                    foreach (Transform grandchild in child.transform)
                    {
                        grandchild.gameObject.SetActive(false);
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Prefab not found at path: " + prefabPath);
        }
    }

    private Vector3 GetPosition(int position)
    {
        switch (position)
        {
            // enemy positions
            case 0: return new Vector3(18f, 0f, 14f);
            case 1: return new Vector3(21.5f, 0f, 16f);
            case 2: return new Vector3(25f, 0f, 14f);
            case 3: return new Vector3(28.5f, 0f, 16f);
            case 4: return new Vector3(32f, 0f, 14f);

            // ally positions
            case 6: return new Vector3(18f, 0f, 4f);
            case 7: return new Vector3(21.5f, 0f, 2f);
            case 8: return new Vector3(25f, 0f, 4f);
            case 9: return new Vector3(28.5f, 0f, 2f);
            case 10: return new Vector3(32f, 0f, 4f);

            default: return Vector3.zero;
        }
    }

    private int GetPositionFromVector3(Vector3 position)
    {
        if (17f < position.x && position.x < 19f
            && 13f < position.z && position.z < 15f)
            return 0;
        else if (20.5f < position.x && position.x < 22.5f
            && 15f < position.z && position.z < 17f)
            return 1;
        else if (24f < position.x && position.x < 26f
            && 13f < position.z && position.z < 15f)
            return 2;
        else if (27.5f < position.x && position.x < 29.5f
            && 15f < position.z && position.z < 17f)
            return 3;
        else if (31f < position.x && position.x < 33f
            && 13f < position.z && position.z < 15f)
            return 4;
        else if (17f < position.x && position.x < 19f
            && 3f < position.z && position.z < 5f)
            return 6;
        else if (20.5f < position.x && position.x < 22.5f
            && 1f < position.z && position.z < 3f)
            return 7;
        else if (24f < position.x && position.x < 26f
            && 3f < position.z && position.z < 5f)
            return 8;
        else if (27.5f < position.x && position.x < 29.5f
            && 1f < position.z && position.z < 3f)
            return 9;
        else if (31f < position.x && position.x < 33f
            && 3f < position.z && position.z < 5f)
            return 10;
        else
            return -1;
    }

    public void UpdateClientChampLayer()
    {
        foreach(OnFieldCharacter champ in FindObjectsOfType<OnFieldCharacter>())
        {
            champ.Position = GetPositionFromVector3(champ.gameObject.transform.position);

            if (new[] { 0, 1, 2, 3, 4 }.Contains(champ.Position))
                champ.gameObject.layer = 7;
            else
                champ.gameObject.layer = 6;
        }      
    }
}
