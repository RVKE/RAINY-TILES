    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public bool cheatsEnabled; // <- cheats

    public bool movementEnabled; //
    public bool buildingEnabled; //

    public bool allowSmoothAppear;

    public bool setPosition; // voor intro

    public bool enableExclusiveBuildPos = false;
    public Vector3 exclusiveBuildPos;
    public bool placedFirstTile;

    public Texture2D defaultCursor;
    public Texture2D buildCursor;

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

    [SerializeField] private float walkingSoundSpeed;
    private float walkTime;

    public bool moving;
    public bool hasMovedAtleastOnce;

    private bool switchedToBuildMode = false;

    public GameObject moveMarker;
    public GameObject moveMarkerHolder;

    public bool positionedOnArrow;

    public float playerYOffset;

    private GameObject canvas;
    public GameObject cameraObject;

    public Vector3 desiredPos;
    public bool onDesiredTile;
    public bool positionedEXACTLYOnTile;
    public float moveSpeed;
    public bool inBuildingMode = false;

    public int buildRadius;

    public GameObject characterModel;

    [SerializeField] private GameObject dababyHammer;

    public Vector3 tileMouseHitPos;

    public GameObject debugObject; // shush

    public List<GameObject> buildTiles = new List<GameObject>();

    [SerializeField] private GameObject runParticlesObject;
    private ParticleSystem runParticles;

    private void Awake()
    {
        cheatsEnabled = GameManager.cheatsEnabled;
    }

    void Start()
    {
        tileCount = tileParent.GetComponent<TileMechanics>().Tiles.Count; // count tiles
        mapLength = Mathf.Sqrt(tileCount);
        tilesToSide = (mapLength - 1) / 2;
        positionedOnArrow = false;
        UpgradeText = tileParent.GetComponent<TileMechanics>().UpgradeText;

        canvas = GameObject.FindGameObjectWithTag("Canvas");
        runParticles = runParticlesObject.GetComponent<ParticleSystem>();

        desiredPos = new Vector3(0, playerYOffset, 0);
    }

    void Update()
    {
        if (inBuildingMode && !canvas.GetComponent<MenuMechanics>().inShop && !canvas.GetComponent<MenuMechanics>().gamePaused)
        {
            Cursor.SetCursor(buildCursor, Vector2.zero, CursorMode.Auto);
        } else
        {
            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
        }

        if (canvas.GetComponent<MenuMechanics>().theGameHasEnded)
        {
            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
        }

        //UpdateUpgradeText(); //oud

        var emmision = runParticles.emission;

        if (transform.position != desiredPos)
        {
            moving = true;
            DoWalkingSounds();
            emmision.enabled = true;
            characterModel.GetComponent<Animator>().SetBool("IsWalking", true);
            positionedEXACTLYOnTile = false;
        }
        else
        {
            moving = false;
            walkTime = 0;
            emmision.enabled = false;
            characterModel.GetComponent<Animator>().SetBool("IsWalking", false);
        }

        if (setPosition)
            transform.position = Vector3.MoveTowards(transform.position, desiredPos, Time.deltaTime * moveSpeed);

        if (canvas.GetComponent<MenuMechanics>().theGameHasEnded)
            return;
        
        if (GameObject.FindGameObjectWithTag("Canvas").GetComponent<MenuMechanics>().inShop == false)
        {
            if (Input.GetKey(KeyCode.LeftShift) && onTile == true && onDesiredTile && buildingEnabled)
            {
                inBuildingMode = true;
                characterModel.GetComponent<Animator>().SetBool("IsBuilding", true);

                if (switchedToBuildMode == false)
                {
                    FindObjectOfType<AudioManager>().Play("ToBuildMode");
                    dababyHammer.SetActive(true);
                    switchedToBuildMode = true;
                }

            } else
            {
                characterModel.GetComponent<Animator>().SetBool("IsBuilding", false);
                dababyHammer.SetActive(false);
                inBuildingMode = false;
                switchedToBuildMode = false;
            }

            positionInTilesx = transform.position.x / tileDistance;
            positionInTilesz = transform.position.z / tileDistance;

            RaycastHit[] hits = Physics.RaycastAll(cameraObject.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition));
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.tag == "Tile")
                {
                    tileMouseHitPos = hit.point;
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (inBuildingMode == false)
                        {
                            if (movementEnabled == false)
                                return;

                            hasMovedAtleastOnce = true;

                            if (hit.collider.gameObject != selectedTile)
                            {
                                if (buildTiles.Count > 0)
                                {
                                    foreach (GameObject tile in tileParent.GetComponent<TileMechanics>().Tiles)
                                    {
                                        buildTiles.Remove(tile);
                                    }
                                }

                                FindObjectOfType<AudioManager>().Play("MoveClick");
                                GameObject marker = Instantiate(moveMarker, hit.point, tileParent.transform.rotation);

                                marker.transform.parent = moveMarkerHolder.transform;
                            }

                            desiredPos = new Vector3(hit.transform.position.x, playerYOffset, hit.transform.position.z);
                        } else 
                        {
                            if (enableExclusiveBuildPos == true)
                            {
                                if (exclusiveBuildPos == hit.transform.position)
                                {
                                    if (hit.transform.GetComponent<TileState>().isSelected)
                                    {
                                        placedFirstTile = true;
                                        tileParent.GetComponent<TileMechanics>().RefreshTileList();
                                        characterModel.GetComponent<Animator>().Play("Place", 0, 0f);
                                        hit.transform.GetComponent<TileState>().UpgradeTile(true);
                                        FindObjectOfType<AudioManager>().Play("TileAction");
                                    }
                                }
                            } else
                            {
                                if (hit.transform.GetComponent<TileState>().isSelected)
                                {
                                    tileParent.GetComponent<TileMechanics>().RefreshTileList();
                                    characterModel.GetComponent<Animator>().Play("Place", 0, 0f);
                                    hit.transform.GetComponent<TileState>().UpgradeTile(true);
                                    FindObjectOfType<AudioManager>().Play("TileAction");
                                }
                            }

                        }
                        break;
                    }
                }
            }
        }
    }

    private void DoWalkingSounds()
    {
        if (walkTime == 0 && selectedTile != null)
        {
            if (selectedTile.GetComponent<TileState>().tileLevel == 0)
            {
                FindObjectOfType<AudioManager>().Play("StepSoundGrass");
            }
            else if (selectedTile.GetComponent<TileState>().tileLevel >= 1 && selectedTile.GetComponent<TileState>().tileLevel <= 3)
            {
                FindObjectOfType<AudioManager>().Play("StepSoundWood");
            }
            else if (selectedTile.GetComponent<TileState>().tileLevel >= 4 && selectedTile.GetComponent<TileState>().tileLevel <= 7)
            {
                FindObjectOfType<AudioManager>().Play("StepSoundStone");
            }
            else if (selectedTile.GetComponent<TileState>().tileLevel >= 8 && selectedTile.GetComponent<TileState>().tileLevel <= 9)
            {
                FindObjectOfType<AudioManager>().Play("StepSoundMetal");
            } else
            {
                FindObjectOfType<AudioManager>().Play("StepSoundSponge");
            }
        }
        
        walkTime += Time.deltaTime * walkingSoundSpeed;
        if (walkTime >= 1f)
        {
            walkTime = 0;
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
                    UpdateBuildTiles();
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
            {
                tileParent.GetComponent<MoneyMechanics>().RemoveCoinPos(positionHolderScript.startPosition);
                tileParent.GetComponent<MoneyMechanics>().RemoveCoin(col.gameObject);
            }
            
            FindObjectOfType<AudioManager>().Play("CoinPickup");
            FindObjectOfType<AudioManager>().Play("Pickup");
        }
        if (col.gameObject.tag == "Diamond")
        {
            Destroy(col.gameObject);
            tileParent.GetComponent<MoneyMechanics>().AddMoney(MoneyPerDiamond);
            tileParent.GetComponent<MoneyMechanics>().RemoveDiamondPos(col.transform.GetComponent<PositionHolder>().startPosition);
            tileParent.GetComponent<MoneyMechanics>().RemoveDiamond(col.gameObject);
            FindObjectOfType<AudioManager>().Play("CoinPickup");
            FindObjectOfType<AudioManager>().Play("Pickup");
            FindObjectOfType<AudioManager>().Play("PickupDiamond");
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Arrow" && positionedEXACTLYOnTile)
        {
            positionedOnArrow = true;
        }
    }

    public void UpdateBuildTiles()
    {
        foreach (GameObject tile in tileParent.GetComponent<TileMechanics>().Tiles)
        {
            buildTiles.Remove(tile);
        }
        Collider[] hitColliders = Physics.OverlapBox(this.transform.position, new Vector3(buildRadius, 1, buildRadius), Quaternion.identity);
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

    private void OnCollisionEnter(Collision col)
    {
        //geen idee of deze code nodig is??
        if (col.gameObject.tag == "Tile")
        {
            foreach (GameObject tile in tileParent.GetComponent<TileMechanics>().Tiles)
            {
                buildTiles.Remove(tile);
            }
        }
    }
}