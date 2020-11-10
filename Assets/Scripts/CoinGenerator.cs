using System.Collections.Generic;
using UnityEngine;

public class CoinGenerator : MonoBehaviour {

    // Note single integer denotes x and z coordinates
    // encoded as x*10 + z
    int[][][] setpieces =  new int[][][] {
        // empty type
        new int [][] {new int [] {}},
        
        // straight coin types
        new int [][] {
            // left lane
            new int[] {
                00, 01, 02, 03, 04, 05, 06, 07, 08, 09
            },
            // centre lane
            new int[] {
                10, 11, 12, 13, 14, 15, 16, 17, 18, 19
            },
            // left lane
            new int[] {
                20, 21, 22, 23, 24, 25, 26, 27, 28, 29
            },
        },

        // mixed lanes coins
        new int[][] {
            new int[] {
                00, 01, 02,                     08, 09,
                            13, 14,     16, 17, 
                                    25,
            },
            new int[] {
                00, 01, 02, 03,
                                14, 15, 16,
                                            27, 28, 29,
            },
            new int[] {
                                            07, 08, 09,
                                14, 15, 16,
                20, 21, 22, 23,
            },
            new int[] {
                                    05,
                            13, 14,     16, 17, 
                20, 21, 22,                     28, 29,
            },
            new int[] {

                10, 11, 12, 13,
                                24, 25, 26, 27, 28, 29,
            },
            new int[] {
                                04, 05, 06, 07, 08, 09,
                10, 11, 12, 13,

            },
        },

        // stop and go
        new int[][] {
            new int[] {
                00, 01, 02,             06, 07,

                            23, 24, 25,         28, 29,
            },
            new int[] {
                00, 01, 02,    04,05,       08, 09,
            },
            new int[] {

                10, 11, 12,    14,15,       18, 19,
            },
            new int[] {
                20, 21, 22,    24,25,       28, 29,
            },
            new int[] {
                00, 01, 02, 03,             06, 07, 08, 09,

                            23, 24, 25, 26, 27,
            },
            new int[] {

                            13, 14, 15, 16, 17,
                20, 21, 22, 23,             26, 27, 28, 29,
            },
        },

        // multi column mix
        new int[][] {
            new int[] {
                            02, 03, 04, 05,
                10, 11, 12,
                    21, 22, 23, 24, 25, 26, 27, 28, 29
            },
            new int[] {
                            02, 03, 04, 05,
                10, 11, 12,
                    21, 22, 23, 24, 25, 26, 27, 28, 29
            },
            new int[] {
                00, 01, 02, 03, 04, 05, 06, 07, 08, 09,
                10, 11, 12, 13, 14, 15, 16, 17, 18, 19,
                20, 21, 22, 23, 24, 25, 26, 27, 28, 29,
            },
            new int[] {
                00, 01,         04, 05, 06, 07, 08, 09,
                10, 11, 12, 13,     15, 16,     18, 19,
                20, 21,     23, 24, 25, 26,     28, 29,
            },
            new int[] {
                                04, 05, 06,     08, 09,
                                14, 15, 16, 17, 18, 19,
                20, 21, 22, 23, 24, 25, 26, 27, 28, 29,
            },
            new int[] {
                00, 01, 02, 03, 04, 05, 06, 07, 08, 09,
                                    15, 16, 17, 18, 19,
                                24, 25, 26, 27,     29,
            },
        }
    };
    float offsetx = 1.4f;
    float offsety = 0f;
    float offsetz = 1f;

    Vector3 buildPoint = new Vector3(0.6f, 0.8f, 0.5f);
    System.Random rng = new System.Random();
    public GameObject coinPrefab;

    public void nextCoins(PathPrefab prefab, PathPrefab prev, Difficulty level) {
        // bias type of coin path based on difficulty
        switch (level) {
            case Difficulty.EASY: {
                int type = new int[] {0, 0, 1, 1, 2, 2, 3, 4}[rng.Next(7)];
                RenderSetPiece(CreateLigature(type, rng.Next(setpieces[type].Length), prefab, prev), prefab);
                break;
            }
            case Difficulty.MED: {
                int type = new int[] {0, 0, 0, 1, 2, 2, 3, 3, 4, 4}[rng.Next(10)];
                RenderSetPiece(CreateLigature(type, rng.Next(setpieces[type].Length), prefab, prev), prefab);
                break;
            }
            case Difficulty.HARD: {
                int type = new int[] {0, 0, 0, 0, 1, 3, 3, 3, 4, 4}[rng.Next(10)];
                RenderSetPiece(CreateLigature(type, rng.Next(setpieces[type].Length), prefab, prev), prefab);
                break;
            }
        }
    }

    public void DebugNextCoins(int type, int piece, PathPrefab prefab) {
        RenderSetPiece(new List<int>(setpieces[type][piece]), prefab);
    }

    public void DebugNextCoins(int type, int piece, PathPrefab prefab, PathPrefab prev) {
        RenderSetPiece(CreateLigature(type, piece, prefab, prev), prefab);
    }

    void RenderSetPiece(List<int> coinPos, PathPrefab prefab) {
        // handle empty set piece
        if (coinPos.Count == 0) {
            // set any last coint block and return
            prefab.lastCoin = rng.Next(3);
            return;
        }

        GameObject path = prefab.gameObject;
        
        foreach (int val in coinPos) {

            // coin cannot be placed on obstacle
            // TODO: perform other checks
            if (prefab.pathObjects[val] != ObjectType.NONE) continue;

            Vector3 pos = buildPoint + new Vector3(val / 10 * offsetx, offsety, val % 10 * offsetz);
            GameObject coin = Instantiate(coinPrefab) as GameObject;
            coin.GetComponent<Animator>().SetFloat("Offset", Random.Range(0f, 1f));
            coin.transform.parent = path.transform;
            coin.transform.localPosition = pos;
            coin.transform.rotation = Quaternion.identity;
        }

        prefab.lastCoin = coinPos[coinPos.Count - 1];
    }

    List<int> CreateLigature(int type, int piece, PathPrefab prefab, PathPrefab prev) {
        List<int> coinPos = new List<int>(setpieces[type][piece]);
        RelativeDirection turn = prev.blockDirection.relative(prefab.blockDirection);

        // trim excess coins
        switch (turn) {
            // In a left turn trim excess coins to the right
            case RelativeDirection.LEFT: {
                switch (prev.lastCoin / 10) {
                    case 0: {
                        coinPos.Remove(1);
                        coinPos.Remove(11);
                        coinPos.Remove(21);
                        goto case 1;
                    }
                    case 1: {
                        coinPos.Remove(0);
                        coinPos.Remove(10);
                        coinPos.Remove(20);
                        goto default;
                    }
                    default: break;
                }
                break;
            }

            // In a right turn trim excess coins to the left
            case RelativeDirection.RIGHT: {
                switch (prev.lastCoin / 10) {
                    case 2: {
                        coinPos.Remove(1);
                        coinPos.Remove(11);
                        coinPos.Remove(21);
                        goto case 1;
                    }
                    case 1: {
                        coinPos.Remove(0);
                        coinPos.Remove(10);
                        coinPos.Remove(20);
                        goto default;
                    }
                    default: break;
                }
                break;
            }
        }

        // join lines by extending from previous block vertically up until it
        // meets horizontal (left, right) line in current block
        switch (turn) {
            case RelativeDirection.LEFT: {
                switch (prev.lastCoin / 10) {
                    case 0: {
                        if (coinPos.Contains(2)) break;
                        coinPos.Add(2);

                        if (coinPos.Contains(12)) break;
                        coinPos.Add(12);

                        if (coinPos.Contains(22)) break;
                        coinPos.Add(22);

                        break;
                    }

                    case 1: {
                        if (coinPos.Contains(1)) break;
                        coinPos.Add(1);

                        if (coinPos.Contains(11)) break;
                        coinPos.Add(11);

                        if (coinPos.Contains(21)) break;
                        coinPos.Add(21);

                        break;
                    }

                    case 2: {
                        if (coinPos.Contains(0)) break;
                        coinPos.Add(0);

                        if (coinPos.Contains(10)) break;
                        coinPos.Add(10);

                        if (coinPos.Contains(20)) break;
                        coinPos.Add(20);

                        break;
                    }
                }

                break;
            }
            case RelativeDirection.RIGHT: {
                switch (prev.lastCoin / 10) {
                    case 0: {
                        if (coinPos.Contains(20)) break;
                        coinPos.Add(20);

                        if (coinPos.Contains(10)) break;
                        coinPos.Add(10);

                        if (coinPos.Contains(0)) break;
                        coinPos.Add(0);

                        break;
                    }

                    case 1: {
                        if (coinPos.Contains(21)) break;
                        coinPos.Add(21);

                        if (coinPos.Contains(11)) break;
                        coinPos.Add(11);

                        if (coinPos.Contains(1)) break;
                        coinPos.Add(1);

                        break;
                    }

                    case 2: {
                        if (coinPos.Contains(22)) break;
                        coinPos.Add(22);

                        if (coinPos.Contains(12)) break;
                        coinPos.Add(12);

                        if (coinPos.Contains(2)) break;
                        coinPos.Add(2);

                        break;
                    }
                }
                break;
            }
        }

        return coinPos;
    }
}
