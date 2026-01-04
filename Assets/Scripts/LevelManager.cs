using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject transitionObject;
    [SerializeField] private float speedOfTransition_before;
    [SerializeField] private float speedOfTransition_after;
    [SerializeField] private float radiusToStop;

    [SerializeField] private MovePlayer player_1;
    [SerializeField] private MovePlayer player_2;

    Material circleFade;
    Camera mainCamera;
    RectTransform imageRect;

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
        player_1.gameObject.SetActive(false);

        player_1.canMove = false; player_1.canPickUpBox = false;
        player_1.gameObject.GetComponent<Shooter>().canShoot = false;
        player_1.gameObject.GetComponent<Shooter>().haveBullet = true;
        player_1.gameObject.GetComponent<Shooter>().aim.gameObject.SetActive(false);

        if (player_2 != null)
        {
            player_2.canMove = false;
            player_2.canPickUpBox = false;
        }

        SetCircleOnPos(player_1.transform.position);

        float currentRadius = -0.01f;
        circleFade.SetFloat("_Radius", currentRadius);

        while (currentRadius < radiusToStop)
        {
            currentRadius += speedOfTransition_before * Time.deltaTime;
            circleFade.SetFloat("_Radius", currentRadius);
            yield return null;
        }

        DoorManager[] doorManagers = FindObjectsOfType<DoorManager>();
        foreach (DoorManager door in doorManagers)
        {
            if (!door.isFinishDoor && door.player_1 == player_1.gameObject)
            {
                player_1.gameObject.SetActive(true);
                yield return StartCoroutine(door.PlayerAnimation(door.player_1, door.maskObject.transform.position, door.transform.position, true));
                break;
            }
        }

        while (currentRadius < 3f)
        {
            currentRadius += speedOfTransition_after * Time.deltaTime;
            circleFade.SetFloat("_Radius", currentRadius);
            yield return null;
        }

        player_1.canMove = true;
        player_1.gameObject.GetComponent<Shooter>().canShoot = true;
        player_1.gameObject.GetComponent<Shooter>().aim.gameObject.SetActive(true);
    }

    IEnumerator OutroAnimation(MovePlayer player)
    {
        player.canMove = false; player.canPickUpBox = false;
        if (player.gameObject.GetComponent<Shooter>())
        {
            player.gameObject.GetComponent<Shooter>().canShoot = false;
            player_1.gameObject.GetComponent<Shooter>().aim.gameObject.SetActive(false);
        }

        SetCircleOnPos(player.transform.position);

        float currentRadius = 3f;
        circleFade.SetFloat("_Radius", currentRadius);

        while (currentRadius > radiusToStop)
        {
            currentRadius -= speedOfTransition_after * Time.deltaTime;
            circleFade.SetFloat("_Radius", currentRadius);
            yield return null;
        }

        DoorManager[] doorManagers = FindObjectsOfType<DoorManager>();
        foreach (DoorManager door in doorManagers)
        {
            if (door.isFinishDoor && door.player_1 == player.gameObject)
            {
                yield return StartCoroutine(door.PlayerAnimation(door.player_1, door.transform.position, door.maskObject.transform.position, false));
                break;
            }
        }

        while (currentRadius > -0.01f)
        {
            currentRadius -= speedOfTransition_before * Time.deltaTime;
            circleFade.SetFloat("_Radius", currentRadius);
            yield return null;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Debug.Log("Next Level!");
    }

    public void PlayerInTheDoor(MovePlayer player)
    {
        if (player == player_2 && player_2 != null)
        {
            StartCoroutine(OutroAnimation(player_2));
        }
        else if (player_2 != null)
        {
            player_2.canMove = true;
            player_2.canPickUpBox = true;
            player_1.canMove = false;
            player_1.canPickUpBox = false;
            player_1.gameObject.GetComponent<Shooter>().canShoot = false;
            player_1.gameObject.GetComponent<Shooter>().aim.gameObject.SetActive(false);

            DoorManager[] doorManagers = FindObjectsOfType<DoorManager>();
            foreach (DoorManager door in doorManagers)
            {
                if (door.isFinishDoor && door.player_1 == player.gameObject)
                {
                    StartCoroutine(door.PlayerAnimation(door.player_1, door.transform.position, door.maskObject.transform.position, false));
                    break;
                }
            }
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
