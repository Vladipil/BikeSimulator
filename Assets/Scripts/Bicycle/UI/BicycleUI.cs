using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BicycleUI : MonoBehaviour
{
    private Dictionary<string, Color> statusColors = new Dictionary<string, Color>()
    {
        { "Disconnected", Color.red },
        { "Connecting", Color.yellow },
        { "Connected", Color.green },
    };

    public Text speed_text;
    public Text odometer_text;
    public Text calories_text;
    public Slider steering_slider;
    public Text rawSteering_text;
    public Image status_image;

    private float distance = 0;
    private float currentSpeed = 0;
    private float targetSpeed = 0;
    private float damp_vel = 0;
    private float smoothTime = 0.1f;

    private void OnEnable()
    {
        NetworkController.OnStatusChanged += NetworkController_OnStatusChanged;
        NetworkController.OnSpeedReceived += NetworkController_OnSpeedReceived;
        NetworkController.OnDistanceReceived += NetworkController_OnDistanceReceived;
        NetworkController.OnTurnReceived += NetworkController_OnTurnReceived;
    }

    private void OnDisable()
    {
        NetworkController.OnStatusChanged -= NetworkController_OnStatusChanged;
        NetworkController.OnSpeedReceived -= NetworkController_OnSpeedReceived;
        NetworkController.OnDistanceReceived -= NetworkController_OnDistanceReceived;
        NetworkController.OnTurnReceived -= NetworkController_OnTurnReceived;
    }

    private void Start()
    {
        steering_slider.value = .5f;
        odometer_text.text = "0";
        speed_text.text = "0";
        calories_text.text = "0";
        rawSteering_text.text = "";
    }

    private void Update()
    {
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref damp_vel, smoothTime);
        speed_text.text = FloatToString(currentSpeed);
    }

    private void NetworkController_OnSpeedReceived(string data)
    {
        float speed;
        if(float.TryParse(data, out speed))
        {
            targetSpeed = speed;
        }
    }

    private void NetworkController_OnDistanceReceived(string data)
    {
        if (float.TryParse(data, out distance))
        {
            calcCalories();
        }
        odometer_text.text = data;
    }

    private void NetworkController_OnTurnReceived(string data)
    {
        int rawSteering;
        if (int.TryParse(data, out rawSteering))
        {
            steering_slider.value = Mathf.InverseLerp(BicycleUserController.maxLeft, BicycleUserController.maxRight, rawSteering);
            rawSteering_text.text = string.Format("{0} ... {1} ... {2}", BicycleUserController.maxLeft, rawSteering, BicycleUserController.maxRight);
        }
    }

    private void NetworkController_OnStatusChanged(string status)
    {
        status_image.color = statusColors[status];
    }

    private void calcCalories()
    {
        float calories = distance * 30;
        calories_text.text = FloatToString(calories);
    }

    private string FloatToString(float num)
    {
        return num >= 10 ? num.ToString("N0") : num.ToString("N1");
    }

    public void VibrateTest()
    {
        NetworkController.VibrateForSec(1);
    }
}
