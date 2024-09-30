public class Taunt : Effect
{
    public Taunt()
    {
        effectName = "Taunt";
        effectAvatar = "Art/UI/Effect Avatars/Taunt";
    }

    public override void EffectFunction() { }

    public override void RemoveEffect()
    {  
        for (int i = 0; i < champion.Effects.Count; i++)
            if (champion.Effects[i] is Taunt)
                // remove effect out of champion effect list
                champion.Effects.RemoveAt(i); 
    }
}
