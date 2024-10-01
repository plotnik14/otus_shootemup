using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace ShootEmUp
{
    public class GameManager : MonoBehaviour
    {
        private EGameState _state = EGameState.NotStarted;

        private readonly List<IGameStartListener> _startListeners = new();
        private readonly List<IGameFinishListener> _finishListeners = new();
        private readonly List<IGamePauseListener> _pauseListeners = new();
        private readonly List<IGameResumeListener> _resumeListeners = new();
        private readonly List<IGameUpdateListener> _updateListeners = new();
        private readonly List<IGameFixedUpdateListener> _fixedUpdateListeners = new();

        public bool IsNotStarted => _state == EGameState.NotStarted;
        public bool IsInProgress => _state == EGameState.InProgress;
        public bool IsPaused => _state == EGameState.Paused;
        public bool IsFinished => _state == EGameState.Finished;

        [UsedImplicitly]
        private void Update()
        {
            if (!IsInProgress)
                return;

            float deltaTime = Time.deltaTime;

            for (var index = 0; index < _updateListeners.Count; index++)
            {
                _updateListeners[index].OnGameUpdate(deltaTime);
            }
        }

        [UsedImplicitly]
        private void FixedUpdate()
        {
            if (!IsInProgress)
                return;

            float deltaTime = Time.fixedDeltaTime;

            for (var index = 0; index < _fixedUpdateListeners.Count; index++)
            {
                _fixedUpdateListeners[index].OnGameFixedUpdate(deltaTime);
            }
        }

        public void RegisterListener(IGameLifecycleListener listener)
        {
            if (listener is IGameStartListener startListener)
                _startListeners.Add(startListener);

            if (listener is IGameFinishListener finishListener)
                _finishListeners.Add(finishListener);

            if (listener is IGamePauseListener pauseListener)
                _pauseListeners.Add(pauseListener);

            if (listener is IGameResumeListener resumeListener)
                _resumeListeners.Add(resumeListener);

            if (listener is IGameUpdateListener updateListener)
                _updateListeners.Add(updateListener);

            if (listener is IGameFixedUpdateListener fixedUpdateListener)
                _fixedUpdateListeners.Add(fixedUpdateListener);
        }

        [ContextMenu("Start Game")]
        public void StartGame()
        {
            _state = EGameState.InProgress;

            foreach (IGameStartListener listener in _startListeners)
            {
                listener.OnGameStart();
            }
        }

        [ContextMenu("Finish Game")]
        public void FinishGame()
        {
            _state = EGameState.Finished;

            foreach (IGameFinishListener listener in _finishListeners)
            {
                listener.OnGameFinish();
            }

            Debug.Log("Game over!");
            Time.timeScale = 0;
        }

        [ContextMenu("Pause Game")]
        public void PauseGame()
        {
            _state = EGameState.Paused;

            foreach (IGamePauseListener listener in _pauseListeners)
            {
                listener.OnGamePause();
            }
        }

        [ContextMenu("Resume Game")]
        public void ResumeGame()
        {
            _state = EGameState.InProgress;

            foreach (IGameResumeListener listener in _resumeListeners)
            {
                listener.OnGameResume();
            }
        }
    }
}