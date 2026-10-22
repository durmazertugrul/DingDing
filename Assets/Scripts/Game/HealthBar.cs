using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

// Two bars on top of each other: the red one snaps to the new value,
// the white one behind it catches up a moment later, so the hit reads.
public class HealthBar : MonoBehaviour
{
    public Image fillImage;                  // red, Image Type set to Filled
    public Image trailImage;                 // white, same rect, behind the red

    public float fillDuration = 0.25f;
    public float trailDuration = 0.45f;
    public float trailDelay = 0.15f;
    public float flashDuration = 0.12f;

    public Color normalColor;
    public Color flashColor;

    private Tween fillTween;
    private Tween trailTween;

    public void SetInstantly(int health, int maxHealth)
    {
        float ratio = Ratio(health, maxHealth);
        KillTweens();
        fillImage.fillAmount = ratio;
        trailImage.fillAmount = ratio;
        fillImage.color = normalColor;
    }
        

    public void ApplyDamage(int health, int maxHealth)
    {
        float ratio = Ratio(health, maxHealth);
        KillTweens();

        fillTween = fillImage.DOFillAmount(ratio, fillDuration).SetEase(Ease.OutQuad);

        trailTween = trailImage.DOFillAmount(ratio, trailDuration).SetDelay(trailDelay).SetEase(Ease.InQuad);

        // A short flash so a small hit is still visible.
        fillImage.color = flashColor;
        fillImage.DOColor(normalColor, flashDuration);
    }
        

    private static float Ratio(int health, int maxHealth)
    {
        if (maxHealth <= 0)
            return 0f;

        return Mathf.Clamp01((float)health / maxHealth);
    }
       

    private void KillTweens()
    {
        if (fillTween != null && fillTween.IsActive())
            fillTween.Kill();
        if (trailTween != null && trailTween.IsActive())
            trailTween.Kill();
    }
        
}

    