using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems; // Add this namespace

public class ButtonEffects : MonoBehaviour
{
    public Button[] buttons; // Assign all buttons you want to animate in the inspector

    void Start()
    {
        foreach (Button button in buttons)
        {
            // Initial scale down to 90% to create a 'pop' effect on hover
            button.transform.localScale = Vector3.one * 0.9f;
            button.onClick.AddListener(() => PlayClickAnimation(button));
            // Hover effect using DOTween
            button.gameObject.AddComponent<UIElementHoverEffect>();
        }
    }

    void PlayClickAnimation(Button button)
    {
        // Scale animation on click
        button.transform.DOScale(1.1f, 0.1f).OnComplete(() =>
            button.transform.DOScale(1.0f, 0.1f));
    }
}

// Separate component for hover effects
public class UIElementHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(1.1f, 0.1f); // Scales up when mouse hovers over
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(1f, 0.1f); // Scales back to normal when mouse exits
    }
}
