using UnityEngine;

public abstract class CarouselMember : MonoBehaviour, ICarouselMember
{
    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void SetSize(float size)
    {
        _rectTransform.sizeDelta = new Vector2(size, size);
    }

    public abstract void RunCarouselMember();
}
