using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Image button;

    private Color32 normalColor = new Color32(255, 255, 255, 255);
    private Color32 hoverColor = new Color32(255, 0, 0, 255);


    public void OnPointerEnter(PointerEventData eventData)
    {
        button.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        button.color = normalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Click");
    }


    private void Start()
    {
        button = GetComponent<Image>();
        button.color = normalColor;
    }

}
