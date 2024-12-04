using System;
using System.Collections.Generic;
using TMPro;
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
    private List<Effect> effects;

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

    public void UsingSkill(int whichSkill)
    {
        currentCharacter.Skills[whichSkill].SkillFunction();
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

                for (int i = 0; i < effects.Count; i++)
                {
                    // add effect game object
                    GameObject effectIcon = new GameObject();
                    effectIcon.name = effects[i].EffectName;
                    effectIcon.transform.SetParent(effectIcons);

                    // add effect image game object
                    GameObject effectImage= new GameObject();
                    effectImage.transform.SetParent(effectIcon.transform);
                    Image effectAvatar = effectImage.AddComponent<Image>();
                    effectAvatar.sprite = Resources.Load<Sprite>(effects[i].EffectAvatar);

                    // add effect remain turn text game object
                    GameObject effectText = new GameObject();
                    effectText.transform.SetParent(effectIcon.transform);
                    TextMeshProUGUI effectRemainTurn = effectText.AddComponent<TextMeshProUGUI>();
                    effectRemainTurn.text = (effects[i].EndTurn - effects[i].StartTurn).ToString();
                    effectRemainTurn.fontSize = 0.18f;
                    effectRemainTurn.alignment = TextAlignmentOptions.BottomRight;
                    effectRemainTurn.fontStyle = FontStyles.Bold;

                    // set effect icon transform
                    Transform effectIconTransform = effectIcon.GetComponent<Transform>();
                    effectIconTransform.localPosition = 
                        new Vector3(-0.32f + 0.32f * (i % 3), 0.2f + 0.28f * (i / 3), 0);
                    effectIconTransform.localRotation = Quaternion.identity;
                    effectIconTransform.localScale = Vector3.one;

                   
                    // set effect image transform
                    RectTransform effectImageTransform = effectImage.GetComponent<RectTransform>();
                    effectImageTransform.sizeDelta = new Vector2(0.25f, 0.25f);

                    // set effect remain turn text transform
                    RectTransform effectTextTransform = effectText.GetComponent<RectTransform>();
                    effectTextTransform.sizeDelta = new Vector2(0.25f, 0.25f);
                }
            }
        }
    }
    #endregion
}
