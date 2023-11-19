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
    public KeyCode switchWeapon;
    public KeyCode dropWeapon;
    public KeyCode Interact;
    public KeyCode useHPFlask;
    public KeyCode useManaFlask;
    public int nextMoveSpeed;
    private int moveSpeed;
    private float diagonalMovingCoef;
    private Vector2 moveVector = new Vector2(0, 0);
    private bool isDead = false;

    public GameObject Info;
    public GameObject Rot;
    public GameObject Weapon1;
    public GameObject Weapon2;
    private GameObject WeaponInHands;
    public GameObject InGameMenu;
    public GameObject AbovePlayerText;
    public GameObject FlaskHPImage;
    public GameObject FlaskManaImage;
    public GameObject FlaskHP;
    public GameObject FlaskMana;
    public GameObject FlaskHPProgressBar;
    public GameObject FlaskManaProgressBar;
    public GameObject FlaskHPButtonIcon;
    public GameObject FlaskManaButtonIcon;

    public float HP;
    public GameObject HealthBar;
    public GameObject HealthBarText;
    private float maxHP;
    private float currHP;

    public float Mana;
    public GameObject ManaBar;
    public GameObject ManaBarText;
    private float maxMana;
    private float currMana;

    public float Exp;
    public int Lvl;
    public GameObject ExpBar;
    public GameObject ExpBarText;
    private float maxExp;
    private float currExp;

    public bool isClearingRoom = false;
    private GameObject weaponToPickUp;
    private GameObject flaskToPickUp;
    public float maxDistanceToPickUp;
    private GameObject chestToOpen;
    // Start is called before the first frame update
    void Start()
    {
        FlaskHPProgressBar.SetActive(false);
        FlaskManaProgressBar.SetActive(false);
        if(FlaskHP == null)
        {
            FlaskHPButtonIcon.SetActive(false);
        }
        if (FlaskMana == null)
        {
            FlaskManaButtonIcon.SetActive(false);
        }
        
        AbovePlayerText.GetComponent<Text>().text = "";
        WeaponInHands = Weapon1;
        maxHP = HP;
        currHP = HP;

        maxMana = Mana;
        currMana = Mana;

        maxExp = GetNextMaxExp();
        currExp = Exp;
        UpdateStatsText();
        HealHealth(0);
        ChangeMana(0);
        GetExp(0);
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
                if(currMana >= WeaponInHands.GetComponent<Weapon>().attackManaCost)
                {
                    var isAttacked = WeaponInHands.GetComponent<Weapon>().Attack();
                    if (isAttacked)
                    {
                        ChangeMana(-WeaponInHands.GetComponent<Weapon>().attackManaCost);
                    }
                }
                
            }

            if (Input.GetKeyUp(attack))
            {
                var a = 0;
            }

            if (Input.GetKeyDown(switchWeapon))
            {
                SwitchWeaponInHands();
            }

            if (Input.GetKeyDown(dropWeapon) && !isClearingRoom)
            {
                DropWeaponInHands();
            }

            if (Input.GetKeyDown(Interact) && !isClearingRoom)
            {
                if(chestToOpen != null)
                {
                    chestToOpen.GetComponent<Chest>().Open();
                }
                else if(weaponToPickUp != null)
                {
                    PickUpWeapon(weaponToPickUp);
                }
                else if(flaskToPickUp != null)
                {
                    PickUpFlask(flaskToPickUp);
                }
            }

            if (Input.GetKeyDown(useHPFlask) && FlaskHP != null)
            {
                var isUsed = FlaskHP.GetComponent<Flask>().Use();
            }
            if (Input.GetKeyDown(useManaFlask) && FlaskMana != null)
            {
                var isUsed = FlaskMana.GetComponent<Flask>().Use();
            }

            //PassiveHPRegeneration();

            PassiveManaRegeneration();

            CheckWeaponsAndFlasksOnGround();
            //UpdateStatsText();
        }
        
    }

    public void SetChestToOpen(GameObject chest)
    {
        chestToOpen = chest;
        ShowTextAbovePlayer("Сундук", -1);
    }

    public void DeleteChestToOpen()
    {
        if (AbovePlayerText.GetComponent<Text>().text.Equals("Сундук"))
        {
            StopShowingTextAbovePlayer();
        }
        chestToOpen = null;
        
    }

    private void CheckWeaponsAndFlasksOnGround()
    {
        if(chestToOpen == null)
        {
            var allWeapons = GameObject.FindGameObjectsWithTag("WeaponOnGround");
            var distanceToWeapon = -1.0f;
            if (allWeapons.Length != 0)
            {
                var minDistance = float.MaxValue;
                var minDistanceWeaponIndex = -1;
                for (var i = 0; i < allWeapons.Length; i++)
                {
                    var dis = Monster.GetDistance(allWeapons[i].GetComponent<Weapon>().position, transform.position);
                    if (dis < minDistance)
                    {
                        minDistance = dis;
                        minDistanceWeaponIndex = i;
                    }
                }
                if (minDistance <= maxDistanceToPickUp)
                {
                    distanceToWeapon = minDistance;
                    weaponToPickUp = allWeapons[minDistanceWeaponIndex];
                }
                else
                {
                    weaponToPickUp = null;
                }
            }

            var allFlasks = GameObject.FindGameObjectsWithTag("FlaskOnGround");
            var distanceToFlask = -1.0f;
            if (allFlasks.Length != 0)
            {
                var minDistance = float.MaxValue;
                var minDistanceWeaponIndex = -1;
                for (var i = 0; i < allFlasks.Length; i++)
                {
                    var dis = Monster.GetDistance(allFlasks[i].transform.position, transform.position);
                    if (dis < minDistance)
                    {
                        minDistance = dis;
                        minDistanceWeaponIndex = i;
                    }
                }
                if (minDistance <= maxDistanceToPickUp)
                {
                    distanceToFlask = minDistance;
                    flaskToPickUp = allFlasks[minDistanceWeaponIndex];
                }
                else
                {
                    flaskToPickUp = null;
                }
            }


            if(weaponToPickUp != null && flaskToPickUp != null)
            {
                if(distanceToWeapon < distanceToFlask)
                {
                    flaskToPickUp = null;
                    ShowTextAbovePlayer(weaponToPickUp.name, -1);
                }
                else if(distanceToWeapon >= distanceToFlask)
                {
                    weaponToPickUp = null;
                    ShowTextAbovePlayer(flaskToPickUp.name, -1);
                }
            }
            else if (weaponToPickUp != null && flaskToPickUp == null)
            {
                ShowTextAbovePlayer(weaponToPickUp.name, -1);
            }
            else if (weaponToPickUp == null && flaskToPickUp != null)
            {
                ShowTextAbovePlayer(flaskToPickUp.name, -1);
            }
            else
            {
                StopShowingTextAbovePlayer();
            }
        }
    }

    private void ShowTextAbovePlayer(string text, float seconds)
    {
        AbovePlayerText.GetComponent<Text>().text = text;
        if(seconds != -1)
        {
            Invoke("StopShowingTextAbovePlayer", seconds);
        }
    }

    private void StopShowingTextAbovePlayer()
    {
        AbovePlayerText.GetComponent<Text>().text = "";
    }

    private void PickUpWeapon(GameObject weapon)
    {
        if(Weapon1 == null)
        {
            weapon.transform.parent = gameObject.transform;
            Weapon1 = weapon;
            WeaponInHands = Weapon1;
            WeaponInHands.transform.localPosition = new Vector3(0, 0, -1);
            WeaponInHands.tag = "WeaponInHands";
            WeaponInHands.SetActive(true);
            if(Weapon2 != null)
            {
                Weapon2.SetActive(false);
            }
        }
        else if(Weapon2 == null)
        {
            weapon.transform.parent = gameObject.transform;
            Weapon2 = weapon;
            WeaponInHands = Weapon2;
            WeaponInHands.transform.localPosition = new Vector3(0, 0, -1);
            WeaponInHands.tag = "WeaponInHands";
            WeaponInHands.SetActive(true);
            if (Weapon1 != null)
            {
                Weapon1.SetActive(false);
            }
        }
        else
        {
            weapon.transform.parent = gameObject.transform;
            DropWeaponInHands();

            if (Weapon1 == null)
            {
                weapon.transform.parent = gameObject.transform;
                Weapon1 = weapon;
                WeaponInHands = Weapon1;
                WeaponInHands.transform.localPosition = new Vector3(0, 0, -1);
                WeaponInHands.tag = "WeaponInHands";
                WeaponInHands.SetActive(true);
                if (Weapon2 != null)
                {
                    Weapon2.SetActive(false);
                }
            }
            else if (Weapon2 == null)
            {
                weapon.transform.parent = gameObject.transform;
                Weapon2 = weapon;
                WeaponInHands = Weapon2;
                WeaponInHands.transform.localPosition = new Vector3(0, 0, -1);
                WeaponInHands.tag = "WeaponInHands";
                WeaponInHands.SetActive(true);
                if (Weapon1 != null)
                {
                    Weapon1.SetActive(false);
                }
            }
        }
        weaponToPickUp = null;
    }

    private void PickUpFlask(GameObject flask)
    {
        if (flask.GetComponent<Flask>().flaskType.Equals("HP"))
        {
            var currCharges = -1.0f;
            if(FlaskHP != null)
            {
                currCharges = FlaskHP.GetComponent<Flask>().currentValue;
                FlaskHP.transform.parent = null;
                FlaskHP.transform.position = new Vector3(FlaskHP.transform.position.x, FlaskHP.transform.position.y, -0.5f);
                FlaskHP.tag = "FlaskOnGround";
                FlaskHP.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                FlaskHP.GetComponent<Flask>().StopUsing();
            }
            FlaskHP = flask;
            FlaskHP.transform.parent = transform;
            FlaskHP.GetComponent<Flask>().IconOnScreen = FlaskHPImage;
            FlaskHP.GetComponent<Flask>().usageProgressBar = FlaskHPProgressBar;
            FlaskHP.tag = "FlaskInHands";
            FlaskHP.transform.localPosition = new Vector3(0, 0, 0);
            FlaskHP.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            FlaskHP.GetComponent<Flask>().currentValue = currCharges;
            if (currCharges == -1.0f)
            {
                FlaskHP.GetComponent<Flask>().AddCharges(999);
            }
            FlaskHPImage.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            flaskToPickUp = null;
            FlaskHPButtonIcon.SetActive(true);

        }
        else if (flask.GetComponent<Flask>().flaskType.Equals("Mana"))
        {
            var currCharges = -1.0f;
            if (FlaskMana != null)
            {
                currCharges = FlaskMana.GetComponent<Flask>().currentValue;
                FlaskMana.transform.parent = null;
                FlaskMana.transform.position = new Vector3(FlaskMana.transform.position.x, FlaskMana.transform.position.y, -0.5f);
                FlaskMana.tag = "FlaskOnGround";
                FlaskMana.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                FlaskMana.GetComponent<Flask>().StopUsing();
            }
            FlaskMana = flask;
            FlaskMana.transform.parent = transform;
            FlaskMana.GetComponent<Flask>().IconOnScreen = FlaskManaImage;
            FlaskMana.GetComponent<Flask>().usageProgressBar = FlaskManaProgressBar;
            FlaskMana.tag = "FlaskInHands";
            FlaskMana.transform.localPosition = new Vector3(0, 0, 0);
            FlaskMana.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            FlaskMana.GetComponent<Flask>().currentValue = currCharges;
            if (currCharges == -1.0f)
            {
                FlaskMana.GetComponent<Flask>().AddCharges(999);
            }
            FlaskManaImage.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            flaskToPickUp = null;
            FlaskManaButtonIcon.SetActive(true);
        }
    }

    private void SwitchWeaponInHands()
    {
        if (Weapon1 != null && Weapon2 != null)
        {
            if (WeaponInHands.Equals(Weapon1))
            {
                Weapon1.SetActive(false);
                WeaponInHands = Weapon2;
                Weapon2.SetActive(true);
            }
            else if (WeaponInHands.Equals(Weapon2))
            {
                Weapon2.SetActive(false);
                WeaponInHands = Weapon1;
                Weapon1.SetActive(true);
            }
        }
        
    }

    private void DropWeaponInHands()
    {
        if(Weapon1 != null && Weapon2 != null)
        {
            if (WeaponInHands.name.Equals(Weapon1.name))
            {
                Weapon1.transform.parent = null;
                Weapon1.transform.position = new Vector3(Weapon1.transform.position.x, Weapon1.transform.position.y, -0.5f);
                Weapon1.tag = "WeaponOnGround";
                Weapon1 = null;
                WeaponInHands = Weapon2;
                Weapon2.SetActive(true);
            }
            else if (WeaponInHands.name.Equals(Weapon2.name))
            {
                Weapon2.transform.parent = null;
                Weapon2.transform.position = new Vector3(Weapon2.transform.position.x, Weapon2.transform.position.y, -0.5f);
                Weapon2.tag = "WeaponOnGround";
                Weapon2 = null;
                WeaponInHands = Weapon1;
                Weapon1.SetActive(true);
            }
        }
        
    }

    private void UpdateStatsText()
    {
        HealthBarText.GetComponent<Text>().text = (int)currHP + "/" + (int)maxHP;
        ManaBarText.GetComponent<Text>().text = (int)currMana + "/" + (int)maxMana;
        ExpBarText.GetComponent<Text>().text = "lvl " + Lvl;
    }

    private void PassiveHPRegeneration()
    {
        HealHealth(Time.deltaTime * 2.5f); 
    }

    private void PassiveManaRegeneration()
    {
        ChangeMana(Time.deltaTime * 1f);
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
        UpdateStatsText();
    }

    public void HealHealth(float health)
    {
        currHP += health;
        if (currHP >= maxHP)
        {
            currHP = maxHP;
        }
        var value = currHP / maxHP;
        HealthBar.GetComponent<Slider>().value = value;
        UpdateStatsText();
    }

    public void Die()
    {
        isDead = true;
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        InGameMenu.GetComponent<InGameMenu>().ShowDeathScreen();
    }

    public void ChangeMana(float value)
    {
        currMana += value;
        if (currMana <= 0)
        {
            currMana = 0;
        }
        if (currMana >= maxMana)
        {
            currMana = maxMana;
        }
        var val = currMana / maxMana;
        ManaBar.GetComponent<Slider>().value = val;
        UpdateStatsText();
    }

    public void GetExp(float value)
    {
        currExp += value;
        while(currExp >= maxExp)
        {
            currExp -= maxExp;
            Lvl += 1;
            maxExp = GetNextMaxExp();
            
        }
        var val = currExp / maxExp;
        ExpBar.GetComponent<Slider>().value = val;
        UpdateStatsText();
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
        WeaponInHands.GetComponent<Weapon>().SetAngle(angle);
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

    private float GetNextMaxExp()
    {
        return Lvl * 5 + 10;
    }

    public void AddCharges(float value)
    {
        var toHP = 0.0f;
        if (FlaskHP != null)
        {
            if((FlaskHP.GetComponent<Flask>().maxValue - FlaskHP.GetComponent<Flask>().currentValue) >= value / 2)
            {
                toHP = value / 2;
                if(FlaskMana == null)
                {
                    toHP = value;
                }
            }
            else
            {
                toHP = FlaskHP.GetComponent<Flask>().maxValue - FlaskHP.GetComponent<Flask>().currentValue;
            }
            FlaskHP.GetComponent<Flask>().AddCharges(toHP);
        }
        if (FlaskMana != null)
        {
            FlaskMana.GetComponent<Flask>().AddCharges(value - toHP);
        }
    }

    private void CalculateDiagonalMovingCoef()
    {
        var a = Mathf.Sqrt(moveSpeed * moveSpeed / 2);
        diagonalMovingCoef = a / moveSpeed;
    }
}
