using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OnFieldCharacter : MonoBehaviour 
{
    private Character currentCharacter;
    private int position;
    private float currentAttack;
    private float currentArmor;
    private float currentSpeed;
    private float currentHealth;
    private float currentMana;
    private float currentBurst;
    private float currentShield;
    private Skill[] skills;
    public List<Effect> effects;

    public Character CurrentCharacter { get { return currentCharacter; } set { currentCharacter = value; } }
    public int Position { get { return position; } set { position = value; } }
    public float CurrentAttack { get { return currentAttack; } set { currentAttack = value; } }
    public float CurrentArmor { get { return currentArmor; } set { currentArmor = value; } }
    public float CurrentSpeed { get { return currentSpeed; } set { currentSpeed = value; } }
    public float CurrentHealth { get { return currentHealth; } set { currentHealth = value; } }
    public float CurrentMana { get { return currentMana; } set { currentMana = value; } }
    public float CurrentBurst { get { return currentBurst; } set { currentBurst = value; } }
    public float CurrentShield { get { return currentShield; } set { currentShield = value; } }
    public Skill[] Skills { get { return skills; } }

    public List<Effect> Effects { get { return effects; } set { effects = value; } }

    public void UsingFirstSkill(SkillHandler skillHandler,
        List<OnFieldCharacter> enemyTargets = null , 
        List<OnFieldCharacter> allyTargets = null)
    {
        currentCharacter.Skills[0]
            .SkillFunction(this, skillHandler, enemyTargets, allyTargets);
    }

    public void UsingSecondSkill(SkillHandler skillHandler,
        List<OnFieldCharacter> enemyTargets = null,
        List<OnFieldCharacter> allyTargets = null)
    {
        currentCharacter.Skills[1]
            .SkillFunction(this, skillHandler, enemyTargets, allyTargets);
    }

    public void UsingBurstSkill(SkillHandler skillHandler,
        List<OnFieldCharacter> enemyTargets = null,
        List<OnFieldCharacter> allyTargets = null)
    {
        currentCharacter.Skills[2]
            .SkillFunction(this, skillHandler, enemyTargets, allyTargets);
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
        currentMana = currentCharacter.MaxMana;
        currentBurst = 0f;
        skills = currentCharacter.Skills;
        effects = new List<Effect>();
    }

    #region Process Effect Icon
    public void UpdateEffectIcon()
    {
        Transform overheadBars = gameObject.transform.Find("Health Bar Canvas");
        if (overheadBars != null)
        {
            Transform effectIcons = overheadBars.gameObject.transform.Find("Effect Icons");
            if (effectIcons != null)
            {
                // clear previous effect icons
                foreach (Transform icon in effectIcons)
                    Destroy(icon.gameObject);

                for (int i = 0; i < MakeUniqueIconList(effects).Count; i++)
                {
                    // add effect image to overhead bars child object
                    GameObject effectIcon = new GameObject();
                    effectIcon.name = effects[i].EffectName;
                    effectIcon.transform.SetParent(effectIcons);

                    Image effectIconImage = effectIcon.AddComponent<Image>();
                    effectIconImage.sprite = Resources.Load<Sprite>(effects[i].EffectAvatar);

                    // set it transform
                    RectTransform effectIconTransform = effectIcon.GetComponent<RectTransform>();
                    effectIconTransform.sizeDelta = new Vector2(0.2f, 0.2f);
                    effectIconTransform.localPosition = new Vector2(-0.33f + 0.22f * (i % 4), 0.18f + 0.22f * (i / 4));
                    effectIconTransform.localRotation = Quaternion.identity;
                    effectIconTransform.localScale = new Vector2(1.0f, 1.0f);
                }
            }
        }        
    }

    // make a unique effect icon list to show on UI
    private List<Effect> MakeUniqueIconList(List<Effect> originalList)
    {
        // Using HashSet to remove same effect
        HashSet<Effect> uniqueList = new HashSet<Effect>(originalList);
        return new List<Effect>(uniqueList); // Return new list
    }
    #endregion
}
