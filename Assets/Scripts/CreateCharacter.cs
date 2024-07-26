using UnityEngine;

public class CreateCharacter : MonoBehaviour
{
    private void Start()
    {
        Character maria = new Maria();
        Debug.Log(maria.Name);
    }
}
