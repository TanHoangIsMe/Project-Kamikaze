using System.Collections.Generic;
using System.Diagnostics;

public class testskill : Skill
{
    public testskill()
    {
        skillName = "Hit First, Talk Later";
        avatar = "Art/UI/Skill Avatars/Uriel A Plotexia/Hit First, Talk Later Avatar";
        description = "Not yet";
        manaCost = 50f;
        burstCost = 0f;
        numberOfEnemyTargets = 2;
        numberOfAllyTargets = 0;
        numberOfSelfTarget = 0;
        numberOfSelfOrAllyTarget = 0;
        isGroupEnemy = true;
        isGroupAlly = false;
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
