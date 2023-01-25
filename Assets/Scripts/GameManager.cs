using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static int gameMode;
    public static bool cheatsEnabled;
    public static bool showFps;
    public static bool showElapsedTime;
    public static bool postProcessingEnabled;
    public static bool fullscreenEnabled;
    public static int selectedResolution;

    public static bool initDone = false;

    private void Awake()
    {
        instance = this;

        FindObjectOfType<AudioManager>().JustStopSpecific("Chune");

        if (initDone == false)
        {
            initDone = true;
            selectedResolution = 0;
            GameObject.FindGameObjectWithTag("Canvas").GetComponent<MenuMechanics>().SetResolution();
            fullscreenEnabled = true;
            Screen.fullScreen = true;
            postProcessingEnabled = true;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        //cheatsEnabled = true; <-cheats test
    }

}
