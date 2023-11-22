using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flask : MonoBehaviour
{
    public Sprite[] FlaskSprites;

    public string flaskType;
    public float maxValue;
    public float currentValue;
    public float usageCost;
    public float recoverPerUsage;
    public float duration;
    private float t = 0;
    private bool isUsing = false;
    private float alreadyRecovered = 0;
    public string[] stats;
    public string flaskName;

    public GameObject IconOnScreen;
    public GameObject usageProgressBar;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        if (isUsing)
        {
            var recover = (t / duration * recoverPerUsage) - alreadyRecovered;
            if (flaskType.Equals("HP"))
            {
                transform.parent.GetComponent<Player>().HealHealth(recover);
            }
            else if (flaskType.Equals("Mana"))
            {
                transform.parent.GetComponent<Player>().ChangeMana(recover);
            }
            alreadyRecovered += recover;
            if(t >= duration)
            {
                StopUsing();
            }
        }
        if (tag.Equals("FlaskInHands"))
        {
            UpdateInfoOnScreen();
        }
    }
        

    public bool Use()
    {
        if(currentValue >= usageCost)
        {
            currentValue -= usageCost;
            isUsing = true;
            usageProgressBar.SetActive(true);
            UpdateInfoOnScreen();
            t = 0;
            alreadyRecovered = 0;
            return true;
        }
        return false;
    }

    public void StopUsing()
    {
        isUsing = false;
        t = 0;
        alreadyRecovered = 0;
    }

    public void AddCharges(float value)
    {
        currentValue += value;
        if(currentValue > maxValue)
        {
            currentValue = maxValue;
        }
    }

    public Sprite GetSprite()
    {
        var index = (int)(currentValue / maxValue * 100 / 25);
        return FlaskSprites[index];
    }

    private void UpdateInfoOnScreen()
    {
        IconOnScreen.GetComponent<Image>().sprite = GetSprite();
        if (isUsing)
        {
            usageProgressBar.GetComponent<Slider>().value = 1 - (alreadyRecovered / recoverPerUsage);
        }
        else
        {
            usageProgressBar.SetActive(false);
        }
    }

    public string GetStatsText()
    {
        var result = "";
        for (var i = 0; i < stats.Length; i++)
        {
            result += stats[i];
            if (i != stats.Length - 1)
            {
                result += "\n";
            }

        }
        return result;
    }
}
