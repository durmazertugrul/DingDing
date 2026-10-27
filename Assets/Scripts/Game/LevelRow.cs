using DingDing.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// One line of the rankings list. The same component drives the nine
// generated rows and the title fight row sitting in the scene.
public class LevelRow : MonoBehaviour
{
    public Image background;
    public TMP_Text levelLabel;
    public Image difficultyTag;
    public GameObject lockIcon;
    public GameObject fightButton;
    public GameObject doneTick;

    public Sprite lockedBackground;
    public Sprite currentBackground;
    public Sprite completedBackground;

    public Sprite normalTagOnNavy;
    public Sprite normalTagOnCream;
    public Sprite hardTag;
    public Sprite superHardTag;

    private int levelNumber;
    private RankingsScreen screen;

    public void Bind(RankingsScreen owner, int number, Difficulty difficulty, RowState state)
    {
        screen = owner;
        levelNumber = number;

        levelLabel.text = "LEVEL " + number;

        background.sprite = BackgroundFor(state);
        difficultyTag.sprite = TagFor(difficulty, state);

        lockIcon.SetActive(state == RowState.Locked);
        fightButton.SetActive(state == RowState.Current);
        doneTick.SetActive(state == RowState.Completed);
    }
       

    // The title fight row carries the same sprite in all three fields, so
    // its own background never changes.
    private Sprite BackgroundFor(RowState state)
    {
        if (state == RowState.Locked) return lockedBackground;
        if (state == RowState.Current) return currentBackground;

        return completedBackground;
    }
       

    // A locked row is navy and the others are cream, so the normal tag
    // comes in two versions.
    private Sprite TagFor(Difficulty difficulty, RowState state)
    {
        if (difficulty == Difficulty.Hard) return hardTag;
        if (difficulty == Difficulty.SuperHard) return superHardTag;

        return state == RowState.Locked ? normalTagOnNavy : normalTagOnCream;
    }
      

    // Wired to Btn_Fight in the prefab.
    public void OnFightPressed()
    {
        screen.StartLevel(levelNumber);
    }
        
}

   