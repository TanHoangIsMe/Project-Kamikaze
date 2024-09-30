using System.Collections.Generic;
using UnityEngine;

public class CalculateSkillDamage
{
    // calculate the real damage after being effected by armor,
    // element advantage,class advantage.
    public List<float> CalculateOutputDamage(OnFieldCharacter character,
        List<OnFieldCharacter> enemies, SkillHandler skillHandler, float damagePercent)
    {
        // list to store damages value to show damage deal popup text
        List<float> damages = new List<float>();

        foreach (var enemy in enemies)
        {
            float trueAttackDamage = character.CurrentAttack
                - enemy.CurrentArmor;

            trueAttackDamage = CalculateDamageAfterElement(character, enemy, trueAttackDamage);
            trueAttackDamage = CalculateDamageAfterClass(character, enemy, trueAttackDamage);

            // round damage
            trueAttackDamage = (float)Mathf.Round(trueAttackDamage);

            // make sure damage deal not negative
            if (trueAttackDamage < 0) trueAttackDamage = 0;

            damages.Add(trueAttackDamage);

            CalculateDamageDeal(enemy, trueAttackDamage);
        }

        // send damage list to skill handler
        // for make pop up damage text 
        skillHandler.SkillValues = damages;

        return damages;
    }

    #region Calculate Damage
    // calculate damage after element advantage
    private float CalculateDamageAfterElement(OnFieldCharacter character,
        OnFieldCharacter enemy, float trueAttackDamage)
    {
        foreach (var characterElement in character.CurrentCharacter.ElementTypes)
            foreach (var enemyElement in enemy.CurrentCharacter.ElementTypes)
              trueAttackDamage = DamageAfterElement(characterElement, enemyElement, trueAttackDamage);

        return trueAttackDamage;
    }

    // calculate damage after class advantage
    private float CalculateDamageAfterClass(OnFieldCharacter character,
        OnFieldCharacter enemy, float trueAttackDamage)
    {
        foreach (var characterClass in character.CurrentCharacter.ClassTypes)
            foreach (var enemyClass in enemy.CurrentCharacter.ClassTypes)
               trueAttackDamage = DamageAfterClass(characterClass, enemyClass, trueAttackDamage);

        return trueAttackDamage;
    }

    // element interaction
    private float DamageAfterElement(ElementType characterElement,
        ElementType enemyElement, float trueAttackDamage)
    {
        switch (characterElement)
        {
            case ElementType.Fire:
                if (enemyElement == ElementType.Nature) trueAttackDamage *= 1.25f;
                else if (enemyElement == ElementType.Water) trueAttackDamage *= 0.75f;
                return trueAttackDamage;
            case ElementType.Nature:
                if (enemyElement == ElementType.Water) trueAttackDamage *= 1.25f;
                else if (enemyElement == ElementType.Fire) trueAttackDamage *= 0.75f;
                return trueAttackDamage;
            case ElementType.Water:
                if (enemyElement == ElementType.Fire) trueAttackDamage *= 1.25f;
                else if (enemyElement == ElementType.Nature) trueAttackDamage *= 0.75f;
                return trueAttackDamage;
            case ElementType.Dark:
                if (enemyElement == ElementType.Light) trueAttackDamage *= 1.25f;
                return trueAttackDamage;
            case ElementType.Light:
                if (enemyElement == ElementType.Dark) trueAttackDamage *= 1.25f;
                return trueAttackDamage;
            case ElementType.Mystic:
                return trueAttackDamage;
            default:
                return trueAttackDamage;
        }
    }

    // class interaction
    private float DamageAfterClass(ClassType characterClass,
        ClassType enemyClass, float trueAttackDamage)
    {
        switch (characterClass)
        {
            case ClassType.Warrior:
                if (enemyClass == ClassType.Assassin) trueAttackDamage *= 1.25f;
                else if (enemyClass == ClassType.Ranger) trueAttackDamage *= 0.75f;
                return trueAttackDamage;
            case ClassType.Assassin:
                if (enemyClass == ClassType.Defender) trueAttackDamage *= 1.25f;
                else if (enemyClass == ClassType.Warrior) trueAttackDamage *= 0.75f;
                return trueAttackDamage;
            case ClassType.Defender:
                if (enemyClass == ClassType.Berserker) trueAttackDamage *= 1.25f;
                else if (enemyClass == ClassType.Assassin) trueAttackDamage *= 0.75f;
                return trueAttackDamage;
            case ClassType.Berserker:
                if (enemyClass == ClassType.Mage) trueAttackDamage *= 1.25f;
                else if (enemyClass == ClassType.Defender) trueAttackDamage *= 0.75f;
                return trueAttackDamage;
            case ClassType.Mage:
                if (enemyClass == ClassType.Ranger) trueAttackDamage *= 1.25f;
                else if (enemyClass == ClassType.Berserker) trueAttackDamage *= 0.75f;
                return trueAttackDamage;
            case ClassType.Ranger:
                if (enemyClass == ClassType.Warrior) trueAttackDamage *= 1.25f;
                else if (enemyClass == ClassType.Mage) trueAttackDamage *= 0.75f;
                return trueAttackDamage;
            default:
                return trueAttackDamage;
        }
    }
    #endregion

    #region Damge to Shield and Health
    // calculate damage deal to enemy shield and health
    private void CalculateDamageDeal(OnFieldCharacter enemy, float trueAttackDamage)
    {
        // reduce champion shield before health
        if (enemy.CurrentShield > 0) // champ have shield
        {
            float damageAfterShield = trueAttackDamage - enemy.CurrentShield;
            float shieldLost;

            if (damageAfterShield <= 0)
            {
                enemy.CurrentShield -= trueAttackDamage;
                shieldLost = trueAttackDamage;
            }
            else
            {
                shieldLost = enemy.CurrentShield;
                enemy.CurrentShield = 0;
                enemy.CurrentHealth -= damageAfterShield;
            }

            // check if champion have temporary shield effect 
            // then update shield value remain
            UpdateRemainShield(enemy, shieldLost);
        }
        else // champ not have shield
        {
            enemy.CurrentHealth -= trueAttackDamage;
        }
    }

    private void UpdateRemainShield(OnFieldCharacter enemy, float shieldLost)
    {
        for(int i = 0; i < enemy.Effects.Count; i++) 
            if (enemy.Effects[i] is TemporaryShield)
            {
                TemporaryShield temporaryShield = enemy.Effects[i] as TemporaryShield;

                temporaryShield.RemainShield -= shieldLost;

                // if shield is break then remove shield effect from champion
                if (temporaryShield.RemainShield <= 0)
                {
                    // update shield lost
                    shieldLost = -temporaryShield.RemainShield;

                    temporaryShield.RemoveEffect();
                    enemy.UpdateEffectIcon();

                    // run this method again to find next shield
                    UpdateRemainShield(enemy, shieldLost);
                }
            }
    }
    #endregion
}
