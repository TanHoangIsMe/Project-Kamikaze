public class ArmorBreak : Effect
{
    public ArmorBreak()
    {
        effectName = "Armor Break";
        effectAvatar = "Art/UI/Effect Avatars/Armor Break";
    }

    public override void EffectFunction()
    {
        if (champion != null)
        {
            champion.CurrentArmor -= effectValue;
        }
    }

    public override void RemoveEffect()
    {
        if (champion != null)
        {
            int removeIndex = 0;
            for (int i = champion.Effects.Count - 1; i >= 0; i--)
                if (champion.Effects[i] is ArmorBreak)
                {
                    removeIndex = i;
                }

            // restore armor value
            champion.CurrentArmor += champion.Effects[removeIndex].EffectValue;

            // remove effect out of champion effect list
            champion.Effects.RemoveAt(removeIndex);
        }
    }
}
