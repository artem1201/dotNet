using System;
using System.Collections.Generic;
using System.IO;
using PacMan_model.level.cells;
using PacMan_model.level.cells.ghosts;
using PacMan_model.level.cells.pacman;
using PacMan_model.level.field;
using PacMan_model.util;

namespace PacMan_model.level {
    internal sealed class LevelLoader : ILevelLoader {
        //  number goes in begin of source
        //  from which this level can be loaded
        private const string MagicNumber = "123456";

        private static readonly IDictionary<char, StaticCellType> CharStaticCellToStaticCellType;
        private const char CharPacMan = '\\';
        private const char CharGhost = '=';
        private const char CharFreeSpace = ' ';
        private const char CharWall = '#';
        private const char CharDot = '.';
        private const char CharSuperDot = '*';
        private const char CharFruit = '%';

        private static IGhostFactory _ghostFactory;

        #region Initialization

        static LevelLoader() {
            CharStaticCellToStaticCellType = new Dictionary<char, StaticCellType> {
                {CharFreeSpace, StaticCellType.FreeSpace},
                {CharWall, StaticCellType.Wall},
                {CharDot, StaticCellType.PacDot},
                {CharSuperDot, StaticCellType.Energizer},
                {CharFruit, StaticCellType.Fruit}
            };
        }

        public LevelLoader(IGhostFactory ghostFactory) {
            if (null == ghostFactory) {
                throw new ArgumentNullException("ghostFactory");
            }
            _ghostFactory = ghostFactory;
        }

        #endregion

        #region Loading

        public ILevel LoadFromSource(Stream source) {
            if (null == source) {
                throw new ArgumentNullException("source");
            }
            IPacMan pacman = null;
            var ghosts = new List<IGhost>();
            IField field = new Field();

            var width = 0;
            var height = 0;
            var fieldCells = new List<StaticCell>();


            using (var sourceReader = new StreamReader(source)) {
                //  check first line and source magic number
                if (false == MagicNumber.Equals(sourceReader.ReadLine())) {
                    throw new InvalidLevelSource(0, 0);
                }

                while (!sourceReader.EndOfStream) {
                    var levelLine = sourceReader.ReadLine();

                    if (null == levelLine) {
                        throw new InvalidLevelSource(height, 0);
                    }

                    //  check field's width
                    if (0 == width) {
                        width = levelLine.Length;
                    }
                    else if (levelLine.Length != width) {
                        throw new InvalidLevelSource(height + 1, 0);
                    }

                    //  try to find pacman
                    if (null == pacman) {
                        pacman = FindPacMan(ref levelLine, height, field);
                    }

                    //  try to find ghosts
                    FindGhosts(ref levelLine, ghosts, height, field);

                    for (var x = 0; x < levelLine.Length; ++x) {
                        //  check if unknown cell's char
                        if (false == CharStaticCellToStaticCellType.ContainsKey(levelLine[x])) {
                            throw new InvalidLevelSource(height + 1, 0);
                        }

                        //   add new cell
                        fieldCells.Add(
                            StaticCellFactory.CreateStaticCell(
                                CharStaticCellToStaticCellType[levelLine[x]],
                                new Point(x, height)));
                    }

                    //  increase field's height
                    ++height;
                }
            }

            //  if pacman has not been found - error
            if (null == pacman) {
                throw new InvalidLevelSource("pacman is not found");
            }

            //  initialize field
            field.Init(width, height, fieldCells);
            //  pacman was not set to ghosts as target - do it
            foreach (var ghost in ghosts) {
                ghost.SetTarget(pacman.AsMovingCell());
                ghost.MakeStalker();
            }

            return new Level(pacman, field, ghosts);
        }

        #endregion

        #region Searching of cell

        //  searchs for Pacman
        //  if it is found changes it on freeSpace
        private static IPacMan FindPacMan(ref string levelLine, int y, IField field) {
            if (null == levelLine) {
                throw new ArgumentNullException("levelLine");
            }
            if (null == field) {
                throw new ArgumentNullException("field");
            }

            IPacMan pacman = null;

            var x = levelLine.IndexOf(CharPacMan);

            if (-1 != x) {
                pacman = new PacMan(field, new Point(x, y));

                var levelLineChars = levelLine.ToCharArray();
                levelLineChars[x] = CharFreeSpace;
                levelLine = new string(levelLineChars);
            }

            return pacman;
        }


        //  searchs for Pacman
        //  if it is found changes it on freeSpace
        private static void FindGhosts(ref string levelLine, List<IGhost> ghosts, int y, IField field) {
            if (null == levelLine) {
                throw new ArgumentNullException("levelLine");
            }
            if (null == ghosts) {
                throw new ArgumentNullException("ghosts");
            }
            if (null == field) {
                throw new ArgumentNullException("field");
            }

            var levelLineChars = levelLine.ToCharArray();
            var ghostWasFound = false;

            for (var x = 0; x < levelLineChars.Length; ++x) {
                if (CharGhost == levelLineChars[x]) {
                    ghosts.Add(_ghostFactory.CreateGhost(ghosts.Count, new Point(x, y), field));
                    ghostWasFound = true;
                    levelLineChars[x] = CharFreeSpace;
                }
            }

            if (ghostWasFound) {
                levelLine = new string(levelLineChars);
            }
        }

        #endregion
    }


    public sealed class InvalidLevelSource : CannotPlayGameException {
        private readonly int _line = -1;
        private readonly int _column = -1;

        internal InvalidLevelSource(string message) {
            GameMessage = message;
        }

        internal InvalidLevelSource(int line, int colum) {
            _line = line;
            _column = colum;
        }

        public override string GetMessage() {
            return GameMessage ?? Where();
        }

        /// <summary>
        /// returns message whith information where error occurs
        /// </summary>
        /// <returns>"at line:column"</returns>
        public string Where() {
            if ((-1 == _line) || (-1 == _column)) {
                return "";
            }

            return "at " + _line + ":" + _column;
        }
    }
}