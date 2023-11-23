using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitiesManager : MonoBehaviour
{
    public GameObject ability1DurationImage;
    public GameObject ability1ReloadImage;
    public GameObject ability1Image;

    public GameObject ability2DurationImage;
    public GameObject ability2ReloadImage;
    public GameObject ability2Image;

    public float meleeAbility1Duration;
    public float meleeAbility1Reload;
    public float meleeAbility2Duration;
    public float meleeAbility2Reload;

    private float ability1Time = 0;
    private float ability2Time = 0;
    private bool isAbility1Duration = false;
    private bool isAbility2Duration = false;
    private bool isAbility1Reload = false;
    private bool isAbility2Reload = false;
    private bool canAbility1Click = true;
    private bool canAbility2Click = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ability1Time += Time.deltaTime;
        ability2Time += Time.deltaTime;
        if (isAbility1Duration)
        {
            if(ability1Time < meleeAbility1Duration)
            {
                ability1DurationImage.GetComponent<Image>().fillAmount = (meleeAbility1Duration - ability1Time) / meleeAbility1Duration;
            }
            else
            {
                ability1Time = 0;
                isAbility1Duration = false;
                isAbility1Reload = true;
                ability1DurationImage.GetComponent<Image>().fillAmount = 0;
                ability1ReloadImage.GetComponent<Image>().fillAmount = 1;
                GetComponent<Player>().Ability1Disactivate();
            }
        }
        if (isAbility1Reload)
        {
            if (ability1Time < meleeAbility1Reload)
            {
                ability1ReloadImage.GetComponent<Image>().fillAmount = (meleeAbility1Reload - ability1Time) / meleeAbility1Reload;
            }
            else
            {
                ability1Time = 0;
                isAbility1Duration = false;
                isAbility1Reload = false;
                canAbility1Click = true;
                ability1ReloadImage.GetComponent<Image>().fillAmount = 0;
                ability1DurationImage.GetComponent<Image>().fillAmount = 1;
            }
        }


        if (isAbility2Duration)
        {
            if (ability2Time < meleeAbility2Duration)
            {
                ability2DurationImage.GetComponent<Image>().fillAmount = (meleeAbility2Duration - ability2Time) / meleeAbility2Duration;
            }
            else
            {
                ability2Time = 0;
                isAbility2Duration = false;
                isAbility2Reload = true;
                ability2DurationImage.GetComponent<Image>().fillAmount = 0;
                ability2ReloadImage.GetComponent<Image>().fillAmount = 1;
                GetComponent<Player>().Ability2Disactivate();
            }
        }
        if (isAbility2Reload)
        {
            if (ability2Time < meleeAbility2Reload)
            {
                ability2ReloadImage.GetComponent<Image>().fillAmount = (meleeAbility2Reload - ability2Time) / meleeAbility2Reload;
            }
            else
            {
                ability2Time = 0;
                isAbility2Duration = false;
                isAbility2Reload = false;
                canAbility2Click = true;
                ability2ReloadImage.GetComponent<Image>().fillAmount = 0;
                ability2DurationImage.GetComponent<Image>().fillAmount = 1;
            }
        }
    }

    public bool Ability1Click()
    {
        if (canAbility1Click)
        {
            canAbility1Click = false;
            isAbility1Duration = true;
            ability1Time = 0;
            GetComponent<Player>().Ability1Activate();
            return true;
        }
        return false;
    }

    public bool Ability2Click()
    {
        if(canAbility2Click)
        {
            canAbility2Click = false;
            isAbility2Duration = true;
            ability2Time = 0;
            GetComponent<Player>().Ability2Activate();
            return true;
        }
        return false;
        
    }
}
