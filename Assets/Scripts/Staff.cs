using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staff : Weapon
{
    private float t = 0;
    private float attackCooldown;
    public int startAngle;
    public int endAngle;
    private float aniDuration;
    private float angle;
    private int direction = 1;
    private bool isPlayingAni = false;
    public float aniDownTime = 0.2f;
    public float aniUpTime = 0.3f;
    private bool isStaffGoingDown = false;
    private bool isStaffGoingUp = false;
    public float magicSphereVelocity = 1;
    public float maxDistance = 1;
    public GameObject staffSprite;
    public bool isTriple = false;
    //private List<GameObject> touchingEnemies = new List<GameObject>();

    public GameObject RotatingPoint;
    public GameObject RotatingAniPoint;
    public GameObject MagicSphereStartPoint;
    public GameObject MagicSphere;
    public GameObject mainSpherePoint;
    public GameObject secondarySpherePoint1;
    public GameObject secondarySpherePoint2;
    // Start is called before the first frame update
    void Start()
    {
        aniDuration = aniDownTime + aniUpTime;
        attackCooldown = 1 / attackSpeed;
        RotatingAniPoint.transform.localEulerAngles = new Vector3(0, 0, startAngle);

        //touchingEnemies = SwordTrigger.GetComponent<SwordTrigger>().GetList();

    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;

        RotateWeapon();

        if (isPlayingAni)
        {
            PlayAni();
        }

        if (!isMonsterWeapon)
        {
            position = staffSprite.transform.position;
        }
        

    }

    public override bool Attack()
    {
        if (t > attackCooldown)
        {
            t = 0;
            PlayAni();
            return true;
        }
        return false;
    }

    public override void SetAngle(float angle)
    {
        if (!isPlayingAni)
        {
            this.angle = angle;
            if (angle < 0 && direction == -1)
            {
                direction = 1;
                transform.localScale = new Vector3(1, 1, 1);
            }
            if (angle > 0 && direction == 1)
            {
                direction = -1;
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }

    }

    private void RotateWeapon()
    {
        RotatingPoint.transform.localEulerAngles = new Vector3(0, 0, angle * direction);

        //RotatingPoint.transform.localEulerAngles = new Vector3(0, 0, angle);

    }

    private void PlayAni()
    {
        if (!isPlayingAni)
        {
            isPlayingAni = true;
        }
        if (!isStaffGoingDown && t < aniDownTime)
        {
            isStaffGoingDown = true;
        }
        if (isStaffGoingDown)
        {
            var k = t / aniDownTime;
            var a = ((endAngle - startAngle) * k) + startAngle;
            RotatingAniPoint.transform.localEulerAngles = new Vector3(0, 0, a);
            if (t >= aniDownTime)
            {
                isStaffGoingUp = true;
                isStaffGoingDown = false;
                var isCrit = GetRandomInt(1, 100) <= criticalChance;
                if (!isTriple)
                {
                    var sphere1 = Instantiate(MagicSphere, MagicSphereStartPoint.transform.position, new Quaternion());
                    var dir1 = (mainSpherePoint.transform.position - transform.parent.position) / 5;
                    sphere1.GetComponent<MagicSphere>().GoInDirection(dir1, isMonsterWeapon, magicSphereVelocity, damage, maxDistance, isCrit, criticalMultiplayer);
                }
                else if (isTriple)
                {
                    var sphere1 = Instantiate(MagicSphere, MagicSphereStartPoint.transform.position, new Quaternion());
                    var sphere2 = Instantiate(MagicSphere, MagicSphereStartPoint.transform.position, new Quaternion());
                    var sphere3 = Instantiate(MagicSphere, MagicSphereStartPoint.transform.position, new Quaternion());
                    var dir1 = (mainSpherePoint.transform.position - transform.parent.position) / 5;
                    var dir2 = (secondarySpherePoint1.transform.position - transform.parent.position) / 5;
                    var dir3 = (secondarySpherePoint2.transform.position - transform.parent.position) / 5;
                    sphere1.GetComponent<MagicSphere>().GoInDirection(dir1, isMonsterWeapon, magicSphereVelocity, damage, maxDistance, isCrit, criticalMultiplayer);
                    sphere2.GetComponent<MagicSphere>().GoInDirection(dir2, isMonsterWeapon, magicSphereVelocity, damage, maxDistance, isCrit, criticalMultiplayer);
                    sphere3.GetComponent<MagicSphere>().GoInDirection(dir3, isMonsterWeapon, magicSphereVelocity, damage, maxDistance, isCrit, criticalMultiplayer);
                }
            }
        }
        if (isStaffGoingUp)
        {
            var k = (t - aniDownTime) / aniUpTime;
            var a = endAngle - ((endAngle - startAngle) * k);
            RotatingAniPoint.transform.localEulerAngles = new Vector3(0, 0, a);
            if (t >= aniDownTime + aniUpTime)
            {
                isStaffGoingUp = false;
                isPlayingAni = false;
            }
        }

    }
}
