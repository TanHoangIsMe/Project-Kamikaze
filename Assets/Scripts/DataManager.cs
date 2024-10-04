using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    public Dictionary<int, string> championList;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Not destroy object
        }
        else
        {
            Destroy(gameObject); // destroy object is already existed
        }
    }
}
