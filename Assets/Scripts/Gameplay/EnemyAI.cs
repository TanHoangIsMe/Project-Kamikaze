using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private OnFieldCharacter champion;
    public OnFieldCharacter Champion { get { return champion; } set { champion = value; } }

    private SkillHandler skillHandler;

    private void Awake()
    {
        skillHandler = GetComponent<SkillHandler>();
    }

    private void Start()
    {
        skillHandler.Champion = champion;
        skillHandler.IsCombatSkillMenu = false;

        SwapChampionsLayer();

        if (skillHandler.UsingSkillBurst(2))
        {
            skillHandler.AttackConfirm();
        }
        else
        {
            if (skillHandler.UsingSkill2(1))
            {
                skillHandler.AttackConfirm();
            }
            else
            {
                skillHandler.UsingSkill1(0);
                skillHandler.AttackConfirm();
            }
        }

        SwapChampionsLayer();
    }

    // swap champion layer (enemy -> ally) (ally -> enemy)
    // for enemy use auto find targets function
    private void SwapChampionsLayer()
    {
        foreach(var character in FindObjectsOfType<OnFieldCharacter>())
        {
            if(character != null && character.gameObject.layer == 6)
                character.gameObject.layer = 7;
            else
                character.gameObject.layer = 6;
        }
    }
}
