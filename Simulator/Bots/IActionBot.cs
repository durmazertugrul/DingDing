using DingDing.Core;

namespace DingDing.Simulator.Bots
{
    public interface IActionBot
    {
        PlayerAction ChooseAction(Match match);
    }
}