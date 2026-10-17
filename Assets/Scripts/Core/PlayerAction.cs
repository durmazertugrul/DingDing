using System.Collections.Generic;

namespace DingDing.Core
{
    public enum ActionType { Swap, Tap}

    public class PlayerAction
    {
        public ActionType Type { get;  }
        public Cell FirstCell { get; } //where the drag started or the tapped cell
        public Cell SecondCell { get; } //where the dragged piece landed

        public PlayerAction(ActionType type, Cell firstCell, Cell secondCell) 
        { 
            Type = type;
            FirstCell = firstCell;
            SecondCell = secondCell;
        
        }

    }
}