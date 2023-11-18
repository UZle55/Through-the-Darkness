using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour
{
    public GameObject DeathScreen;
    public GameObject Info;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Info.GetComponent<Text>().text = ((int)(1 / Time.deltaTime)).ToString();
    }

    public void OnAgainButtonClick()
    {
        SceneManager.UnloadScene("Game");
        SceneManager.LoadScene("Game");
    }

    public void ShowDeathScreen()
    {
        DeathScreen.SetActive(true);
    }
}
