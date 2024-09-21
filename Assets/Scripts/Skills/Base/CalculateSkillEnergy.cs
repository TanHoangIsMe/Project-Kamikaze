using System.Collections.Generic;

public class CalculateSkillEnergy
{
    // decrease champion mana
    public void ReduceCharacterMana(OnFieldCharacter character, float manaCost)
    {
        character.CurrentMana -= manaCost;
    }

    // decrease champion burst
    public void ReduceCharacterBurst(OnFieldCharacter character, float burstCost)
    {
        character.CurrentBurst = 0;
    }

    // restore champion mana after using first
    public void RestoreMana(OnFieldCharacter character)
    {
        character.CurrentMana += 10f;
    }

    // increase character burst when deal or receive damage
    public void IncreaseBurstBaseOnDamage(OnFieldCharacter character, 
        List<OnFieldCharacter> enemies, float damage)
    {
        // for every 10 damage receive 1 burst pont
        float restoreBurst = damage / 10f;

        // restore character that deal damage
        character.CurrentBurst += restoreBurst;

        // restore character that receive damage
        foreach (var enemy in enemies) 
            enemy.CurrentBurst += restoreBurst;
    }
}
