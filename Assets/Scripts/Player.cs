using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public KeyCode moveUp;
    public KeyCode moveDown;
    public KeyCode moveLeft;
    public KeyCode moveRight;
    public KeyCode attack;
    public int nextMoveSpeed;
    private int moveSpeed;
    private float diagonalMovingCoef;
    private Vector2 moveVector = new Vector2(0, 0);
    private bool isDead = false;

    public GameObject Info;
    public GameObject Rot;
    public GameObject Weapon;
    public GameObject InGameMenu;

    public float HP;
    //public GameObject HealthBar;
    public GameObject HealthBar;
    private float maxHP;
    private float currHP;
    // Start is called before the first frame update
    void Start()
    {
        maxHP = HP;
        currHP = HP;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            if (nextMoveSpeed != moveSpeed)
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

            RotateRot();

            if (Input.GetKeyDown(attack))
            {
                Weapon.GetComponent<Weapon>().Attack();
            }

            if (Input.GetKeyUp(attack))
            {
                var a = 0;
            }

            PassiveRegeneration();
        }
        
    }

    private void PassiveRegeneration()
    {
        currHP += Time.deltaTime * 2.5f;
        if (currHP >= maxHP)
        {
            currHP = maxHP;
        }
        var value = currHP / maxHP;
        HealthBar.GetComponent<Slider>().value = value;
    }

    public void GetDamage(float damage)
    {
        currHP -= damage;
        if (currHP <= 0)
        {
            Die();
        }
        else
        {
            var value = currHP / maxHP;
            HealthBar.GetComponent<Slider>().value = value;
        }
    }

    public void Die()
    {
        isDead = true;
        InGameMenu.GetComponent<InGameMenu>().ShowDeathScreen();
    }

    private void RotateRot()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var x1 = mousePos.x;
        var y1 = mousePos.y;
        var x2 = transform.position.x;
        var y2 = transform.position.y;
        var len = Mathf.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
        var dx = x2 - x1;
        var dy = y2 - y1;
        var arcsin = Mathf.Asin(dx / len);
        var angle = arcsin * 180 / Mathf.PI;
        if (angle > 0)
        {
            if (dy > 0)
            {
                angle = 180 - angle;
            }
        }
        else
        {
            if (dy > 0)
            {
                angle = -180 - angle;
            }
        }
        Rot.transform.localEulerAngles = new Vector3(0, 0, angle);
        Weapon.GetComponent<Weapon>().SetAngle(angle);
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
