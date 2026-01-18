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