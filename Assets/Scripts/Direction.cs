using System.Collections.Generic;

public enum Direction {
    North,
    East,
    South,
    West
}

public enum RelativeDirection {
    LEFT,
    STRAIGHT,
    RIGHT,
}

public static class DirectionExtension {
    public static Direction Opposite(this Direction d) {
        switch (d)
        {
            case Direction.North: return Direction.South;
            case Direction.East: return Direction.West;
            case Direction.South: return Direction.North;
            case Direction.West: return Direction.East;
            default: return Direction.North;
        }
    }

    public static List<Direction> NextDirections(this Direction d) {
        switch (d)
        {
            case Direction.North: return new List<Direction> {Direction.East, Direction.West, Direction.North};
            case Direction.East: return new List<Direction> {Direction.East, Direction.South, Direction.North};
            case Direction.South: return new List<Direction> {Direction.East, Direction.West, Direction.South};
            case Direction.West: return new List<Direction> {Direction.South, Direction.West, Direction.North};
            default: return new List<Direction>();
        }

    }

    public static RelativeDirection relative(this Direction d, Direction that) {
        switch (d)
        {
            case Direction.East: {
                switch (that)
                {
                    case Direction.East: return RelativeDirection.STRAIGHT;
                    case Direction.North: return RelativeDirection.LEFT;
                    case Direction.South: return RelativeDirection.RIGHT;
                    default: return RelativeDirection.STRAIGHT;
                }
            }
            case Direction.North: {
                switch (that)
                {
                    case Direction.North: return RelativeDirection.STRAIGHT;
                    case Direction.West: return RelativeDirection.LEFT;
                    case Direction.East: return RelativeDirection.RIGHT;
                    default: return RelativeDirection.STRAIGHT;
                }
            }
            case Direction.West: {
                switch (that)
                {
                    case Direction.West: return RelativeDirection.STRAIGHT;
                    case Direction.South: return RelativeDirection.LEFT;
                    case Direction.North: return RelativeDirection.RIGHT;
                    default: return RelativeDirection.STRAIGHT;
                }
            }
            case Direction.South: {
                switch (that)
                {
                    case Direction.South: return RelativeDirection.STRAIGHT;
                    case Direction.East: return RelativeDirection.LEFT;
                    case Direction.West: return RelativeDirection.RIGHT;
                    default: return RelativeDirection.STRAIGHT;
                }
            }
            default: return RelativeDirection.STRAIGHT;
        }
    }
}
