using System.Collections;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;
    [Space(5)]
    [SerializeField] private bool isFinishDoor;
    [SerializeField] private GameObject doorSigne;
    [SerializeField] private BoxCollider2D checkCollider;
    [Space(5)]
    [SerializeField] private GameObject player_1;
    [SerializeField] private GameObject maskObject;
    [SerializeField] private Vector2 offSet;
    [SerializeField] private float durationOfAnimation;
    [SerializeField] private bool fromRight;

    void Awake()
    {
        doorSigne.SetActive(isFinishDoor);
        maskObject.SetActive(false);
        maskObject.transform.position = transform.position + new Vector3((fromRight) ? -1.13265f: 1.13265f, 0, 0);

        if (!isFinishDoor)
        {
            maskObject.SetActive(true);
            StartCoroutine(PlayerOut());
        }
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
                        StartCoroutine(PlayerIn(currHit.collider.gameObject));
                    }
                }
            }
        }
    }

    IEnumerator PlayerIn(GameObject playerObj)
    {
        maskObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        float timer = 0f;

        Vector3 startPos = transform.position + (Vector3)offSet;
        Vector3 endPos = maskObject.transform.position + (Vector3)offSet;

        playerObj.GetComponent<Collider2D>().enabled = false;
        playerObj.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

        playerObj.transform.position = startPos;

        if (playerObj.GetComponent<Shooter>()) playerObj.GetComponent<Shooter>().aim.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);

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
        playerObj.transform.localScale = Vector3.zero;

        levelManager.PlayerInTheDoor(playerObj.GetComponent<MovePlayer>());

        maskObject.SetActive(false);
    }

    IEnumerator PlayerOut()
    {
        yield return new WaitForSeconds(0.1f);
        float timer = 0f;

        Vector3 startPos = maskObject.transform.position + (Vector3)offSet;
        Vector3 endPos = transform.position + (Vector3)offSet;

        player_1.GetComponent<Collider2D>().enabled = false;
        player_1.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

        player_1.transform.position = startPos;

        yield return new WaitForSeconds(0.5f);

        if ((fromRight && player_1.transform.localScale.x > 0) || (!fromRight && player_1.transform.localScale.x < 0))
        {
            Vector3 currentScale = player_1.transform.localScale;
            currentScale.x *= -1;
            player_1.transform.localScale = currentScale;
        }

        while (timer < durationOfAnimation)
        {
            timer += Time.deltaTime;
            float t = timer / durationOfAnimation;
            player_1.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        player_1.transform.position = endPos;

        player_1.GetComponent<Collider2D>().enabled = true;
        player_1.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        maskObject.SetActive(false);
    }
}
