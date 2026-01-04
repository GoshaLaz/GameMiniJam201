using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] public Transform aim;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 15f;
    [HideInInspector] public bool canShoot = true;
    [HideInInspector] public bool haveBullet = true;

    void Update()
    {
        if (Input.GetMouseButton(0) && canShoot && haveBullet)
        {
            Shoot();
            haveBullet = false;
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null || aim == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        Vector2 direction = (aim.position - firePoint.position).normalized;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * bulletSpeed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
