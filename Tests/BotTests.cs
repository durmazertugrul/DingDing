using System;
using System.Collections.Generic;
using DingDing.Core;
using DingDing.Core.Cards;
using DingDing.Simulator.Bots;
using NUnit.Framework;

namespace DingDing.Tests
{
    public class BotTests
    {
        private static Match RunMatch(IActionBot bot, ICardChooser chooser, int seed)
        {
            Match match = MatchFlow.StartMatch(LevelTable.GetLevel(1), seed).Match;

            for (int guard = 0; guard < 500; guard++)
            {
                if (match.Phase == MatchPhase.Finished) break;

                if (match.Phase == MatchPhase.Corner)
                {
                    List<CardOffer> offers = CardRules.OfferCards(match);
                    MatchFlow.SelectCornerCard(match, chooser.Choose(match, offers));
                    continue;
                }

                PlayerAction action = bot.ChooseAction(match);
                if (action == null) break;
                if (MatchFlow.PlayAction(match, action) == null) break;
            }

            return match;
        }

        private static string SignatureOf(Board board)
        {
            string signature = "";

            for (int row = 0; row < Board.Rows; row++)
            {
                for (int column = 0; column < Board.Columns; column++)
                {
                    Piece piece = board[row, column];
                    signature = signature + (piece == null ? "-" : (int)piece.Color + "" + (int)piece.Kind);
                }
            }

            return signature;
        }

        [Test]
        public void BotsFinishMatch()
        {
            IActionBot[] bots = new IActionBot[] { new RandomBot(new Random(1)), new GreedyBot(new Random(1)) };
            ICardChooser[] choosers = new ICardChooser[] { new RandomChooser(new Random(2)), new HeuristicChooser() };

            foreach (var bot in bots)
            {
                foreach (var chooser in choosers)
                {
                    Match match = RunMatch(bot, chooser, 42);

                    Assert.That(match.Phase, Is.EqualTo(MatchPhase.Finished));
                }
            }
        }

        [Test]
        public void CopyDoesNotTouchMatch()
        {
            // The bot plays whole moves on copies; the real match must come
            // out of that untouched.
            Match match = MatchFlow.StartMatch(LevelTable.GetLevel(5), 7).Match;

            string boardBefore = SignatureOf(match.Board);
            int healthBefore = match.OpponentHealth;
            int scoreBefore = match.Score;
            int movesBefore = match.MovesLeftInRound;

            GreedyBot bot = new GreedyBot(new Random(3));
            bot.ChooseAction(match);

            Assert.That(SignatureOf(match.Board), Is.EqualTo(boardBefore));
            Assert.That(match.OpponentHealth, Is.EqualTo(healthBefore));
            Assert.That(match.Score, Is.EqualTo(scoreBefore));
            Assert.That(match.MovesLeftInRound, Is.EqualTo(movesBefore));
        }

        [Test]
        public void SameSeedsSameResult()
        {
            Match firstMatch = RunMatch(new GreedyBot(new Random(9)), new HeuristicChooser(), 99);
            Match secondMatch = RunMatch(new GreedyBot(new Random(9)), new HeuristicChooser(), 99);

            Assert.That(secondMatch.Score, Is.EqualTo(firstMatch.Score));
            Assert.That(secondMatch.Outcome, Is.EqualTo(firstMatch.Outcome));
            Assert.That(secondMatch.KnockoutMove, Is.EqualTo(firstMatch.KnockoutMove));
        }

        [Test]
        public void HeuristicPicksFinisher()
        {
            LevelInfo level = new LevelInfo(1, Difficulty.Normal, 100, 12, 12, 12);
            Match match = new Match(level, 42);
            match.CurrentRound = 1;
            match.OpponentHealth = 30;

            List<CardOffer> offers = new List<CardOffer>();
            offers.Add(new CardOffer(new CardInfo(CardId.Water, CardClass.Tactic, false), PieceColor.None));
            offers.Add(new CardOffer(new CardInfo(CardId.Finisher, CardClass.Attack, false), PieceColor.None));
            offers.Add(new CardOffer(new CardInfo(CardId.HeavyPunch, CardClass.Attack, false), PieceColor.None));

            CardOffer picked = new HeuristicChooser().Choose(match, offers);

            Assert.That(picked.Card.Id, Is.EqualTo(CardId.Finisher));
        }
    }
}