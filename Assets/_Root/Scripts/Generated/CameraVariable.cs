using UnityEngine;

namespace Pancake.Scriptable
{
    [EditorIcon("scriptable_variable")]
    [CreateAssetMenu(fileName = "scriptable_variable_Camera.asset", menuName = "Pancake/Scriptable/Variables/Camera")]
    [System.Serializable]
    public class CameraVariable : ScriptableVariable<Camera>
    {
        
    }
}
