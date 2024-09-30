public class Taunted : Effect
{
    private OnFieldCharacter tauntedBy;
    public OnFieldCharacter TauntedBy { get { return tauntedBy; } set { tauntedBy = value; } }

    public Taunted()
    {
        effectName = "Taunted";
        effectAvatar = "Art/UI/Effect Avatars/Taunted";
    }

    public override void EffectFunction() { }

    public override void RemoveEffect()
    {
        for (int i = 0; i < champion.Effects.Count; i++)
            if (champion.Effects[i] is Taunted)             
                // remove effect out of champion effect list
                champion.Effects.RemoveAt(i);
    }
}
