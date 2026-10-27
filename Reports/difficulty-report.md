# Difficulty Report

Matches per level: 2000, base seed: 1000

Best play tries every legal move and keeps the one that takes the most health. Random play takes any legal move. Weak play takes the move that does the least damage. Random play is not a floor in a match-3 game, since any clear deals damage, so the weak column is the floor. The remaining columns come from the best play run.

Ordinary play matches the damage per move a human reaches, so the bands are checked against the random column. The weak and best columns show how wide the outcome can swing around it.

| Level | Tier | Health | Win% best | Win% random | Win% weak | Dmg/move best | Dmg/move random | Dmg/move weak | KO R1 | KO R2 | KO R3 | Rounds | Score |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| 1 | Normal | 120 | 100.0 | 100.0 | 99.5 | 11.92 | 7.03 | 4.15 | 81.1 | 18.9 | 0.0 | 1.19 | 2447 |
| 2 | Normal | 247 | 100.0 | 85.9 | 0.2 | 12.32 | 7.42 | 4.00 | 1.1 | 89.2 | 9.8 | 2.09 | 3435 |
| 3 | Normal | 231 | 100.0 | 83.7 | 0.1 | 12.36 | 7.43 | 4.00 | 1.0 | 86.2 | 12.9 | 2.12 | 3225 |
| 4 | Normal | 235 | 100.0 | 79.6 | 0.0 | 12.38 | 7.33 | 4.00 | 0.9 | 85.1 | 14.1 | 2.13 | 3255 |
| 5 | Hard | 237 | 100.0 | 62.1 | 0.0 | 12.47 | 7.40 | 3.99 | 0.3 | 73.0 | 26.7 | 2.26 | 3180 |
| 6 | Normal | 239 | 100.0 | 78.8 | 0.1 | 12.41 | 7.38 | 4.01 | 0.4 | 84.1 | 15.6 | 2.15 | 3286 |
| 7 | Normal | 221 | 100.0 | 75.7 | 0.1 | 12.47 | 7.37 | 4.01 | 0.3 | 83.2 | 16.6 | 2.16 | 3072 |
| 8 | Hard | 240 | 100.0 | 57.9 | 0.0 | 12.42 | 7.35 | 4.01 | 0.2 | 70.3 | 29.6 | 2.29 | 3201 |
| 9 | Normal | 224 | 100.0 | 74.1 | 0.1 | 12.46 | 7.42 | 4.01 | 0.6 | 79.6 | 19.9 | 2.19 | 3091 |
| 10 | SuperHard | 244 | 99.8 | 40.3 | 0.0 | 12.24 | 7.22 | 4.02 | 0.1 | 57.7 | 42.1 | 2.42 | 3231 |

## Checks

- Level 1 allows a round one knockout in 81.1% of best play, as intended.
- Level 2 is too easy: ordinary play wins 85.9%, above the band (70 to 85). Floor 0.2%, ceiling 100.0%.
- Level 3 sits in its band (70 to 85): ordinary play wins 83.7%. Floor 0.1%, ceiling 100.0%.
- Level 4 sits in its band (70 to 85): ordinary play wins 79.6%. Floor 0.0%, ceiling 100.0%.
- Level 5 sits in its band (50 to 65): ordinary play wins 62.1%. Floor 0.0%, ceiling 100.0%.
- Level 6 sits in its band (70 to 85): ordinary play wins 78.8%. Floor 0.1%, ceiling 100.0%.
- Level 7 sits in its band (70 to 85): ordinary play wins 75.7%. Floor 0.1%, ceiling 100.0%.
- Level 8 sits in its band (50 to 65): ordinary play wins 57.9%. Floor 0.0%, ceiling 100.0%.
- Level 9 sits in its band (70 to 85): ordinary play wins 74.1%. Floor 0.1%, ceiling 100.0%.
- Level 10 sits in its band (35 to 45): ordinary play wins 40.3%. Floor 0.0%, ceiling 99.8%.
