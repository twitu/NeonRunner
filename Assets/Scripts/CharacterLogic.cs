using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Lane {
    LEFT,
    CENTRE,
    RIGHT,
}

public class CharacterLogic : MonoBehaviour
{
    public AudioClip runSound;
    public AudioClip slideSound;
    public AudioClip jumpSound;
    public AudioClip collectSound;
    public AudioClip collideSound;
    public Director director;
    Animator animator;
    AudioSource audioPlayer;
    public float speed = 6;
    public float nudgeAmount = 0.2f;
    Transform playerTransform;
    CapsuleCollider collider;
    float colliderY;
    float colliderHeight;
    int colliderHeightHash;
    int colliderYHash;
    int jumpTriggerHash;
    int slideTriggerHash;
    int jumpState;
    int runState;
    int slideState;
    bool jumping;
    bool sliding;
    Lane currentLane;
    // Start is called before the first frame update
    void Start()
    {
        currentLane = Lane.CENTRE;
        animator = this.gameObject.GetComponent<Animator>();
        audioPlayer = this.gameObject.GetComponent<AudioSource>();
        collider = this.gameObject.GetComponent<CapsuleCollider>();
        colliderY = collider.center.y;
        colliderHeight = collider.height;
        jumpTriggerHash = Animator.StringToHash("JumpTrigger");
        slideTriggerHash = Animator.StringToHash("SlideTrigger");
        runState = Animator.StringToHash("Base Layer.Running");
        jumpState = Animator.StringToHash("Base Layer.Jump");
        slideState = Animator.StringToHash("Base Layer.RunningSlide");
        colliderHeightHash = Animator.StringToHash("ColliderHeight");
        colliderYHash = Animator.StringToHash("ColliderY");
        playerTransform = this.gameObject.transform;
    }

    void OnEnable() {
        jumping = false;
        sliding = false;
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        if (currentState.fullPathHash == jumpState) {
            if (!animator.IsInTransition(0)) {
                // change collider position based on parameter
                collider.center = new Vector3(collider.center.x, colliderY * animator.GetFloat(colliderYHash), collider.center.z);
            }
        } else if (currentState.fullPathHash == slideState) {
            if (!animator.IsInTransition(0)) {
                // change collider height and position
                float val = animator.GetFloat(colliderHeightHash);
                collider.height = colliderHeight * val;
                collider.center = new Vector3(collider.center.x, colliderY * val, collider.center.z);
            }
        } else if (currentState.fullPathHash == runState) {

            // set running sound
            if (!audioPlayer.isPlaying && audioPlayer.clip != runSound) {
                audioPlayer.clip = runSound;
                audioPlayer.loop = true;
                audioPlayer.Play();
            }

            if (!jumping && Input.GetKeyDown(KeyCode.Space)) {
                jumping = true;
                animator.SetTrigger(jumpTriggerHash);

                // start playing jump sound
                PlayOneShot(jumpSound, 0.2f);
            }

            if (!sliding && Input.GetKeyDown(KeyCode.LeftControl)) {
                sliding = true;
                animator.SetTrigger(slideTriggerHash);

                // start playing slide sound
                PlayOneShot(slideSound);
            }
        } else {
            // Idle state
            return;
        }

        playerTransform.Translate(playerTransform.forward * speed * Time.deltaTime, Space.World);

        if (Input.GetKeyDown(KeyCode.Z)) {
            playerTransform.Rotate(new Vector3(0, -90, 0), Space.Self);
        }

        if (Input.GetKeyDown(KeyCode.X)) {
            playerTransform.Rotate(new Vector3(0, 90, 0), Space.Self);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            Vector3 nudge = -playerTransform.right * nudgeAmount;
            playerTransform.Translate(nudge, Space.World);
            collider.transform.Translate(nudge, Space.World);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            Vector3 nudge = playerTransform.right * nudgeAmount;
            playerTransform.Translate(nudge, Space.World);
            collider.transform.Translate(nudge, Space.World);
        }

        // fallen of the edge
        if (playerTransform.position.y < -1.5f) {
            director.ObstacleCollision(this.gameObject);
        }
    }

    // Jump is over can perform next jump
    void JumpOver() {
        collider.center.Set(collider.center.x, colliderY, collider.center.z);
        jumping = false;
    }

    void SlideOver() {
        collider.height = colliderHeight;
        collider.center.Set(collider.center.x, colliderY, collider.center.z);
        sliding = false;
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Pick Up")) {
            PlayOneShot(collectSound);
            other.gameObject.SetActive(false);
            director.CollectCoin();
        } else if (other.gameObject.CompareTag("Obstacle")) {
            PlayOneShot(collideSound);
            director.ObstacleCollision(this.gameObject);
        } else if (other.gameObject.CompareTag("Path End")) {
            director.ClearBlock();
        }
    }

    void PlayOneShot(AudioClip clip, float delay = 0f) {
        audioPlayer.Stop();
        audioPlayer.clip = clip;
        audioPlayer.loop = false;
        audioPlayer.PlayDelayed(delay);
    }
}
