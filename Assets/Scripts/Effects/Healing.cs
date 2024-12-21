using UnityEngine;

public class Healing : MonoBehaviour
{
    [SerializeField] private ParticleSystem body;
    [SerializeField] private ParticleSystem texture;

    private void Start()
    {
        GameObject character = transform.parent.gameObject;
        BoxCollider boxCollider = character.GetComponent<BoxCollider>();

        if (boxCollider != null && body != null && texture != null)
        {
            var bodyModule = body.main;
            var textureModule = texture.main;
            bodyModule.startLifetime = boxCollider.size.y;
            textureModule.startLifetime = boxCollider.size.y - 0.2f;
        }
    }

    private void Update()
    {
        if (!body.isPlaying) Destroy(gameObject);
    }
}
