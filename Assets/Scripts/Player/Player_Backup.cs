    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

public class Player_Backup : MonoBehaviour
{
    public bool cheatsEnabled; // <- cheats

    public GameObject playerObject;
    public Text currentTileText;
    public Text UpgradeText;
    public GameObject tileParent;
    public GameObject selectedTile;
    public GameObject playerModel;

    public int MoneyPerCoin;
    public int MoneyPerDiamond;

    private float tileCount;
    private float mapLength;
    private float tilesToSide;
    private float tileDistance;
    private float positionInTilesx;
    private float positionInTilesz;
    private bool onTile;

    private GameObject canvas;

    void Start()
    {
        tileCount = tileParent.GetComponent<TileMechanics>().Tiles.Count; // count tiles
        mapLength = Mathf.Sqrt(tileCount);
        tilesToSide = (mapLength - 1) / 2;
        UpgradeText = tileParent.GetComponent<TileMechanics>().UpgradeText;

        canvas = GameObject.FindGameObjectWithTag("Canvas");
    }

    void Update()
    {
        UpdateUpgradeText();

        if (GameObject.FindGameObjectWithTag("Canvas").GetComponent<MenuMechanics>().inShop == false)
        {
            if (Input.GetKeyDown("space") && onTile == true)
            {
                PressSpace();
            }

            positionInTilesx = transform.position.x / tileDistance;
            positionInTilesz = transform.position.z / tileDistance;

            if (Input.GetKeyDown(KeyCode.LeftArrow) && positionInTilesx > -tilesToSide || Input.GetKeyDown("a") && positionInTilesx > -tilesToSide)
            {
                GoLeft();
            }

            if (Input.GetKeyDown(KeyCode.RightArrow) && positionInTilesx < tilesToSide || Input.GetKeyDown("d") && positionInTilesx < tilesToSide)
            {
                GoRight();
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) && positionInTilesz < tilesToSide || Input.GetKeyDown("w") && positionInTilesz < tilesToSide)
            {
                GoUp();
            }

            if (Input.GetKeyDown(KeyCode.DownArrow) && positionInTilesz > -tilesToSide || Input.GetKeyDown("s") && positionInTilesz > -tilesToSide)
            {
                GoDown();
            }
        }
    }

    public void UpdateUpgradeText()
    {
        if (selectedTile != null)
        {
            UpgradeText.text = selectedTile.GetComponent<TileState>().NextUPgradePrice.ToString();
        }
    }

    public void PressSpace()
    {
        tileParent.GetComponent<TileMechanics>().RefreshTileList();
        selectedTile.GetComponent<TileState>().UpgradeTile(true);
        FindObjectOfType<AudioManager>().Play("TileAction");
    }

    public void GoLeft()
    {
        if (positionInTilesx > -tilesToSide)
        {
            transform.position += new Vector3(-tileDistance, 0.0f, 0.0f);
            playerModel.transform.rotation = Quaternion.Euler(0, -90, 0);
            FindObjectOfType<AudioManager>().Play("Move");
            UpdateUpgradeText();
        }

    }

    public void GoRight()
    {
        if (positionInTilesx < tilesToSide)
        {
            transform.position += new Vector3(tileDistance, 0.0f, 0.0f);
            playerModel.transform.rotation = Quaternion.Euler(0, 90, 0);
            FindObjectOfType<AudioManager>().Play("Move");
        }
    }

    public void GoUp()
    {
        if (positionInTilesz < tilesToSide)
        {
            transform.position += new Vector3(0.0f, 0.0f, tileDistance);
            playerModel.transform.rotation = Quaternion.Euler(0, 0, 0);
            FindObjectOfType<AudioManager>().Play("Move");
        }
    }

    public void GoDown()
    {
        if (positionInTilesz > -tilesToSide)
        {
            transform.position += new Vector3(0.0f, 0.0f, -tileDistance);
            playerModel.transform.rotation = Quaternion.Euler(0, 180, 0);
            FindObjectOfType<AudioManager>().Play("Move");
        }
    }

    void OnCollisionStay(Collision col)
    {
        if (col.gameObject.tag == "Tile")
        {
            onTile = true;

            selectedTile = col.gameObject;
            float tileScale = selectedTile.transform.localScale.x;
            tileDistance = tileScale;
        }
        else
        {
            onTile = false;
        }
        if (col.gameObject.tag == "Coin")
        {
            Destroy(col.gameObject);
            //tileParent.GetComponent<MoneyMechanics>().money += MoneyPerCoin;
            //tileParent.GetComponent<MoneyMechanics>().money = (int)Mathf.Lerp(tileParent.GetComponent<MoneyMechanics>().money, tileParent.GetComponent<MoneyMechanics>().money + MoneyPerCoin, 0.1f);
            PositionHolder positionHolderScript = col.transform.GetComponent<PositionHolder>();
            tileParent.GetComponent<MoneyMechanics>().AddMoney(MoneyPerCoin);

            if (positionHolderScript != null)
                tileParent.GetComponent<MoneyMechanics>().RemoveCoinPos(col.transform.GetComponent<PositionHolder>().startPosition);
            
            FindObjectOfType<AudioManager>().Play("CoinPickup");
            FindObjectOfType<AudioManager>().Play("Pickup");
        }
        if (col.gameObject.tag == "Diamond")
        {
            Destroy(col.gameObject);
            //tileParent.GetComponent<MoneyMechanics>().money += MoneyPerCoin;
            //tileParent.GetComponent<MoneyMechanics>().money = (int)Mathf.Lerp(tileParent.GetComponent<MoneyMechanics>().money, tileParent.GetComponent<MoneyMechanics>().money + MoneyPerCoin, 0.1f);
            tileParent.GetComponent<MoneyMechanics>().AddMoney(MoneyPerDiamond);
            tileParent.GetComponent<MoneyMechanics>().RemoveDiamondPos(col.transform.GetComponent<PositionHolder>().startPosition);
            FindObjectOfType<AudioManager>().Play("CoinPickup");
            FindObjectOfType<AudioManager>().Play("Pickup");
            FindObjectOfType<AudioManager>().Play("PickupDiamond");
        }

    }
}