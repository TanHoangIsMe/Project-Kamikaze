using UnityEngine;

public class TemporaryShield : Effect
{
    private float remainShield;
    public float RemainShield { get {  return remainShield; } set { remainShield = value; } }

    public TemporaryShield()
    {
        effectAvatar = "";
        effectValue = 0;
        startTurn = 0;
        endTurn = 0;
        remainShield = 1;
    }

    public override void EffectFunction()
    {
        OnFieldCharacter champion = gameObject.GetComponent<OnFieldCharacter>();
        champion.CurrentShield += effectValue;
        remainShield = 200f;
    }

    private void Update()
    {
        if (remainShield <= 0)
        {
            OnFieldCharacter champion = gameObject.GetComponent<OnFieldCharacter>();
            if (champion != null)
                champion.Effects.Remove("Temporary Shield");
            Destroy(this);
        }
    }

    public override void RemoveEffect()
    {
        OnFieldCharacter champion = gameObject.GetComponent<OnFieldCharacter>();
        if (champion != null)
        {
            champion.Effects.Remove("Temporary Shield");
            champion.CurrentShield -= remainShield;
        }
        Destroy(this);
    }
}
