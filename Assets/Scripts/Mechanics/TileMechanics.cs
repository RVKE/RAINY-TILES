using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileMechanics : MonoBehaviour
{
    public List<GameObject> Tiles;

    //alle tiles
    public TileLevel level0Card;
    public TileLevel level1Card;
    public TileLevel level2Card;
    public TileLevel level3Card;
    public TileLevel level4Card;
    public TileLevel level5Card;
    public TileLevel level6Card;
    public TileLevel level7Card;
    public TileLevel level8Card;
    public TileLevel level9Card;
    public TileLevel level10Card;
    public GameObject WinScreenUI;
    public int RainDamage;
    public int RainDamageBossWave;
    public float BossRains;
    private float BossRainsStart;
    public bool Bossmode = false;
    public Camera Cam;
    public Light MainLT;
    public GameObject tileParent;
    public GameObject rainParticle;
    public GameObject randomTile;
    public Text UpgradeText;
    public GameObject BossFightTxt;
    public Image UpgradeImage;

    private bool hasWonTheGame = false;

    public bool enableMouseOverTileMaterial;

    public GameObject bossBarUI;
    public GameObject bossBar;
    public GameObject bossBarTextObject;

    [SerializeField] private GameObject rainWarningIntro;

    public float rainParticleTime;

    [SerializeField] private GameObject rainHolder;

    public int aantalTilesMaxLvl;

    public Animator warningAnimator;

    public GameObject canvas;

    void Start()
    {
        bossBarUI.SetActive(false);
        RefreshTileList();
        BossRainsStart = BossRains;
    }

    IEnumerator Wait(GameObject rainedOnTile)
    {
        //start

        if (canvas.GetComponent<MenuMechanics>().theGameHasEnded == false)
        {
            yield return new WaitForSeconds(2.4f);
            //regen raakt de tile
            if (rainedOnTile != null)
            {
                rainedOnTile.GetComponent<TileState>().isRaining = true;
                yield return new WaitForSeconds(4.5f);
                //regen is gestopt
                if (rainedOnTile != null)
                {
                    rainedOnTile.GetComponent<TileState>().isRaining = false;
                }
            }
        }
    }

    public IEnumerator BossRain()
    {
        if (canvas.GetComponent<MenuMechanics>().theGameHasEnded == false)
        {
            BossRains -= 1.0f;
            yield return new WaitForSeconds(0.25f);
            RainOnRandomPos();
            if (BossRains > 0)
            {
                StartCoroutine(BossRain());
            }
        }
    }

    IEnumerator MakeSceneDarkAnimation()
    {
        yield return new WaitForSeconds(1f);
        FindObjectOfType<AudioManager>().Play("BossMode");
        GetComponent<LevelMechanics>().LevelTEXT.text = "► FINAL WAVE ◄";
        FindObjectOfType<AudioManager>().Play("NewLevel");
        BossFightTxt.SetActive(true);
        Cam.GetComponent<Camera>().backgroundColor = new Color(0.1f, 0.1f, 0.1f);
        MainLT.GetComponent<Light>().color = new Color(0.2f, 0.2f, 0.2f);
        yield return new WaitForSeconds(6.0f);
        BossFightTxt.SetActive(false);
        yield return new WaitForSeconds(6.0f);
        Cam.GetComponent<Camera>().backgroundColor = new Color(0.53f, 0f, 0.05f);
        MainLT.GetComponent<Light>().color = new Color(1f, 0.2f, 0f);
        FindObjectOfType<AudioManager>().Play("LightningStrike");
        CameraShake.Shake(0.2f, 0.2f);
        canvas.GetComponent<MenuMechanics>().ChangeMusicPitch(3.0f, false, 0.05f);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(BossRain());
    }

    public void RefreshTileList()
    {
        Tiles.Clear();
        foreach (Transform child in tileParent.transform)
        {
            GameObject Tile = child.gameObject;
            Tiles.Add(Tile);
            //Debug.Log(Tile);
        }
    }

    public void MakeSceneDark()
    {
        StartCoroutine(MakeSceneDarkAnimation());
    }

    public void MakeSceneNormal()
    {
        Cam.GetComponent<Camera>().backgroundColor = new Color(0.62f, 0.85f, 0.99f);
        MainLT.GetComponent<Light>().color = new Color(1f, 1f, 1f);
    }

    public void RainOnRandomPos()
    {
        RefreshTileList();
        randomTile = Tiles[Random.Range(0, Tiles.Count)];
        Vector3 rainPos = randomTile.transform.position;
        GameObject tempRain = Instantiate(rainParticle, rainPos, Quaternion.Euler(0, 0, 0));
        tempRain.transform.parent = rainHolder.transform;
        if (randomTile != null)
        {
            StartCoroutine(Wait(randomTile));
        }
        FindObjectOfType<AudioManager>().Play("Warning");
        Destroy(tempRain, rainParticleTime);
    }

    public void DoCustomRainWarning(Vector3 pos)
    {
        Instantiate(rainWarningIntro, pos, transform.rotation);
        FindObjectOfType<AudioManager>().Play("Warning");
    }

    public void playWarningAnim()
    {
        warningAnimator.Play("Warning", 0, 0.25f);
    }

    void Update()
    {
        RefreshTileList();

        aantalTilesMaxLvl = 0;

        foreach (GameObject tile in Tiles)
        {
            if (tile.GetComponent<TileState>().tileLevel >= 9)
            {
                aantalTilesMaxLvl += 1;
            }
        }

        if (Bossmode)
        {
            bossBar.transform.localScale = new Vector3(1 - (BossRains/BossRainsStart), bossBar.transform.localScale.y, bossBar.transform.localScale.z);
            bossBarTextObject.GetComponent<Text>().text = "RAIN REMAINING: " + BossRains;
        }


        if (Bossmode && BossRains <= 0.0f && hasWonTheGame == false && canvas.GetComponent<MenuMechanics>().theGameHasEnded == false)
        {
            canvas.GetComponent<MenuMechanics>().theGameHasEnded = true;
            hasWonTheGame = true;
            canvas.GetComponent<MenuMechanics>().EndGame(true);
        }
        if (Bossmode == false)
        {
            if (aantalTilesMaxLvl == 25)
            {
                Bossmode = true;
                bossBarUI.SetActive(true);
                canvas.GetComponent<MenuMechanics>().goalUI.SetActive(false);
                StartCoroutine(GetComponent<DialogueMechanics>().StartDialogueBeforeBossWave());
                bossBarUI.GetComponent<UIFixedAnimation>().Play();
                FindObjectOfType<AudioManager>().Play("Lightning");
                RainDamage = RainDamageBossWave;
                MakeSceneDark();
            }
            else
            {
                Bossmode = false;
                RainDamage = 1;
                MakeSceneNormal();
            }
        }
    }
}
