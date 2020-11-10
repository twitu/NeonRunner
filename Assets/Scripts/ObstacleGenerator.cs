using System;
using System.Collections.Generic;
using UnityEngine;
public class ObstacleGenerator : MonoBehaviour {
    // unit offsets
    float offsetx = 1.5f;
    float offsety = 0f;
    float offsetz = 1f;

    // obstacle prefabs
    public GameObject leftTree;
    public GameObject centreTree;
    public GameObject rightTree;
    public GameObject rock;
    public GameObject water;
    public GameObject invinci;
    System.Random rng = new System.Random();

    // array of thirty values represents an obstacle set piece along with its position
    // on the path block
    Tuple<ObjectType, int>[][][] setpieces = new Tuple<ObjectType, int>[][][] {
        // empty path
        new Tuple<ObjectType, int> [][] {
            new Tuple<ObjectType, int> [] {
            },
        },

        // simple
        new Tuple<ObjectType, int> [][] {
            new Tuple<ObjectType, int> [] {
                Tuple.Create(ObjectType.ROCK, 15),
                Tuple.Create(ObjectType.ROCK, 08),
                Tuple.Create(ObjectType.ROCK, 03),
            },
            new Tuple<ObjectType, int> [] {
                Tuple.Create(ObjectType.ROCK, 14),
                Tuple.Create(ObjectType.ROCK, 17),
            },
            new Tuple<ObjectType, int> [] {
                Tuple.Create(ObjectType.ROCK, 04),
                Tuple.Create(ObjectType.ROCK, 07),
            },
            new Tuple<ObjectType, int> [] {
                Tuple.Create(ObjectType.ROCK, 24),
                Tuple.Create(ObjectType.ROCK, 27),
            },
            new Tuple<ObjectType, int> [] {
                Tuple.Create(ObjectType.LEFT_TREE, 4),
                Tuple.Create(ObjectType.RIGHT_TREE, 6),
            },
            new Tuple<ObjectType, int> [] {
                Tuple.Create(ObjectType.RIGHT_TREE, 4),
                Tuple.Create(ObjectType.LEFT_TREE, 7),
            },
            new Tuple<ObjectType, int> [] {
                Tuple.Create(ObjectType.CENTRE_TREE, 9),
                Tuple.Create(ObjectType.LEFT_TREE, 5),
            },
        },

        // multi-mix
        new Tuple<ObjectType, int> [][] {
            new Tuple<ObjectType, int> [] {
                Tuple.Create(ObjectType.ROCK, 03),
                Tuple.Create(ObjectType.ROCK, 23),
                Tuple.Create(ObjectType.LEFT_TREE, 6),
            },
            new Tuple<ObjectType, int> [] {
                Tuple.Create(ObjectType.ROCK, 03),
                Tuple.Create(ObjectType.ROCK, 23),
                Tuple.Create(ObjectType.RIGHT_TREE, 6),
            },
            new Tuple<ObjectType, int> [] {
                Tuple.Create(ObjectType.ROCK, 03),
                Tuple.Create(ObjectType.ROCK, 23),
                Tuple.Create(ObjectType.CENTRE_TREE, 6),
            },
            new Tuple<ObjectType, int> [] {
                Tuple.Create(ObjectType.ROCK, 03),
                Tuple.Create(ObjectType.ROCK, 23),
                Tuple.Create(ObjectType.LEFT_TREE, 6),
            },
            new Tuple<ObjectType, int> [] {
                Tuple.Create(ObjectType.ROCK, 04),
                Tuple.Create(ObjectType.ROCK, 14),
                Tuple.Create(ObjectType.ROCK, 24),
                Tuple.Create(ObjectType.CENTRE_TREE, 6),
            },
            new Tuple<ObjectType, int> [] {
                Tuple.Create(ObjectType.CENTRE_TREE, 9),
                Tuple.Create(ObjectType.LEFT_TREE, 6),
                Tuple.Create(ObjectType.RIGHT_TREE, 3),
            },
            new Tuple<ObjectType, int> [] {
                Tuple.Create(ObjectType.LEFT_TREE, 3),
                Tuple.Create(ObjectType.RIGHT_TREE, 8),
            },
        },

        // simulataneous
        new Tuple<ObjectType, int> [][] {
            new Tuple<ObjectType, int> [] {
                Tuple.Create(ObjectType.ROCK, 04),
                Tuple.Create(ObjectType.ROCK, 14),
                Tuple.Create(ObjectType.ROCK, 24),
                Tuple.Create(ObjectType.ROCK, 07),
                Tuple.Create(ObjectType.ROCK, 17),
                Tuple.Create(ObjectType.ROCK, 27),
            },
            new Tuple<ObjectType, int> [] {
                Tuple.Create(ObjectType.CENTRE_TREE, 02),
                Tuple.Create(ObjectType.CENTRE_TREE, 06),
                Tuple.Create(ObjectType.CENTRE_TREE, 07),
                Tuple.Create(ObjectType.ROCK, 03),
                Tuple.Create(ObjectType.ROCK, 28),
            },
            new Tuple<ObjectType, int> [] {
                Tuple.Create(ObjectType.LEFT_TREE, 02),
                Tuple.Create(ObjectType.RIGHT_TREE, 06),
                Tuple.Create(ObjectType.ROCK, 03),
                Tuple.Create(ObjectType.ROCK, 05),
                Tuple.Create(ObjectType.ROCK, 08),
            },
        },
    };

    public void nextObstacles(PathPrefab prefab, PathPrefab prev, Difficulty level) {
        switch (level) {
            case Difficulty.EASY: {
                int type = new int[] {0, 0, 0, 0, 1, 1, 1, 2}[rng.Next(8)];
                int pattern  = rng.Next(setpieces[type].Length);
                RenderSetPiece(new List<Tuple<ObjectType, int>>(setpieces[type][pattern]), prefab);
                break;
            }
            case Difficulty.MED: {
                int type = new int[] {0, 0, 0, 1, 1, 1, 2, 3}[rng.Next(7)];
                int pattern  = rng.Next(setpieces[type].Length);
                RenderSetPiece(new List<Tuple<ObjectType, int>>(setpieces[type][pattern]), prefab);
                break;
            }
            case Difficulty.HARD: {
                int type = new int[] {0, 0, 1, 1, 2, 2, 3, 3}[rng.Next(7)];
                int pattern  = rng.Next(setpieces[type].Length);
                RenderSetPiece(new List<Tuple<ObjectType, int>>(setpieces[type][pattern]), prefab);
                break;
            }
        }
    }

    void RenderSetPiece(List<Tuple<ObjectType, int>> piece, PathPrefab prefab) {
        GameObject path = prefab.gameObject;
        foreach (Tuple<ObjectType, int> tup in piece) {
            Vector3 pos;
            if (tup.Item1 == ObjectType.ROCK) {
                pos = new Vector3(tup.Item2 / 10 * offsetx, offsety, tup.Item2 % 10 * offsetz);
            } else {
                // pre configured x and y values
                pos = new Vector3(0f, 0f, tup.Item2 % 10 * offsetz);
            }
            GameObject obs = Instantiate(GetObjectPrefab(tup.Item1)) as GameObject;
            obs.transform.parent = path.transform;
            obs.transform.localPosition = pos;
            obs.transform.rotation = path.transform.rotation;  // parent path orientation
            prefab.pathObjects[tup.Item2] = tup.Item1;
        }
    }
    public GameObject GetObjectPrefab (ObjectType name) {
        switch (name)
        {
            case ObjectType.LEFT_TREE: return leftTree;
            case ObjectType.CENTRE_TREE: return centreTree;
            case ObjectType.RIGHT_TREE: return rightTree;
            case ObjectType.ROCK: return rock;
            case ObjectType.WATER: return water;
            case ObjectType.INVINCI: return invinci;
            default: return rock;
        }
    }
}
