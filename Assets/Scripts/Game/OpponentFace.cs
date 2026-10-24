using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

// Picks the opponent's expression from the health left, and blinks now
// and then so the face does not look frozen.
public class OpponentFace : MonoBehaviour
{
    public Image faceImage;
    public float blinkInterval = 4f;
    public float blinkDuration = 0.12f;

    private BoxerDefinition boxer;
    private FaceState state = FaceState.Fresh;
    private float timeUntilBlink;
    private float blinkTimeLeft;

    public float hitShakeStrength = 12f;
    public float hitShakeDuration = 0.25f;
    public Color hitFlashColor = new Color(1f, 0.78f, 0.78f);
    private Tween hitTween;
    private Vector2 restPosition;

    private void Awake()
    {
        restPosition = faceImage.rectTransform.anchoredPosition;
    }

    public void SetBoxer(BoxerDefinition definition)
    {
        boxer = definition;
        timeUntilBlink = blinkInterval;
        blinkTimeLeft = 0f;
        ShowOpenEyes();
    }
       

    public void Refresh(int health, int maxHealth, bool isKnockout)
    {
        if (isKnockout)
            state = FaceState.Dazed;

        else if (health * 100 < maxHealth * 33)
            state = FaceState.Hurt;

        else if (health * 100 < maxHealth * 66)
            state = FaceState.Tired;

        else
            state = FaceState.Fresh;

        ShowOpenEyes();
    }
       

    private void Update()
    {
        if (boxer == null || state == FaceState.Dazed)
            return;

        if (blinkTimeLeft > 0f)
        {
            blinkTimeLeft = blinkTimeLeft - Time.deltaTime;
            if (blinkTimeLeft <= 0f)
                ShowOpenEyes();
            return;
        }


        timeUntilBlink = timeUntilBlink - Time.deltaTime;
        if (timeUntilBlink <= 0f)
        {
            faceImage.sprite = boxer.blinkSprites[(int)state];
            blinkTimeLeft = blinkDuration;
            timeUntilBlink = blinkInterval + Random.Range(-1f, 1f);
        }
           
    }
        

    private void ShowOpenEyes()
    {
        if (boxer == null)
            return;

        faceImage.sprite = boxer.openSprites[(int)state];
    }


    // A hit has to be felt somewhere other than the bar, so the face takes
    // a knock. Strength runs from 0 to 1; a cascade hit lands harder.
    public void TakeHit(float strength)
    {
        if (hitTween != null && hitTween.IsActive())
            hitTween.Kill();

        faceImage.rectTransform.anchoredPosition = restPosition;
        faceImage.color = Color.white;

        float punch = hitShakeStrength * Mathf.Clamp01(0.4f + strength);

        hitTween = faceImage.rectTransform
            .DOPunchAnchorPos(new Vector2(-punch, -punch * 0.4f),
                              hitShakeDuration, 8, 0.6f)
            .OnComplete(() =>
                faceImage.rectTransform.anchoredPosition = restPosition);

        faceImage.color = hitFlashColor;
        faceImage.DOColor(Color.white, hitShakeDuration);
    }
}

    