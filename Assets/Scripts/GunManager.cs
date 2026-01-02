using UnityEngine;

public class GunManager : MonoBehaviour
{
    public Transform player;

    public float radius = 1.5f;
    public bool rotateAim = true;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (player == null) return;

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        Vector2 direction = (mouseWorldPos - player.position).normalized;

        transform.position = (Vector2)player.position + direction * radius;

        if (rotateAim)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}