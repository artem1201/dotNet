using System;

namespace PacMan_model.level {
    public interface IGame : IDisposable {

        

        void NewGame(int bestScore, string pathToLevels = null);
        bool LoadNextLevel();

        ILevelObserverable Level { get; }

        void Start();
        void Pause();

        bool IsOn();

        int GetGameScore();
        int GetLevelScore();
        int GetBestScore();

        void Win();
        void Loose();

        bool IsWon();
        bool IsFinished();

        event EventHandler LevelFinished;

        void RegisterOnDirectionObserver(IDirectionEventObserver directionEventObserver);
    }

    public class InvalidLevelDirectory : Exception {
        public InvalidLevelDirectory(string directoryName) {
            DirectoryName = directoryName;
        }

        public string DirectoryName { get; private set; }
    }
}
