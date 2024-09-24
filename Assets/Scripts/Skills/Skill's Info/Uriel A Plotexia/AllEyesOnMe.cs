using System.Collections.Generic;

public class AllEyesOnMe : Skill
{
    public AllEyesOnMe()
    {
        skillName = "All Eyes On Me";
        avatar = "Art/UI/Skill Avatars/Uriel A Plotexia/All Eyes On Me Avatar";
        description = "Not yet";
        manaCost = 0f;
        burstCost = 0f;
        numberOfEnemyTargets = 3;
        numberOfAllyTargets = 0;
        priorityStat = StatType.CurrentAttack;
        skillTypes = new SkillType[] { SkillType.Crowd_Control };
        activateTypes = new ActivateType[] { ActivateType.Active };
        targetTypes = new TargetType[] { TargetType.Enemy };
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
            // add taunt effect to champion effect list
            Effect taunt = new Taunt();
            taunt.Champion = character;

            if (gameplayController != null)
            {
                taunt.StartTurn = gameplayController.Phase;
                taunt.EndTurn = taunt.StartTurn + 1;
            }

            character.effects.Add(taunt);
            character.UpdateEffectIcon();

            // add taunted effect to enemies effect list
            foreach (var target in enemyTargets)
            {
                Taunted taunted = new Taunted();
                taunted.Champion = target;
                taunted.TauntedBy = character;

                if (gameplayController != null)
                {
                    taunted.StartTurn = gameplayController.Phase;
                    taunted.EndTurn = taunted.StartTurn + 1;
                }

                target.effects.Add(taunted);
                target.UpdateEffectIcon();
            }
        }
    }
}
