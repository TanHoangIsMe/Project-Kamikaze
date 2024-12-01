using UnityEngine;

public class PopUpDamage : MonoBehaviour
{
    private Camera cam;

    private void Awake()
    {
        // set camera 
        cam = Camera.main;
        if (cam == null) // if player 2 switch cam
            cam = GameObject.Find("Player 2 Camera").GetComponent<Camera>();

        transform.rotation = cam.transform.rotation;
    }

    private void Start()
    {
        Destroy(gameObject, 1.5f);
    }
}
