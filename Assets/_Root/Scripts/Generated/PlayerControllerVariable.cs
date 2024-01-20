using UnityEngine;

namespace Pancake.Scriptable
{
    [EditorIcon("scriptable_variable")]
    [CreateAssetMenu(fileName = "scriptable_variable_PlayerController.asset", menuName = "Pancake/Scriptable/Variables/PlayerController")]
    [System.Serializable]
    public class PlayerControllerVariable : ScriptableVariable<PlayerController>
    {
        
    }
}
