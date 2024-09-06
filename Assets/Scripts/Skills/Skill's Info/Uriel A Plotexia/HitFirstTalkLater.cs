using System.Collections.Generic;

public class HitFirstTalkLater : Skill
{
    public HitFirstTalkLater()
    {
        skillName = "Hit First, Talk Later";
        avatar = "Art/UI/Skill Avatars/Uriel A Plotexia/Hit First, Talk Later Avatar";
        description = "Not yet";
        manaCost = 50f;
        burstCost = 0f;
        numberOfEnemyTargets = 1;
        numberOfAllyTargets = 0;
        numberOfSelfTarget = 1;
        numberOfSelfOrAllyTarget = 0;
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
        if (enemyTargets != null)
        {
            List<float> damages = new List<float>();

            // calculate the real damage deal to enemy 
            float trueAttackDamage = character.CurrentAttack
                - enemyTargets[0].CurrentArmor;
            if (trueAttackDamage < 0) // make sure damage deal not negative
                trueAttackDamage = 0;

            enemyTargets[0].CurrentHealth -= trueAttackDamage;

            damages.Add(trueAttackDamage);

            // send damage list to skill handler
            // for make pop up damage text 
            skillHandler.SkillValues = damages;

            // heal character by 70% damage deal
            character.CurrentHealth += trueAttackDamage * 0.7f;           
        }
    }
}
