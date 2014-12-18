using System;

namespace PacMan_model.level {
    public interface IGame : IDisposable {
        void NewGame(int bestScore);
        void NewGame(int bestScore, string pathToLevels);
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

    public class CannotPlayGameException : Exception {
        protected string GameMessage;

        internal CannotPlayGameException() {}

        internal CannotPlayGameException(string message) {
            GameMessage = message;
        }

        public virtual string GetMessage() {
            return GameMessage;
        }
    }

    public sealed class InvalidLevelDirectory : Exception {
        public InvalidLevelDirectory(string directoryName) {
            if (null == directoryName) {
                throw new ArgumentNullException("directoryName");
            }
            DirectoryName = directoryName;
        }

        public InvalidLevelDirectory(string directoryName, Exception reason) : this(directoryName) {
            if (null == reason) {
                throw new ArgumentNullException("reason");
            }
            CausException = reason;
        }

        public string DirectoryName { get; private set; }

        public Exception CausException { get; private set; }
    }
}