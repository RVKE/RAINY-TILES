using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public UpgradeCard card;
    public bool boughtUpgrade;
    public bool canBuyUpgrade;
    public static int researchLevel = 1;
    public GameObject arrowPrefab;

    public Text nameText;
    public Text descriptionText;
    public Text priceText;

    public GameObject previousCard;

    //private GameObject canvas;

    void Start()
    {
        nameText.text = card.name;
        descriptionText.text = card.description;
        priceText.text = card.price.ToString() + " XP";
        //canvas = GameObject.FindGameObjectWithTag("Canvas");

        if (card.researchOrder == 1)
            return;

        StartCoroutine(WaitForPosition());
    }

    IEnumerator WaitForPosition()
    {
        yield return new WaitForEndOfFrame();

        GameObject newArrow;

        if (previousCard.transform.position.x < transform.position.x)
        {
            newArrow = Instantiate(arrowPrefab, transform.position + new Vector3(-150, 0, 0), Quaternion.Euler(0, 0, 0));
            newArrow.transform.parent = this.transform;
        }
        if (previousCard.transform.position.x > transform.position.x)
        {
            newArrow = Instantiate(arrowPrefab, transform.position + new Vector3(150, 0, 0), Quaternion.Euler(0, 0, 180));
            newArrow.transform.parent = this.transform;
        }
        if (previousCard.transform.position.y < transform.position.y)
        {
            newArrow = Instantiate(arrowPrefab, transform.position + new Vector3(0, -150, 0), Quaternion.Euler(0, 0, 90));
            newArrow.transform.parent = this.transform;
        }
        if (previousCard.transform.position.y > transform.position.y)
        {
            newArrow = Instantiate(arrowPrefab, transform.position + new Vector3(0, 150, 0), Quaternion.Euler(0, 0, -90));
            newArrow.transform.parent = this.transform;
        }

    }

    private void Update()
    {
        setButton();
    }

    public void setButton()
    {
        var children = GetComponentsInChildren<Transform>();
        foreach (var child in children)
            if (child.name == "Button")
            {
                if (boughtUpgrade == false)
                {
                    if (GameObject.FindGameObjectWithTag("TileParent").GetComponent<XpMechanics>().xp >= card.price && (previousCard.GetComponent<CardDisplay>().boughtUpgrade == true || card.researchOrder == 1)) // + && researchLevel >= card.researchOrder
                    {
                        child.GetComponent<Image>().color = new Color32(127, 207, 135, 255);
                        canBuyUpgrade = true;
                    }
                    else
                    {
                        child.GetComponent<Image>().color = new Color32(226, 120, 118, 255);
                        canBuyUpgrade = false;
                    }
                }
                else
                {
                    child.GetComponent<Image>().color = new Color32(129, 124, 161, 255);
                    canBuyUpgrade = false;
                }
            }
    }

    public void Upgrade()
    {
        FindObjectOfType<AudioManager>().Play("BuyButton");

        if (GameObject.FindGameObjectWithTag("TileParent").GetComponent<XpMechanics>().xp >= card.price && boughtUpgrade == false && (previousCard.GetComponent<CardDisplay>().boughtUpgrade == true || card.researchOrder == 1)) // + && researchLevel >= card.researchOrder
        {
            boughtUpgrade = true;
            GameObject.FindGameObjectWithTag("TileParent").GetComponent<XpMechanics>().RemoveXp(card.price);
            GameObject.FindGameObjectWithTag("TileParent").GetComponent<UpgradeMechanics>().SelectUpgrade(card.name);


            var children = GetComponentsInChildren<Transform>();
            foreach (var child in children)
            {
                if (child.name == "BuyText")
                {
                    child.GetComponent<Text>().text = "[UPGRADED]";
                }
                else if (child.name == "LockImage")
                {
                    child.GetComponent<Image>().enabled = false;
                }
            }
        } else
        {
            FindObjectOfType<AudioManager>().Play("BuyDeclined");
        }
    }
}
