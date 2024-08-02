using System.Collections.Generic;
using UnityEngine;

public class UrielAPlotexia : Character
{
    public UrielAPlotexia()
    {
        fullName = "Uriel A Plotexia";
        roleTypes = new RoleType[] { RoleType.Tanker };
        classTypes = new ClassType[] { ClassType.Defender };
        elementTypes = new ElementType[] { ElementType.Light };
        attack = 20f;
        armor = 100f;
        speed = 20f;
        health = 1000f;
        maxMana = 80f;
        maxBurst = 100f;
        skills = new Skill[] {
            new SwingTheSword(),
            new SwingTheSword(),
            new SwingTheSword()
        };
    }
    public override void UsingFirstSkill(GameObject character, List<GameObject> targets)
    {
        skills[0].SkillFunction(character, targets);
    }
    public override void UsingSecondSkill(GameObject character, List<GameObject> targets)
    {
        skills[1].SkillFunction(character, targets);
    }
    public override void UsingBurstSkill(GameObject character, List<GameObject> targets)
    {
        skills[2].SkillFunction(character, targets);
    }
}
