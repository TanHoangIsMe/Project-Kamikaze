using UnityEngine;

public class PopUpDamage : MonoBehaviour
{
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
        transform.rotation = cam.transform.rotation;
    }
}
