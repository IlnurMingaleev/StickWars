using TonkoGames.StateMachine;
using TonkoGames.StateMachine.Enums;
using Tools.GameTools;
using UnityEngine.SceneManagement;
using AsyncOperation = UnityEngine.AsyncOperation;

namespace TonkoGames.MultiScene
{    
    
    public interface IMultiSceneManager
    {
        void LoadScene(ScenesStateEnum scene, MultiSceneManager.SceneLoaded sceneLoaded = null);
        void UnloadLastScene(MultiSceneManager.SceneLoaded sceneLoaded = null);
        void UnloadScene(ScenesStateEnum scene, MultiSceneManager.SceneLoaded sceneLoaded = null);
        void SetActiveScene(ScenesStateEnum scene);
        void SetActiveLastLoadScene();
    }

    public class MultiSceneManager : IMultiSceneManager
    {
        public delegate void SceneLoaded();
        private ScenesStateEnum _lastLoadedLevel = ScenesStateEnum.Base;
        private ScenesStateEnum _lasScene = ScenesStateEnum.Base;

        public void LoadScene(ScenesStateEnum scene, SceneLoaded sceneLoaded)
        {
            AsyncOperation load = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);

            load.completed += (AsyncOperation result) =>
            {
                Debugger.Log($"{scene} loaded!");
                _lasScene = _lastLoadedLevel;
                _lastLoadedLevel = scene;
                sceneLoaded?.Invoke();
            };
        }
        
        public void UnloadLastScene(SceneLoaded sceneLoaded)
        {
            AsyncOperation load = SceneManager.UnloadSceneAsync(_lasScene.ToString());

            load.completed += (AsyncOperation result) =>
            {
                Debugger.Log($"{_lasScene} scene Unload!");
                sceneLoaded?.Invoke();
            };
        }

        public void UnloadScene(ScenesStateEnum scene, SceneLoaded sceneLoaded)
        {
            AsyncOperation load = SceneManager.UnloadSceneAsync(scene.ToString());

            load.completed += (AsyncOperation result) =>
            {
                Debugger.Log($"{scene} scene Unload!");
                sceneLoaded?.Invoke();
            };
        }
        
        public void SetActiveScene(ScenesStateEnum scene) => SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene.ToString()));
        public void SetActiveLastLoadScene() => SceneManager.SetActiveScene(SceneManager.GetSceneByName(_lastLoadedLevel.ToString()));
        
        public void Dispose()
        {
            Debugger.Remove(this);
        }

        public void Initialize()
        {
            Debugger.Add(this);
        }
    }
}