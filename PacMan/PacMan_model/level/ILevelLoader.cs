using System.IO;

namespace PacMan_model.level {
    interface ILevelLoader {

        ILevel LoadFromSource(Stream source);
    }
}
