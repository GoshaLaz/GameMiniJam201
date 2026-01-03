using UnityEngine;

public class LazerManager : MonoBehaviour
{
    [SerializeField] private LayerMask solidLayer;
    [SerializeField] private LineRenderer lazerRender;

    const int MAX_REFLECTIONS = 10;
    const float MAX_DISTANCE = 100f;

    bool isActive = true;
    bool interacted = false;
    bool interactedLastFrame = false;

    ActivationManager lastReciever = null;

    void Update()
    {
        lazerRender.enabled = isActive;
        if (!isActive) return;

        CastLazer();
    }

    private void CastLazer()
    {
        Vector2 origin = transform.position;
        Vector2 direction = -transform.right;

        lazerRender.positionCount = 1;
        lazerRender.SetPosition(0, origin);

        for (int i = 0; i < MAX_REFLECTIONS; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(origin, direction, MAX_DISTANCE, solidLayer);

            if (!hit)
            {
                AddPoint(origin + direction * MAX_DISTANCE);
                break;
            }

            AddPoint(hit.point);

            if (hit.collider.CompareTag("Receiver"))
            {
                if (!interactedLastFrame)
                {
                    hit.collider.GetComponent<ActivationManager>().Interaction();
                    lastReciever = hit.collider.GetComponent<ActivationManager>();
                    interactedLastFrame = true;
                    interacted = true;
                }

                return;
            }

            if (hit.collider.CompareTag("Mirror"))
            {
                direction = Vector2.Reflect(direction, hit.normal).normalized;
                origin = hit.point + direction * 0.02f;
                continue;
            }

            break;
        }

        if (interacted && lastReciever != null)
        {
            interacted = false;
            interactedLastFrame = false;
            lastReciever.Interaction();
            lastReciever = null;
        }
    }

    private void AddPoint(Vector2 point)
    {
        lazerRender.positionCount++;
        lazerRender.SetPosition(lazerRender.positionCount - 1, point);
    }

    public void Activate()
    {
        isActive = !isActive;
    }
}
