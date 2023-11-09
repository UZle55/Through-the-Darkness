using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon
{
    private float t = 0;
    public float attackSpeed;
    private float attackCooldown;
    public int startAngle;
    public int endAngle;
    public float aniDuration;
    private float angle;
    private int direction = 1;
    private bool isPlayingAni = false;
    public float aniDownTime = 0.2f;
    public float aniUpTime = 0.3f;
    private bool isSwordGoingDown = false;
    private bool isSwordGoingUp = false;

    public GameObject RotatingPoint;
    public GameObject RotatingAniPoint;
    // Start is called before the first frame update
    void Start()
    {
        attackCooldown = 1 / attackSpeed;
        RotatingAniPoint.transform.localEulerAngles = new Vector3(0, 0, startAngle);
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
    }

    public override void Attack()
    {
        if(t > attackCooldown)
        {
            t = 0;
            PlayAni();
        }
        else
        {

        }
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
        if(!isSwordGoingDown && t < aniDownTime)
        {
            isSwordGoingDown = true;
        }
        if (isSwordGoingDown)
        {
            var k = t / aniDownTime;
            var a = ((endAngle - startAngle) * k) + startAngle;
            RotatingAniPoint.transform.localEulerAngles = new Vector3(0, 0, a);
            if(t >= aniDownTime)
            {
                isSwordGoingUp = true;
                isSwordGoingDown = false;
            }
        }
        if (isSwordGoingUp)
        {
            var k = (t - aniDownTime) / aniUpTime;
            var a = endAngle - ((endAngle - startAngle) * k);
            RotatingAniPoint.transform.localEulerAngles = new Vector3(0, 0, a);
            if(t >= aniDownTime + aniUpTime)
            {
                isSwordGoingUp = false;
                isPlayingAni = false;
            }
        }

    }

    
}
