using TMPro;
using Unity.Netcode;
using UnityEngine;

public class SkillPriority : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI skillPriorityTittle;

    private CheckNumberOfTargets checkNumberOfTarget;
    public CheckNumberOfTargets CheckNumberOfTargets { set { checkNumberOfTarget = value; } }

    private bool isHostClick;
    public bool IsHostClick { set { isHostClick = value; } }

    public void SetTittle(string tittle)
    {
        skillPriorityTittle.text = tittle;
    }

    public void CheckCanOpen()
    {
        // turn off choose panel on player who not click
        if(isHostClick && !IsHost || !isHostClick && IsHost)
            gameObject.SetActive(false);
    }

    [ServerRpc (RequireOwnership = false)]
    public void ChoosingLowestPriorityServerRpc()
    {
        ChoosingLowestPriorityClientRpc();
    }

    [ServerRpc (RequireOwnership = false)]
    public void ChoosingHighestPriorityServerRpc()
    {
        ChoosingHighestPriorityClientRpc();
    }

    [ClientRpc]
    private void ChoosingHighestPriorityClientRpc()
    {
        ChoosingHighestPriority();
    }

    [ClientRpc]
    private void ChoosingLowestPriorityClientRpc()
    {
        ChoosingLowestPriority();
    }

    public void ChoosingHighestPriority()
    {
        if (checkNumberOfTarget != null)
            checkNumberOfTarget.AutoFindTargetsBasedOnPriority(false);
    }

    public void ChoosingLowestPriority()
    {
        if (checkNumberOfTarget != null)
            checkNumberOfTarget.AutoFindTargetsBasedOnPriority(true);
    }
}
