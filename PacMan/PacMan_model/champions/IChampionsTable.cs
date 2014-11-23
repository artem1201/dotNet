using System;

namespace PacMan_model.champions {
    public interface IChampionsTable : IChampionsTableOberverable, IDisposable {

        void LoadFromFile();

        void SaveToFile();

        bool IsNewRecord(int result);

        void AddNewResult(string name, int result);
        void AddNewResult(int result, string name);

    }

    public class InvalidChampionsSource : Exception {

        private readonly string _message;

        public InvalidChampionsSource(string message) {
            _message = message;
        }

        public string GetMessage() {
            return _message;
        }

    }
}
