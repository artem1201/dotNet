using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PacMan_model.util;

namespace PacMan_model.champions {
    public sealed class ChampionsTable : IChampionsTableOberverable {
        private static readonly string RootDir = Directory.GetCurrentDirectory();
        private static readonly string PathToChampions = RootDir + "\\" /*+ "\\Champions"*/;
        private static readonly string ChampionsFileName = PathToChampions + "Champions.txt";

        private const int MaxRecords = 10;
        private readonly IList<ChampionsRecord> _championsRecords = new List<ChampionsRecord>(MaxRecords);

        public struct ChampionsRecord {
            private const string Separator = " ";

            private readonly string _name;
            private readonly int _score;

            public ChampionsRecord(string name, int score) {
                if (null == name) {
                    throw new ArgumentNullException("name");
                }
                if (0 >= score) {
                    throw new ArgumentOutOfRangeException("score");
                }
                _name = name;
                _score = score;
            }

            public ChampionsRecord(int score, string name)
                : this(name, score) {}

            public static ChampionsRecord FromString(string str) {
                var splitedString = str.Split(Separator.ToCharArray());

                if (2 != splitedString.Length) {
                    throw new ArgumentException("invalid string for record");
                }

                try {
                    var name = splitedString[0];
                    var score = int.Parse(splitedString[1]);

                    return new ChampionsRecord(name, score);
                }
                catch (ArgumentException) {
                    throw new InvalidChampionsSource("invalid line: " + PathToChampions);
                }
                catch (FormatException) {
                    throw new InvalidChampionsSource("invalid line: " + PathToChampions);
                }
            }

            public override string ToString() {
                return _name + Separator + _score;
            }

            public string GetName() {
                return _name;
            }

            public int GetScore() {
                return _score;
            }
        }

        private bool _isInit;

        public ChampionsTable() {
            LoadFromFile();
        }

        #region Work with file

        public void LoadFromFile() {
            if (!File.Exists(ChampionsFileName)) {
                return;
            }
            using (var input = File.OpenText(ChampionsFileName)) {
                while (!input.EndOfStream) {
                    var readLine = input.ReadLine();

                    if (null == readLine) {
                        throw new InvalidChampionsSource("unexpected end of file: " + PathToChampions);
                    }

                    AddNewResult(ChampionsRecord.FromString(readLine));
                }
            }

            _isInit = true;
        }

        public void SaveToFile() {
            using (var output = new StreamWriter(File.Create(ChampionsFileName))) {
                foreach (var championsRecord in _championsRecords) {
                    output.WriteLine(championsRecord.ToString());
                }

                output.Flush();
            }
        }

        #endregion

        #region Adding of new record

        public bool IsNewRecord(int result) {
            if (result <= 0) {
                return false;
            }

            return _championsRecords.Count < MaxRecords || _championsRecords.Any(record => record.GetScore() < result);
        }

        public void AddNewResult(ChampionsRecord record) {
            AddNewResult(record.GetName(), record.GetScore());
        }

        public void AddNewResult(string name, int result) {
            if (result <= 0) {
                throw new ArgumentNullException("result");
            }
            if (null == name) {
                throw new ArgumentNullException("name");
            }
            AddNewResult(result, name);
        }

        public void AddNewResult(int result, string name) {
            if (result <= 0) {
                throw new ArgumentNullException("result");
            }
            if (null == name) {
                throw new ArgumentNullException("name");
            }


            var newRecord = new ChampionsRecord(result, name);

            if (false == IsNewRecord(result)) {
                throw new ArgumentException("score is not enough");
            }

            var newIndex = -1;

            for (var index = 0; index < _championsRecords.Count; index++) {
                if (result > _championsRecords[index].GetScore()) {
                    newIndex = index;
                    break;
                }
            }

            if (_championsRecords.Count == MaxRecords) {
                _championsRecords.Remove(_championsRecords.Last());
            }

            if (-1 == newIndex) {
                //  add in the end
                _championsRecords.Add(newRecord);
            }
            else {
                //  insert in specific place
                _championsRecords.Insert(newIndex, newRecord);
            }

            NotifyChangedStatement();
        }

        #endregion

        #region Getters

        public int GetBestScore() {
            return _championsRecords.First().GetScore();
        }

        #endregion

        #region Events

        public event EventHandler<ChampionsTableChangedEventArs> ChampionsTableState;

        public void ForceNotify() {
            NotifyChangedStatement();
        }

        private void NotifyChangedStatement() {
            if (!_isInit) {
                return;
            }

            OnStatementChangedNotify(new ChampionsTableChangedEventArs(_championsRecords));
        }

        private void OnStatementChangedNotify(ChampionsTableChangedEventArs e) {
            if (null == e) {
                throw new ArgumentNullException("e");
            }

            e.Raise(this, ref ChampionsTableState);
        }

        #endregion

        #region Getters

        public ICollection<ChampionsRecord> GetResults() {
            return _championsRecords;
        }

        #endregion

        #region Disposing

        public void Dispose() {
            SaveToFile();
        }

        #endregion
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