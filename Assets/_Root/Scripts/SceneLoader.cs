using Pancake.Scriptable;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pancake.SceneFlow
{
    public class SceneLoader : GameComponent
    {
        [SerializeField] private ScriptableEventString changeSceneEvent;


        private void Start() { changeSceneEvent.OnRaised += OnChangeScene; }

        private void OnChangeScene(string sceneName)
        {
            foreach (var scene in GetAllLoadedScene())
            {
                if (!scene.name.Equals(Constant.PERSISTENT_SCENE)) SceneManager.UnloadSceneAsync(scene.name);
            }


            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            App.Delay(0.016f, () => SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName)));
        }

        private Scene[] GetAllLoadedScene()
        {
            int countLoaded = SceneManager.sceneCount;
            var loadedScenes = new Scene[countLoaded];

            for (var i = 0; i < countLoaded; i++)
            {
                loadedScenes[i] = SceneManager.GetSceneAt(i);
            }

            return loadedScenes;
        }
    }
}