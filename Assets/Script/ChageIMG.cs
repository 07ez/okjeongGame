using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChageIMG : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public Image img;
    public Sprite normalSprite;
    public Sprite pressedSprite;
    public void OnPointerDown(PointerEventData eventData)
    {
        img.sprite = pressedSprite;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        img.sprite = normalSprite;
    }
}
