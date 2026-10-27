# Balance Report

Ding Ding! Boxing Match-3

This document records how the ten levels were balanced: the design targets, the measurement method, the model used to turn a target into a health value, and the results that confirm the final table. The generated run output lives beside this file in `difficulty-report.md` and is overwritten on every simulator run; this document is written by hand and is not. It is published in English only.

## 1. Match rules that drive difficulty

A match is three rounds against one opponent with a single health pool.

- The player wins by knockout if the pool reaches zero in any round.
- A round is scored as won if the damage dealt in that round is at least one third of the starting pool.
- If no knockout occurs, the match is decided on rounds won.
- Each round has its own move budget. Unused moves are worth score, not carried damage.
- Between rounds the player picks one corner card. Cards modify damage, moves or scoring for the rounds that follow.

Two values therefore set a level's difficulty: the opponent's starting health and the three move budgets. Everything else is shared across levels.

## 2. Design targets

The tiers and their intent:

| Tier | Levels | Intent |
|---|---|---|
| Opening | 1 | A strong opening round must be able to end the match. Losing should be close to impossible. |
| Normal | 2, 3, 4, 6, 7, 9 | Difficulty rises steadily. A normal level must never feel like a hard one. |
| Hard | 5, 8 | A knockout in round two must stay reachable for a strong performance. Winning on the last moves of round three is the exception, not the rule. |
| SuperHard | 10 | Round three is required. Losing is the ordinary outcome. |

Target win rates for an ordinary player:

| Tier | Band |
|---|---|
| Normal | 70 to 85 percent |
| Hard | 50 to 65 percent |
| SuperHard | 35 to 45 percent |

Level 1 is not judged by a win band. Its only requirement is that a round one knockout is reachable in roughly four matches out of five under strong play.

A knockout in round one on any level above the first means the corner cards never entered the match. The threshold for that check is 5 percent of best-play matches.

## 3. Measurement method

Three bots play each level. They differ only in how they choose a move.

| Bot | Move choice | Damage per move |
|---|---|---|
| Weak | The legal move that deals the least damage | 4.0 |
| Random | Any legal move | 7.3 |
| Best | The legal move that deals the most damage | 12.4 |

Random play is not a floor in a match-3 game. Any clear deals damage and cascades arrive on their own, so a bot playing at random still reaches 7.3 damage per move. The weak bot exists because a real floor has to choose badly on purpose.

Measured human play sits at 7.0 to 7.3 damage per move, which matches the random bot. An earlier estimate of 8.6 was drawn only from won matches; since a won match is a high damage match by definition, that sample was biased upward and produced a table about 20 percent too hard. The bands are therefore checked against the random column, with the weak and best columns showing how far the outcome can swing around it.

Each bot pairs with a card chooser that follows a fixed card priority. Every match uses a distinct seed derived from `baseSeed + matchIndex`, so a run is reproducible. When a bot evaluates a move it works on a copy of the match whose random stream is fresh, so it cannot see which pieces will fall.

The final run is 2000 matches per level per bot, base seed 1000.

## 4. The difficulty model

Difficulty reduces to a single ratio:

```
R = health / (damage per move * total moves)
```

R below 1 means the player deals more than the pool over a full match. R above 1 means the pool outlasts the player.

The relation between R and win rate was measured, not assumed:

| R | Win rate |
|---|---|
| 0.70 | 97 |
| 0.81 | 87 |
| 0.86 | 79 |
| 0.91 | 62 |
| 0.95 | 46 |
| 1.05 | 26 |
| 1.24 | 7 |

Inverting this curve turns any target win rate into an R, and `health = R * 7.3 * total moves` turns that R into a health value.

The curve is not identical for every move budget. Shorter levels carry more variance per match, which flattens the curve. The measured slope, in win rate points lost per point of health added:

| Total moves | Slope |
|---|---|
| 42 | 0.80 |
| 39 | 0.90 |
| 36 | 1.00 |
| 33 | 0.95 |

These slopes were used for the final correction pass instead of the generic curve.

## 5. Final level table

| Level | Tier | Health | Moves | Total moves | Target | Measured |
|---|---|---|---|---|---|---|
| 1 | Normal | 120 | 12 / 14 / 16 | 42 | round one knockout reachable | 81.1 percent of best play |
| 2 | Normal | 247 | 12 / 14 / 16 | 42 | 85 | 85.9 |
| 3 | Normal | 231 | 11 / 13 / 15 | 39 | 83 | 83.7 |
| 4 | Normal | 235 | 11 / 13 / 15 | 39 | 80 | 79.6 |
| 5 | Hard | 237 | 10 / 12 / 14 | 36 | 60 | 62.1 |
| 6 | Normal | 239 | 11 / 13 / 15 | 39 | 77 | 78.8 |
| 7 | Normal | 221 | 10 / 12 / 14 | 36 | 74 | 75.7 |
| 8 | Hard | 240 | 10 / 12 / 14 | 36 | 55 | 57.9 |
| 9 | Normal | 224 | 10 / 12 / 14 | 36 | 71 | 74.1 |
| 10 | SuperHard | 244 | 9 / 11 / 13 | 33 | 40 | 40.3 |

The largest deviation from target is 3 points.

Health values do not rise monotonically, and they are not meant to. A level with 36 moves is harder than a level with 39 moves at the same health. What rises monotonically is the ratio R, which is what the player experiences.

Move budgets rise within a match so that the final round is the longest. The cost of this choice is that damage concentrates in round three; the benefit is that the closing round is the decisive one, which is what the theme calls for.

## 6. Verification

Win rates for ordinary play, 2000 matches per level:

| Level | Win rate | Band | Result |
|---|---|---|---|
| 1 | 100.0 | not banded | round one knockout in 81.1 percent of best play |
| 2 | 85.9 | 70 to 85 | 0.9 above band |
| 3 | 83.7 | 70 to 85 | in band |
| 4 | 79.6 | 70 to 85 | in band |
| 5 | 62.1 | 50 to 65 | in band |
| 6 | 78.8 | 70 to 85 | in band |
| 7 | 75.7 | 70 to 85 | in band |
| 8 | 57.9 | 50 to 65 | in band |
| 9 | 74.1 | 70 to 85 | in band |
| 10 | 40.3 | 35 to 45 | in band |

Knockout distribution under best play:

| Level | Round 1 | Round 2 | Round 3 | Average rounds |
|---|---|---|---|---|
| 1 | 81.1 | 18.9 | 0.0 | 1.19 |
| 2 | 1.1 | 89.2 | 9.8 | 2.09 |
| 3 | 1.0 | 86.2 | 12.9 | 2.12 |
| 4 | 0.9 | 85.1 | 14.1 | 2.13 |
| 5 | 0.3 | 73.0 | 26.7 | 2.26 |
| 6 | 0.4 | 84.1 | 15.6 | 2.15 |
| 7 | 0.3 | 83.2 | 16.6 | 2.16 |
| 8 | 0.2 | 70.3 | 29.6 | 2.29 |
| 9 | 0.6 | 79.6 | 19.9 | 2.19 |
| 10 | 0.1 | 57.7 | 42.1 | 2.42 |

Three design rules are confirmed by this table. The round one knockout is the opening level's own property and drops away immediately after it. The hard levels keep a round two knockout open in roughly seven matches out of ten under strong play, so a strong performance still has score upside. The final level pushes the average match to 2.42 rounds, the longest in the game.

## 7. Manual verification

Every level was played by hand on the final table. No level proved unwinnable, and the levels that were intended to be lost on a first attempt were lost on a first attempt. Earlier tables were rejected on exactly this basis: an earlier build won level 10 three times out of three while level 8 ran out of moves, which is the inversion the current table removes.

## 8. Known limitations

Individual match variance remains wide. Measured human play swings between 5.3 and 10.8 damage per move depending on board luck and card draw, a spread larger than the gap between two adjacent levels. A single match therefore does not reliably place the player on the difficulty curve; a handful of matches does. Raising the move budgets would damp this at the cost of longer matches, and was left out of scope.

The bots are not human. They are a bracket, not a simulation of play. The weak and best columns bound the outcome; the random column is the stand-in for an ordinary player, and it holds only because its damage per move was measured against real play rather than assumed.

## 9. Authorship

All game rules, systems, level structure, difficulty targets, tier definitions and balance decisions in this project are the work of Tuğrul Durmazer. AI assistance was used for writing code only.