using System;
using UnityEngine;

public class OnFieldCharacter : MonoBehaviour 
{
    public Character currentCharacter;
    private float currentAttack;
    private float currentArmor;
    private float currentHealth;

    public Character CurrentCharacter { set { currentCharacter = value; } }
    public float CurrentAttack { get { return currentAttack; } set { currentAttack = value; } }
    public float CurrentArmor { get { return currentArmor; } set { currentArmor = value; } }
    public float CurrentHealth { get { return currentHealth; } set { currentHealth = value; } }

    private void Awake()
    {
        CompareToCharacterNameList();
    }

    private void Start()
    {
        currentAttack = currentCharacter.Attack;
        currentArmor = currentCharacter.Armor;
        currentHealth = currentCharacter.Health;
    }

    public void UsingSecondSkill(OnFieldCharacter character, OnFieldCharacter[] targets)
    {
        currentCharacter.UsingSecondSkill(character,targets);
    }

    private void CompareToCharacterNameList()
    {
        // Get all values in enum CharacterNameList
        Array characterNames = Enum.GetValues(typeof(CharacterNameList));

        foreach (var characterName in characterNames)
        {
            if (characterName.ToString() == transform.tag.ToString())
            {
                SetRealCharacterToCurrentCharacter(characterName);
            }
        }
    }

    private void SetRealCharacterToCurrentCharacter(object characterName)
    {
        string className = characterName.ToString();

        Type type = Type.GetType(className);

        if (type != null)
        {
            object instance = Activator.CreateInstance(type);

            currentCharacter = (Character)instance;
        }
        else
        {
            Debug.LogError("ClassName does not existed");
        }
    }
}
