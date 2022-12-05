using UnityEngine;

public class HueShifter : MonoBehaviour
{
    public float Speed;
    private Light rend;

    void Start()
    {
        rend = GetComponent<Light>();
    }

    void Update()
    {
        rend.color = HSBColor.ToColor(new HSBColor(Mathf.PingPong(Time.time * Speed, 1), 1, 1));
    }
}
