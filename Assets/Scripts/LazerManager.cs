using UnityEngine;

public class LazerManager : MonoBehaviour
{
    public float maxDistance = 100f;
    public LayerMask solidLayer;

    public SpriteRenderer spriteRenderer;

    void Update()
    {
        CastLaser();
    }

    void CastLaser()
    {
        Vector2 origin = transform.position;
        Vector2 direction = transform.right; 

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, maxDistance, solidLayer);

        float distance = maxDistance;

        if (hit.collider != null)
        {
            distance = hit.distance;
        }

        spriteRenderer.size = new Vector2(distance, spriteRenderer.size.y);

        spriteRenderer.transform.localPosition = new Vector3(distance / 2f, 0f, 0f);
    }
}
