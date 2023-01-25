using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuMechanics : MonoBehaviour
{
    public GameObject shopUI;
    public GameObject pauseUI;
    public GameObject gameUI;

    public GameObject xpUI;
    public GameObject coinUI;
    public GameObject levelUI;
    public GameObject healthUI;
    public GameObject shopButtonUI;
    public GameObject pauseButtonUI;
    public GameObject goalUI;
    public GameObject timerUI;

    public GameObject victoryUI;
    public GameObject loseUI;

    public GameObject Player;
    public GameObject tileParent;
    public GameObject cheatUI;

    public GameObject timerTextLose;
    public GameObject timerTextWin;

    public GameObject mainMenuUI;
    public GameObject settingsUI;
    public GameObject newGameUI;

    public GameObject timerUIText;

    public GameObject fpsTextUI;
    public GameObject fpsTextUIMenu;

    public GameObject currentSkinText;
    public GameObject codeInputField;

    public List<ResItem> resolutions = new List<ResItem>();
    public GameObject resTextObject;

    public bool pauseEnabled;

    public bool theGameHasEnded;

    public Light mainLT;

    public bool wentBackToGameAtleastOnce;

    private GameObject player;

    public bool shopEnabled; //

    public bool gamePaused = false;

    public bool inShop = false;

    private List<GameObject> upgradeCards = new List<GameObject>();
    public List<GameObject> upgradeCardsUpgradable = new List<GameObject>();

    public GameObject IshowSpeed;

    public bool showUpgradeHint;
    public bool shownUpgradeHint;

    public bool musicEnabled;

    private bool masterPitchChanging;
    private float desiredMasterPitch;
    private float masterPitchChangeLerpTime;

    private bool musicPitchChanging;
    private float desiredMusicPitch;
    private float musicPitchChangeLerpTime;

    [SerializeField] private GameObject menuCamera;

    //public GameObject upgradeCard1;

    public AudioMixerGroup tileAudioMixer;
    public AudioMixerGroup musicMixer;

    [SerializeField] private GameObject boombox;

    void Start()
    {
        pauseEnabled = true;

        masterPitchChanging = false;
        musicPitchChanging = false;

        ChangeMasterPitch(1.0f, true, 0.0f);
        ChangeMusicPitch(1.0f, true, 0.0f);
        EnableMusic();

        musicEnabled = true;

        wentBackToGameAtleastOnce = false;

        //shopUI.SetActive(true); normaal...
        FindObjectOfType<AudioManager>().Play("ThemeSong");
        FindObjectOfType<AudioManager>().Play("BirdsAmbience");
        FindObjectOfType<AudioManager>().Play("WindAmbience");

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            menuCamera = GameObject.FindGameObjectWithTag("MainCamera");
            tileAudioMixer.audioMixer.SetFloat("TileVolume", 0.0f);
            return;
        }

        if (GameManager.showFps)
        {
            fpsTextUI.SetActive(true);
        }

        pauseEnabled = false;

        Invoke("EnablePause", 7.0f);

        loseUI.SetActive(false);
        victoryUI.SetActive(false);

        if (tileParent.GetComponent<DialogueMechanics>().disableIntro == false) {
            xpUI.SetActive(false);
            coinUI.SetActive(false);
            levelUI.SetActive(false);
            healthUI.SetActive(false);
            shopButtonUI.SetActive(false);
            pauseButtonUI.SetActive(false);
            timerUI.SetActive(false);
        }

        foreach (GameObject card in GameObject.FindGameObjectsWithTag("UpgradeCard"))
        {
            upgradeCards.Add(card);
        }

        player = GameObject.FindGameObjectWithTag("Player");

        if (player.GetComponent<Player>().cheatsEnabled)
            cheatUI.SetActive(true);
        else
            cheatUI.SetActive(false);

        //shopUI.SetActive(false); normaal...
        shopUI.transform.parent = transform.parent;
    }

    void Update()
    {
        if (masterPitchChanging)
        {
            tileAudioMixer.audioMixer.GetFloat("MasterPitch", out float currentMasterPitch);
            tileAudioMixer.audioMixer.SetFloat("MasterPitch", Mathf.Lerp(currentMasterPitch, desiredMasterPitch, Time.deltaTime * masterPitchChangeLerpTime));
        }

        if (musicPitchChanging)
        {
            tileAudioMixer.audioMixer.GetFloat("MusicPitch", out float currentMusicPitch);
            tileAudioMixer.audioMixer.SetFloat("MusicPitch", Mathf.Lerp(currentMusicPitch, desiredMusicPitch, Time.deltaTime * musicPitchChangeLerpTime));
        }


        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            fpsTextUIMenu.SetActive(GameManager.showFps);
            return;
        }

        if (GameManager.showElapsedTime && timerUI != null && timerUI.activeInHierarchy == true)
        {
            timerUIText.GetComponent<Text>().text = tileParent.GetComponent<LevelMechanics>().timer.Elapsed.ToString("mm\\:ss\\.ff");
        }

        //DEBUG TIMER:
        //if (tileParent.GetComponent<LevelMechanics>().timer != null) /////////////////////////////////////////////////////////////////////////////////
        //   Debug.Log(tileParent.GetComponent<LevelMechanics>().timer.Elapsed); //////////////////////////////////////////////////////////////////////


        if (Input.GetKeyDown(KeyCode.Tab) && gamePaused == false && shopEnabled)
        {
            SwitchShop();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && inShop == false)
        {
            PauseGame();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && gamePaused == true)
        {
            PauseGame();
        }

        foreach(GameObject card in upgradeCards)
        {
            if (card.GetComponent<CardDisplay>().canBuyUpgrade)
            {
                if (!upgradeCardsUpgradable.Contains(card))
                {
                    upgradeCardsUpgradable.Add(card);
                }
            } else
            {
                if (upgradeCardsUpgradable.Contains(card))
                {
                    upgradeCardsUpgradable.Remove(card);
                }
            }
        }

        if (upgradeCardsUpgradable.Count > 0)
        {
            showUpgradeHint = true;
            if (shownUpgradeHint == false)
            {
                IshowSpeed.GetComponent<Animator>().SetBool("ShowNotification", true);
                GameManager.FindObjectOfType<AudioManager>().Play("ShowHint");
                shownUpgradeHint = true;
            }
        } else
        {
            IshowSpeed.GetComponent<Animator>().SetBool("ShowNotification", false);
            showUpgradeHint = false;
            shownUpgradeHint = false;
        }
    }

    private void EnablePause()
    {
        pauseEnabled = true;
    }

    public void EndGame(bool victory)
    {
        tileParent.GetComponent<LevelMechanics>().timer.Stop();

        string timeText = "TIME: " + tileParent.GetComponent<LevelMechanics>().timer.Elapsed.ToString("mm\\:ss\\.ff");

        boombox.GetComponent<Boombox>().StopAllCoroutines();

        timerTextLose.GetComponent<Text>().text = timeText;
        timerTextWin.GetComponent<Text>().text = timeText;

        player.GetComponent<BoxCollider>().enabled = false;

        GetComponent<MenuMechanics>().ChangeMusicPitch(1.0f, false, 1.0f);

        shopEnabled = false;
        player.GetComponent<Player>().movementEnabled = false;
        player.GetComponent<Player>().buildingEnabled = false;
        tileParent.GetComponent<LevelMechanics>().disableWaves = true;
        tileParent.GetComponent<LevelMechanics>().StopAllCoroutines();
        tileParent.GetComponent<LevelMechanics>().disableRain = true;
        tileParent.GetComponent<MoneyMechanics>().spawnItems = false;
        tileParent.GetComponent<TileMechanics>().enableMouseOverTileMaterial = false;
        tileParent.GetComponent<UpgradeMechanics>().goldMineEnabled = false;
        gameUI.SetActive(false);
        tileParent.GetComponent<TileMechanics>().RainDamage = 0;


        if (victory)
        {
            StartCoroutine(tileParent.GetComponent<DialogueMechanics>().WonDaGame());
            mainLT.GetComponent<Light>().color = new Color(1.0f, 1.0f, 1.0f);
        } else
        {
            StartCoroutine(tileParent.GetComponent<DialogueMechanics>().LostDaGame());
        }
    }

    public void SwitchShop()
    {
        if (!shopEnabled)
            return;

        if (inShop)
        {
            tileAudioMixer.audioMixer.SetFloat("TileVolume", 0.0f);
        }
        else
        {
            tileAudioMixer.audioMixer.SetFloat("TileVolume", -80.0f);
        }
        tileParent.GetComponent<XpMechanics>().CorrectXPText();

        //shopUI.SetActive(!shopUI.activeInHierarchy); normaal...
        if (!inShop)
        {
            FindObjectOfType<AudioManager>().PauseSpecificAudioObject("Chune");
            shopUI.transform.parent = transform;
            Time.timeScale = 0;
            //gameUI.SetActive(false); //slecht voor optimization als uitstaat
            inShop = true;
        }
        else
        {
            shopUI.transform.parent = transform.parent;
            FindObjectOfType<AudioManager>().UnpauseSpecificAudioObject("Chune");
            Time.timeScale = 1;
            //gameUI.SetActive(true); //slecht voor optimization als uitstaat
            wentBackToGameAtleastOnce = true;
            inShop = false;
        }

        //FindObjectOfType<AudioManager>().Stop(inShop);
        FindObjectOfType<AudioManager>().Play("ButtonSound");
        FindObjectOfType<AudioManager>().PauseAllAudioObjects();
    }

    public void PauseGame()
    {
        if (!pauseEnabled)
            return;

        FindObjectOfType<AudioManager>().Stop(!inShop);
        if (gamePaused)
        {
            if (tileParent.GetComponent<LevelMechanics>().timer != null)
                tileParent.GetComponent<LevelMechanics>().timer.Start(); // timer shit
            
            tileAudioMixer.audioMixer.SetFloat("TileVolume", 0.0f);
            FindObjectOfType<AudioManager>().UnpauseSpecificAudioObject("Chune");
        }
        else
        {
            if (tileParent.GetComponent<LevelMechanics>().timer != null)
                tileParent.GetComponent<LevelMechanics>().timer.Stop(); // timer shit

            tileAudioMixer.audioMixer.SetFloat("TileVolume", -80.0f);
            FindObjectOfType<AudioManager>().PauseSpecificAudioObject("Chune");
        }

        gamePaused = !gamePaused;
        inShop = !inShop;
        pauseUI.SetActive(gamePaused);
        FindObjectOfType<AudioManager>().Play("ButtonSound");
        FindObjectOfType<AudioManager>().PauseAllAudioObjects(); /////

        Time.timeScale = Convert.ToInt32(!gamePaused);
    }

    public void GoToGame(int gameMode)
    {
        newGameUI.SetActive(false);

        GameManager.gameMode = gameMode;

        Initiate.Fade("Main", Color.black, 0.4f);

        FindObjectOfType<AudioManager>().Play("ButtonSound");
    }

    public void NoOption()
    {
        FindObjectOfType<AudioManager>().Play("ButtonSound");
    }

    public void GoToNewGameMenu()
    {
        menuCamera.GetComponent<MenuCamera>().changeTransform(new Vector3(0.1f, 3.0f, -0.78f), Quaternion.Euler(316, 0, 0));
        mainMenuUI.SetActive(false);
        newGameUI.SetActive(true);
        FindObjectOfType<AudioManager>().Play("ButtonSound");
    }

    public void GoToSettings()
    {
        menuCamera.GetComponent<MenuCamera>().changeTransform(new Vector3(0.1f, 1.18f, 1.47f), Quaternion.Euler(377, 0, 0));
        mainMenuUI.SetActive(false);
        FindObjectOfType<AudioManager>().Play("ButtonSound");
        settingsUI.SetActive(true);
        UpdateResLabel();
        ChangeMasterPitch(0.5f, false, 3.0f);
    }

    public void GoToMainMenu()
    {
        menuCamera.GetComponent<MenuCamera>().ResetTransform();
        mainMenuUI.SetActive(true);
        newGameUI.SetActive(false);
        settingsUI.SetActive(false);
        FindObjectOfType<AudioManager>().Play("ButtonSound");
        ChangeMasterPitch(1.0f, false, 3.0f);
    }

    public void ExitGame()
    {
        FindObjectOfType<AudioManager>().Play("ButtonSound");
        Application.Quit();
    }

    public void GoToMenu()
    { 
        if (tileParent.GetComponent<DialogueMechanics>().introGaande == false)
        {
            tileParent.GetComponent<LevelMechanics>().timer.Stop();
            tileParent.GetComponent<LevelMechanics>().timer.Reset();
        }
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
        FindObjectOfType<AudioManager>().Play("ButtonSound");
    }

    public void BackToMenu()
    {
        PauseGame();
        SceneManager.LoadScene(0);
        FindObjectOfType<AudioManager>().Play("ButtonSound");
    }

    public void ActivateXPUI(bool withAnimation)
    {
        xpUI.SetActive(true);
        if (withAnimation)
            xpUI.GetComponent<UIFixedAnimation>().Play();
        else
            xpUI.transform.position = xpUI.GetComponent<UIFixedAnimation>().finalV3;
    }
    public void ActivateCoinUI(bool withAnimation)
    {
        coinUI.SetActive(true);
        if (withAnimation)
            coinUI.GetComponent<UIFixedAnimation>().Play();
        else
            coinUI.transform.position = coinUI.GetComponent<UIFixedAnimation>().finalV3;
    }
    public void ActivateLevelUI(bool withAnimation)
    {
        levelUI.SetActive(true);
        if (withAnimation)
            levelUI.GetComponent<UIFixedAnimation>().Play();
        else
            levelUI.transform.position = levelUI.GetComponent<UIFixedAnimation>().finalV3;
    }
    public void ActivateHealthUI(bool withAnimation)
    {
        healthUI.SetActive(true);
        if (withAnimation)
            healthUI.GetComponent<UIFixedAnimation>().Play();
        else
            healthUI.transform.position = healthUI.GetComponent<UIFixedAnimation>().finalV3;
    }
    public void ActivateShopButtonUI(bool withAnimation)
    {
        shopButtonUI.SetActive(true);
        if (withAnimation)
            shopButtonUI.GetComponent<UIFixedAnimation>().Play();
        else
            shopButtonUI.transform.position = shopButtonUI.GetComponent<UIFixedAnimation>().finalV3;
    }
    public void ActivatePauseButtonUI(bool withAnimation)
    {
        pauseButtonUI.SetActive(true);
        if (withAnimation)
            pauseButtonUI.GetComponent<UIFixedAnimation>().Play();
        else
            pauseButtonUI.transform.position = pauseButtonUI.GetComponent<UIFixedAnimation>().finalV3;
    }

    public void ActivateGoalUI(bool withAnimation)
    {
        goalUI.SetActive(true);
        if (withAnimation)
            goalUI.GetComponent<UIFixedAnimation>().Play();
        else
            goalUI.transform.position = goalUI.GetComponent<UIFixedAnimation>().finalV3;
    }

    public void ActivateTimerUI(bool withAnimation)
    {
        timerUI.SetActive(true);
        if (withAnimation)
            timerUI.GetComponent<UIFixedAnimation>().Play();
        else
            timerUI.transform.position = goalUI.GetComponent<UIFixedAnimation>().finalV3;
    }

    public void EnableFpsTextUI()
    {
        fpsTextUI.SetActive(true);
    }

    public void ActivateVictoryUI()
    {
        victoryUI.SetActive(true);

        foreach (Transform child in victoryUI.transform)
        {
            if (child.GetComponent<UIFixedAnimation>() != null)
            {
                child.GetComponent<UIFixedAnimation>().Play();
            }
        }
    }

    public void ActivateLoseUI()
    {
        loseUI.SetActive(true);

        foreach (Transform child in loseUI.transform)
        {
            if (child.GetComponent<UIFixedAnimation>() != null)
            {
                child.GetComponent<UIFixedAnimation>().Play();
            }
        }
    }

    public void MultiplyTimeScale(float Choppa)
    {
        if (Time.timeScale * Choppa < 100)
            Time.timeScale *= Choppa;
    }

    public void ResetTimeScale()
    {
        Time.timeScale = 1;
    }

    public void ChangeMasterPitch(float pitch, bool instant, float lerpTime)
    {
        if (instant)
        {
            masterPitchChanging = false;
            tileAudioMixer.audioMixer.SetFloat("MasterPitch", pitch);
        }
        else
        {
            masterPitchChangeLerpTime = lerpTime;
            desiredMasterPitch = pitch;
            masterPitchChanging = true;
        }
    }

    public void ChangeMusicPitch(float pitch, bool instant, float lerpTime)
    {
        if (instant)
        {
            musicPitchChanging = false;
            tileAudioMixer.audioMixer.SetFloat("MusicPitch", pitch);
        }
        else
        {
            musicPitchChangeLerpTime = lerpTime;
            desiredMusicPitch = pitch;
            musicPitchChanging = true;
        }
    }

    public void SetMasterVolume(float sliderValue)
    {
        float maxVolume = 12;
        float volume = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * (maxVolume + 80) / 4f + maxVolume;
        tileAudioMixer.audioMixer.SetFloat("MasterVolume", volume);
    }

    public void SetMusicVolume(float sliderValue)
    {
        float maxVolume = 0;
        float volume = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * (maxVolume + 80) / 4f + maxVolume;
        tileAudioMixer.audioMixer.SetFloat("MusicVolume", volume);
    }

    public void SetAmbienceVolume(float sliderValue)
    {
        float maxVolume = 0;
        float volume = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * (maxVolume + 80) / 4f + maxVolume;
        tileAudioMixer.audioMixer.SetFloat("AmbienceVolume", volume);
    }

    public void ToggleShowFps(bool toggle)
    {
        GameManager.showFps = toggle;
        FindObjectOfType<AudioManager>().Play("MoveClick");
    }

    public void ToggleShowElapsedTime(bool toggle)
    {
        GameManager.showElapsedTime = toggle;
        FindObjectOfType<AudioManager>().Play("MoveClick");
    }

    public void ToggleCheats()
    {
        GameManager.cheatsEnabled = !GameManager.cheatsEnabled;
        FindObjectOfType<AudioManager>().Play("MoveClick");
    }

    public void ToggleFullscreen(bool toggle)
    {
        Screen.fullScreen = toggle;
        GameManager.fullscreenEnabled = toggle;
        FindObjectOfType<AudioManager>().Play("MoveClick");
    }

    public void TogglePostProcessing(bool toggle)
    {
        GameManager.postProcessingEnabled = toggle;
        FindObjectOfType<AudioManager>().Play("MoveClick");
    }

    public void ReadSkinCodeInput(string code)
    {
        //GameManager.skinCode = code;
    }

    public void ToggleMusic()
    {
        if (musicEnabled)
        {
            tileAudioMixer.audioMixer.SetFloat("MusicVolume", -80.0f);
            musicEnabled = false;
        }
        else
        {
            tileAudioMixer.audioMixer.SetFloat("MusicVolume", 0.0f);
            musicEnabled = true;
        }
    }

    public void EnableMusic()
    {
        tileAudioMixer.audioMixer.SetFloat("MusicVolume", 0.0f);
        musicEnabled = true;
    }

    public void ResLeft()
    {
        FindObjectOfType<AudioManager>().Play("MoveClick");
        GameManager.selectedResolution--;
        if (GameManager.selectedResolution < 0)
            GameManager.selectedResolution = 0;

        UpdateResLabel();
        SetResolution();
    }

    public void ResRight()
    {
        FindObjectOfType<AudioManager>().Play("MoveClick");
        GameManager.selectedResolution++;
        if (GameManager.selectedResolution > resolutions.Count - 1)
            GameManager.selectedResolution = resolutions.Count - 1;

        UpdateResLabel();
        SetResolution();
    }

    public void UpdateResLabel()
    {
        resTextObject.GetComponent<Text>().text = resolutions[GameManager.selectedResolution].horizontal.ToString() + "x" + resolutions[GameManager.selectedResolution].vertical.ToString();
    }

    public void SetResolution()
    {
        Screen.SetResolution(resolutions[GameManager.selectedResolution].horizontal, resolutions[GameManager.selectedResolution].vertical, GameManager.fullscreenEnabled);
    }

}

[System.Serializable]
public class ResItem 
{
    public int horizontal, vertical;
}
