using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_BackupNew : MonoBehaviour
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

    public bool moving;

    public float playerYOffset;

    private GameObject canvas;
    public GameObject cameraObject;

    public Vector3 desiredPos;
    public bool onDesiredTile;
    public bool positionedEXACTLYOnTile;
    public float moveSpeed;
    public bool inBuildingMode = false;

    public GameObject debugObject; // shush

    public List<GameObject> buildTiles = new List<GameObject>();

    void Start()
    {
        tileCount = tileParent.GetComponent<TileMechanics>().Tiles.Count; // count tiles
        mapLength = Mathf.Sqrt(tileCount);
        tilesToSide = (mapLength - 1) / 2;
        UpgradeText = tileParent.GetComponent<TileMechanics>().UpgradeText;

        canvas = GameObject.FindGameObjectWithTag("Canvas");

        desiredPos = new Vector3(0, playerYOffset, 0);
    }

    void Update()
    {
        UpdateUpgradeText();

        if (transform.position != desiredPos)
        {
            moving = true;
            positionedEXACTLYOnTile = false;
        }
        else
        {
            moving = false;
        }

        transform.position = Vector3.MoveTowards(transform.position, desiredPos, Time.deltaTime * moveSpeed);

        if (GameObject.FindGameObjectWithTag("Canvas").GetComponent<MenuMechanics>().inShop == false)
        {
            if (Input.GetKey(KeyCode.LeftShift) && onTile == true && onDesiredTile)
            {
                inBuildingMode = true;
            }
            else
            {
                inBuildingMode = false;
            }

            positionInTilesx = transform.position.x / tileDistance;
            positionInTilesz = transform.position.z / tileDistance;

            RaycastHit[] hits = Physics.RaycastAll(cameraObject.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition));
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.tag == "Tile")
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (inBuildingMode == false)
                        {
                            desiredPos = new Vector3(hit.transform.position.x, playerYOffset, hit.transform.position.z);
                        }
                        else
                        {
                            if (hit.transform.GetComponent<TileState>().isSelected)
                            {
                                tileParent.GetComponent<TileMechanics>().RefreshTileList();
                                hit.transform.GetComponent<TileState>().UpgradeTile(true);
                                FindObjectOfType<AudioManager>().Play("TileAction");
                            }
                        }
                        break;
                    }
                }
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

            onDesiredTile = true;

            if (transform.position == new Vector3(selectedTile.transform.position.x, playerYOffset, selectedTile.transform.position.z) && positionedEXACTLYOnTile == false)
            {
                positionedEXACTLYOnTile = true;
                if (col.gameObject.tag == "Tile")
                {
                    foreach (GameObject tile in tileParent.GetComponent<TileMechanics>().Tiles)
                    {
                        buildTiles.Remove(tile);
                    }
                    Collider[] hitColliders = Physics.OverlapBox(this.transform.position, new Vector3(1, 1, 1), Quaternion.identity);
                    for (int i = 0; i < hitColliders.Length; i++)
                    {
                        if (hitColliders[i].transform.tag == "Tile")
                        {
                            if (hitColliders[i].transform.position + new Vector3(0, playerYOffset, 0) != this.transform.position)
                            {
                                buildTiles.Add(hitColliders[i].transform.gameObject);
                                //Instantiate(debugObject, hitColliders[i].transform.position, transform.rotation);
                            }
                        }
                    }
                }
            }

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
            tileParent.GetComponent<MoneyMechanics>().AddMoney(MoneyPerDiamond);
            tileParent.GetComponent<MoneyMechanics>().RemoveDiamondPos(col.transform.GetComponent<PositionHolder>().startPosition);
            FindObjectOfType<AudioManager>().Play("CoinPickup");
            FindObjectOfType<AudioManager>().Play("Pickup");
            FindObjectOfType<AudioManager>().Play("PickupDiamond");
        }

    }
}