using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;

public class LevelMechanics : MonoBehaviour
{
    public bool gameStarted;

    public bool disableWaves;

    public List<GameObject> Tiles;
    public Text LevelTEXT;
    public int Levels;
    public float downTime;
    public float factor;
    public float Rains;
    public float TimeBetweenRains;
    public float RainTimereducerFactor;
    public float RainsInThisLevel;
    public float rainTimeRandomness;
    public float downTimeBonusLengthMultiplier;
    public float moneyGiveWaitTimeMultiplier;
    public float coolDownPeriod; // 7.1f

    public Stopwatch timer;

    private GameObject player;

    public bool disableRain;

    private GameObject canvas;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        canvas = GameObject.FindGameObjectWithTag("Canvas");
        player.GetComponent<Player>().movementEnabled = false;
        player.GetComponent<Player>().buildingEnabled = false;
        canvas.GetComponent<MenuMechanics>().shopEnabled = false;
        gameStarted = false;
        disableRain = true;
        GetComponent<MoneyMechanics>().spawnItems = false;
    }

    public void StartGame() // <--- START DE GAME
    {
        if (GameManager.gameMode == 1) //CASUAL GAME
        {
            player.GetComponent<Player>().buildingEnabled = true;
            canvas.GetComponent<MenuMechanics>().shopEnabled = true;
            canvas.GetComponent<MenuMechanics>().ActivateCoinUI(false);
            canvas.GetComponent<MenuMechanics>().ActivateHealthUI(false);
            canvas.GetComponent<MenuMechanics>().ActivateLevelUI(false);
            canvas.GetComponent<MenuMechanics>().ActivatePauseButtonUI(false);
            canvas.GetComponent<MenuMechanics>().ActivateShopButtonUI(false);
            canvas.GetComponent<MenuMechanics>().ActivateXPUI(false);
            player.GetComponent<Player>().movementEnabled = true;
            player.GetComponent<Player>().setPosition = true;
            GetComponent<TileMechanics>().enableMouseOverTileMaterial = true;

            gameStarted = true;
            disableRain = false;
            GetComponent<MoneyMechanics>().spawnItems = true;

            timer = new Stopwatch();

            timer.Start(); //start de timer;

            if (GameManager.showElapsedTime)
                canvas.GetComponent<MenuMechanics>().ActivateTimerUI(true);

            player.GetComponent<Player>().allowSmoothAppear = true;

            StartCoroutine(Wait());
        }
        if (GameManager.gameMode == 2)
        {
            //belangrijk

            player.GetComponent<Player>().buildingEnabled = true;
            canvas.GetComponent<MenuMechanics>().shopEnabled = true;
            canvas.GetComponent<MenuMechanics>().ActivateCoinUI(false);
            canvas.GetComponent<MenuMechanics>().ActivateHealthUI(false);
            canvas.GetComponent<MenuMechanics>().ActivateLevelUI(false);
            canvas.GetComponent<MenuMechanics>().ActivatePauseButtonUI(false);
            canvas.GetComponent<MenuMechanics>().ActivateShopButtonUI(false);
            canvas.GetComponent<MenuMechanics>().ActivateXPUI(false);
            player.GetComponent<Player>().movementEnabled = true;
            player.GetComponent<Player>().setPosition = true;
            GetComponent<TileMechanics>().enableMouseOverTileMaterial = true;

            gameStarted = true; 
            disableRain = false;
            GetComponent<MoneyMechanics>().spawnItems = true;

            GameManager.showElapsedTime = true;

            timer = new Stopwatch();

            timer.Start(); //start de timer;

            canvas.GetComponent<MenuMechanics>().ActivateTimerUI(true);

            player.GetComponent<Player>().allowSmoothAppear = true;

            StartCoroutine(Wait()); //
        }

    }

    void Update()
    {
        Tiles = GetComponent<TileMechanics>().Tiles;
    }

    void LetItRain(float HowManyTimes)
    {
        if (GetComponent<TileMechanics>().Bossmode == false && disableRain == false)
        {
            if (HowManyTimes > 0)
            {
                GetComponent<TileMechanics>().RainOnRandomPos();
                RainsInThisLevel -= 1;
            }
        }
    }

    IEnumerator Wait()
    {
        if (disableWaves == false && canvas.GetComponent<MenuMechanics>().theGameHasEnded == false)
        {
            //time to get ready
            yield return new WaitForSeconds(TimeBetweenRains + Random.Range(-rainTimeRandomness, rainTimeRandomness));
            //rain start to come
            LetItRain(RainsInThisLevel);
            if (RainsInThisLevel > 0)
            {
                StartCoroutine(Wait());
            }
            else
            {
                StartCoroutine(DownTime());
            }
        }
    }

    IEnumerator DownTime()
    {
        yield return new WaitForSeconds(coolDownPeriod); // <- wachten voor rain

        if (disableWaves == false)
        {
            LevelTEXT.text = "► WAVE INCOMING ◄";
            LevelTEXT.gameObject.GetComponent<UISmoothAppear>().PlayAnimation();
            FindObjectOfType<AudioManager>().Play("NewLevel");

            if (GameManager.gameMode != 2)
            {
                if (Levels == 1)
                {
                    StartCoroutine(GetComponent<DialogueMechanics>().StartDialogueAfterWave1());
                    yield return new WaitForSeconds(1.5f);
                    GetComponent<MoneyMechanics>().spawnItems = false;
                    player.GetComponent<Player>().movementEnabled = false;
                    player.GetComponent<Player>().buildingEnabled = false;
                    GetComponent<TileMechanics>().enableMouseOverTileMaterial = false;
                    yield return new WaitForSeconds(21);
                    GetComponent<MoneyMechanics>().spawnItems = true;
                    player.GetComponent<Player>().movementEnabled = true;
                    player.GetComponent<Player>().buildingEnabled = true;
                    GetComponent<TileMechanics>().enableMouseOverTileMaterial = true;
                }

                if (Levels == 2)
                {
                    StartCoroutine(GetComponent<DialogueMechanics>().StartDialogueAfterWave2());
                }

                if (Levels == 3)
                {
                    StartCoroutine(GetComponent<DialogueMechanics>().StartDialogueAfterWave3());
                }

                if (Levels == 4)
                {
                    StartCoroutine(GetComponent<DialogueMechanics>().StartDialogueAfterWave4());
                }

                if (Levels == 5)
                {
                    StartCoroutine(GetComponent<DialogueMechanics>().StartDialogueAfterWave5());
                }

                if (Levels == 6)
                {
                    StartCoroutine(GetComponent<DialogueMechanics>().StartDialogueAfterWave6());
                }

                if (Levels == 7)
                {
                    StartCoroutine(GetComponent<DialogueMechanics>().StartDialogueAfterWave7());
                }

                if (Levels == 8)
                {
                    StartCoroutine(GetComponent<DialogueMechanics>().StartDialogueAfterWave8());
                }

                if (Levels == 9)
                {
                    StartCoroutine(GetComponent<DialogueMechanics>().StartDialogueAfterWave9());
                }

                if (Levels >= 10)
                {
                    StartCoroutine(GetComponent<DialogueMechanics>().StartDialogueAfterWave10());
                }
            }


            yield return new WaitForSeconds(downTime + (downTimeBonusLengthMultiplier * Levels));
            FindObjectOfType<AudioManager>().Play("NewLevel");
            //speel animatie af hier!
            //reduce time between rains
            Levels += 1;
            TimeBetweenRains = TimeBetweenRains * RainTimereducerFactor;
            rainTimeRandomness *= RainTimereducerFactor;
            LevelTEXT.gameObject.GetComponent<UISmoothAppear>().PlayAnimation();
            LevelTEXT.text = "► WAVE " + Levels + " ◄";
            //say level here
            //geen idee wat die code is

            RainsInThisLevel = Mathf.Ceil(Rains * factor);
            Rains = RainsInThisLevel;
            StartCoroutine(Wait());
        }
    }

    public void ToggleRain()
    {
        disableRain = !disableRain;
    }
}
