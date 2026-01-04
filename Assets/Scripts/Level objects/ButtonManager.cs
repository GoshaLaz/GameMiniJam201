using Unity.VisualScripting;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private GameObject tutorial;

    private void Start()
    {
        if (tutorial != null) tutorial.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.CompareTag("Box"))
            {
                GetComponent<ActivationManager>().Interaction();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.CompareTag("Box"))
            {
                GetComponent<ActivationManager>().Interaction();

                if (tutorial != null) tutorial.SetActive(true);
            }
        }
    }
}
