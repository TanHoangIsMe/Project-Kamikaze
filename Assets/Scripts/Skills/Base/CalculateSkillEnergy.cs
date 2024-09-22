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
        List<OnFieldCharacter> enemies, List<float> damages)
    {
        // calculate total damage that character deal to enemies
        float totalDamage = 0;
        foreach(var damage in damages)
            totalDamage += damage;
            
        // for every 10 damage receive 1 burst point

        // restore character that deal damage
        character.CurrentBurst += totalDamage / 10f;

        // restore character that receive damage
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].CurrentBurst += damages[i] / 10f;
        }          
    }
}