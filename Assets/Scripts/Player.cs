using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public KeyCode moveUp;
    public KeyCode moveDown;
    public KeyCode moveLeft;
    public KeyCode moveRight;
    public int nextMoveSpeed;
    private int moveSpeed;
    private float diagonalMovingCoef;
    private Vector2 moveVector = new Vector2(0, 0);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(nextMoveSpeed != moveSpeed)
        {
            moveSpeed = nextMoveSpeed;
            CalculateDiagonalMovingCoef();
        }

        if (Input.GetKeyDown(moveUp))
        {
            moveVector += new Vector2(0, moveSpeed);
        }
        if (Input.GetKeyDown(moveDown))
        {
            moveVector += new Vector2(0, -moveSpeed);
        }
        if (Input.GetKeyDown(moveLeft))
        {
            moveVector += new Vector2(-moveSpeed, 0);
        }
        if (Input.GetKeyDown(moveRight))
        {
            moveVector += new Vector2(moveSpeed, 0);
        }


        if (Input.GetKeyUp(moveUp))
        {
            moveVector += new Vector2(0, -moveSpeed);
        }
        if (Input.GetKeyUp(moveDown))
        {
            moveVector += new Vector2(0, moveSpeed);
        }
        if (Input.GetKeyUp(moveLeft))
        {
            moveVector += new Vector2(moveSpeed, 0);
        }
        if (Input.GetKeyUp(moveRight))
        {
            moveVector += new Vector2(-moveSpeed, 0);
        }

        SetVelocity();
    }

    private void SetVelocity()
    {
        if(moveVector.x != 0 && moveVector.y != 0)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(moveVector.x * diagonalMovingCoef, moveVector.y * diagonalMovingCoef);
        }
        else
        {
            GetComponent<Rigidbody2D>().velocity = moveVector;
        }
    }

    private void CalculateDiagonalMovingCoef()
    {
        var a = Mathf.Sqrt(moveSpeed * moveSpeed / 2);
        diagonalMovingCoef = a / moveSpeed;
    }
}
