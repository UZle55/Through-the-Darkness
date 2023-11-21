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

    public GameObject info;
    public GameObject rot;
    public GameObject weapon1;
    public GameObject weapon2;
    private GameObject weaponInHands;
    public GameObject inGameMenu;
    public GameObject abovePlayerText;
    public GameObject flaskHPImage;
    public GameObject flaskManaImage;
    public GameObject flaskHP;
    public GameObject flaskMana;
    public GameObject flaskHPProgressBar;
    public GameObject flaskManaProgressBar;
    public GameObject flaskHPButtonIcon;
    public GameObject flaskManaButtonIcon;
    public GameObject levelsGenerator;
    public GameObject coinsCountText;
    private int coinsCount;
    public GameObject miniMap;

    public float hp;
    public GameObject healthBar;
    public GameObject healthBarText;
    private float maxHP;
    private float currHP;

    public float mana;
    public GameObject manaBar;
    public GameObject manaBarText;
    private float maxMana;
    private float currMana;

    public float exp;
    public int lvl;
    public GameObject expBar;
    public GameObject expBarText;
    private float maxExp;
    private float currExp;

    public bool isClearingRoom = false;
    private GameObject weaponToPickUp;
    private GameObject flaskToPickUp;
    public float maxDistanceToPickUp;
    private GameObject chestToOpen;
    private GameObject portal;
    private bool canClickOnPortal = false;
    // Start is called before the first frame update
    void Start()
    {
        flaskHPProgressBar.SetActive(false);
        flaskManaProgressBar.SetActive(false);
        if(flaskHP == null)
        {
            flaskHPButtonIcon.SetActive(false);
        }
        if (flaskMana == null)
        {
            flaskManaButtonIcon.SetActive(false);
        }
        
        abovePlayerText.GetComponent<Text>().text = "";
        weaponInHands = weapon1;
        maxHP = hp;
        currHP = hp;

        maxMana = mana;
        currMana = mana;

        maxExp = GetNextMaxExp();
        currExp = exp;
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
                if(currMana >= weaponInHands.GetComponent<Weapon>().attackManaCost)
                {
                    var isAttacked = weaponInHands.GetComponent<Weapon>().Attack();
                    if (isAttacked)
                    {
                        ChangeMana(-weaponInHands.GetComponent<Weapon>().attackManaCost);
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
                if(portal != null && canClickOnPortal)
                {
                    portal.GetComponent<Portal>().OnPortalClick();
                }
                else if(chestToOpen != null)
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

            if (Input.GetKeyDown(useHPFlask) && flaskHP != null)
            {
                var isUsed = flaskHP.GetComponent<Flask>().Use();
            }
            if (Input.GetKeyDown(useManaFlask) && flaskMana != null)
            {
                var isUsed = flaskMana.GetComponent<Flask>().Use();
            }

            //PassiveHPRegeneration();

            PassiveManaRegeneration();

            CheckWeaponsAndFlasksOnGround();

            UpdateCoinsCount();
            //UpdateStatsText();
        }
        
    }

    private void UpdateCoinsCount()
    {
        coinsCountText.GetComponent<Text>().text = coinsCount.ToString();
    }

    public int GetCoinsCount()
    {
        return coinsCount;
    }

    public void ChangeCoinsCount(int value)
    {
        coinsCount += value;
    }

    public void SetChestToOpen(GameObject chest)
    {
        chestToOpen = chest;
        ShowTextAbovePlayer("Сундук", -1);
    }

    public void DeleteChestToOpen()
    {
        if (abovePlayerText.GetComponent<Text>().text.Equals("Сундук"))
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
            if(portal != null)
            {
                var distanceToPortal = Monster.GetDistance(portal.transform.position, transform.position);
                if(weaponToPickUp != null && flaskToPickUp != null)
                {
                    if(distanceToWeapon < distanceToPortal && distanceToWeapon < distanceToFlask)
                    {
                        flaskToPickUp = null;
                        canClickOnPortal = false;
                        ShowTextAbovePlayer(weaponToPickUp.name, -1);
                    }
                    else if (distanceToFlask < distanceToPortal && distanceToFlask < distanceToWeapon)
                    {
                        weaponToPickUp = null;
                        canClickOnPortal = false;
                        ShowTextAbovePlayer(flaskToPickUp.name, -1);
                    }
                    else if(distanceToPortal <= distanceToWeapon && distanceToPortal <= distanceToFlask)
                    {
                        weaponToPickUp = null;
                        flaskToPickUp = null;
                        canClickOnPortal = true;
                        ShowTextAbovePlayer(portal.name, -1);
                    }
                }
                else if (weaponToPickUp != null && flaskToPickUp == null)
                {
                    if (distanceToWeapon < distanceToPortal)
                    {
                        canClickOnPortal = false;
                        ShowTextAbovePlayer(weaponToPickUp.name, -1);
                    }
                    else
                    {
                        weaponToPickUp = null;
                        canClickOnPortal = true;
                        ShowTextAbovePlayer(portal.name, -1);
                    }
                }
                else if (weaponToPickUp == null && flaskToPickUp != null)
                {
                    if (distanceToFlask < distanceToPortal)
                    {
                        canClickOnPortal = false;
                        ShowTextAbovePlayer(flaskToPickUp.name, -1);
                    }
                    else
                    {
                        flaskToPickUp = null;
                        canClickOnPortal = true;
                        ShowTextAbovePlayer(portal.name, -1);
                    }
                }
                else
                {
                    canClickOnPortal = true;
                    ShowTextAbovePlayer(portal.name, -1);
                }
            }
            else
            {
                if (weaponToPickUp != null && flaskToPickUp != null)
                {
                    if (distanceToWeapon < distanceToFlask)
                    {
                        flaskToPickUp = null;
                        ShowTextAbovePlayer(weaponToPickUp.name, -1);
                    }
                    else if (distanceToWeapon >= distanceToFlask)
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
    }

    private void ShowTextAbovePlayer(string text, float seconds)
    {
        abovePlayerText.GetComponent<Text>().text = text;
        if(seconds != -1)
        {
            Invoke("StopShowingTextAbovePlayer", seconds);
        }
    }

    private void StopShowingTextAbovePlayer()
    {
        abovePlayerText.GetComponent<Text>().text = "";
    }

    private void PickUpWeapon(GameObject weapon)
    {
        if(weapon1 == null)
        {
            weapon.transform.parent = gameObject.transform;
            weapon1 = weapon;
            weaponInHands = weapon1;
            weaponInHands.transform.localPosition = new Vector3(0, 0, -1);
            weaponInHands.tag = "WeaponInHands";
            weaponInHands.SetActive(true);
            if(weapon2 != null)
            {
                weapon2.SetActive(false);
            }
        }
        else if(weapon2 == null)
        {
            weapon.transform.parent = gameObject.transform;
            weapon2 = weapon;
            weaponInHands = weapon2;
            weaponInHands.transform.localPosition = new Vector3(0, 0, -1);
            weaponInHands.tag = "WeaponInHands";
            weaponInHands.SetActive(true);
            if (weapon1 != null)
            {
                weapon1.SetActive(false);
            }
        }
        else
        {
            weapon.transform.parent = gameObject.transform;
            DropWeaponInHands();

            if (weapon1 == null)
            {
                weapon.transform.parent = gameObject.transform;
                weapon1 = weapon;
                weaponInHands = weapon1;
                weaponInHands.transform.localPosition = new Vector3(0, 0, -1);
                weaponInHands.tag = "WeaponInHands";
                weaponInHands.SetActive(true);
                if (weapon2 != null)
                {
                    weapon2.SetActive(false);
                }
            }
            else if (weapon2 == null)
            {
                weapon.transform.parent = gameObject.transform;
                weapon2 = weapon;
                weaponInHands = weapon2;
                weaponInHands.transform.localPosition = new Vector3(0, 0, -1);
                weaponInHands.tag = "WeaponInHands";
                weaponInHands.SetActive(true);
                if (weapon1 != null)
                {
                    weapon1.SetActive(false);
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
            if(flaskHP != null)
            {
                currCharges = flaskHP.GetComponent<Flask>().currentValue;
                flaskHP.transform.parent = levelsGenerator.GetComponent<LevelsGenerator>().GetCurrentFloorParent().transform;
                flaskHP.transform.position = new Vector3(flaskHP.transform.position.x, flaskHP.transform.position.y, -0.5f);
                flaskHP.tag = "FlaskOnGround";
                flaskHP.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                flaskHP.GetComponent<Flask>().StopUsing();
            }
            flaskHP = flask;
            flaskHP.transform.parent = transform;
            flaskHP.GetComponent<Flask>().IconOnScreen = flaskHPImage;
            flaskHP.GetComponent<Flask>().usageProgressBar = flaskHPProgressBar;
            flaskHP.tag = "FlaskInHands";
            flaskHP.transform.localPosition = new Vector3(0, 0, 0);
            flaskHP.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            flaskHP.GetComponent<Flask>().currentValue = currCharges;
            if (currCharges == -1.0f)
            {
                flaskHP.GetComponent<Flask>().AddCharges(999);
            }
            flaskHPImage.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            flaskToPickUp = null;
            flaskHPButtonIcon.SetActive(true);

        }
        else if (flask.GetComponent<Flask>().flaskType.Equals("Mana"))
        {
            var currCharges = -1.0f;
            if (flaskMana != null)
            {
                currCharges = flaskMana.GetComponent<Flask>().currentValue;
                flaskMana.transform.parent = levelsGenerator.GetComponent<LevelsGenerator>().GetCurrentFloorParent().transform;
                flaskMana.transform.position = new Vector3(flaskMana.transform.position.x, flaskMana.transform.position.y, -0.5f);
                flaskMana.tag = "FlaskOnGround";
                flaskMana.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                flaskMana.GetComponent<Flask>().StopUsing();
            }
            flaskMana = flask;
            flaskMana.transform.parent = transform;
            flaskMana.GetComponent<Flask>().IconOnScreen = flaskManaImage;
            flaskMana.GetComponent<Flask>().usageProgressBar = flaskManaProgressBar;
            flaskMana.tag = "FlaskInHands";
            flaskMana.transform.localPosition = new Vector3(0, 0, 0);
            flaskMana.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            flaskMana.GetComponent<Flask>().currentValue = currCharges;
            if (currCharges == -1.0f)
            {
                flaskMana.GetComponent<Flask>().AddCharges(999);
            }
            flaskManaImage.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            flaskToPickUp = null;
            flaskManaButtonIcon.SetActive(true);
        }
    }

    private void SwitchWeaponInHands()
    {
        if (weapon1 != null && weapon2 != null)
        {
            if (weaponInHands.Equals(weapon1))
            {
                weapon1.SetActive(false);
                weaponInHands = weapon2;
                weapon2.SetActive(true);
            }
            else if (weaponInHands.Equals(weapon2))
            {
                weapon2.SetActive(false);
                weaponInHands = weapon1;
                weapon1.SetActive(true);
            }
        }
        
    }

    private void DropWeaponInHands()
    {
        if(weapon1 != null && weapon2 != null)
        {
            if (weaponInHands.name.Equals(weapon1.name))
            {
                weapon1.transform.parent = levelsGenerator.GetComponent<LevelsGenerator>().GetCurrentFloorParent().transform;
                weapon1.transform.position = new Vector3(weapon1.transform.position.x, weapon1.transform.position.y, -0.5f);
                weapon1.tag = "WeaponOnGround";
                weapon1 = null;
                weaponInHands = weapon2;
                weapon2.SetActive(true);
            }
            else if (weaponInHands.name.Equals(weapon2.name))
            {
                weapon2.transform.parent = levelsGenerator.GetComponent<LevelsGenerator>().GetCurrentFloorParent().transform;
                weapon2.transform.position = new Vector3(weapon2.transform.position.x, weapon2.transform.position.y, -0.5f);
                weapon2.tag = "WeaponOnGround";
                weapon2 = null;
                weaponInHands = weapon1;
                weapon1.SetActive(true);
            }
        }
        
    }

    public void TouchedPortal(GameObject portal)
    {
        this.portal = portal;
    }

    public void StoppedTouchingPortal()
    {
        portal = null;
    }

    private void UpdateStatsText()
    {
        healthBarText.GetComponent<Text>().text = (int)currHP + "/" + (int)maxHP;
        manaBarText.GetComponent<Text>().text = (int)currMana + "/" + (int)maxMana;
        expBarText.GetComponent<Text>().text = "lvl " + lvl;
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
            healthBar.GetComponent<Slider>().value = value;
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
        healthBar.GetComponent<Slider>().value = value;
        UpdateStatsText();
    }

    public void Die()
    {
        isDead = true;
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        inGameMenu.GetComponent<InGameMenu>().ShowDeathScreen();
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
        manaBar.GetComponent<Slider>().value = val;
        UpdateStatsText();
    }

    public void GetExp(float value)
    {
        currExp += value;
        while(currExp >= maxExp)
        {
            currExp -= maxExp;
            lvl += 1;
            maxExp = GetNextMaxExp();
            
        }
        var val = currExp / maxExp;
        expBar.GetComponent<Slider>().value = val;
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
        rot.transform.localEulerAngles = new Vector3(0, 0, angle);
        weaponInHands.GetComponent<Weapon>().SetAngle(angle);
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
        return lvl * 5 + 10;
    }

    public void AddCharges(float value)
    {
        var toHP = 0.0f;
        if (flaskHP != null)
        {
            if((flaskHP.GetComponent<Flask>().maxValue - flaskHP.GetComponent<Flask>().currentValue) >= value / 2)
            {
                toHP = value / 2;
                if(flaskMana == null)
                {
                    toHP = value;
                }
            }
            else
            {
                toHP = flaskHP.GetComponent<Flask>().maxValue - flaskHP.GetComponent<Flask>().currentValue;
            }
            flaskHP.GetComponent<Flask>().AddCharges(toHP);
        }
        if (flaskMana != null)
        {
            flaskMana.GetComponent<Flask>().AddCharges(value - toHP);
        }
    }

    private void CalculateDiagonalMovingCoef()
    {
        var a = Mathf.Sqrt(moveSpeed * moveSpeed / 2);
        diagonalMovingCoef = a / moveSpeed;
    }
}
