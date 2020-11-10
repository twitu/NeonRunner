using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum ObjectType {
    NONE,
    LEFT_TREE,
    CENTRE_TREE,
    RIGHT_TREE,
    ROCK,
    WATER,
    INVINCI,
}

enum CameraType {
    FOLLOW,
    BIRD,
    TPS,
}

public enum Difficulty {
    EASY,
    MED,
    HARD,
}

public class Director : MonoBehaviour {
    private int coinsCollected;
    private int blocksCleared;
    public GameObject rampObject;
    public GameObject ragDollObject;
    public GameObject characterObject;
    public UILogic uILogic;
    public PathGenerator pathGenerator;
    public CoinGenerator coinGenerator;
    public ObstacleGenerator obstacleGenerator;
    public GameObject particleObject;
    public GameObject RampPrefab;
    public GameObject CharacterPrefab;
    public Camera cameraObject;
    public GameObject FollowCameraObject;
    public GameObject BirdCameraObject;
    public GameObject TPSCameraObject;
    public Transform cameraTransform;
    public Transform particleTransform;
    public ParticleSystem particleSystem;
    public GameObject RagDollPrefab;
    public Mode gameMode;
    public float transitionTime;
    private Quaternion cameraStart;
    private Quaternion cameraEnd;
    private Color BackgroundColorStart;
    private Color BackgroundColorEnd;
    private Color ParticleColorStart;
    private Color ParticleColorEnd;
    private float transitionElapsed = 0f;
    private int score;
    private int highScore;
    private int coinScore = 100;
    private int distScore = 1000;

    public enum Mode {
        NORMAL,
        TRANSITION,
        INVINCIBLE_MODE,
        TRANSITION_BACK,
    }

    void Start() {
        // intialize reference to components
        cameraTransform = cameraObject.GetComponent<Transform>();
        particleTransform = particleObject.GetComponent<Transform>();
        particleSystem = particleObject.GetComponent<ParticleSystem>();

        // set mode
        gameMode = Mode.NORMAL;
        cameraStart = cameraTransform.rotation;
        BackgroundColorStart = cameraObject.backgroundColor;
        BackgroundColorEnd = new Color(106f, 23f, 14f);

        cameraEnd = Quaternion.Euler(cameraStart.eulerAngles.x, 0, 180);
        transitionTime = 0.5f;
        transitionElapsed = 0f;

        // setup scene
        SetupScene();
    }

    void Update() {
        switch (gameMode) {
            case Mode.NORMAL: {
                // nothing doing
                break;
            }
            case Mode.TRANSITION: {
                transitionElapsed += Time.deltaTime / transitionTime;
                cameraTransform.rotation = Quaternion.Slerp(cameraStart, cameraEnd, transitionElapsed);
                particleTransform.rotation = Quaternion.Slerp(cameraStart, cameraEnd, transitionElapsed);
                if (transitionElapsed >= 1f) {
                    gameMode = Mode.INVINCIBLE_MODE;
                    transitionElapsed = 0f;
                }
                break;
            }
            case Mode.INVINCIBLE_MODE: {
                // nothing doing for now
                break;
            }
            case Mode.TRANSITION_BACK: {
                transitionElapsed = transitionElapsed + Time.deltaTime / transitionTime;
                cameraTransform.rotation = Quaternion.Slerp(cameraEnd, cameraStart, transitionElapsed);
                particleTransform.rotation = Quaternion.Slerp(cameraStart, cameraEnd, transitionElapsed);
                if (transitionElapsed >= 1f) {
                    gameMode = Mode.NORMAL;
                    transitionElapsed = 0f;
                }
                break;
            }
        }
    }

    public void SetupScene() {
        // update counts
        coinsCollected = 0;
        blocksCleared = 0;
        score = 0;
        highScore = (highScore > score) ? highScore : score;
        uILogic.ChangeScore(score);
        uILogic.ChangeHighScore(highScore);

        // Create ramp
        rampObject = Instantiate(RampPrefab, new Vector3(0, 0, 0), Quaternion.identity);

        // reset path generator
        pathGenerator.SetupRamp(rampObject);
        while (pathGenerator.pathQueue.Peek() == null) {
            AddNewBlock();
        }

        // setup character
        ragDollObject.SetActive(false);
        characterObject.transform.position = new Vector3(2, 0, 1);
        characterObject.transform.rotation = Quaternion.identity;
        characterObject.GetComponent<CapsuleCollider>().height = 1.8f;
        characterObject.GetComponent<CapsuleCollider>().center = new Vector3(0, 0.9f, 0);
        characterObject.GetComponent<Animator>().Play("Idle", 0);
        characterObject.SetActive(true);

        // set UI panel to start
        uILogic.SetPanelActive(Panel.START);

        // set cameras
        ChangeCamera(CameraType.TPS);
    }

    public void InScene() {
        uILogic.SetPanelActive(Panel.IN);
        // change nums
        ChangeCamera(CameraType.FOLLOW);
        characterObject.GetComponent<Animator>().SetTrigger("RunTrigger");
    }

    public void EndScene() {
        highScore = (highScore < score) ? score : highScore;
        StartCoroutine(EndSceneRoutine());
    }

    IEnumerator EndSceneRoutine() {
        ChangeCamera(CameraType.BIRD);
        StartCoroutine(pathGenerator.TearDownRamp());
        uILogic.ChangeBreakdown(coinsCollected, blocksCleared, score);
        uILogic.SetPanelActive(Panel.END);
        yield return new WaitUntil(() => pathGenerator.tearDownComplete);
        Destroy(rampObject);
        SetupScene();
    }

    public void CollectCoin() {
        coinsCollected++;
        score += coinScore;
        uILogic.ChangeScore(score);
    }

    public void ClearBlock() {
        blocksCleared++;
        score += distScore;
        uILogic.ChangeScore(score);

        // decide difficulty
        Difficulty level;
        double ratio = coinsCollected / (double) blocksCleared;
        if (ratio > 9) {
            level = Difficulty.HARD;
        } else if (ratio > 7) {
            level = Difficulty.MED;
        } else {
            level = Difficulty.EASY;
        }

        // start pipeline to create new block
        (PathPrefab prefab, PathPrefab prev) = pathGenerator.AddPathBlock();
        obstacleGenerator.nextObstacles(prefab, prev, level);
        coinGenerator.nextCoins(prefab, prev, level);
    }

    public void AddNewBlock() {
        (PathPrefab prefab, PathPrefab prev) = pathGenerator.AddPathBlock();
        obstacleGenerator.nextObstacles(prefab, prev, Difficulty.EASY);
        coinGenerator.nextCoins(prefab, prev, Difficulty.EASY);
    }

    public void TriggerInvincible() {
        transitionElapsed = 0f;
        gameMode = Mode.INVINCIBLE_MODE;
    }

    public void ObstacleCollision(GameObject player) {
        // throw rag doll in place of player
        Rigidbody temprb = player.GetComponent<Rigidbody>();
        Rigidbody ragDollrb = ragDollObject.GetComponent<Rigidbody>();
        ragDollrb.velocity = 5 * temprb.velocity;
        ragDollrb.angularVelocity = temprb.angularVelocity;
        ragDollObject.SetActive(true);
        player.SetActive(false);

        EndScene();
    }

    void ChangeCamera(CameraType type) {
        switch (type) {
            case CameraType.FOLLOW: {
                FollowCameraObject.SetActive(true);
                BirdCameraObject.SetActive(false);
                TPSCameraObject.SetActive(false);
                return;
            }
            case CameraType.BIRD: {
                FollowCameraObject.SetActive(false);
                BirdCameraObject.SetActive(true);
                TPSCameraObject.SetActive(false);
                return;
            }
            case CameraType.TPS: {
                FollowCameraObject.SetActive(false);
                BirdCameraObject.SetActive(false);
                TPSCameraObject.SetActive(true);
                return;
            }
        }
    }
}
