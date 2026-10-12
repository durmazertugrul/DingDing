# Ding Ding! Boxing Match-3

A match-3 puzzle game for mobile, built in Unity, where every match is a punch. You face an opponent over three rounds, drain his health bar with your combos, and pick a strategy card in the corner between rounds. Knock him out before the final bell.

> **Status:** in development. The game design and the art and audio assets are complete. Implementation of the shared rules, the Unity game and the balancing simulator is starting now; the architecture and testing sections below describe the plan.

---

## Contents

- [Gameplay](#gameplay)
- [Special Pieces](#special-pieces)
- [Corner Cards](#corner-cards)
- [Levels and Opponents](#levels-and-opponents)
- [Planned Architecture](#planned-architecture)
- [Project Structure](#project-structure)
- [Testing and Balancing Plan](#testing-and-balancing-plan)
- [Credits](#credits)
- [Author](#author)

---

## Gameplay

| Rule | Description |
|---|---|
| Board | 8x8 grid with five piece types: glove (red), punching bag (blue), water bottle (orange), headgear (green), hand wrap (purple). |
| Rounds | Every match has three rounds, each with its own move limit. Unused moves do not carry over. |
| Damage | Every cleared piece damages the opponent, however it was cleared. Special pieces add a bonus on top. |
| Knockout | The match ends the moment the opponent's health reaches zero, even in the middle of a cascade. |
| Rounds won | A round counts as won when the damage dealt in it reaches a third of the opponent's health. |
| Losing | If the opponent is still standing after the third round, the match is lost on points. |
| Board persistence | The board, including any special pieces on it, carries over between rounds. |
| Shuffle | When no valid move is left, the board shuffles automatically at no cost. |

A knockout before the final round awards a bonus for every unplayed round and for the moves left in the knockout round.

## Special Pieces

Special pieces are created from larger matches and fire when swapped with a neighbour or tapped. A special piece caught in another one's blast fires as well, so chains are possible.

| Piece | Created by | Effect |
|---|---|---|
| Medal | Horizontal match of 4 | Clears its row |
| Trophy | Vertical match of 4 | Clears its column |
| Belt | L or T shaped match | Clears the 3x3 area around it |
| Gong | Match of 5 or more | Clears every piece of the most common colour (red wins ties) |

## Corner Cards

After rounds one and two the player goes to the corner. The board is hidden, three face-down cards are dealt, and a 60 second timer starts, just like the one-minute rest between rounds. Tap a card to turn it over, tap again to choose it. If time runs out, a card is picked at random. A card only affects the round that follows.

| Card | Type | Effect |
|---|---|---|
| Heavy Punch | Attack | Damage +30%. Round is 2 moves shorter. |
| Combination | Attack | Special bonus doubled. Tile damage -25%. |
| Finisher | Attack | Damage +50% while the opponent is under 33% health, otherwise -20%. |
| Early Attack | Attack | Round starts with a medal on the board. Round is 1 move shorter. |
| Water | Tactic | Round is 3 moves longer. Damage -15%. |
| Coach's Advice | Tactic | Round starts with a belt on the board. Round is 1 move shorter. |
| Conditioning | Tactic | Damage +40% in the last 5 moves, -20% in the first half. |
| Read the Opponent | Tactic | Tiles of a randomly drawn colour deal double damage. Other colours -25%. |

## Levels and Opponents

Ten levels form a rankings ladder that ends in a title fight. Levels 5 and 8 are hard, level 10 is super hard.

| Levels | Opponent |
|---|---|
| 1, 2 | Rookie |
| 3, 4 | Veteran |
| 5, 6 | Mustache |
| 7, 8, 9 | Heavyweight |
| 10 (title fight) | Champion Tyson Spong |

The opponent's face reacts to the damage he takes: fresh above 66% health, tired below it, bruised below 33%, and dazed on the knockout.

## Planned Architecture

The rules of the game will live apart from Unity, so the same code can run in the game and in a console simulator.

```
           +-------------------------------+
           |      Assets/Scripts/Core      |   rules, board, cards, scoring
           |   plain C#, no UnityEngine    |   seeded System.Random
           +---------------+---------------+
                           |
        +------------------+------------------+
        |                  |                  |
+-------v--------+ +-------v--------+ +-------v--------+
|  Unity game    | |   Simulator    | |     Tests      |
| views, input,  | | bots, matches, | | NUnit tests    |
| animation,     | | difficulty     | | for every rule |
| audio          | | report         | |                |
+----------------+ +----------------+ +----------------+
```

**Shared rules.** Every rule (matching, special pieces and chains, gravity and refill, shuffling, damage, rounds, cards and scoring) will be written as plain C# classes in `Assets/Scripts/Core`. These classes will not inherit from MonoBehaviour or use `UnityEngine`. Unity compiles them with the rest of the game, and the console simulator compiles the very same files through a single line in its project file, so the rules exist in one place only. The core will grow out of my earlier [Match3-Console-Engine](https://github.com/durmazertugrul/Match3-Console-Engine) project.

**Event-driven presentation.** The core will resolve a move completely and return an ordered list of events: pieces swapped, special pieces activated (each blast in chain order, with the cells it cleared), cells cleared, special created (with the pieces that formed it), pieces fell, opponent damaged and so on. On the Unity side, an event player will turn that list into animations one by one and lock input until it finishes. The view will never decide a rule.

**Step-by-step damage.** Damage will be applied after every clear step of a cascade, not once per move. This lets the health bar drop with each step and allows a knockout to stop a cascade halfway.

**Card effects** will use the strategy pattern: each card will be its own class that produces an effect for the coming round, so adding a card will not touch the match rules. Multipliers are stored as whole percentages (130 for +30%) and calculated with `decimal`, so results are exact.

**Level data.** Moves per round and opponent health for all ten levels will live in one shared file, `LevelTable.cs`, read by both the game and the simulator. ScriptableObjects only hold the boxers' face sprites.

## Project Structure

The repository is the Unity project. The simulator, the tests and the report folder sit next to `Assets`, so Unity does not compile them.

```
Assets/
    Sprites/
        Audio/                  Sound effects
        Backgrounds/            Screen backgrounds
        Board/                  Board frame and cell
        Boxers/
            Champion/
            Heavyweight/
            Mustache/
            Rookie/
            Veteran/
        Cards/
            Backs/              Card backs, large and HUD size
            Frames/             Blank card front
            Icons/              Card illustrations and type icons
        Fonts/                  Titan One, Baloo 2 and their licenses
        FX/                     White, tintable effect sprites
        Tiles/                  Normal and special pieces
        UI/
            Buttons/
                Bases/          Blank button backgrounds
            Common/             Shared markers: tick, KO badge, lock
            Corner/             Corner screen scene, title, timer
            HUD/
            MainMenu/
            Rankings/
            Results/
    Scripts/
        Core/                   Game rules, plain C# (planned)
        Game/                   MonoBehaviours: views, input, audio (planned)
Simulator/                      Console app: bots and difficulty report (planned)
Tests/                          NUnit tests for the rules (planned)
Reports/                        Difficulty report written by the simulator (planned)
```

## Testing and Balancing Plan

Level design starts from research into how established match-3 games pace their difficulty. Those targets will then be checked with data: a console simulator will play each level many times with bots, using the same rule files as the game, so the difficulty it measures is the difficulty the player gets.

- **Unit tests:** NUnit tests will cover the rules step by step: match shapes and spawn cells, special pieces and chains, gravity, cascades, shuffling, damage per clear step, mid-cascade knockout, round results and every card effect.
- **Play bots:** a random bot and a greedy bot. The greedy bot tries each move on a copy of the match with its own random generator, so it cannot see which pieces will drop next, just like a real player.
- **Card choosers:** a heuristic chooser that picks from what is visible on screen, and a random chooser that shows how a player with poor card choices fares.
- **Difficulty report:** for each level the simulator raises the opponent's health in small steps until the win rate meets its target (about 83% normal, 63% hard, 40% super hard), then prints a table with win rate, expected attempts and the round in which knockouts happen. Every match uses a numbered seed, so the same command always prints the same report. The same table is written to `Reports/difficulty-report.md`, which GitHub renders as a table, so every balancing pass leaves a readable record in the commit history.
- **Checks:** a first-round knockout must stay below one in a thousand, so the corner cards are always seen, and second-round knockouts should stay rare.

The values from the report will be entered by hand into `LevelTable.cs`, and the final numbers will be checked against real playtests.

The latest results will be in [Reports/difficulty-report.md](Reports/difficulty-report.md).

## Credits

- **Fonts:** [Titan One](https://fonts.google.com/specimen/Titan+One) and [Baloo 2](https://fonts.google.com/specimen/Baloo+2), both under the SIL Open Font License 1.1. License files are in `Assets/Sprites/Fonts`.
- **Art and audio:** created for this project.

## Author

**Tuğrul Durmazer**, game developer (Unity and C#)

- GitHub: [durmazertugrul](https://github.com/durmazertugrul)
- itch.io: [tugruldurmazer.itch.io](https://tugruldurmazer.itch.io)
- LinkedIn: [tugruldurmazer](https://www.linkedin.com/in/tugruldurmazer/)
