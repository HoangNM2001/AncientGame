using Pancake.Apex;
using Pancake.Scriptable;
using Pancake.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pancake.SceneFlow
{
    public class Initazation : GameComponent
    {
        [SerializeField] private ScriptableEventString changeSceneEvent;
        [SerializeField] private BoolVariable loadingCompleted;
        [SerializeField] private BoolVariable remoteConfigFetchCompleted;

        [Space]
        [SerializeField] private bool isWaitLevelLoad;
        [SerializeField, ShowIf("isWaitLevelLoad")] private BoolVariable loadLevelCompleted;
        
        private bool _flagLoadedPersistent;

        private void Awake() { Execute(); }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            _flagLoadedPersistent = true;
        }

        private async void Execute()
        {
            await UniTask.WaitUntil(() => loadingCompleted.Value);
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(Constant.PERSISTENT_SCENE);
            await UniTask.WaitUntil(() => _flagLoadedPersistent);
            if (remoteConfigFetchCompleted != null) await UniTask.WaitUntil(() => remoteConfigFetchCompleted.Value);
            if (isWaitLevelLoad) await UniTask.WaitUntil(() => loadLevelCompleted.Value);
            
            // TODO : wait something else before load menu

            changeSceneEvent.Raise(Constant.MENU_SCENE);
        }
    }
}