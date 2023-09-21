#if UNITY_EDITOR
using Pancake.SceneFlow;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PancakeEditor
{
    internal static class EditorInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            string startScene = SceneManager.GetActiveScene().name;
            if (startScene.Equals(Constant.LAUNCHER_SCENE)) return;

            if (startScene.Equals(Constant.PERSISTENT_SCENE) || startScene.Equals(Constant.MENU_SCENE) || startScene.Equals(Constant.GAMEPLAY_SCENE))
                SceneManager.LoadScene(Constant.LAUNCHER_SCENE);
        }
    }
}

#endif