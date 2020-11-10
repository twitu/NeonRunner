using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPrefab : MonoBehaviour
{
    private Director generator;
    public Direction blockDirection;
    public GameObject blockObject;
    public int[] obstacleMap;
    public ObjectType[] pathObjects = new ObjectType[30];
    public int lastCoin;

    void Start() {
        generator = (Director) FindObjectOfType<Director>();
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            gameObject.GetComponent<BoxCollider>().enabled = false;
            generator.ClearBlock();
        }
    }
}
