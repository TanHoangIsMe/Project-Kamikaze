public class Taunted : Effect
{
    private OnFieldCharacter tauntedBy;
    public OnFieldCharacter TauntedBy { get { return tauntedBy; } set { tauntedBy = value; } }

    public Taunted()
    {
        effectName = "Taunted";
        effectAvatar = "Art/UI/Effect Avatars/Taunted";
    }
}
