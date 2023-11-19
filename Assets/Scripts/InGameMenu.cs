using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour
{
    public GameObject DeathScreen;
    public GameObject Info;
    public GameObject KeyBindsAndOther;
    public KeyCode pause;
    // Start is called before the first frame update
    void Start()
    {
        KeyBindsAndOther.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 9999;

        Info.GetComponent<Text>().text = ((int)(1 / Time.deltaTime)).ToString();
        if (Input.GetKeyDown(pause))
        {
            KeyBindsAndOther.SetActive(true);
            Time.timeScale = 0;
        }
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

    public void OnCloseKeyBindsAndOtherClick()
    {
        KeyBindsAndOther.SetActive(false);
        Time.timeScale = 1;
    }
}
