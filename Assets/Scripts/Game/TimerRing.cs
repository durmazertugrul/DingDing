using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


// The gold ring drains with the clock and turns red at the end.
public class TimerRing : MonoBehaviour
{
    public Image ringImage;            // Filled, Radial 360
    public TMP_Text secondsText;

    public Color normalColor;
    public Color warningColor;
    public int warningSeconds = 3;

    private Tween pulseTween;

    public void Show(float secondsLeft, float totalSeconds)
    {
        ringImage.fillAmount = secondsLeft / totalSeconds;

        int wholeSeconds = Mathf.CeilToInt(secondsLeft);
        secondsText.text = wholeSeconds.ToString();

        bool isWarning = wholeSeconds <= warningSeconds;
        ringImage.color = isWarning ? warningColor : normalColor;
        secondsText.color = isWarning ? warningColor : normalColor;
    }
        

    // Called once per second over the last few seconds.
    public void Pulse()
    {
        if (pulseTween != null && pulseTween.IsActive())
            pulseTween.Kill();

        secondsText.rectTransform.localScale = Vector3.one;
        pulseTween = secondsText.rectTransform.DOPunchScale(Vector3.one * 0.35f, 0.3f, 1, 0.5f);
    }
        

    public void Stop()
    {
        if (pulseTween != null && pulseTween.IsActive())
            pulseTween.Kill();

        secondsText.rectTransform.localScale = Vector3.one;
    }
        
}

   