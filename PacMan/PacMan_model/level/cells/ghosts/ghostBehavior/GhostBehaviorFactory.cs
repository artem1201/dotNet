//  author: Artem Sumanev

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using PacMan_model.level.field;

namespace PacMan_model.level.cells.ghosts.ghostBehavior {
    internal sealed class GhostBehaviorFactory : IGhostBehaviorFactory {
        private readonly string _pathToGhostsBehaviors;

        //  key is ghost name, pair is two ghost's behaviors - staking and frightening
        private readonly IDictionary<string, Tuple<Type, Type>> _ghostsBehaviors =
            new Dictionary<string, Tuple<Type, Type>>();

        //  contains names of ghosts ordered by difficulty which were loaded
        //  must be not empty
        private string[] _orderedLoadedGhostNames;

        #region Initialization

        public GhostBehaviorFactory(string pathToGhostsBehaviors) {
            _pathToGhostsBehaviors = pathToGhostsBehaviors;

            try {
                LoadBehaviorsFromFiles(Directory.GetFiles(pathToGhostsBehaviors, "*.dll"));
            }
            catch (DirectoryNotFoundException) {
                throw new InvalidBehaviorsDirectory(pathToGhostsBehaviors);
            }
        }

        #endregion

        #region Behavior by name

        public bool ContainsName(string name) {
            return _ghostsBehaviors.ContainsKey(name);
        }

        public string GetGhostNameByNumber(int ghostNumber) {
            if (ghostNumber < 0) {
                throw new ArgumentOutOfRangeException("ghostNumber");
            }

            return _orderedLoadedGhostNames[ghostNumber % _orderedLoadedGhostNames.Length];
        }

        public ICollection<string> GetAvaialbleNames() {
            return _ghostsBehaviors.Keys;
        }

        public GhostStalkerBehavior GetStalkerBehavior(string name, INotChanebleableField field, MovingCell target) {
            if (null == name) {
                throw new ArgumentNullException("name");
            }
            if (null == field) {
                throw new ArgumentNullException("field");
            }
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            if (!_ghostsBehaviors.ContainsKey(name)) {
                throw new UnknownGhostName(name);
            }
            return Activator.CreateInstance(_ghostsBehaviors[name].Item1, field, target) as GhostStalkerBehavior;
        }

        public GhostFrightedBehavior GetFrightedBehavior(string name, INotChanebleableField field, MovingCell target) {
            if (null == name) {
                throw new ArgumentNullException("name");
            }
            if (null == field) {
                throw new ArgumentNullException("field");
            }
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            if (!_ghostsBehaviors.ContainsKey(name)) {
                throw new UnknownGhostName(name);
            }

            return Activator.CreateInstance(_ghostsBehaviors[name].Item2, field, target) as GhostFrightedBehavior;
        }

        public GhostBehavior GetBehavior(
            string name,
            INotChanebleableField field,
            MovingCell target,
            bool isFrightModeEnabled = false) {
            if (null == name) {
                throw new ArgumentNullException("name");
            }
            if (null == field) {
                throw new ArgumentNullException("field");
            }
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            return isFrightModeEnabled
                ? (GhostBehavior) GetFrightedBehavior(name, field, target)
                : GetStalkerBehavior(name, field, target);
        }

        #endregion

        #region Behavior by number

        public GhostStalkerBehavior GetStalkerBehavior(int ghostNumber, INotChanebleableField field, MovingCell target) {
            if (null == field) {
                throw new ArgumentNullException("field");
            }
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            return GetStalkerBehavior(GetGhostNameByNumber(ghostNumber), field, target);
        }

        public GhostFrightedBehavior GetFrightedBehavior(
            int ghostNumber,
            INotChanebleableField field,
            MovingCell target) {
            if (null == field) {
                throw new ArgumentNullException("field");
            }
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            return GetFrightedBehavior(GetGhostNameByNumber(ghostNumber), field, target);
        }

        public GhostBehavior GetBehavior(
            int ghostNumber,
            INotChanebleableField field,
            MovingCell target,
            bool isFrightModeEnabled = false) {
            if (null == field) {
                throw new ArgumentNullException("field");
            }
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            return isFrightModeEnabled
                ? (GhostBehavior) GetFrightedBehavior(ghostNumber, field, target)
                : GetStalkerBehavior(ghostNumber, field, target);
        }

        #endregion

        #region Loading from file

        private void LoadBehaviorsFromFiles(string[] behaviorFiles) {
            if (0 == behaviorFiles.Length) {
                throw new InvalidBehaviorsDirectory(_pathToGhostsBehaviors);
            }

            foreach (var behaviorFile in behaviorFiles.Where(IsAssemblyVerified)) {
                try {
                    LoadBehaviorFromFile(behaviorFile);
                }
                catch (Exception e) {
                    if (GhostAlreadyExitstsErrorMessage.Equals(e.Message)) {}
                    else {
                        throw;
                    }
                }
            }

            //  find which ghosts were loaded
            _orderedLoadedGhostNames = (
                from name
                    in GhostsInfo.OrderedPossibleGhostNames
                where _ghostsBehaviors.ContainsKey(name)
                select name
                ).ToArray();
        }

        #region Verification

        [DllImport("mscoree.dll", CharSet = CharSet.Unicode)]
        private static extern bool StrongNameSignatureVerificationEx(
            [MarshalAs(UnmanagedType.LPWStr)] string wszFilePath,
            [MarshalAs(UnmanagedType.U1)] bool fForceVerification,
            [MarshalAs(UnmanagedType.U1)] out bool pfWasVerified);

        private static bool IsAssemblyVerified(string path) {
            bool result;
            return StrongNameSignatureVerificationEx(path, true, out result);
        }

        #endregion

        private const string GhostAlreadyExitstsErrorMessage = "ghost already exists";

        private void LoadBehaviorFromFile(string path) {
            if (null == path) {
                throw new ArgumentNullException("path");
            }

            var ghostAssembly = Assembly.LoadFile(path);
            var ghostName = ghostAssembly.GetName().Name;
            if (_ghostsBehaviors.ContainsKey(ghostName)) {
                throw new Exception(GhostAlreadyExitstsErrorMessage);
            }

            Type stalkerBehavior = null;
            Type frightedBehavior = null;

            //  load first of IGhostStaklerBehavior and GhostFrightedBehavior in assembly
            foreach (var exportedType in ghostAssembly.GetExportedTypes()) {
                if (exportedType.IsSubclassOf(typeof (GhostStalkerBehavior))) {
                    if (null != stalkerBehavior) {
                        continue;
                    }
                    stalkerBehavior = exportedType;

                    if (null != frightedBehavior) {
                        break;
                    }
                }
                else if (exportedType.IsSubclassOf(typeof (GhostFrightedBehavior))) {
                    if (null != frightedBehavior) {
                        continue;
                    }
                    frightedBehavior = exportedType;

                    if (null != stalkerBehavior) {
                        break;
                    }
                }
            }

            _ghostsBehaviors.Add(ghostName, new Tuple<Type, Type>(stalkerBehavior, frightedBehavior));
        }

        #endregion
    }
}