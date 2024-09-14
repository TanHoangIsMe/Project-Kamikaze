using UnityEngine;

public class TemporaryShield : Effect
{
    public TemporaryShield()
    {
        effectAvatar = "";
        duration = 2;
        effectValue = 200f;
        startTurn = 0;
    }

    public override void EffectFunction(OnFieldCharacter champ)
    {
        champ.CurrentShield += effectValue;
    }
}
