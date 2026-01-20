using UnityEngine;

namespace Player.Base.Utils {
    public static class DirectionUtils {
        public static Vector2Int NumpadToVector(int direction) => direction switch {
            1 => new(-1, -1),
            2 => new(0, -1),
            3 => new(1, -1),
            4 => new(-1, 0),
            5 => new(0, 0),
            6 => new(1, 0),
            7 => new(-1, 1),
            8 => new(0, 1),
            9 => new(1, 1),
            _ => Vector2Int.zero
        };

        public static Vector3 GetDebugGUIMenuPosition(int direction) => direction switch {
            1 => new (-50, -50),
            2 => new (0, -50),
            3 => new (50, -50),
            4 => new (-50, 0),
            5 => new (0, 0),
            6 => new (50, 0),
            7 => new (-50, 50),
            8 => new (0, 50),
            9 => new (50, 50),
            _ => new (0, 0)
        };
        
        public static Vector2 GetLinePosition(int direction) => direction switch {
            1 => new (4.16f, -0.59f),
            2 => new (4.95f, -0.59f),
            3 => new (5.69f, -0.59f),
            4 => new (4.16f, 0.17f),
            5 => new (4.95f, 0.17f),
            6 => new (5.69f, 0.17f),
            7 => new (4.16f, 0.95f),
            8 => new (4.95f, 0.95f),
            9 => new (5.69f, 0.95f),
            _ => new (0, 0)
        };

        private static readonly int[,] _inputToValue = {
            { 7, 8, 9 },
            { 4, 5, 6 },
            { 1, 2, 3 }
        };

        public static int GetDirection(Vector2 directionInput) {
            int x = Mathf.RoundToInt(directionInput.x);
            int y = Mathf.RoundToInt(directionInput.y);

            x = Mathf.Clamp(x, -1, 1);
            y = Mathf.Clamp(y, -1, 1);

            int column = x + 1;
            int row = 1 - y;

            return _inputToValue[row, column];
        }

        public static bool DirectionMatches(int required, int actual, int tolerance) {
            if (required == 10) return true;
            if (required == actual) return true;
            if (tolerance == 0) return false;

            Vector2Int r = NumpadToVector(required);
            Vector2Int a = NumpadToVector(actual);

            return Vector2Int.Distance(r, a) <= tolerance;
        }
    }
}