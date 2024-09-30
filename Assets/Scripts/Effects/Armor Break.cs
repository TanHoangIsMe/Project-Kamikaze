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
            champion.CurrentArmor += effectValue;

            //for (int i = 0; i < champion.Effects.Count; i++)
            //    if (champion.Effects[i] is ArmorBreak)
            //    {
            //        // remove effect out of champion effect list
            //        champion.Effects.RemoveAt(i);
            //        break;
            //    }
        }
    }
}
