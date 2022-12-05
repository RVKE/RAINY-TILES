using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XpMechanics : MonoBehaviour
{
    public int xp = 0;
    private int xpTextValue = 0;

    public Text xpTextGame;
    public Text xpTextShop;

    public float xpTextLerpTimeElapsed = 0;

    private GameObject canvas;

    private void Start()
    {
        canvas = GameObject.FindGameObjectWithTag("Canvas");
    }


    void Update()
    {
  
        if (xpTextLerpTimeElapsed < 1)
        {
            xpTextValue = (int)Mathf.Lerp(xpTextValue, xp, xpTextLerpTimeElapsed / 1);
            if (canvas.GetComponent<MenuMechanics>().inShop)
                xpTextLerpTimeElapsed += Time.fixedDeltaTime;
            else
                xpTextLerpTimeElapsed += Time.deltaTime;

            if (xpTextValue == xp - 2 || xpTextValue == xp + 2)
                xpTextValue = xp;
        }

        xpTextGame.text = xpTextValue.ToString();
        xpTextShop.text = xpTextValue.ToString();
    }

    public void AddXP(int newXP)
    {
        xp += newXP;
        xpTextLerpTimeElapsed = 0;
    }

    public void RemoveXp(int removedXP)
    {
        xp -= removedXP;
        xpTextLerpTimeElapsed = 0;
    }

    public void CorrectXPText()
    {
        xpTextValue = xp;
    }
}
