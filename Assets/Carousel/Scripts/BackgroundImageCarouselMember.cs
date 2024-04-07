using UnityEngine;
using UnityEngine.UI;

public class BackgroundImageCarouselMember : CarouselMember
{
    private Image _image;
    [SerializeField] private Image _backgroundDisplay;

    private void Start()
    {
        _image = GetComponent<Image>();
    }

    public override void RunCarouselMember()
    {
        _backgroundDisplay.sprite = _image.sprite;
    }
}
