using UnityEngine;

public class PopUpDamage : MonoBehaviour
{
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
        transform.rotation = cam.transform.rotation;
    }

    private void Start()
    {
        Destroy(gameObject, 1.5f);
    }
}
