using System.Xml.Linq;
using UnityEngine;

public class RopeManager : MonoBehaviour
{
    [SerializeField] private GameObject boxObject;
    [Space(4)]
    [SerializeField] private LayerMask layerMask;

    LineRenderer lineRenderer;
    BoxCollider2D boxCollider;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        boxCollider  = GetComponent<BoxCollider2D>();

        Vector2 origin = transform.position;
        Vector2 direction = Vector2.up;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, 1000f, layerMask);

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, hit.point);

        Debug.Log(hit.point);

        boxCollider.size = new Vector2(0.4f, Vector2.Distance(transform.position, hit.point));
        boxCollider.offset = new Vector2(0, Vector2.Distance(transform.position, hit.point) /2);

        boxObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        boxObject.transform.position = transform.position;
    }

    public void FreeTheBox()
    {
        boxObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
    }
}
