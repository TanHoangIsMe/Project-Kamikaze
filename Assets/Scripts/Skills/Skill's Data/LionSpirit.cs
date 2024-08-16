using System.Collections.Generic;

public class LionSpirit : Skill
{
    public LionSpirit()
    {
        name = "Lion's Spirit";
        description = "Uriel A Plotexia increase his or an ally's armor by 30";
        manaCost = 50f;
        numberOfEnemyTargets = 0;
        numberOfAllyTargets = 0;
        numberOfSelfTarget = 0;
        numberOfSelfOrAllyTarget = 1;
        priorityStat = StatType.CurrentArmor;
        skillTypes = new SkillType[] { SkillType.Defend };
        activateTypes = new ActivateType[] { ActivateType.Active };
        targetTypes = new TargetType[] { TargetType.SelfOrAlly};
    }

    public override void SkillFunction(OnFieldCharacter character,
        List<OnFieldCharacter> enemyTargets = null,
        List<OnFieldCharacter> allyTargets = null)
    {
        if(allyTargets != null)
        {
            character.CurrentArmor += 30;
        }
        else
        {
            allyTargets[0].CurrentArmor += 30;
        }
    }
}
