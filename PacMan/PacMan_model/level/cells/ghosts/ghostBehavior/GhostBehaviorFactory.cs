using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PacMan_model.level.cells.ghosts.ghostBehavior {
    class GhostBehaviorFactory : IGhostBehaviorFactory {

        private readonly string _pathToGhostsBehaviors;

        //  key is ghost name, pair is two ghost's behaviors - staking and frightening
        private readonly IDictionary<string, Tuple<Type, Type>> _ghostsBehaviors = new Dictionary<string, Tuple<Type, Type>>();

        //  contains names of ghosts ordered by difficulty which were loaded
        //  must be not empty
        private string[] _orderedLoadedGhostNames;

        public GhostBehaviorFactory(string pathToGhostsBehaviors) {

            _pathToGhostsBehaviors = pathToGhostsBehaviors;

            try {
                LoadBehaviorsFromFiles(Directory.GetFiles(pathToGhostsBehaviors, "*.dll"));
            }
            catch (DirectoryNotFoundException) {
                throw new InvalidBehaviorsDirectory(pathToGhostsBehaviors);
            }

            
        }


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

        public GhostBehavior GetBehavior(string name, INotChanebleableField field, MovingCell target, bool isFrightModeEnabled = false) {
            if (null == name) {
                throw new ArgumentNullException("name");
            }
            if (null == field) {
                throw new ArgumentNullException("field");
            }
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            if (isFrightModeEnabled) {
                return GetFrightedBehavior(name, field, target);
            }
            else {
                return GetStalkerBehavior(name, field, target);
            }
        }

        public GhostStalkerBehavior GetStalkerBehavior(int ghostNumber, INotChanebleableField field, MovingCell target) {

            if (null == field) {
                throw new ArgumentNullException("field");
            }
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            return GetStalkerBehavior(GetGhostNameByNumber(ghostNumber), field, target);
        }

        public GhostFrightedBehavior GetFrightedBehavior(int ghostNumber, INotChanebleableField field, MovingCell target) {

            if (null == field) {
                throw new ArgumentNullException("field");
            }
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            return GetFrightedBehavior(GetGhostNameByNumber(ghostNumber), field, target);
        }

        public GhostBehavior GetBehavior(int ghostNumber, INotChanebleableField field, MovingCell target, bool isFrightModeEnabled = false) {

            if (null == field) {
                throw new ArgumentNullException("field");
            }
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            if (isFrightModeEnabled) {
                return GetFrightedBehavior(ghostNumber, field, target);
            }
            else {
                return GetStalkerBehavior(ghostNumber, field, target);
            }
        }

        private void LoadBehaviorsFromFiles(string[] behaviorFiles) {

            if (0 == behaviorFiles.Length) {
                throw new InvalidBehaviorsDirectory(_pathToGhostsBehaviors);
            }

            foreach (var behaviorFile in behaviorFiles) {
                LoadBehaviorFromFile(behaviorFile);
            }

            //  find which ghosts were loaded
            _orderedLoadedGhostNames = (
                from name 
                in GhostsInfo.OrderedPossibleGhostNames 
                where _ghostsBehaviors.ContainsKey(name) 
                select  name
            ).ToArray();
        }

        private void LoadBehaviorFromFile(string path) {
            if (null == path) {
                throw new ArgumentNullException("path");
            }

            var dll = Assembly.LoadFile(path);
            //TODO: verify assembly

            var ghostName = dll.GetName().Name;
            Type stalkerBehavior = null;
            Type frightedBehavior = null;

            //  load first of IGhostStaklerBehavior and GhostFrightedBehavior in assembly
            foreach (var exportedType in dll.GetExportedTypes()) {
                
                if (exportedType.IsSubclassOf(typeof (GhostStalkerBehavior))) {
                    if (null != stalkerBehavior) {
                        continue;
                    }
                    stalkerBehavior = exportedType;

                    if (null != frightedBehavior) {
                        break;
                    }
                }
                else if (exportedType.IsSubclassOf(typeof(GhostFrightedBehavior))) {
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
    }
}
