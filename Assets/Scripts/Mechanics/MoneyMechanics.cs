using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyMechanics : MonoBehaviour
{

    public GameObject coinPrefab;
    public GameObject diamondPrefab;

    public bool spawnItems;

    public List<Vector3> coinPositions;
    public List<GameObject> coins;
    public List<Vector3> diamondPositions;
    public List<GameObject> diamonds;

    public int money = 0;
    private int moneyTextValue = 0;

    public float moneySpawnHeight;
    public float diamondSpawnHeight;

    public float giveWaitTimeMin;
    public float giveWaitTimeMax;
    public float giveWaitTimeDiamondMin;
    public float giveWaitTimeDiamondMax;
    public float diamondSpawnChance;

    [SerializeField] public GameObject diamondsHolder;
    [SerializeField] public GameObject coinsHolder;

    public bool treasureMagnetEnabled = false;
    public float treasureMagnetSpeed;

    [SerializeField] private GameObject coinSpawnAudio;
    [SerializeField] private GameObject diamondSpawnAudio;

    public Text moneyText;

    public bool giveMoneyReady = true;
    public bool giveDiamondReady = false;

    List<GameObject> tileList;

    private GameObject canvas;

    private GameObject player;

    public float moneyTextLerpTimeElapsed = 0;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        tileList = GetComponent<TileMechanics>().Tiles;
        canvas = GameObject.FindGameObjectWithTag("Canvas");
    }

    void Update()
    {
        if (canvas.GetComponent<MenuMechanics>().theGameHasEnded == true)
            return;

        if (moneyTextLerpTimeElapsed < 1)
        {
            moneyTextValue = (int)Mathf.Lerp(moneyTextValue, money, moneyTextLerpTimeElapsed / 1);
            moneyTextLerpTimeElapsed += Time.deltaTime;
            if (moneyTextValue == money - 2 || moneyTextValue == money + 2)
                moneyTextValue = money;
        }

        moneyText.text = moneyTextValue.ToString();

        if (spawnItems == false)
            return;

        if (canvas.GetComponent<MenuMechanics>().gamePaused == false && canvas.GetComponent<MenuMechanics>().inShop == false)
        {
            if (treasureMagnetEnabled)
            {
                foreach (GameObject coin in coins)
                {
                    if (coin != null)
                    {
                        coin.GetComponent<HoverAnimation>().enabled = false;
                        coin.transform.position += (player.transform.position - coin.transform.position) * Time.deltaTime * treasureMagnetSpeed;
                    }
                }
                foreach (GameObject diamond in diamonds)
                {
                    if (diamond != null)
                    {
                        diamond.GetComponent<HoverAnimation>().enabled = false;
                        diamond.transform.position += (player.transform.position - diamond.transform.position) * Time.deltaTime * treasureMagnetSpeed;
                    }
                }
            }


            if (giveMoneyReady == true && coinPositions.Count + diamondPositions.Count != 25)
            {
                spawnCoin(true);
            }
            if (giveDiamondReady == false && coinPositions.Count + diamondPositions.Count != 25)
            {
                StartCoroutine(GiveDiamond());
            }
            if (player.GetComponent<Player>().cheatsEnabled && Input.GetKeyDown(KeyCode.R))
            {
                spawnCoin(false);
            }
            if (player.GetComponent<Player>().cheatsEnabled && Input.GetKeyDown(KeyCode.T)) {
                SpawnDiamond();
            }
        }
    }

    public void spawnCoin(bool withCoroutine, Vector3 customSpawnPos = default(Vector3))
    {
        GameObject randomTile = tileList[Random.Range(0, tileList.Count)];
        Vector3 tilePos = randomTile.transform.position;

        if (!coinPositions.Contains(tilePos) && !diamondPositions.Contains(tilePos) && (tilePos.x != player.transform.position.x || tilePos.z != player.transform.position.z))
        {
            Vector3 daCoinPos;
            if (customSpawnPos == Vector3.zero)
                daCoinPos = new Vector3(tilePos.x, (tilePos.y + moneySpawnHeight), tilePos.z);
            else
                daCoinPos = new Vector3(customSpawnPos.x, (customSpawnPos.y + moneySpawnHeight), customSpawnPos.z);

            GameObject coin = Instantiate(coinPrefab, daCoinPos, transform.rotation);
            coin.transform.name = "Coin: " + tilePos;
            if (!treasureMagnetEnabled)
                coin.GetComponentInChildren<AudioSource>().enabled = false;
            coinPositions.Add(tilePos);
            coin.transform.parent = coinsHolder.transform;
            coins.Add(coin);
            FindObjectOfType<AudioManager>().Play("CoinSpawn");
            GameObject coinAudio = Instantiate(coinSpawnAudio, tilePos, transform.rotation);
            coinAudio.transform.parent = coinsHolder.transform;
            if (withCoroutine)
                StartCoroutine(GiveMoney());
        } else
        {
            if (diamondPositions.Count + coinPositions.Count >= 24)
                return;

            spawnCoin(withCoroutine);
        }
    }
    public void SpawnDiamond()
    {
        GameObject randomTile = tileList[Random.Range(0, tileList.Count)];
        Vector3 tilePos = randomTile.transform.position;

        if (!coinPositions.Contains(tilePos) && !diamondPositions.Contains(tilePos) && (tilePos.x != player.transform.position.x || tilePos.z != player.transform.position.z) && spawnItems)
        {
            GameObject diamond = Instantiate(diamondPrefab, new Vector3(tilePos.x, (tilePos.y + diamondSpawnHeight), tilePos.z), transform.rotation);
            if (!treasureMagnetEnabled)
                diamond.GetComponentInChildren<AudioSource>().enabled = false;
            diamond.transform.name = "Diamond " + tilePos;
            diamond.transform.parent = diamondsHolder.transform;
            diamondPositions.Add(tilePos);
            diamonds.Add(diamond);
            FindObjectOfType<AudioManager>().Play("CoinSpawn");
            GameObject diamondAudio = Instantiate(diamondSpawnAudio, tilePos, transform.rotation);
            diamondAudio.transform.parent = diamondsHolder.transform;
            giveDiamondReady = false;
        }
        else
        {
            if (diamondPositions.Count + coinPositions.Count >= 24)
                return;

            SpawnDiamond();
        }
    }

    public void RemoveCoinPos(Vector3 coinPos)
    {
        foreach (Vector3 pos in coinPositions)
        {
            if (pos == new Vector3(coinPos.x, (int)(coinPos.y - moneySpawnHeight), (int)coinPos.z))
            {
                coinPositions.Remove(pos);
                break;
            }
        }
    }

    public void RemoveCoin(GameObject coin)
    {
        coins.Remove(coin);
    }

    public void RemoveDiamondPos(Vector3 diamondPos)
    {
        foreach (Vector3 pos in diamondPositions)
        {
            if (pos == new Vector3(diamondPos.x, (int)(diamondPos.y - moneySpawnHeight), (int)diamondPos.z))
            {
                diamondPositions.Remove(pos);
                break;
            }
        }
    }

    public void RemoveDiamond(GameObject diamond)
    {
        diamonds.Remove(diamond);
    }

    IEnumerator GiveMoney()
    {
        if (canvas.GetComponent<MenuMechanics>().theGameHasEnded == false)
        {
            giveMoneyReady = false;
            float newWaitTime = Random.Range(giveWaitTimeMin, giveWaitTimeMax) * Mathf.Pow(GetComponent<LevelMechanics>().moneyGiveWaitTimeMultiplier, GetComponent<LevelMechanics>().Levels - 1);
            yield return new WaitForSeconds(newWaitTime);
            giveMoneyReady = true;
        }
    }

    IEnumerator GiveDiamond()
    {
        if (canvas.GetComponent<MenuMechanics>().theGameHasEnded == false)
        {
            giveDiamondReady = true;
            yield return new WaitForSeconds(Random.Range(giveWaitTimeDiamondMin, giveWaitTimeDiamondMax));

            if (Random.value <= diamondSpawnChance)
                SpawnDiamond();

            giveDiamondReady = false;
        }
    }

    public void AddMoney(int newMoney)
    {
        money += newMoney;
        moneyTextLerpTimeElapsed = 0;
    }

    public void UpdateMoneyText()
    {
        moneyText.text = money.ToString();
    }

    public void RemoveMoney(int moneyGone)
    {
        money -= moneyGone;
        moneyTextLerpTimeElapsed = 0;
    }

    public void ToggleSpawns()
    {
        spawnItems = !spawnItems;
    }
}
