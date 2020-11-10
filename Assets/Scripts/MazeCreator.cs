using System.Collections.Generic;

public static class MazeCreator {

    private static Direction[] directionOrder = new Direction[] {Direction.North, Direction.East, Direction.South, Direction.West};
    private static int[] offsetOrder;
    private static int limit;
    private static int row;
    private static int col;
    private static System.Random rng = new System.Random();

    // public api which initializes maze configuration and calls maze creating function
    public static List<Direction> FindPath(int rowSize, int colSize, Direction orientation) {
        row = rowSize;
        col = colSize;
        limit = row * col;
        offsetOrder = new int[] {1, row, -1, -row};

        List<int> path = CreateMaze();
        int total = directionOrder.Length;
        int index = FindOrientationIndex(orientation);

        List<Direction> directionPath = new List<Direction>();
        directionPath.Add(orientation);
        for (int i = 1; i < path.Count; i++) {
            int offset = MapOffset(path[i] - path[i-1]);
            directionPath.Add(directionOrder[(index + offset) % total]);
        }

        return directionPath;
    }

    private static List<int> CreateMaze() {
        int limitIndex = limit - 1;

        bool[] visited = new bool[row * col];
        Stack<int> path = new Stack<int>();
        Stack<int> offsets = new Stack<int>();
        offsets.Push(1);
        path.Push(0);
        visited[0] = true;

        int[] offsetOrder = new int[] {1, row, -1, -row};
        while (path.Peek() != limitIndex) {
            int cur = path.Peek();
            int prevOffset = offsets.Peek();
            int next = choice(visited, cur, prevOffset);
            int offset = next - cur;

            if (next == -1) {
                path.Pop();
                offsets.Pop();
            } else {
                // pre visit
                visited[next] = true;
                path.Push(next);
                offsets.Push(offset);
            }
        }

        List<int> reverse = new List<int>(path.ToArray());
        reverse.Reverse();
        return reverse;
    }

    // offset used to calculate direction from maze orientation
    private static int MapOffset(int offset) {
        if (offset == 0) return 0; // first point which is a parent off itself, // North
        else if (offset == 1) return 0;  // step in direction of orientation, // North
        else if (offset == -1) return 2; // step opposite of orientation, // South
        else if (offset > 0) return 1; // one clockwise rotate, offset == row, // East
        else return 3;  // one anti clockwise, offset == -row, // West
    }

    private static int FindOrientationIndex(Direction orientation) {
        for (int i = 0; i < directionOrder.Length; i++) {
            if (directionOrder[i] == orientation) {
                return i;
            }
        }

        return 0;
    }

    private static int choice(bool[] visited, int cur, int prevOffset) {
        List<int> values = new List<int>();
        foreach (int offset in offsetOrder) {
            int val = cur + offset;
            // check for limits and directly opposite direction
            if (val < 0 || val >= limit || visited[val] || offset == -prevOffset) {
                continue;
            } else {
                int border = (cur + 1) % row;
                // check for edge of a row
                if ((border == 0 && offset == 1) || (border == 1 && offset == -1)) {
                    continue;
                } else {
                    values.Add(val);
                    if (offset == prevOffset) {  // bias for straight lines
                        values.Add(val);
                    }
                }
            }
        }

        if (values.Count == 0) {
            return -1;
        } else {
            return values[rng.Next(values.Count)];
        }
    }
}
