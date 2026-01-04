using UnityEngine;

public class GateManager : MonoBehaviour
{
    [SerializeField] private bool isOpened = false;

    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        animator.SetBool("isOpen", isOpened);
    }

    public void Activate()
    {
        isOpened = !isOpened;
        Debug.Log("Some thing is here!");
    }
}
