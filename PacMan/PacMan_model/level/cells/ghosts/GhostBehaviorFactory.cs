using System;
using System.Collections.Generic;
using PacMan_model.level.cells.ghosts.ghostBehavior;

namespace PacMan_model.level.cells.ghosts {
    class GhostBehaviorFactory : IGhostBehaviorFactory {


        private const int BlinkyStalkingSpeed = 130;
        private const int PinkyStalkingSpeed = 132;
        private const int InkyStalkingSpeed = 134;
        private const int ClydeStalkingSpeed = 136;

        private const int BlinkyFrightedSpeed = 195;
        private const int PinkyFrightedSpeed = 198;
        private const int InkyFrightedSpeed = 201;
        private const int ClydeFrightedSpeed = 204;

        private static readonly IDictionary<string, int> GhostsStalkingSpeeds;
        private static readonly IDictionary<string, int> GhostsFrightedSpeeds;

        private static readonly IDictionary<string, IGhostBehavior> GhostsStalkingBehaviors;
        private static readonly IDictionary<string, IGhostBehavior> GhostsFrightedBehaviors;

        static GhostBehaviorFactory() {

            //TODO: change all initialization with loading

            GhostsStalkingSpeeds = new Dictionary<string, int> {
                {"Blinky", BlinkyStalkingSpeed},
                {"Pinky", PinkyStalkingSpeed},
                {"Inky", InkyStalkingSpeed},
                {"Clyde", ClydeStalkingSpeed}
            };

            GhostsFrightedSpeeds = new Dictionary<string, int> {
                {"Blinky", BlinkyFrightedSpeed},
                {"Pinky", PinkyFrightedSpeed},
                {"Inky", InkyFrightedSpeed},
                {"Clyde", ClydeFrightedSpeed}
            };

            //TODO: initialize ghosts behaviors
            GhostsStalkingBehaviors = new Dictionary<string, IGhostBehavior>();

            GhostsFrightedBehaviors = new Dictionary<string, IGhostBehavior>();
        }


        public IGhostBehavior GetStalkerBehavior(string name) {
            if (null == name) {
                throw new ArgumentNullException("name");
            }
            return GhostsStalkingBehaviors[name];
        }

        public IGhostBehavior GetFrightedBehavior(string name) {
            if (null == name) {
                throw new ArgumentNullException("name");
            }
            return GhostsFrightedBehaviors[name];
        }

        public IGhostBehavior GetBehavior(string name, bool isFrightModeEnabled = false) {
            if (null == name) {
                throw new ArgumentNullException("name");
            }
            return isFrightModeEnabled ? GetFrightedBehavior(name) : GetStalkerBehavior(name);
        }

        public int GetStalkerSpeed(string name) {
            if (null == name) {
                throw new ArgumentNullException("name");
            }
            return GhostsStalkingSpeeds[name];
        }

        public int GetFrightedSpeed(string name) {
            if (null == name) {
                throw new ArgumentNullException("name");
            }
            return GhostsFrightedSpeeds[name];
        }

        public int GetSpeed(string name, bool isFrightModeEnabled = false) {
            if (null == name) {
                throw new ArgumentNullException("name");
            }
            return isFrightModeEnabled ? GetFrightedSpeed(name) : GetStalkerSpeed(name);
        }
    }
}
