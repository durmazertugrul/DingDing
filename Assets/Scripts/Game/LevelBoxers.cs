using UnityEngine;

// Which boxer stands in the corner on each level.
public class LevelBoxers : MonoBehaviour
{
    public BoxerDefinition[] boxers;         // ten entries, set in the Inspector

    public BoxerDefinition BoxerFor(int levelNumber) 
    { 
        return boxers[levelNumber - 1]; 
    }
        
}
