//  author: Artem Sumanev

using System.IO;

namespace PacMan_model.level {
    internal interface ILevelLoader {
        ILevel LoadFromSource(Stream source);
    }
}