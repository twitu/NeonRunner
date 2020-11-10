using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : MonoBehaviour {
    public GameObject ramp;
    public int queueLength;
    public GameObject pathPrefab;
    public Transform buildPoint;
    public CoinGenerator coinGenerator;
    public bool tearDownComplete;
    public float tearDownTime = 0.3f;
    public CircularQueue<PathPrefab> pathQueue;
    private Direction nextDirection;
    private Transform planeTransform;
    private float pathLength;
    private float pathBreadth;
    private int planeMeshMultiplier = 10;
    private PathPrefab lastPath;
    private Direction lastDirection;
    private System.Random rng = new System.Random();
    private List<Direction> mazePathA;
    private List<Direction> mazePathB;
    private bool choosePathA;
    private int mazePathIndex;
    private int mazePathBuffer;
    private int count = 0;
    private int mazeDim = 100;  // TODO fix background maze generation

    void Start() {
        lastDirection = Direction.North;

        planeTransform = pathPrefab.transform.Find("Plane").GetComponent<Transform>();
        pathLength = planeTransform.localScale.z * planeMeshMultiplier;
        pathBreadth = planeTransform.localScale.x * planeMeshMultiplier;
    }

    public void SetupRamp(GameObject rampObject) {
        pathQueue = new CircularQueue<PathPrefab>(queueLength);
        buildPoint.transform.position = new Vector3(0, 0, 30);
        tearDownComplete = false;
        foreach (Transform child in rampObject.transform) {
            ramp = child.gameObject;
            ramp.GetComponent<BoxCollider>().enabled = false;
            pathQueue.Add(ramp.GetComponent<PathPrefab>());
        }

        lastDirection = Direction.North;
        lastPath = ramp.GetComponent<PathPrefab>();

        mazePathA = MazeCreator.FindPath(mazeDim, mazeDim, lastDirection);
        choosePathA = true;
        mazePathIndex = 0;
        mazePathBuffer = 10;
    }

    public IEnumerator TearDownRamp() {
        while (pathQueue.Peek() != null) {
            GameObject pathObj = pathQueue.Add(null).gameObject;
            Destroy(pathObj);
            yield return new WaitForSeconds(tearDownTime);
        }

        tearDownComplete = true;
    }

    private List<Direction> ChoosePath() {
        return (choosePathA) ? mazePathA : mazePathB;
    }

    private List<Direction> CheckAndChoose() {
        if (choosePathA) {
            if (mazePathA.Count - mazePathIndex == 0) {
                choosePathA = false;
                mazePathIndex = 0;
                return mazePathB;
            } else {
                return mazePathA;
            }
        } else {
            if (mazePathB.Count - mazePathIndex == 0) {
                choosePathA = true;
                mazePathIndex = 0;
                return mazePathA;
            } else {
                return mazePathB;
            }
        }
    }

    private void CheckPathBuffer() {
        List<Direction> mazePath = ChoosePath();
        if (mazePath.Count - mazePathBuffer < mazePathBuffer) {
            StartCoroutine("GenerateMazePath");
        }
    }

    private void GenerateMazePath() {
        List<Direction> cur = ChoosePath();
        List<Direction> newpaths = MazeCreator.FindPath(mazeDim, mazeDim, cur[cur.Count - 1]);
        Debug.Log("Background maze path");
        if (choosePathA) {
            mazePathB = newpaths;
        } else {
            mazePathA = newpaths;
        }
    }

    public (PathPrefab, PathPrefab) AddPathBlock() {
        nextDirection = CheckAndChoose()[mazePathIndex++];

        // calculate next path values if this path is about to finish
        CheckPathBuffer();

        buildPoint.position = buildPoint.position + DirectedOffset(lastDirection, nextDirection);
        buildPoint.rotation = DirectedRotation(nextDirection);
        GameObject nextPath = Instantiate(pathPrefab, buildPoint.position, buildPoint.rotation);
        nextPath.name = "Clone " + count.ToString();
        count++;
        PathPrefab prefab = nextPath.GetComponent<PathPrefab>();

        // set path values
        prefab.blockDirection = nextDirection;
        prefab.blockObject = nextPath;

        PathPrefab temp = pathQueue.Add(prefab);
        if (temp != null) {
            Destroy(temp.gameObject);
        }

        temp = lastPath;
        lastPath = prefab;
        lastDirection = nextDirection;

        return (prefab, temp);
    }

    private Quaternion DirectedRotation(Direction d) {
        switch (d)
        {
            case Direction.North: return Quaternion.Euler(0, 0, 0);
            case Direction.East: return Quaternion.Euler(0, 90, 0);
            case Direction.South: return Quaternion.Euler(0, 180, 0);
            case Direction.West: return Quaternion.Euler(0, 270, 0);
            default: return Quaternion.Euler(0, 0, 0);
        }
    }

    private Vector3 DirectedOffset(Direction prev, Direction next) {
        switch (prev)
        {
            case Direction.North: {
                switch (next)
                {
                    case Direction.North: return new Vector3(0, 0, pathLength);
                    case Direction.East: return new Vector3(0, 0, pathBreadth + pathLength);
                    case Direction.West: return new Vector3(pathBreadth, 0, pathLength);
                    default: return new Vector3(0, 0, pathLength);
                }
            }
            case Direction.East: {
                switch (next)
                {
                    case Direction.North: return new Vector3(pathLength, 0, -pathBreadth);
                    case Direction.East: return new Vector3(pathLength, 0, 0);
                    case Direction.South: return new Vector3(pathBreadth + pathLength, 0, 0);
                    default: return new Vector3(pathLength, 0, 0);
                }
            }
            case Direction.South: {
                switch (next)
                {
                    case Direction.West: return new Vector3(0, 0, - pathBreadth - pathLength);
                    case Direction.East: return new Vector3(-pathBreadth, 0, -pathLength);
                    case Direction.South: return new Vector3(0, 0, -pathLength);
                    default: return new Vector3(0, 0, -pathLength);
                }
            }
            case Direction.West: {
                switch (next)
                {
                    case Direction.West: return new Vector3(-pathLength, 0, 0);
                    case Direction.North: return new Vector3(- pathBreadth - pathLength, 0, 0);
                    case Direction.South: return new Vector3(-pathLength, 0, pathBreadth);
                    default: return new Vector3(-pathLength, 0, 0);
                }
            }
            default: return new Vector3(0, 0, 0);
        }
    }

    // Debug function
    IEnumerator TestCoinOrientation() {
        buildPoint.transform.position = new Vector3(-30, 0, 30);
        GameObject stand, head;
        Direction cur = Direction.North;  // testing only one direction
        foreach (Direction e in System.Enum.GetValues(typeof(Direction))) {
            foreach (int firstpiece in new int[] {0, 1, 2}) {
                foreach (int secondpiece in new int[] {0, 1, 2}) {
                    // ignore opposite direction block
                    if (e == cur.Opposite()) continue;

                    // bootstrap first piece
                    stand = Instantiate(pathPrefab, buildPoint.transform.position, DirectedRotation(cur));
                    PathPrefab standPrefab = stand.GetComponent<PathPrefab>();
                    standPrefab.blockDirection = cur;
                    standPrefab.blockObject = stand;
                    coinGenerator.DebugNextCoins(1, firstpiece, standPrefab);

                    // create second piece as it would be created in game
                    head = Instantiate(pathPrefab, buildPoint.position + DirectedOffset(cur, e), DirectedRotation(e));
                    PathPrefab headPrefab = head.GetComponent<PathPrefab>();
                    headPrefab.blockDirection = e;
                    headPrefab.blockObject = head;
                    coinGenerator.DebugNextCoins(1, secondpiece, headPrefab, standPrefab);

                    // next iteration after 2 seconds
                    yield return new WaitForSeconds(2);
                    Destroy(head);
                    Destroy(stand);
                }
            }
        }
    }


}
