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
       
}

    