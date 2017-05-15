using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class theStackController : MonoBehaviour {

    private const float BOUNDS_SIZE = 3.5f;
    private const float STACK_MOVING_SPEED = 5.0f;
    private const float ERROR_MARGIN = 0.2f;
    private Vector2 stackBounds = new Vector2(BOUNDS_SIZE,BOUNDS_SIZE);
    private GameObject[] theStack;
	private int ScoreCount=0;
	private int stackIndex;
    private float tileTransition = 0.0f;
    private float tileSpeed = 2.5f;
    private bool isMovingOnX = true;
    private float secondPosition;
    private Vector3 DesiredPosition;
    private Vector3 LastTilePosition;
    private int combo = 0;
    private bool gameOver = false;
	// Use this for initialization
	private void Start () {
		
		theStack = new GameObject[transform.childCount];
		for(int i=0; i<transform.childCount; i++){
			theStack [i] = transform.GetChild (i).gameObject;
		}
		stackIndex = transform.childCount - 1;
	}
	
	// Update is called once per frame
	private void Update () {
		if(Input.GetMouseButtonDown (0)){
            if (placeTile()) {
                spawnTile();
                ScoreCount++;
            } else {
                EndGame();
            }
		}
        MoveTile();
        transform.position = Vector3.Lerp(transform.position, DesiredPosition, STACK_MOVING_SPEED * Time.deltaTime);
	}

    private void MoveTile()
    {
        if (gameOver) {
            return;
        }
        tileTransition += Time.deltaTime * tileSpeed;
        if (isMovingOnX) {
            theStack[stackIndex].transform.localPosition = new Vector3(Mathf.Sin(tileTransition) * BOUNDS_SIZE, ScoreCount, secondPosition);
        } else {
            theStack[stackIndex].transform.localPosition = new Vector3(secondPosition, ScoreCount, Mathf.Sin(tileTransition) * BOUNDS_SIZE);
        }
    }

    private void EndGame()
    {
        gameOver = true;
        theStack[stackIndex].AddComponent<Rigidbody>();
    }

    private void spawnTile()
    {
        LastTilePosition = theStack[stackIndex].transform.localPosition;
        stackIndex--;
        if (stackIndex < 0)
        {
            stackIndex = transform.childCount - 1;
        }
        DesiredPosition = Vector3.down * ScoreCount;
        theStack[stackIndex].transform.localPosition = new Vector3(0f, ScoreCount, 0f);
        theStack[stackIndex].transform.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

    }

	private bool placeTile(){
        Transform t = theStack[stackIndex].transform;
        if (isMovingOnX) {
            float deltaX = LastTilePosition.x - t.position.x;
            if (Mathf.Abs(deltaX) > ERROR_MARGIN) {
                combo = 0;
                stackBounds.x -= Mathf.Abs(deltaX);
                if (stackBounds.x <= 0) {
                    return false;
                }
                float middle = (LastTilePosition.x + t.localPosition.x) / 2;
                t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                t.localPosition = new Vector3(middle - (LastTilePosition.x / 2), ScoreCount, LastTilePosition.z);
            } else {
                print("Hello");
                combo++;
                t.localPosition = new Vector3(LastTilePosition.x,ScoreCount,LastTilePosition.z);
            }
        } else {
            float deltaZ = LastTilePosition.z - t.position.z;
            if (Mathf.Abs(deltaZ) > ERROR_MARGIN) {
                combo = 0;
                stackBounds.y -= Mathf.Abs(deltaZ);
                if (stackBounds.y <= 0) {
                    return false;
                }
                float middle = (LastTilePosition.z + t.localPosition.z) / 2;
                t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                t.localPosition = new Vector3(LastTilePosition.x, ScoreCount, middle - (LastTilePosition.z / 2));
            } else {
                print("Hello");
                combo++;
                t.localPosition = new Vector3(LastTilePosition.x, ScoreCount, LastTilePosition.z);
            }
        }
        secondPosition = (isMovingOnX)
            ? t.localPosition.x
            : t.localPosition.z;

        isMovingOnX = !isMovingOnX;
        return true;
	}
}
