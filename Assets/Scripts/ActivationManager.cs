using UnityEngine;

public class ActivationManager : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsToInteract;

    private void Awake()
    {
        if (objectsToInteract.Length <= 0) Debug.Log("No object to interact");
    }

    public void Interaction()
    {
        foreach (GameObject objectToInteract in objectsToInteract)
        {
            if (objectToInteract.GetComponent<LazerManager>() != null)
            {
                objectToInteract.GetComponent<LazerManager>().Activate();
            }
            else if (objectToInteract.GetComponent<GateManager>() != null)
            {
                objectToInteract.GetComponent<GateManager>().Activate();
            }
        }
    }
}
