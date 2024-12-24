using UnityEngine;

public class FallingSword : MonoBehaviour
{
    [SerializeField] ParticleSystem swordFalling;
    private AutoFindTargets autoFindTargets;

    private void Start()
    {
        autoFindTargets = FindAnyObjectByType<AutoFindTargets>();

        Vector3 startPosition = Vector3.zero;
        BoxCollider boxCollider;

        if (autoFindTargets != null && autoFindTargets.EnemyTargets[0] != null)
        {
            boxCollider = autoFindTargets.EnemyTargets[0].GetComponent<BoxCollider>();
            if (boxCollider != null)
                startPosition = new Vector3(
                    autoFindTargets.EnemyTargets[0].transform.position.x,
                    boxCollider.size.y + 5f,
                    autoFindTargets.EnemyTargets[0].transform.position.z);
        }

        if (startPosition != Vector3.zero)
            transform.position = startPosition;
    }

    private void Update()
    {
       if (!swordFalling.isPlaying) Destroy(gameObject);
    }
}
