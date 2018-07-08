using UnityEngine;

public class BicycleUserController : MonoBehaviour
{

    private BicycleController m_bicycle;

    public float speed = 0;
    public float steering = 0;

    public static int maxLeft = 108;
    public static int maxRight = 216;

    private int rawSteering;


    private void OnEnable()
    {
        NetworkController.OnSpeedReceived += NetworkController_OnSpeedReceived;
        NetworkController.OnTurnReceived += NetworkController_OnTurnReceived;
    }

    private void OnDisable()
    {
        NetworkController.OnSpeedReceived -= NetworkController_OnSpeedReceived;
        NetworkController.OnTurnReceived -= NetworkController_OnTurnReceived;
    }

    private void Awake()
    {
        m_bicycle = GetComponent<BicycleController>();
    }

    private void FixedUpdate()
    {
#if UNITY_EDITOR
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        speed = Mathf.Clamp(speed + (v * 10 * Time.deltaTime), 0, 50);
        steering = Mathf.Clamp(steering + (h * Time.deltaTime), -1, 1);
#endif

        m_bicycle.Move(steering, speed);
    }

    private void NetworkController_OnSpeedReceived(string data)
    {
        float rawSpeed;
        if (float.TryParse(data, out rawSpeed))
        {
            speed = rawSpeed;
        }
    }

    private void NetworkController_OnTurnReceived(string data)
    {
        if (int.TryParse(data, out rawSteering))
        {
            steering = Mathf.InverseLerp(maxLeft, maxRight, rawSteering) * 2 - 1;
        }
    }

    public void ResetForward()
    {
        maxLeft = rawSteering - 100;
        maxRight = rawSteering + 100;
    }

    public void ResetLevel()
    {
        m_bicycle.ResetPosition();
        speed = 0;
        steering = 0;
    }
}
