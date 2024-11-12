using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private OnFieldCharacter champion;
    public OnFieldCharacter Champion { get { return champion; } set { champion = value; } }

    private SkillHandler skillHandler;

    private void Awake()
    {
        skillHandler = FindObjectOfType<SkillHandler>();
    }

    public void StartEnemyTurn()
    {
        //if (skillHandler == null) Debug.LogWarning("Skill Handler Null On AI");
        //else
        //{
        //    skillHandler.Champion = champion;
        //    skillHandler.IsCombatSkillMenu = false;

        //    skillHandler.SwapChampionsLayer();

        //    if (skillHandler.UsingSkillBurst())
        //    {
        //        skillHandler.AttackConfirm();
        //    }
        //    else
        //    {
        //        if (skillHandler.UsingSkill2())
        //        {
        //            skillHandler.AttackConfirm();
        //        }
        //        else
        //        {
        //            skillHandler.UsingSkill1();
        //            skillHandler.AttackConfirm();
        //        }
        //    }
        //}
    }
}
