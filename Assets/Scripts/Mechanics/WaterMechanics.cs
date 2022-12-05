using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterMechanics : MonoBehaviour
{

    public float waterAmount;
    public float waterPercentage;
    public float healthPercentage;
    public float houseHealth;
    public float maxWaterAmount;
    public float houseDamageAmount;
    public float rainDamageAmount;

    public Text WaterPercentage;
    public Text HealthPercentage;

    public GameObject waterBar;
    public GameObject healthBar;

    public GameObject waterObject;

    public GameObject canvas;

    private bool houseHasDied = false;

    void Start()
    {
        waterAmount = 0;
        maxWaterAmount = 1000;
        houseHealth = 100;
    }

    void Update()
    {
        waterPercentage = (waterAmount / maxWaterAmount) * 100;
        if (Mathf.Ceil(waterPercentage) < 0)
        {
            WaterPercentage.text = 0.ToString() + "%";
        }
        else
        {
            WaterPercentage.text = Mathf.Ceil(waterPercentage).ToString() + "%";
        }
        HealthPercentage.text = Mathf.Ceil(houseHealth).ToString() + "%";


        waterBar.transform.localScale = new Vector3((waterAmount / 1000), waterBar.transform.localScale.y, waterBar.transform.localScale.z);
        healthBar.transform.localScale = new Vector3((houseHealth / 100), healthBar.transform.localScale.y, healthBar.transform.localScale.z);
        //calculation for scaling water object
        waterObject.transform.localScale = new Vector3(0.9975f, ((waterAmount / 1800) + 0.1f), 1);
        waterObject.transform.position = new Vector3(-0.0063f, (((waterAmount / 1800)*2) - 1.425f), 0);

        if (waterAmount >= maxWaterAmount && houseHealth > 0 && (canvas.GetComponent<MenuMechanics>().gamePaused == false && canvas.GetComponent<MenuMechanics>().inShop == false))
        {
            houseHealth -= houseDamageAmount * Time.deltaTime;
        }
        if (houseHealth <= 0 && houseHasDied == false && canvas.GetComponent<MenuMechanics>().theGameHasEnded == false)
        {
            canvas.GetComponent<MenuMechanics>().theGameHasEnded = true;
            houseHasDied = true;
            canvas.GetComponent<MenuMechanics>().EndGame(false);
        }
        if (waterBar.transform.localScale.x < 0)
        {
            waterBar.transform.localScale = new Vector3(0, waterBar.transform.localScale.y, waterBar.transform.localScale.z);
        }
    }
}
