using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileState : MonoBehaviour
{
    public int tileLevel = 0;
    public int tileHealth = 0;
    public int UpgradePrice;
    public int NextUPgradePrice;
    public float TimeToRain = 0.0f;
    public float TimeThatItHasToRain = 0.0f;
    public float WaitingTime = 0.0f;
    public GameObject tilePlaceParticle;
    public GameObject tileParent;
    public TileLevel levelCard;
    public Text UpgradeText;
    public bool isTile = false;
    public bool isWaiting = false;
    public bool isRaining = false;
    public bool StopRainCountDown = false;
    private GameObject canvas;
    private GameObject player;
    public Vector3 startPos; // <- handig voor andere scripts
    public bool isSelected;
    public bool mouseOverTile;

    private GameObject model;

    [SerializeField] private GameObject tilePlaceCanvas;
    private bool tilePlaceCanvasInstantiated;

    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material highlightedMaterial;
    [SerializeField] private Material buildableMaterial;
    [SerializeField] private Material unbuildableMaterial;
    [SerializeField] private Material damageMaterial;
    [SerializeField] private Material mouseOverMaterial;

    void Start()
    {
        model = this.gameObject.transform.GetChild(0).gameObject;
        tileParent = GameObject.FindGameObjectWithTag("TileParent");
        player = GameObject.FindGameObjectWithTag("Player");
        canvas = GameObject.FindGameObjectWithTag("Canvas");
        startPos = this.transform.position;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.E) && player.GetComponent<Player>().cheatsEnabled)
        {
            UpgradeTile(false);
        }

        if (player.GetComponent<Player>().inBuildingMode && player.GetComponent<Player>().moving == false)
        {
            if (player.GetComponent<Player>().buildTiles.Contains(this.gameObject))
            {
                if (isSelected)
                    model.GetComponent<MeshRenderer>().material = highlightedMaterial;
                else
                    model.GetComponent<MeshRenderer>().material = buildableMaterial;
            } else
            {
                model.GetComponent<MeshRenderer>().material = unbuildableMaterial;
            }
        } else
        {
            if (!canvas.GetComponent<MenuMechanics>().theGameHasEnded)
                HandleDamageMaterial();
        }
        
        if (mouseOverTile && player.GetComponent<Player>().inBuildingMode == false && tileParent.GetComponent<TileMechanics>().enableMouseOverTileMaterial && canvas.GetComponent<MenuMechanics>().gamePaused == false)
        {
            model.GetComponent<MeshRenderer>().material = mouseOverMaterial;
        }

        if (canvas.GetComponent<MenuMechanics>().theGameHasEnded)
            model.GetComponent<MeshRenderer>().material = defaultMaterial;

        /* //oud
        if ((tileParent.GetComponent<MoneyMechanics>().money - UpgradePrice) > 0)
        {
            tileParent.GetComponent<TileMechanics>().UpgradeText.color = new Color(1f, 1f, 1f);
            tileParent.GetComponent<TileMechanics>().UpgradeImage.color = new Color(1f, 1f, 1f);
        }
        else
        {
            tileParent.GetComponent<TileMechanics>().UpgradeText.color = new Color(0.9f, 0.3f, 0.3f);
            tileParent.GetComponent<TileMechanics>().UpgradeImage.color = new Color(0.9f, 0.3f, 0.3f);
        }*/

        if (canvas.GetComponent<MenuMechanics>().gamePaused == true || canvas.GetComponent<MenuMechanics>().inShop == true && canvas.GetComponent<MenuMechanics>().theGameHasEnded == true)
            return;

        if (isWaiting == true)
        {
            if (TimeThatItHasToRain < 6.9f)
            {
                TimeThatItHasToRain += 1 * Time.deltaTime;
            }
        }

        if (isRaining == true && isTile == true)
        {
            if (tileHealth <= 0)
            {
                DowngradeTile();
            }
            else
            {
                tileHealth = tileHealth - tileParent.GetComponent<TileMechanics>().RainDamage;
            }

        }

        if (isRaining == true && isTile == false)
        {
            // waterlevel ++
            if (tileParent.GetComponent<WaterMechanics>().waterAmount < tileParent.GetComponent<WaterMechanics>().maxWaterAmount)
            {
                tileParent.GetComponent<WaterMechanics>().waterAmount += Time.deltaTime * tileParent.GetComponent<WaterMechanics>().rainDamageAmount;
            }
            else
            {
                tileParent.GetComponent<WaterMechanics>().waterAmount = tileParent.GetComponent<WaterMechanics>().maxWaterAmount;
                if (tileParent.GetComponent<WaterMechanics>().houseHealth > 0)
                {
                    tileParent.GetComponent<WaterMechanics>().houseHealth -= Time.deltaTime * tileParent.GetComponent<WaterMechanics>().houseDamageAmount;
                }
            }
        }
    }

    public void UpgradeTile(bool withCameraShake)
    {
        if ((tileParent.GetComponent<MoneyMechanics>().money - NextUPgradePrice) > 0 && tileLevel <= 8)
        {
            foreach (GameObject tilePlaceCanvas in GameObject.FindGameObjectsWithTag("TilePlaceCanvas"))
            {
                Destroy(tilePlaceCanvas);
            }

            FindObjectOfType<AudioManager>().Play("UpgradeTile");
            tileLevel += 1;
            SwitchLevels();

            if (withCameraShake)
                CameraShake.Shake(0.07f, 0.07f);

            tileParent.GetComponent<XpMechanics>().AddXP(100 * tileLevel);
            tileParent.GetComponent<MoneyMechanics>().RemoveMoney(UpgradePrice);
            tileParent.GetComponent<TileMechanics>().RefreshTileList();

        } else
        {
            FindObjectOfType<AudioManager>().Play("BuildRefused");
            tileParent.GetComponent<TileMechanics>().playWarningAnim();

        }
    }

    private void HandleDamageMaterial() {
        if (isRaining)
        {
            damageMaterial.color = new Color(1, (Mathf.Sin(Time.time * 10) / 8) + 0.875f, (Mathf.Sin(Time.time * 10) / 8) + 0.875f);
            model.GetComponent<MeshRenderer>().material = damageMaterial;
        } else
        {
            model.GetComponent<MeshRenderer>().material = defaultMaterial;
        }
    }

    void OnMouseOver()
    {
        mouseOverTile = true;

        if (player.GetComponent<Player>().buildTiles.Contains(this.gameObject))
        {
            isSelected = true;

            if (player.GetComponent<Player>().inBuildingMode && tilePlaceCanvasInstantiated == false && tileLevel < 9 && canvas.GetComponent<MenuMechanics>().theGameHasEnded == false)
            {
                tilePlaceCanvasInstantiated = true;
                InstantiateTilePlaceCanvas();
            }
        }

        if (player.GetComponent<Player>().inBuildingMode == false)
        {

            foreach (GameObject tilePlaceCanvas in GameObject.FindGameObjectsWithTag("TilePlaceCanvas"))
            {
                Destroy(tilePlaceCanvas);
            }

            tilePlaceCanvasInstantiated = false;
        }

    }

    private void InstantiateTilePlaceCanvas()
    {
        GameObject daCanvas = Instantiate(tilePlaceCanvas, transform.position, transform.rotation);
        daCanvas.GetComponent<TilePlaceCanvas>().SetTilePlaceCanvas(tileLevel);
    }

    private void OnMouseExit()
    {
        foreach (GameObject tilePlaceCanvas in GameObject.FindGameObjectsWithTag("TilePlaceCanvas"))
        {
            Destroy(tilePlaceCanvas);
        }

        tilePlaceCanvasInstantiated = false;

        mouseOverTile = false;
        isSelected = false;
    }

    public void DowngradeTile()
    {
        if (tileLevel > 0)
        {
            foreach (GameObject tilePlaceCanvas in GameObject.FindGameObjectsWithTag("TilePlaceCanvas"))
            {
                Destroy(tilePlaceCanvas);
            }
            tileLevel = 0;
            FindObjectOfType<AudioManager>().Play("TileBreaking");
            FindObjectOfType<AudioManager>().Play("TileRemoved");
            SwitchLevels();
        }
        tileParent.GetComponent<TileMechanics>().RefreshTileList();
    }

    public void ReplaceTileLevel(TileLevel tileLevelCard)
    {
        GameObject tempTilePlaceParticle = Instantiate(tilePlaceParticle, transform.position, Quaternion.identity);
        GameObject tempTilePrefab = tileLevelCard.prefab;
        GameObject tempTile = Instantiate(tempTilePrefab, transform.position, Quaternion.identity);
        tempTilePlaceParticle.transform.parent = tempTile.transform;
        tempTile.transform.parent = tileParent.transform;
        tempTile.GetComponent<TileState>().tileLevel = tileLevel;
        tempTile.GetComponent<TileState>().tileHealth = tileHealth = tileLevelCard.health;
        tempTile.GetComponent<TileState>().isTile = isTile = tileLevelCard.isTile;
        tempTile.GetComponent<TileState>().UpgradePrice = UpgradePrice = tileLevelCard.price;
        tempTile.GetComponent<TileState>().NextUPgradePrice = NextUPgradePrice = tileLevelCard.nextPrice;
        player.GetComponent<Player>().buildTiles.Remove(this.gameObject);
        Destroy(gameObject);
        player.GetComponent<Player>().buildTiles.Add(tempTile);
    }

    void SwitchLevels()
    {
        switch (tileLevel)
        {
            case 0:
                levelCard = tileParent.GetComponent<TileMechanics>().level0Card;
                ReplaceTileLevel(levelCard);
                break;
            case 1:
                levelCard = tileParent.GetComponent<TileMechanics>().level1Card;
                ReplaceTileLevel(levelCard);
                break;
            case 2:
                levelCard = tileParent.GetComponent<TileMechanics>().level2Card;
                ReplaceTileLevel(levelCard);
                break;
            case 3:
                levelCard = tileParent.GetComponent<TileMechanics>().level3Card;
                ReplaceTileLevel(levelCard);
                break;
            case 4:
                levelCard = tileParent.GetComponent<TileMechanics>().level4Card;
                ReplaceTileLevel(levelCard);
                break;
            case 5:
                levelCard = tileParent.GetComponent<TileMechanics>().level5Card;
                ReplaceTileLevel(levelCard);
                break;
            case 6:
                levelCard = tileParent.GetComponent<TileMechanics>().level6Card;
                ReplaceTileLevel(levelCard);
                break;
            case 7:
                levelCard = tileParent.GetComponent<TileMechanics>().level7Card;
                ReplaceTileLevel(levelCard);
                break;
            case 8:
                levelCard = tileParent.GetComponent<TileMechanics>().level8Card;
                ReplaceTileLevel(levelCard);
                break;
            case 9:
                levelCard = tileParent.GetComponent<TileMechanics>().level9Card;
                ReplaceTileLevel(levelCard);
                break;
            /*case 10:
                levelCard = tileParent.GetComponent<TileMechanics>().level10Card;
                ReplaceTileLevel(levelCard);
                break;*/
        }
    }
}