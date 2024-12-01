using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    public void BackToMainMenu()
    {
        Setting setting = FindObjectOfType<Setting>();
        if (setting != null )
            setting.BackToMainMenu();
    }
}
