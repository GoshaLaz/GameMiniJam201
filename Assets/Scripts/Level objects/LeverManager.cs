using UnityEngine;

public class LeverManager : MonoBehaviour
{
    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;
    [SerializeField] private bool isActive;
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = (isActive) ? onSprite : offSprite;
    }

    public void Interaction()
    {
        isActive = !isActive;

        spriteRenderer.sprite = (isActive) ? onSprite : offSprite;
    }
}
