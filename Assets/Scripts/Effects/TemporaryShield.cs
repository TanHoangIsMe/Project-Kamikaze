public class TemporaryShield : Effect
{
    private float remainShield;
    public float RemainShield { get {  return remainShield; } set { remainShield = value; } }

    public TemporaryShield()
    {
        effectName = "Temporary Shield";
        effectAvatar = "Art/UI/Effect Avatars/Temporary Shield";
    }

    public override void EffectFunction()
    {
        if (champion != null)
        {
            champion.CurrentShield += effectValue;
            remainShield = 200f;
        }
    }

    public override void RemoveEffect()
    {
        if (champion != null)
        {
            if (remainShield > 0) 
            {
                // if shield is still on champion then remove it 
                champion.CurrentShield -= remainShield;

                // play champion health bars animation
                SkillHandler skillHandler = FindObjectOfType<SkillHandler>();
                if (skillHandler != null)
                    skillHandler.PlayHealthBarEffect();
            }

            int removeIndex = 0;
            for (int i = champion.Effects.Count - 1; i >= 0; i--)
                if (champion.Effects[i] is TemporaryShield)
                {
                    removeIndex = i;
                }

            // remove effect out of champion effect list
            champion.Effects.RemoveAt(removeIndex);
        }
    }
}
