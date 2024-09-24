using UnityEngine;

public class Effect : MonoBehaviour
{
    protected string effectName;
    protected string effectAvatar;
    protected float effectValue;
    protected int startTurn;
    protected int endTurn;
    protected OnFieldCharacter champion;

    public string EffectName { get { return effectName; } }
    public string EffectAvatar { get { return effectAvatar; } }
    public float EffectValue { get { return effectValue; } set { effectValue = value; } }
    public int StartTurn { get { return startTurn; } set { startTurn = value; } }
    public int EndTurn { get { return endTurn; } set { endTurn = value; } }
    public OnFieldCharacter Champion { get { return champion; } set { champion = value; } }

    public virtual void EffectFunction() { }

    // update effect turn remain
    public void UpdateEffect()
    {
        if (endTurn - startTurn > 0)
        {
            endTurn--;
            if (endTurn - startTurn == 0)
            {
                RemoveEffect();
            }
        }
    }

    public virtual void RemoveEffect()
    {
        // remove effect out of champion effect list
        champion.Effects.Remove(this);
    }
}
