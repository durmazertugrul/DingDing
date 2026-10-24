using DingDing.Core.Cards;
using UnityEngine;

// What a card looks like on screen. The rules live in Core; this is the
// face the player reads.
[CreateAssetMenu(menuName = "DingDing/Card")]
public class CardDisplay : ScriptableObject
{
    public CardId id;
    public string displayName;

    [TextArea]
    public string effectText;

    public Sprite art;
}