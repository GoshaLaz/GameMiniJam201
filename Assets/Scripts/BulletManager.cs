using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public float timerDestroy;

    private void FixedUpdate()
    {
        timerDestroy -= Time.deltaTime;
        if (timerDestroy <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "DestroyObj")
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
