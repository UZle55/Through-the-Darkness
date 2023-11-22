using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageMarker : MonoBehaviour
{
    public GameObject damageText;
    public GameObject damageTextTargets;
    public Color lessThanAvgDamage;
    public Color moreThanAvgDamage;
    public Color critDamage;
    public GameObject worldCanvas;
    private bool isGoing = false;
    private GameObject target;
    private float t = 0;
    // Start is called before the first frame update
    void Start()
    {
        if(!isGoing)
            damageText.GetComponent<Text>().text = "";
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        if (isGoing)
        {
            damageText.transform.position = Vector3.Lerp(damageText.transform.position, target.transform.position, 5.0f * Time.deltaTime);
            if(Mathf.Abs(damageText.transform.position.x - target.transform.position.x) < 0.01f 
                && Mathf.Abs(damageText.transform.position.y - target.transform.position.y) < 0.01f)
            {
                isGoing = false;
                Destroy(this.gameObject);
            }
        }
    }

    public void ShowDamage(int damage, int avgDamage, bool isCrit)
    {
        var color = new Color();
        if (!isCrit)
        {
            var delta = damage - avgDamage;
            if(delta >= 0)
            {
                color = moreThanAvgDamage;
            }
            else
            {
                color = lessThanAvgDamage;
            }
        }
        else
        {
            color = critDamage;
        }
        var index = GetRandomInt(0, damageTextTargets.transform.childCount - 1);
        var damageMarker = Instantiate(this.gameObject, worldCanvas.transform);
        damageMarker.transform.position = transform.position;
        damageMarker.GetComponent<DamageMarker>().Go(damage, color, index);
    }

    public void Go(int damage, Color color, int index)
    {
        damageText.GetComponent<Text>().color = color;
        damageText.GetComponent<Text>().text = (damage * -1).ToString();
        target = damageTextTargets.transform.GetChild(index).gameObject;
        isGoing = true;
    }

    private int GetRandomInt(int from, int to)
    {
        if (from == to)
            return from;
        var rnd = (int)Random.Range((float)from, (float)(to + 1));
        rnd = (int)Random.Range((float)from, (float)(to + 1));
        rnd = (int)Random.Range((float)from, (float)(to + 1));
        if (rnd == to + 1)
        {
            rnd = to;
        }
        return rnd;
    }
}
