using UnityEngine;

public class GateManager : MonoBehaviour
{
    [SerializeField] private bool isOpened = false;

    public void Activate()
    {
        isOpened = !isOpened;
        Debug.Log("UP");
    }
}
