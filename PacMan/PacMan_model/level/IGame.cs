using System;

namespace PacMan_model.level {
    public interface IGame {

        

        void NewGame(int bestScore, string pathToLevels = null);

        ILevelObserverable Level { get; }

        void Start();
        void Pause();
        void Stop();

        bool IsOn();

        int GetGameScore();
        int GetLevelScore();
        int GetBestScore();

        void Win();
        void Loose();

        bool IsWon();
        bool IsLost();

        event EventHandler<LevelFinishedEventArs> LevelFinished;

        void RegisterOnDirectionObserver(IDirectionEventObserver directionEventObserver);

    }

    public class LevelFinishedEventArs : EventArgs {
        
        public LevelFinishedEventArs(bool hasNextLevel) {
            HasNextLevel = hasNextLevel;
        }

        public bool HasNextLevel { get; private set; }
    }

    public class InvalidLevelDirectory : Exception {
        public InvalidLevelDirectory(string directoryName) {
            DirectoryName = directoryName;
        }

        public string DirectoryName { get; private set; }
    }
}
