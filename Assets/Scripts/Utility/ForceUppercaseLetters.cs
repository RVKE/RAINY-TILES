using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForceUppercaseLetters : MonoBehaviour
{

    public InputField inputField;

    void Start()
    {
        inputField.onValidateInput += delegate (string s, int i, char c) { return char.ToUpper(c); };
    }

    public char Val(char c)
    {
        c = char.ToUpper(c);
        return char.IsLetter(c) ? c : '\0';
    }
}
