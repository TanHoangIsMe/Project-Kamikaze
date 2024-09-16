using System.Collections.Generic;

public class AllEyesOnMe : Skill
{
    public AllEyesOnMe()
    {
        skillName = "All Eyes On Me";
        avatar = "Art/UI/Skill Avatars/Uriel A Plotexia/All Eyes On Me Avatar";
        description = "Not yet";
        manaCost = 0f;
        burstCost = 100f;
        numberOfEnemyTargets = 1;
        numberOfAllyTargets = 0;
        priorityStat = StatType.CurrentHealth;
        skillTypes = new SkillType[] { SkillType.Attack, SkillType.Heal };
        activateTypes = new ActivateType[] { ActivateType.Active };
        targetTypes = new TargetType[] { TargetType.Enemy, TargetType.Self };
    }

    public override void SkillFunction(OnFieldCharacter character,
        SkillHandler skillHandler,
        List<OnFieldCharacter> enemyTargets = null,
        List<OnFieldCharacter> allyTargets = null)
    {

    }
}
