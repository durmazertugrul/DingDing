using UnityEngine;

// Every clip in the game, in one asset. The manager reads from here; no
// scene object holds a clip of its own.
[CreateAssetMenu(menuName = "DingDing/Sounds")]
public class GameSounds : ScriptableObject
{
    public AudioClip swap;
    public AudioClip swapInvalid;
    public AudioClip tileNudge;

    public AudioClip[] matchPops;           // five, quiet to loud

    public AudioClip specialCreate;
    public AudioClip medalBlast;
    public AudioClip trophyBlast;
    public AudioClip beltBlast;
    public AudioClip gongBlast;

    public AudioClip shuffle;
    public AudioClip movesWarning;

    public AudioClip punchLight;
    public AudioClip punchHeavy;
    public AudioClip knockout;
    public AudioClip crowdCheer;

    public AudioClip bell;
    public AudioClip bellLong;

    public AudioClip cardFlip;
    public AudioClip cardSelect;
    public AudioClip cardActivate;

    public AudioClip uiTap;
    public AudioClip uiBack;
    public AudioClip pause;

    public AudioClip levelUnlock;
    public AudioClip resultWin;
    public AudioClip resultLose;
    public AudioClip scoreTick;
    public AudioClip titleWin;
}