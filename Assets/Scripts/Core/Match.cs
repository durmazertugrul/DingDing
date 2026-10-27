using DingDing.Core.Cards;
using System;
using System.Collections.Generic;

namespace DingDing.Core
{
    public class Match
    {
        public LevelInfo Level { get; }
        public Board Board { get; set; }

        // Indexed by round number; [0] stays empty. A round with no card
        // keeps a null here.
        public CardEffect[] RoundEffects { get; }

        public List<CardInfo> ChosenCards { get; }     // not offered again
        public List<CardOffer> CurrentOffers { get; set; }
        public int CurrentRound { get; set; }
        public int RoundMoveCount { get; set; }      // this round's total moves
        public int MovesLeftInRound { get; set; }
        public Random Random { get; private set; }

        public int OpponentHealth { get; set; }      // one pool for the whole match
        public int Score { get; set; }

        // [1], [2] and [3] are used, like MovesPerRound.
        public RoundResult[] RoundResults { get; }
        public MatchPhase Phase { get; set; }
        public MatchOutcome Outcome { get; set; }

        public int DamageThisRound { get; set; }

        // Indexed by round number; [0] stays empty.
        public int[] RoundDamage { get; }

        public int KnockoutMove { get; set; }    // 0 when there was no knockout
        public Match(LevelInfo level, int seed)
        {
            Level = level;
            Random = new Random(seed);
            OpponentHealth = level.StartingHealth;
            CurrentRound = 0;                         // no round opened yet
            RoundResults = new RoundResult[GameRules.RoundCount + 1];
            RoundEffects = new CardEffect[GameRules.RoundCount + 1];
            ChosenCards = new List<CardInfo>();
            CurrentOffers = new List<CardOffer>();
            Phase = MatchPhase.Playing;
            Outcome = MatchOutcome.Undecided;
            RoundDamage = new int[GameRules.RoundCount + 1];
        }
        // A throwaway copy for a bot to try a move on. It gets a random
        // stream of its own, so the bot cannot see which pieces will fall
        // next; a human cannot see them either.
        public Match CopyForBot(Random botRandom)
        {
            Match copy = new Match(Level, 0);

            copy.Random = botRandom;
            copy.Board = Board.Copy();

            copy.CurrentRound = CurrentRound;
            copy.RoundMoveCount = RoundMoveCount;
            copy.MovesLeftInRound = MovesLeftInRound;
            copy.OpponentHealth = OpponentHealth;
            copy.Score = Score;
            copy.DamageThisRound = DamageThisRound;
            copy.KnockoutMove = KnockoutMove;
            copy.Phase = Phase;
            copy.Outcome = Outcome;

            for (int round = 0; round <= GameRules.RoundCount; round++)
            {
                copy.RoundResults[round] = RoundResults[round];
                copy.RoundDamage[round] = RoundDamage[round];
                copy.RoundEffects[round] = RoundEffects[round];
            }

            copy.ChosenCards.AddRange(ChosenCards);

            return copy;
        }
    }
}