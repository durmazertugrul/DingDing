using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FxView : MonoBehaviour
{
    public Image image;
    public RectTransform RectTransform {  get; private set; }


    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
    }

    public void Setup(Sprite sprite, Color color, Vector2 position, float size)
    {
        image.sprite = sprite;
        image.color = color;
        RectTransform.anchoredPosition = position;
        RectTransform.sizeDelta = new Vector2(size, size);
        RectTransform.localScale = Vector3.one;
        RectTransform.localRotation = Quaternion.identity;
    }

    public void ResetVisualState()
    {
        RectTransform.DOKill();
        image.DOKill();
    }
}
