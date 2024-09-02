using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private GameObject descriptionPanel;

    private OnFieldCharacter champion;
    public OnFieldCharacter Champion { set { champion = value; } }

    private CombatSkillMenu combatSkillMenu;
    private Coroutine showSkillDescriptionCoroutine;
    private float showDelay; // time to show description
    private float hideDelay; // time to wait after mouse click
    private char lastChar;
    private bool isBlocked; // value to control if can show description 

    private void Awake()
    {
        descriptionPanel.SetActive(false);
        combatSkillMenu = FindObjectOfType<CombatSkillMenu>();
        lastChar = '0';
        showDelay = 0.7f;
        hideDelay = 1f;
        isBlocked = true;
    }

    // when mouse hold on button
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (showSkillDescriptionCoroutine != null)
        {
            StopCoroutine(showSkillDescriptionCoroutine);
        }

        if (isBlocked)
        {
            // game object of this script should be Skill 1 , Skill 2, Skill Burst
            // so get last char of its name to know which skill is mouse hold
            lastChar = name[name.Length - 1];

            // get champion info from combat skill menu
            combatSkillMenu.SendInfoToSkillDescription(lastChar);

            showSkillDescriptionCoroutine = StartCoroutine(ShowSkillDescriptionAfterDelay());
        }
    }

    // when mouse exit hold on button
    public void OnPointerExit(PointerEventData eventData)
    {
        if (showSkillDescriptionCoroutine != null || isBlocked)
        {
            StopCoroutine(showSkillDescriptionCoroutine);
        }
        descriptionPanel.SetActive(false);
    }

    // when player click mouse 
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("click");
        StartCoroutine(HandleClickDelay());
    }

    private IEnumerator HandleClickDelay()
    {
        isBlocked = false; 
        descriptionPanel.SetActive(false);

        // Wait a bit before show panel after player click
        yield return new WaitForSeconds(hideDelay);

        isBlocked = true; 
    }

    private IEnumerator ShowSkillDescriptionAfterDelay()
    {
        yield return new WaitForSeconds(showDelay);

        // set up skill description text by champion skill info
        ShowSkillDescription();

        // Show description panel
        descriptionPanel.SetActive(true);
    }

    private void ShowSkillDescription()
    {
        if (champion != null)
        {
            int whichSkill = -1;

            // get which skill is mouse on by get las char of skill name
            whichSkill = CheckWhichSkill(whichSkill);

            // get all texts of skill description panel
            GameObject
                descriptionPanel1Child,
                descriptionPanel2Child,
                descriptionPanel3Child,
                descriptionPanel4Child,
                descriptionPanel5Child;

            GetAllDescriptionTexts(
                out descriptionPanel1Child,
                out descriptionPanel2Child,
                out descriptionPanel3Child,
                out descriptionPanel4Child,
                out descriptionPanel5Child);

            SetSkillDescriptionTexts(whichSkill, 
                descriptionPanel1Child, 
                descriptionPanel2Child, 
                descriptionPanel3Child, 
                descriptionPanel4Child, 
                descriptionPanel5Child);
        }
    }

    private void SetSkillDescriptionTexts(int whichSkill, 
        GameObject descriptionPanel1Child, 
        GameObject descriptionPanel2Child, 
        GameObject descriptionPanel3Child, 
        GameObject descriptionPanel4Child, 
        GameObject descriptionPanel5Child)
    {
        // skill name
        descriptionPanel1Child.GetComponent<TextMeshProUGUI>().text =
                        champion.Skills[whichSkill].SkillName;

        // skill cost mana or burst depend on skill
        if (whichSkill != 2)
            descriptionPanel2Child.GetComponent<TextMeshProUGUI>().text =
                champion.Skills[whichSkill].ManaCost.ToString() + " Cost";
        else
            descriptionPanel2Child.GetComponent<TextMeshProUGUI>().text =
                champion.Skills[whichSkill].BurstCost.ToString() + " Cost";

        // Skill Type
        descriptionPanel3Child.GetComponent<TextMeshProUGUI>().text = "Type: ";
        foreach (SkillType skillType in champion.Skills[whichSkill].SkillTypes)
        {
            descriptionPanel3Child.GetComponent<TextMeshProUGUI>().text
                += skillType.ToString() + " ";
        }

        // skill activate type (Active skill or Passive Skill)
        descriptionPanel4Child.GetComponent<TextMeshProUGUI>().text = "";
        foreach (ActivateType activateType in champion.Skills[whichSkill].ActivateTypes)
        {
            descriptionPanel4Child.GetComponent<TextMeshProUGUI>().text
                += activateType.ToString() + " ";
        }

        // Skill Detail
        descriptionPanel5Child.GetComponent<TextMeshProUGUI>().text =
            champion.Skills[whichSkill].Description;
    }

    private void GetAllDescriptionTexts(out GameObject descriptionPanel1Child, 
        out GameObject descriptionPanel2Child, 
        out GameObject descriptionPanel3Child, 
        out GameObject descriptionPanel4Child, 
        out GameObject descriptionPanel5Child)
    {
        descriptionPanel1Child = descriptionPanel.transform.GetChild(0).gameObject;
        descriptionPanel2Child = descriptionPanel.transform.GetChild(1).gameObject;
        descriptionPanel3Child = descriptionPanel.transform.GetChild(2).gameObject;
        descriptionPanel4Child = descriptionPanel.transform.GetChild(3).gameObject;
        descriptionPanel5Child = descriptionPanel.transform.GetChild(4).gameObject;
    }

    private int CheckWhichSkill(int whichSkill)
    {
        if (lastChar == '1')
        {
            whichSkill = 0;
        }
        else if (lastChar == '2')
        {
            whichSkill = 1;
        }
        else if (lastChar == 't')
        {
            whichSkill = 2;
        }

        return whichSkill;
    }
}
