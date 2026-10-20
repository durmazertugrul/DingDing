using DG.Tweening;
using UnityEngine;


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

}
