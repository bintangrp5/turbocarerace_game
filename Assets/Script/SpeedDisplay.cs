using UnityEngine;
using TMPro;

public class SpeedDisplay : MonoBehaviour
{
    public TextMeshProUGUI speedText;
    public Rigidbody rb;

    void Update()
    {
        float speed = rb.linearVelocity.magnitude * 3.6f;

        int unit = PlayerPrefs.GetInt("SpeedUnit", 0);

        if (unit == 0)
        {
            speedText.text = speed.ToString("0") + " km/h";
        }
        else
        {
            float mph = speed * 0.621371f;
            speedText.text = mph.ToString("0") + " mph";
        }
    }
}