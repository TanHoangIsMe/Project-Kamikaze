using UnityEngine;

public abstract class Effect : MonoBehaviour
{
    protected string effectAvatar;
    protected float effectValue;
    protected int startTurn;
    protected int endTurn;
    protected OnFieldCharacter champion;

    public string EffectAvatar { get { return effectAvatar; } }
    public float EffectValue { get { return effectValue; } set { effectValue = value; } }
    public int StartTurn { get { return startTurn; } set { startTurn = value; } }
    public int EndTurn { get { return endTurn; } set { endTurn = value; } }
    public OnFieldCharacter Champion { get { return champion; } set { champion = value; } }

    public abstract void EffectFunction();

    public void UpdateEffect()
    {
        if (endTurn > 0)
        {
            endTurn--;
            if (endTurn == 0)
            {
                RemoveEffect();
            }
        }
    }

    public abstract void RemoveEffect();
}
