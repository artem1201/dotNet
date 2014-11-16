using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using PacMan_gui.Annotations;
using PacMan_model.level;

namespace PacMan_gui.ViewModel.level {

    internal static class ColorResolver {
        public static Brush PauseColor { get { return Brushes.Gray; } }
        public static Brush StalkingColor { get { return Brushes.DarkSlateBlue; } }
        public static Brush FrightedColor { get { return Brushes.Red; } }

        private static readonly IDictionary<LevelCondition, Brush> LevelConditionariesBrushes;

        static ColorResolver() {
                LevelConditionariesBrushes = new Dictionary<LevelCondition, Brush> {
                    {LevelCondition.Stalking, StalkingColor},
                    {LevelCondition.Fright, FrightedColor}
                };
        }

        public class LevelConditionToColorConverter : IValueConverter {

           public object Convert([NotNull] object value, Type targetType, object parameter, CultureInfo culture) {
                if (null == value) {
                    throw new ArgumentNullException("value");
                }
                var condition = value as LevelCondition?;

                if (null == condition) {
                    throw new ArgumentException("convert value is not LevelCondition");
                }

                return LevelConditionariesBrushes[(LevelCondition) condition];

            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {

                var color = value as Brush;

                if (null == color) {
                    throw new ArgumentException("convert value is not Brush");
                }

                foreach (var levelCondition in LevelConditionariesBrushes.Keys) {
                    if (LevelConditionariesBrushes[levelCondition].Equals(color)) {
                        return levelCondition;
                    }
                }

                return null;
            }
        }
    }

    
}
