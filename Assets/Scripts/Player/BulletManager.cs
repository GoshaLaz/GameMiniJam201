using Unity.VisualScripting;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    [SerializeField] private LayerMask layerToCheck;
    [SerializeField] private float timerDestroy;

    BoxCollider2D boxCollider;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        timerDestroy -= Time.deltaTime;
        if (timerDestroy <= 0) Destroy(gameObject);

        RaycastHit2D hit = Physics2D.BoxCast(transform.position + (Vector3)boxCollider.offset, boxCollider.size, 0, Vector2.up, 0, layerToCheck);

        if (hit)
        {
            if (hit.collider.GetComponent<RopeManager>() != null)
            {
                hit.collider.GetComponent<RopeManager>().FreeTheBox();
                Destroy(hit.collider.gameObject);
            }
            Destroy(gameObject);
        }
    }
}
