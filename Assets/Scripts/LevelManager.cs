using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject transitionObject;
    [SerializeField] private float speedOfTransition_before;
    [SerializeField] private float speedOfTransition_after;
    [SerializeField] private float timeToStop;

    [SerializeField] private MovePlayer player_1;
    [SerializeField] private MovePlayer player_2;

    Material circleFade;
    Camera mainCamera;
    RectTransform imageRect;
    bool hadStop;

    void Awake()
    {
        mainCamera = Camera.main;
        circleFade = transitionObject.GetComponent<Image>().material;
        imageRect = transitionObject.GetComponent<Image>().GetComponent<RectTransform>();

        circleFade.SetFloat("_ScreenAspect", (float)Screen.width / Screen.height);

        StartCoroutine(IntroAnimation());
    }

    IEnumerator IntroAnimation()
    {
        hadStop = false;

        player_1.canMove = false;  player_1.canPickUpBox = false;
        player_1.gameObject.GetComponent<Shooter>().canShoot = false;
        player_1.gameObject.GetComponent<Shooter>().haveBullet = true;

        if (player_2 != null)
        {
            player_2.canMove = false;
            player_2.canPickUpBox = false;
        }

        SetCircleOnPos(player_1.transform.position);

        float currentRadius = -0.01f, timer = 0;
        circleFade.SetFloat("_Radius", currentRadius);

        while (currentRadius < 3f)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                hadStop = true;
                continue;
            }

            float speedOfTransition = (hadStop) ? speedOfTransition_after : speedOfTransition_before;

            currentRadius += speedOfTransition * Time.deltaTime;
            circleFade.SetFloat("_Radius", currentRadius);

            if (Mathf.Abs(currentRadius - 0.15f) < 0.01f)
            {
                timer = timeToStop;
                currentRadius = 0.152f;
            }

            yield return null;
        }

        hadStop = false;

        player_1.canMove = true;
        player_1.gameObject.GetComponent<Shooter>().canShoot = true;
    }

    IEnumerator OutroAnimation(MovePlayer player)
    {
        hadStop = false;

        player.canMove = false; player.canPickUpBox = false;
        if (player.gameObject.GetComponent<Shooter>())
        {
            player.gameObject.GetComponent<Shooter>().canShoot = false;
            player.gameObject.GetComponent<Shooter>().haveBullet = true;
        }

        SetCircleOnPos(player.transform.position);

        float currentRadius = 3f, timer = 0;
        circleFade.SetFloat("_Radius", currentRadius);

        while (currentRadius >= -0.01f)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                hadStop = true;
                continue;
            }

            float speedOfTransition = (hadStop) ? speedOfTransition_after : speedOfTransition_before;

            currentRadius -= speedOfTransition * Time.deltaTime;
            circleFade.SetFloat("_Radius", currentRadius);

            if (Mathf.Abs(currentRadius - 0.15f) < 0.01f)
            {
                timer = timeToStop;
                currentRadius = 0.148f;
            }

            yield return null;
        }

        hadStop = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void PlayerInTheDoor(MovePlayer player)
    {
        if (player == player_2 && player_2 != null)
        {
            StartCoroutine(OutroAnimation(player_2));
            Debug.Log("End of lvl");
        }
        else if (player_2 != null)
        {
            player_2.canMove = true;
            player_2.canPickUpBox = true;
            player_1.canMove = false;
            player_1.canPickUpBox = false;
            player_1.gameObject.GetComponent<Shooter>().canShoot = false;
        } else
        {
            StartCoroutine(OutroAnimation(player_1));
        }
    }

    void SetCircleOnPos(Vector3 pos)
    {
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(pos);
        circleFade.SetVector("_CenterUV", new Vector2(viewportPos.x, viewportPos.y));
    }
}
