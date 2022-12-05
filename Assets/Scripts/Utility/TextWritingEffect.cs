using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

// attach to UI Text component (with the full text already there)

public class TextWritingEffect: MonoBehaviour
{
	private TMP_Text text;

	void Awake()
	{
		if (GetComponent<TMP_Text>() != false)
        {
			text = GetComponent<TMP_Text>();
			text.text = "";
		}
	}

    public void SetText(string content, float letterDelay, string sound)
    {
		//if (this == null)
		//	return;

		text.text = "";
		StartCoroutine(DoTextAnimation(content, letterDelay, sound));
	}

	IEnumerator DoTextAnimation(string content, float letterDelay, string sound)
	{
		foreach (char letter in content)
		{
			text.text += letter;

			FindObjectOfType<AudioManager>().Play(sound);

			if (letter.ToString() != " ")
				yield return new WaitForSeconds(letterDelay);
		}
	}
}