using System.Collections;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;
    [Space(5)]
    [SerializeField] public bool isFinishDoor;
    [SerializeField] private GameObject doorSigne;
    [SerializeField] private BoxCollider2D checkCollider;
    [Space(5)]
    [SerializeField] public GameObject player_1;
    [SerializeField] public GameObject maskObject;
    [SerializeField] private Vector2 offSet;
    [SerializeField] private float durationOfAnimation;
    [SerializeField] private bool fromRight;

    void Awake()
    {
        doorSigne.SetActive(isFinishDoor);
        maskObject.SetActive(false);
        maskObject.transform.position = transform.position + new Vector3((fromRight) ? -1.13265f: 1.13265f, 0, 0);
    }

    void Update()
    {
        RaycastHit2D[] hit = Physics2D.BoxCastAll((Vector2)transform.position + checkCollider.offset, checkCollider.size, 0, Vector2.zero);

        if (Input.GetKeyDown(KeyCode.Z) && isFinishDoor) {
            if (hit.Length > 0)
            {
                foreach (RaycastHit2D currHit in hit)
                {
                    if (currHit.collider.CompareTag("Player_1") || currHit.collider.CompareTag("Player_2")) {
                        player_1 = currHit.collider.gameObject;
                        levelManager.PlayerInTheDoor(player_1.GetComponent<MovePlayer>());
                    }
                }
            }
        }
    }

    public IEnumerator PlayerAnimation(GameObject playerObj, Vector3 startPos, Vector3 endPos, bool needCollider)
    {
        maskObject.SetActive(true);

        float timer = 0f;

        startPos += (Vector3)offSet;
        endPos += (Vector3)offSet;

        playerObj.GetComponent<Collider2D>().enabled = false;
        playerObj.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

        playerObj.transform.position = startPos;

        if ((fromRight && playerObj.transform.localScale.x > 0) || (!fromRight && playerObj.transform.localScale.x < 0))
        {
            Vector3 currentScale = playerObj.transform.localScale;
            currentScale.x *= -1;
            playerObj.transform.localScale = currentScale;
        }

        while (timer < durationOfAnimation)
        {
            timer += Time.deltaTime;
            float t = timer / durationOfAnimation;
            playerObj.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        playerObj.transform.position = endPos;

        if (needCollider)
        {
            playerObj.GetComponent<Collider2D>().enabled = true;
            playerObj.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        } else
        {
            playerObj.transform.localScale = Vector3.zero;
        }

        maskObject.SetActive(false);
    }
}
