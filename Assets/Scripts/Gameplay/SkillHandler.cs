using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SkillHandler : NetworkBehaviour
{
    [SerializeField] private GameObject popUpDamageText;

    private CheckNumberOfTargets checkNumberOfTargets;
    private AutoFindTargets autoFindTargets;
    private SkillPriority skillPriority;
    private CombatSkillMenu combatSkillMenu;

    private OnFieldCharacter champion;
    public OnFieldCharacter Champion { get { return champion; } set { champion = value; } }

    private List<float> skillValues;
    public List<float> SkillValues { set { skillValues = value; } }

    private bool isPlayer;
    public bool IsPlayer { set { isPlayer = value; } }

    private bool canReuse; // flat to know is skill can use instance after cast
    public bool CanReuse { get { return canReuse; } set { canReuse = value; } }

    private bool canDestroyObject;
    public bool CanDestroyObject { get { return canDestroyObject; } }

    private void Awake()
    {
        checkNumberOfTargets = GetComponent<CheckNumberOfTargets>();
        autoFindTargets = GetComponent<AutoFindTargets>();
        combatSkillMenu = FindObjectOfType<CombatSkillMenu>();
        skillValues = new List<float>();
        canReuse = false;

        AddListenerToButton();
    }

    private void Start()
    {
        skillPriority = FindObjectOfType<SkillPriority>();

        if (IsHost) canDestroyObject = true;
        else canDestroyObject = false;
    }

    #region Add Listener
    private void AddListenerToButton()
    {
        Button skill1Button = GameObject.Find("Skill 1").GetComponent<Button>();
        Button skill2Button = GameObject.Find("Skill 2").GetComponent<Button>();
        Button skillBurstButton = GameObject.Find("Skill Burst").GetComponent<Button>();
        Button confirmAttackButton = GameObject.Find("Confirm Attack").GetComponent<Button>();

        bool isPvE = FindObjectOfType<EnemyAI>();

        if (skill1Button != null)
            skill1Button.onClick.AddListener(() =>
            {
                if (isPvE)
                    UsingSkill(0);
                else
                    UsingSkill1ServerRpc();
            });

        if (skill2Button != null)
            skill2Button.onClick.AddListener(() =>
            {
                if (isPvE)
                    UsingSkill(1);
                else
                    UsingSkill2ServerRpc();
            });

        if (skillBurstButton != null)
            skillBurstButton.onClick.AddListener(() =>
            {
                if (isPvE)
                    UsingSkill(2);
                else
                    UsingSkillBurstServerRpc();
            });

        if (confirmAttackButton != null)
            confirmAttackButton.onClick.AddListener(() =>
            {
               AttackConfirmServerRpc();
            });
    }
    #endregion

    #region Set Up Champ Layer 
    private void ChangeLayerToSelf()
    {
        if (champion != null)
        {
            champion.gameObject.layer = 8;
        }
        else
            Debug.Log("Something wrong in change layer to self");
    }

    // swap champion layer (enemy -> ally) (ally -> enemy)
    // for enemy use auto find targets function
    public void SwapChampionsLayer()
    {
        foreach (var character in FindObjectsOfType<OnFieldCharacter>())
        {
            if (character != null && character.gameObject.layer == 6)
            {
                character.gameObject.layer = 7;
            }
            else
            {
                character.gameObject.layer = 6;
            }
        }
    }

    private void SetUpToAutoFindTargets(int whichSkill)
    {
        bool isHost;
        if(new[] { 6, 7, 8, 9, 10 }.Contains(champion.Position)) isHost = true;
        else isHost = false;

        // change champion layer = 8 - self
        ChangeLayerToSelf();

        // check if champion is taunted
        bool isTaunted = false;
        OnFieldCharacter taunter = null;
        foreach (var effect in champion.Effects)
            if (effect is Taunted taunted)
            { 
                isTaunted = true;
                taunter = taunted.TauntedBy;
            }

        // set up information need to auto find targets 
        checkNumberOfTargets.Champion = champion;
        checkNumberOfTargets.WhichSkill = whichSkill;
        if(skillPriority != null) skillPriority.IsHostClick = isHost;
        checkNumberOfTargets.CheckInfoToAutoFindTargets(isPlayer, isTaunted, taunter);
        

        // if player champion using skill show UI
        if (isPlayer)
        {
            // clear all selected ring and turn on based on targets
            autoFindTargets.TurnOffShowTargets();
            autoFindTargets.TurnOnShowTargets();
        }
    }

    public void ResetThings()
    {
        // turn off can select target
        checkNumberOfTargets.IsFinishChoosing = false;
        checkNumberOfTargets.CanSelectTarget = false;
        checkNumberOfTargets.IsFinishFinding = false;

        // reset target lists
        ClearTargetsList();

        // reset skill value list
        skillValues.Clear();

        // change champion layer back
        if (new[] { 6, 7, 8, 9, 10 }.Contains(champion.Position))
        {
            champion.gameObject.layer = 6;           
        }
        else
        {
            SwapChampionsLayer();
            champion.gameObject.layer = 7;

            champion.gameObject.transform.eulerAngles = 
                new Vector3(
                    transform.eulerAngles.x,
                    180f,
                    transform.eulerAngles.z);
        }
    }
    #endregion

    #region Reset Value
    private void ClearTargetsList()
    {
        autoFindTargets.AllyTargets.Clear();
        autoFindTargets.EnemyTargets.Clear();
        autoFindTargets.SelfTarget = null;
    }

    private void ResetCheckNumberOfTargetsFlags()
    {
        //checkNumberOfTargets.ChoosePriorityPanel.gameObject.SetActive(false);
        checkNumberOfTargets.IsFinishChoosing = false;
        checkNumberOfTargets.IsChoosePriorityOpen = false;
        checkNumberOfTargets.CanSelectTarget = false;
    }
    #endregion

    #region Server Rpc
    [ServerRpc (RequireOwnership = false)]
    public void UsingSkill1ServerRpc()
    {
        UsingSkill1ClientRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void UsingSkill2ServerRpc()
    {
        UsingSkill2ClientRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void UsingSkillBurstServerRpc()
    {
        UsingSkillBurstClientRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void AttackConfirmServerRpc()
    {
        AttackConfirmClientRpc();
    }
    #endregion

    #region Client Rpc
    [ClientRpc]
    private void UsingSkill1ClientRpc()
    {
        UsingSkill(0);
    }

    [ClientRpc]
    private void UsingSkill2ClientRpc()
    {
        UsingSkill(1);
    }

    [ClientRpc]
    private void UsingSkillBurstClientRpc()
    {
        UsingSkill(2);
    }

    public bool UsingSkill(int whichSkill)
    {
        if(whichSkill != 2 && champion.CurrentMana < champion.Skills[whichSkill].ManaCost)
        {
            Debug.Log("Not enough mana to use skill");
            return false;
        }
        else if (whichSkill == 2 && champion.CurrentBurst != champion.Skills[2].BurstCost)
        {
            Debug.Log("Not enough mana to use skill");
            return false;
        }
        else
        {
            // Clear targets list and reset flag properties
            ResetCheckNumberOfTargetsFlags();
            ClearTargetsList();

            SetUpToAutoFindTargets(whichSkill);
            return true;
        }
    }

    [ClientRpc]
    public void AttackConfirmClientRpc()
    {
        if (checkNumberOfTargets.IsFinishFinding)
        {
            if(isPlayer)
                // turn off skill menu
                combatSkillMenu.gameObject.SetActive(false);
            
            // find champion animation controller script
            List<OnFieldCharacter> enemies = autoFindTargets.EnemyTargets;
            IAnimationPlayable animationController = champion.GetComponent<IAnimationPlayable>();
            animationController.SetEnemyTargets(enemies);

            if (checkNumberOfTargets.WhichSkill == 0) // using skill 1
                // play animation
                animationController.PlayFirstSkillAnimation();
            else if (checkNumberOfTargets.WhichSkill == 1) // using skill 2
                // play animation
                animationController.PlaySecondSkillAnimation();
            else // using burst
                // play animation
                animationController.PlayBurstSkillAnimation();

            // turn off show targets
            autoFindTargets.TurnOffShowTargets();
        }
        else
            Debug.Log("Please choose a skill");   
    }
    #endregion

    #region Using Skill Info
    public void SendInfoToUsingFirstSkill()
    {
        SendInfoToUsingSkill(0);
    }

    public void SendInfoToUsingSecondSkill()
    {
        SendInfoToUsingSkill(1);
    }

    public void SendInfoToUsingBurstSkill()
    {
        SendInfoToUsingSkill(2);
    }

    private void SendInfoToUsingSkill(int whichSkill)
    {
        champion.Skills[whichSkill].Character = champion;
        champion.Skills[whichSkill].EnemyTargets = autoFindTargets.EnemyTargets;
        champion.Skills[whichSkill].AllyTargets = autoFindTargets.AllyTargets;

        champion.UsingSkill(whichSkill);

        // play health bar reduce or increase animation
        // when champion current health change
        PlayHealthBarEffect();
    }
    #endregion

    #region OverHead Champion UI
    public void PlayHealthBarEffect()
    {
        List<OnFieldCharacter> enemies = autoFindTargets.EnemyTargets;
        List<OnFieldCharacter> allies = autoFindTargets.AllyTargets;
        OnFieldCharacter self = autoFindTargets.SelfTarget;

        // play health bar fill animation on enemies
        if (enemies != null && skillValues.Count > 0)
        {
            int totalEnemies = enemies.Count();
            for (int i = 0; i < totalEnemies; i++)
            {
                PlayPopUpDamageText(enemies, i);

                // Play Update Health Bar Animation
                enemies[i].gameObject.GetComponent<OverHealthBar>().UpdateHealthFill();
            }
        }

        // play health bar fill animation on allies
        if (allies != null)
            foreach (var ally in allies)
                ally.gameObject.GetComponent<OverHealthBar>().UpdateHealthFill();

        // play health bar fill animation on self
        if (self != null)
            self.gameObject.GetComponent<OverHealthBar>().UpdateHealthFill();
    }

    private void PlayPopUpDamageText(List<OnFieldCharacter> enemies, int i)
    {
        // get over head position of enemy 
        Vector3 overHeadEnemyPosition = enemies[i].gameObject.transform.position 
            + Vector3.up * enemies[i].gameObject.transform.localScale.y * 2.3f;

        Vector3 randomOverHeadPosition = overHeadEnemyPosition + new Vector3(Random.Range(-0.2f, 0.2f), 0, 0);

        // spawn damage text
        GameObject popUp = Instantiate(popUpDamageText, randomOverHeadPosition, Quaternion.identity);
        TextMeshProUGUI popUpText = popUp.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        popUpText.text = skillValues[i].ToString();

        ChangeTextColorBasedOnElement(i, popUpText);
    }

    private void ChangeTextColorBasedOnElement(int i, TextMeshProUGUI popUpText)
    {
        // get character elements info
        ElementType[] elementTypes = champion.CurrentCharacter.ElementTypes;
        ElementType elementType = new ElementType();
        int numberOfElements = elementTypes.Length;

        if (numberOfElements == 1) // character has 1 element
        {
            elementType = elementTypes[0];
        }
        else // character has > 1 elements
        {
            GetRandomElement(elementTypes, elementType, numberOfElements);
        }

        ChangeTextColor(popUpText, elementType);
    }

    private static void ChangeTextColor(TextMeshProUGUI popUpText, ElementType elementType)
    {
        switch (elementType)
        {
            case ElementType.Fire:
                popUpText.color = Color.red;
                break;
            case ElementType.Water:
                popUpText.color = Color.blue;
                break;
            case ElementType.Nature:
                popUpText.color = Color.green;
                break;
            case ElementType.Light:
                popUpText.color = Color.yellow;
                break;
            case ElementType.Dark: // purple color
                popUpText.color = new Color(180f / 255f, 0f / 255f, 255f / 255f);
                break;
            case ElementType.Mystic:
                popUpText.color = Color.magenta;
                break;
        }
    }

    private void GetRandomElement(ElementType[] elementTypes, ElementType elementType, int numberOfElements)
    {
        // create random object
        System.Random random = new System.Random();
        // create random number based on number of element
        int index = random.Next(numberOfElements);
        // get random element
        elementType = elementTypes[index];
    }
    #endregion
}
