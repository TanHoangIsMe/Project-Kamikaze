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
        targetTypes = new TargetType[] { TargetType.Enemy, TargetType.Self };
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
            SetTauntEffect(character);
            SetTauntedEffect(enemyTargets, character);
        }
    }

    private void SetTauntEffect(OnFieldCharacter character)
    {
        // value to store if target is taunting
        bool isTaunt = false;

        // check if target is taunting
        foreach (var effect in character.Effects)
            if (effect is Taunt previousTaunt && gameplayController != null)
            {
                previousTaunt.StartTurn = gameplayController.Phase;
                previousTaunt.EndTurn = previousTaunt.StartTurn + 1;
                isTaunt = true;
                character.UpdateEffectIcon();
                break;
            }

        if (isTaunt) return;

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
    }

    private void SetTauntedEffect(List<OnFieldCharacter> enemyTargets, OnFieldCharacter character)
    {
        // add taunted effect to enemies effect list
        foreach (var target in enemyTargets)
        {
            // value to store if target is being taunted
            bool isTaunted = false;

            // check if target are being taunt
            foreach (var effect in target.Effects)
                if (effect is Taunted previousTaunted && gameplayController != null)
                {
                    previousTaunted.TauntedBy = character;
                    previousTaunted.StartTurn = gameplayController.Phase;
                    previousTaunted.EndTurn = previousTaunted.StartTurn + 1;
                    isTaunted = true;
                    target.UpdateEffectIcon();
                    break;
                }

            if (isTaunted) continue;

            // if target not being taunt
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
