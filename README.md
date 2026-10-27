<p align="center">
  <img src="Docs/logo.png" alt="Ding Ding!" width="420">
</p>

<h1 align="center">Ding Ding! Boxing Match-3</h1>

A boxing themed match-3 game built in Unity. The player fights ten opponents over three rounds each, clearing pieces to take the opponent's health down and picking a corner card between rounds.

Unity 6.4 (6000.4.5f1), built for WebGL. [Play it on itch.io](https://tugruldurmazer.itch.io/dingding).

## Design

A match is three rounds against one opponent with a single health pool.

- Clearing pieces deals damage. The match ends in a knockout when the pool reaches zero.
- A round counts as won if the damage dealt in it is at least one third of the starting pool.
- With no knockout, the match is decided on rounds won.
- Each round has its own move budget. Unused moves convert to score.
- Between rounds the player picks one of three corner cards, which modify damage, moves or scoring for the rounds that follow.

Matches of four or more pieces create special pieces: medal, trophy, belt and gong. Specials only form from the player's own match, never from a cascade, so creating one is always a deliberate act.

Ten levels span four tiers: an opening level, six normal levels, two hard levels and a title fight. Difficulty is set by opponent health and the three move budgets. The balancing method and the measured results are documented in [`Reports/balance.md`](Reports/balance.md).

## Repository layout

```
Assets/Scripts/Core      Game rules as plain C#, no Unity dependency
Assets/Scripts/Game      MonoBehaviours: board view, input, HUD, screens, audio
Simulator                Console application that plays the game with bots
Tests                    NUnit test suite for the Core rules
Reports                  Balance documentation and generated run output
Docs                     Logo and supporting assets
```

The `Core` namespace contains no reference to `UnityEngine`. The rules are a library that the Unity layer drives and that the simulator and the tests consume directly, which is what makes the game testable without the editor and simulable at thousands of matches per minute.

The Unity layer communicates with the rules through an event list. `MatchFlow.StartMatch`, `MatchFlow.PlayAction` and `MatchFlow.SelectCornerCard` are the only entry points; each returns the events that occurred, and the view layer replays them as animation.

## Tests

87 NUnit tests across 14 files cover the board and match shapes, swap legality and starting board generation, special piece creation and activation, gravity and cascade resolution, shuffling on a dead board, the level table, card effects and damage arithmetic, match flow, and bot behaviour.

Run them from the Unity Test Runner, or from the command line in the `Tests` directory:

```
dotnet test
```

## Simulator

The simulator plays every level with three bots and writes a markdown table of the results.

```
cd Simulator
dotnet run            # 2000 matches per level per bot
dotnet run 300        # fewer matches, for a quick tuning pass
```

The bots differ only in move choice: the weak bot takes the legal move that deals the least damage, the random bot takes any legal move, and the best bot takes the move that deals the most damage. Measured human play matches the random bot, so the other two bound the outcome around it. Each match uses a distinct seed derived from a base seed, so a run is reproducible, and a bot evaluating a move works on a copy of the match with a fresh random stream so it cannot see which pieces will fall.

Output is written to `Reports/difficulty-report.md` and regenerated on every run. The hand written analysis lives in [`Reports/balance.md`](Reports/balance.md) and is not overwritten.

## Building

Open the project in Unity 6.4, switch the platform to WebGL and build. The UI is authored for a 1080 by 1920 reference resolution with the Canvas Scaler match set to 0.

## Authorship

All game rules, systems, level design, difficulty targets and balance decisions are the work of Tuğrul Durmazer. AI assistance was used for writing code only.

## License

See `LICENSE`.