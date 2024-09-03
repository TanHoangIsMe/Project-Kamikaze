using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CombatSkillMenu : MonoBehaviour
{
    [SerializeField] private Image skill1Avatar;
    [SerializeField] private Image skill2Avatar;
    [SerializeField] private Image skillBurstAvatar;
    [SerializeField] private Image characterAvatar;
    [SerializeField] private Image healthBarFill;
    [SerializeField] private Image manaBarFill;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private TextMeshProUGUI burstText;

    private OnFieldCharacter champion;
    public OnFieldCharacter Champion { get { return champion; } set { champion = value; } }

    private SkillHandler skillHandler;

    private void Awake()
    {
        skillHandler = GetComponent<SkillHandler>();
    }

    private void Start()
    {
        skillHandler.Champion = champion;
        skillHandler.IsCombatSkillMenu = true;
    }

    public void SetUpBarsUI()
    {
        /// <summary>
        /// Set up character choosing skill canvas UI
        /// </summary>

        // load character avatar image
        Sprite avatarSprite = Resources.Load<Sprite>(champion.CurrentCharacter.Avatar);

        if (avatarSprite != null)
        {
            characterAvatar.sprite = avatarSprite;
        }

        healthBarFill.fillAmount = champion.CurrentHealth / champion.CurrentCharacter.Health;
        manaBarFill.fillAmount = champion.CurrentMana / champion.CurrentCharacter.MaxMana;

        healthText.text = $"{champion.CurrentHealth} / {champion.CurrentCharacter.Health}";
        manaText.text = $"{champion.CurrentMana} / {champion.CurrentCharacter.MaxMana}";
        burstText.text = $"Burst: {champion.CurrentBurst / champion.CurrentCharacter.MaxBurst * 100}%";
    }

    public void SetUpSkillAvatar()
    {
        // load skill avatars
        Sprite skill1Sprite = Resources.Load<Sprite>(champion.Skills[0].Avatar);
        Sprite skill2Sprite = Resources.Load<Sprite>(champion.Skills[1].Avatar);
        Sprite skillBurstSprite = Resources.Load<Sprite>(champion.Skills[2].Avatar);

        // set avatar to skill button

        if(skill1Sprite != null)
        {
            skill1Avatar.sprite = skill1Sprite;
        }

        if (skill2Sprite != null)
        {
            skill2Avatar.sprite = skill2Sprite;
        }

        if (skillBurstSprite != null)
        {
            skillBurstAvatar.sprite = skillBurstSprite;
        }
    }

    // Function for pressing Skill 1 button
    public void UsingSkill1()
    {
        skillHandler.UsingSkill1(0);
    }

    // Function for pressing Skill 2 button
    public void UsingSkill2()
    {
        skillHandler.UsingSkill2(1);
    }

    // Function for pressing Skill Burst button
    public void UsingSkillBurst()
    {
        skillHandler.UsingSkillBurst(2);
    }

    public void AttackConfirm()
    {
        skillHandler.AttackConfirm();
    }

    #region Skill Description UI
    public void SendInfoToSkillDescription(char whichSkill)
    {
        if (whichSkill == '1')
            skill1Avatar.gameObject
                .GetComponent<SkillDescription>().Champion = champion;
        else if (whichSkill == '2')
            skill2Avatar.gameObject
                .GetComponent<SkillDescription>().Champion = champion;
        else if (whichSkill == 't') // burst skill
            skillBurstAvatar.gameObject
                .GetComponent<SkillDescription>().Champion = champion;
    }
    #endregion
}
