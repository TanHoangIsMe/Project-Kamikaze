using System;
using System.Collections.Generic;
using UnityEngine;

public class OnFieldCharacter : MonoBehaviour 
{
    public Character currentCharacter;
    private float currentAttack;
    private float currentArmor;
    private float currentSpeed;
    private float currentHealth;
    private float currentMana;
    private float currentBurst;

    public Character CurrentCharacter { get { return currentCharacter; } set { currentCharacter = value; } }
    public float CurrentAttack { get { return currentAttack; } set { currentAttack = value; } }
    public float CurrentArmor { get { return currentArmor; } set { currentArmor = value; } }
    public float CurrentSpeed { get { return currentSpeed; } set { currentSpeed = value; } }
    public float CurrentHealth { get { return currentHealth; } set { currentHealth = value; } }
    public float CurrentMana { get { return currentMana; } set { currentMana = value; } }
    public float CurrentBurst { get { return currentBurst; } set { currentBurst = value; } }

    public void UsingFirstSkill(List<OnFieldCharacter> enemyTargets = null , 
        List<OnFieldCharacter> allyTargets = null)
    {
        currentCharacter.Skills[0]
            .SkillFunction(this, enemyTargets,allyTargets);
    }

    public void UsingSecondSkill(List<OnFieldCharacter> enemyTargets = null,
        List<OnFieldCharacter> allyTargets = null)
    {
        currentCharacter.Skills[1]
            .SkillFunction(this, enemyTargets, allyTargets);
    }

    public void UsingBurstSkill(List<OnFieldCharacter> enemyTargets = null,
        List<OnFieldCharacter> allyTargets = null)
    {
        currentCharacter.Skills[2]
            .SkillFunction(this, enemyTargets, allyTargets);
    }

    private void Awake()
    {
        CompareToCharacterNameList();
        SetUpCurrentStats();
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

    private void SetUpCurrentStats()
    {
        currentAttack = currentCharacter.Attack;
        currentArmor = currentCharacter.Armor;
        currentSpeed = currentCharacter.Speed;
        currentHealth = currentCharacter.Health;
        currentMana = 0f;
        currentBurst = 0f;
    }
}
