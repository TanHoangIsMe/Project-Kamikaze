using System.Collections.Generic;

public class HeavensEdge: Skill
{
    public HeavensEdge()
    {
        skillName = "Heaven's Edge";
        avatar = "Art/UI/Skill Avatars/Maria/Heaven's Edge Avatar";
        description = "Uriel A Plotexia increase his or an ally's armor by 30";
        manaCost = 0f;
        burstCost = 80f;
        numberOfEnemyTargets = 3;
        numberOfAllyTargets = 3;
        priorityStat = StatType.CurrentHealth;
        skillTypes = new SkillType[] { SkillType.Attack};
        activateTypes = new ActivateType[] { ActivateType.Active };
        targetTypes = new TargetType[] { TargetType.Ally, TargetType.Enemy };
    }

    public override void SkillFunction(OnFieldCharacter character,
        SkillHandler skillHandler,
        List<OnFieldCharacter> enemyTargets = null,
        List<OnFieldCharacter> allyTargets = null)
    {
        // decrease champion burst
        calculateSkillEnergy.ReduceCharacterBurst(character);

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
