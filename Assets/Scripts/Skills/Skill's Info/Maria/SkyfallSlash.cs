using System.Collections.Generic;

public class SkyfallSlash: Skill
{
    public SkyfallSlash()
    {
        skillName = "Skyfall Slash";
        avatar = "Art/UI/Skill Avatars/Maria/Skyfall Slash Avatar";
        description = "Uriel A Plotexia increase his or an ally's armor by 30";
        manaCost = 50f;
        numberOfEnemyTargets = 3;
        numberOfAllyTargets = 0;
        numberOfSelfTarget = 0;
        numberOfSelfOrAllyTarget = 0;
        priorityStat = StatType.CurrentHealth;
        skillTypes = new SkillType[] { SkillType.Defend };
        activateTypes = new ActivateType[] { ActivateType.Active };
        targetTypes = new TargetType[] { TargetType.Enemy};
    }

    public override void SkillFunction(OnFieldCharacter character,
        CombatSkillMenu combatSkillMenu,
        List<OnFieldCharacter> enemyTargets = null,
        List<OnFieldCharacter> allyTargets = null)
    {
        if (enemyTargets != null)
        {
            List<float> damages = new List<float>();

            foreach (var target in enemyTargets)
            {
                float trueAttackDamage = character.CurrentAttack
                - target.CurrentArmor;

                if (trueAttackDamage > 0)
                    target.CurrentHealth -= trueAttackDamage;

                damages.Add(trueAttackDamage);

                combatSkillMenu.SkillValues = damages;
            }
        }
    }
}
