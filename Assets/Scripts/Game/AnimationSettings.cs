using DG.Tweening;


// Every duration and easing curve in one place, shown as one foldout
// in the EventPlayer inspector.
[System.Serializable]
public class AnimationSettings
{
    public float swapDuration = 0.16f;
    public float invalidSwapDuration = 0.2f;
    public float popGrowDuration = 0.06f;
    public float popShrinkDuration = 0.12f;
    public float fallDurationPerCell = 0.05f;
    public float specialMergeDuration = 0.12f;
    public float specialGrowDuration = 0.18f;
    public float blastStepDelay = 0.035f;
   
    public Ease swapEase = Ease.OutQuad;
    public Ease popGrowEase = Ease.OutQuad;
    public Ease popShrinkEase = Ease.InBack;
    public Ease fallEase = Ease.OutBounce;
    public Ease specialGrowEase = Ease.OutBack;

    public float burstDuration = 0.35f;
    public float burstEndScale = 1.6f;
    public float shardDuration = 0.4f;
    public float shardDistance = 70f;
    public float sparkDuration = 0.3f;
    public float sparkDistance = 45f;
    public float ringDuration = 0.3f;
    public float ringEndScale = 2.2f;
    public float trailDuration = 0.25f;
    public float trailStepDelay = 0.02f;

    public float shortShakeDuration = 0.15f;
    public float shortShakeStrength = 6f;
    public float hardShakeDuration = 0.3f;
    public float hardShakeStrength = 14f;
    public float longShakeDuration = 0.5f;
    public float longShakeStrength = 10f;
                                           
    public float landSquashDuration = 0.14f;
    public float dragGrowScale = 1.08f;
    public float shuffleGatherDuration = 0.3f;
    public float shuffleSpreadDuration = 0.4f;

}
