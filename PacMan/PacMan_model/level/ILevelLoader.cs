//  author: Artem Sumanev

using System.IO;

namespace PacMan_model.level {
    internal interface ILevelLoader {
        Level LoadFromSource(Stream source);
    }
}