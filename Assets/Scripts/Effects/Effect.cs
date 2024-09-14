using UnityEngine;

public abstract class Effect : MonoBehaviour
{
    protected string effectAvatar;
    protected int duration;
    protected float effectValue;
    protected int startTurn;

    public string EffectAvatar {  get { return effectAvatar; } }
    public int Duration { get { return duration; } set { duration = value; } }
    public float EffectValue { get { return effectValue; } set { effectValue = value; } }
    public int StartTurn { get { return startTurn; } set { startTurn = value; } }

    public abstract void EffectFunction(OnFieldCharacter champ);

    public void UpdateEffect(OnFieldCharacter champ)
    {
        int endTurn = startTurn + duration;
        if (endTurn > 0)
        {
            endTurn--;
            if (endTurn == 0)
            {
                RemoveEffect(champ);
            }
        }
    }

    public void RemoveEffect(OnFieldCharacter champ) 
    {
        champ.Effects.Remove(this);
    }
}
