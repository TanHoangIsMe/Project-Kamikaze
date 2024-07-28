using UnityEngine;

public class GameplayController : MonoBehaviour
{
    OnFieldCharacter[] onFieldCharacters = new OnFieldCharacter[2];

    private void Awake()
    {
        for (int i = 0; i < onFieldCharacters.Length; i++)
        {
            CreateCharacter(onFieldCharacters,new Maria(),i);
        }      
    }

    private void Start()
    {
        foreach(var onFieldCharacter in onFieldCharacters)
        {
            Debug.Log(onFieldCharacter.CurrentHealth);
            onFieldCharacter.UsingSecondSkill(onFieldCharacters[0],onFieldCharacters);
            Debug.Log(onFieldCharacter.CurrentHealth);
        }        
    }

    private void CreateCharacter(OnFieldCharacter[] onFieldCharacters,Character character, int index)
    {
        OnFieldCharacter onFieldCharacter = FindObjectOfType<OnFieldCharacter>();
        onFieldCharacter.CurrentCharacter = character;
        onFieldCharacters[index] = onFieldCharacter;
    }
}
