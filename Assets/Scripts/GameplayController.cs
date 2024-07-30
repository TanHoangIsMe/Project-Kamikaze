using System;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    public GameObject target; 
    //OnFieldCharacter[] onFieldCharacters = new OnFieldCharacter[2];
    private string a = "Maria";
    private string prefabPath;
    private void Awake()
    {
        //for (int i = 0; i < onFieldCharacters.Length; i++)
        //{
        //    CreateCharacter(onFieldCharacters, new Maria(), i);
        //}      
    }

    private void Start()
    {
        //foreach(var onFieldCharacter in onFieldCharacters)
        //{
        //    Debug.Log(onFieldCharacter.CurrentHealth);
        //    onFieldCharacter.UsingSecondSkill(onFieldCharacters[0],onFieldCharacters);
        //    Debug.Log(onFieldCharacter.CurrentHealth);
        //}
        prefabPath = $"Prefabs/Characters/{a}";

        GameObject prefab = Resources.Load<GameObject>(prefabPath);

        if (prefab != null)
        {
            Instantiate(prefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Prefab not found at path: " + prefabPath);
        }

    }

    //private void CreateCharacter(OnFieldCharacter[] onFieldCharacters,Character character, int index)
    //{
    //    OnFieldCharacter onFieldCharacter = FindObjectOfType<OnFieldCharacter>();
    //    onFieldCharacter.CurrentCharacter = character;
    //    onFieldCharacters[index] = onFieldCharacter;
    //}
}
