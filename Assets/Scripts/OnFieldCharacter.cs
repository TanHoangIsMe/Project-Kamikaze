using UnityEngine;

public class OnFieldCharacter : MonoBehaviour 
{
    private Character currentCharacter;
    private float currentAttack;
    private float currentArmor;
    private float currentHealth;

    public Character CurrentCharacter { set { currentCharacter = value; } }
    public float CurrentAttack { get { return currentAttack; } set { currentAttack = value; } }
    public float CurrentArmor { get { return currentArmor; } set { currentArmor = value; } }
    public float CurrentHealth { get { return currentHealth; } set { currentHealth = value; } }
    private void Start()
    {
        currentAttack = currentCharacter.Attack;
        currentArmor = currentCharacter.Armor;
        currentHealth = currentCharacter.Health;
    }

    public void UsingSecondSkill(OnFieldCharacter character, OnFieldCharacter[] targets)
    {
        currentCharacter.UsingSecondSkill(character,targets);
    }
}
