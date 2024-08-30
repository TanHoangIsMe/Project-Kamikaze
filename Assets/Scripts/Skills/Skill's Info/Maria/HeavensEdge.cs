using System.Collections.Generic;

public class HeavensEdge: Skill
{
    public HeavensEdge()
    {
        skillName = "Heaven's Edge";
        avatar = "Art/UI/Skill Avatars/Maria/Heaven's Edge Avatar";
        description = "Uriel A Plotexia increase his or an ally's armor by 30";
        manaCost = 50f;
        numberOfEnemyTargets = 3;
        numberOfAllyTargets = 3;
        numberOfSelfTarget = 0;
        numberOfSelfOrAllyTarget = 0;
        priorityStat = StatType.CurrentHealth;
        skillTypes = new SkillType[] { SkillType.Defend };
        activateTypes = new ActivateType[] { ActivateType.Active };
        targetTypes = new TargetType[] { TargetType.Ally, TargetType.Enemy };
    }

    public override void SkillFunction(OnFieldCharacter character,
        CombatSkillMenu combatSkillMenu,
        List<OnFieldCharacter> enemyTargets = null,
        List<OnFieldCharacter> allyTargets = null)
    {
        if (enemyTargets != null)
        {
            foreach (var target in enemyTargets)
            {
                target.CurrentHealth -= 10f;
            }
        }
        if (allyTargets != null)
        {
            foreach (var target in allyTargets)
            {
                target.CurrentHealth += 10f;
            }
        }
    }
}
