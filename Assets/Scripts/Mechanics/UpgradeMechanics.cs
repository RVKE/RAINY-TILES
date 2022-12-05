using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMechanics : MonoBehaviour
{

    private int waterPumpLevel = 0;
    private float waterExtractionSpeed = 1;
    private float waterExtractionAmount = 0;
    private bool waterExtractionReady = true;
    private bool goldCollectionReady = true;
    private bool hasGoldCollector = false;
    public GameObject Player;
    public GameObject solarPanels;

    [SerializeField] private GameObject coinsHolder;

    public bool goldMineEnabled;

    public GameObject goldMine;
    public GameObject goldMineUpgraded;
    public GameObject coinEmitter;
    public GameObject mineCoin;

    public GameObject dieselWaterpumpHolder;
    public GameObject electricWaterpumpHolder;
    public GameObject nuclearWaterpumpHolder;
    public GameObject houseReinforcement;


    public GameObject newHealthBar;

    public float mineOutputWaitTime;

    private GameObject canvas;

    private GameObject tileParent;

    private GameObject player;

    public GameObject magnetObject;

    private void Start()
    {
        goldMineEnabled = true;
        player = GameObject.FindGameObjectWithTag("Player");
        canvas = GameObject.FindGameObjectWithTag("Canvas");
    }

    public void SelectUpgrade(string upgradeName)
    {
        //Waterpump upgrades
        if (upgradeName == "DIESEL WATERPUMP")
        {
            dieselWaterpumpHolder.SetActive(true);
            waterPumpLevel = 1;
            waterExtractionAmount = 10;
            //FindObjectOfType<AudioManager>().Play("GeneratorSound");
            FindObjectOfType<AudioManager>().Play("UpgradeCard");
            //smokeLevel1.SetActive(true);
        }
        if (upgradeName == "ADVANCED GOLD MINING")
        {
            FindObjectOfType<AudioManager>().Play("UpgradeCard");
            goldMine.SetActive(false);
            goldMineUpgraded.SetActive(true);
            mineOutputWaitTime /= 4;
        }
        if (upgradeName == "INCREASED BUILD RADIUS")
        {
            FindObjectOfType<AudioManager>().Play("UpgradeCard");
            player.GetComponent<Player>().buildRadius *= 2;
            player.GetComponent<Player>().UpdateBuildTiles();
        }
        if (upgradeName == "ELECTRIC WATERPUMP")
        {
            dieselWaterpumpHolder.SetActive(false);
            electricWaterpumpHolder.SetActive(true);
            waterPumpLevel = 4;
            waterExtractionAmount = 30;
            FindObjectOfType<AudioManager>().Play("UpgradeCard");
            //smokeLevel3.SetActive(false);
        }
        if (upgradeName == "NUCLEAR WATERPUMP")
        {
            electricWaterpumpHolder.SetActive(false);
            nuclearWaterpumpHolder.SetActive(true);
            waterPumpLevel = 5;
            waterExtractionAmount = 80;
            FindObjectOfType<AudioManager>().Play("UpgradeCard");
        }
        //Bonus upgrades
        if (upgradeName == "GOLD MINING")
        {
            hasGoldCollector = true;
            goldMine.SetActive(true);
            FindObjectOfType<AudioManager>().Play("UpgradeCard");
        }
        if (upgradeName == "SOLAR ENERGY")
        {
            waterExtractionSpeed = 0.67f;
            solarPanels.SetActive(true);
            FindObjectOfType<AudioManager>().Play("UpgradeCard");
        }
        if (upgradeName == "TRIPLE MONEY")
        {
            Player.GetComponent<Player>().MoneyPerCoin = 300;
            FindObjectOfType<AudioManager>().Play("UpgradeCard");
        }
        if (upgradeName == "REINFORCED HOUSE")
        {
            GetComponent<WaterMechanics>().houseDamageAmount /= 5;
            FindObjectOfType<AudioManager>().Play("UpgradeCard");
            newHealthBar.SetActive(true);
            houseReinforcement.SetActive(true);
        }
        if (upgradeName == "TREASURE MAGNET")
        {
            GetComponent<MoneyMechanics>().treasureMagnetEnabled = true;
            FindObjectOfType<AudioManager>().Play("UpgradeCard");
            magnetObject.SetActive(true);
        }
    }

    void Update()
    {
        if (canvas.GetComponent<MenuMechanics>().gamePaused == true || canvas.GetComponent<MenuMechanics>().inShop == true)
            return;

        if (waterPumpLevel > 0 && waterExtractionReady)
        {
            StartCoroutine(waitForWaterExtraction());
        }
        if ((hasGoldCollector == true && goldMineEnabled == true) && goldCollectionReady)
        {
            StartCoroutine(waitForGoldIncome());
        }
    }

    IEnumerator waitForWaterExtraction()
    {
        if(GetComponent<WaterMechanics>().waterAmount > 0)
        {
            GetComponent<WaterMechanics>().waterAmount -= waterExtractionAmount;
        }
        waterExtractionReady = false;
        yield return new WaitForSeconds(waterExtractionSpeed);
        waterExtractionReady = true;
    }

    IEnumerator waitForGoldIncome()
    {
        GameObject newMineCoin = Instantiate(mineCoin, coinEmitter.transform.position, Quaternion.identity);
        newMineCoin.transform.parent = coinsHolder.transform;
        goldCollectionReady = false;
        yield return new WaitForSeconds(mineOutputWaitTime);
        goldCollectionReady = true;
    }

    public void toggleGoldMine()
    {
        goldMineEnabled = !goldMineEnabled;
    }
}
