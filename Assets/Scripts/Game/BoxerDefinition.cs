using UnityEngine;

// The order the face sprites are stored in.
public enum FaceState
{
    Fresh,
    Tired,
    Hurt,
    Dazed
}

[CreateAssetMenu(menuName = "DingDing/Boxer")]
public class BoxerDefinition : ScriptableObject
{
    public string displayName;               // shown only for the champion
    public Sprite[] openSprites;             // four, in FaceState order
    public Sprite[] blinkSprites;            // four, the same order
}

