using UnityEngine;
using TechTweaking.Bluetooth;
using System.Collections;

public class NetworkController : MonoBehaviour
{
    private const string DeviceMacAddress = "98:D3:33:81:14:26";
    private const string DeviceName = "HC-05";

    private static BluetoothDevice device;
    private string line = "";

    public delegate void StatusChanged(string status);
    public delegate void DataReceived(string data);

    public static event StatusChanged OnStatusChanged;
    public static event DataReceived OnSpeedReceived;
    public static event DataReceived OnDistanceReceived;
    public static event DataReceived OnTurnReceived;

    private static string status = "Disconnected";
    private static bool currentVibrateState = false;
    private static float vibrateTime = 0;


    // Use this for initialization
    void Awake()
    {
        BluetoothAdapter.enableBluetooth(); //Force Enabling Bluetooth

        device = new BluetoothDevice();
        device.OnConnected += Device_OnConnected;
        device.OnDisconnected += Device_OnDisconnected;
        device.OnConnectionError += Device_OnConnectionError;

        //device.Name = DeviceName;
        device.MacAddress = DeviceMacAddress;
    }

    private void OnDestroy()
    {
        if (device != null)
        {
            device.OnConnected -= Device_OnConnected;
            device.OnDisconnected -= Device_OnDisconnected;
            device.OnConnectionError -= Device_OnConnectionError;
        }
        device.close();
    }

    private void Start()
    {
        updateStatus();
        connect();
    }

    public void connect()
    {
        updateStatus("Connecting");
        line = "";

        device.connect();
    }

    public void disconnect()
    {
        device.close();
    }

    private static void updateStatus()
    {
        if (OnStatusChanged != null)
            OnStatusChanged(status);
    }

    private static void updateStatus(string newStatus)
    {
        status = newStatus;
        updateStatus();
    }

    public static bool isConnected()
    {
        if (device == null)
            return false;
        return device.IsConnected;
    }

    public static void VibrateToggle()
    {
        SetVibrate(currentVibrateState);
        currentVibrateState = !currentVibrateState;
    }

    public static void SetVibrate(bool on)
    {
        SendToDevice(on ? "1" : "0");
    }

    public static void SendToDevice(string toSend)
    {
        if (device != null)
        {
            device.send(System.Text.Encoding.ASCII.GetBytes(toSend));
        }
    }

    public static void VibrateForSec(float time)
    {
        vibrateTime = Time.time + time;
    }

    public void HandleVibration()
    {
        if (vibrateTime >= Time.time && currentVibrateState == false)
        {
            currentVibrateState = true;
            SendToDevice("1");
        }
        else if (vibrateTime < Time.time && currentVibrateState == true)
        {
            currentVibrateState = false;
            SendToDevice("0");
        }
    }

    void Update()
    {
        ReadData();
    }

    private void ReadData()
    {
        if (device.IsReading)
        {

            byte[] msg = device.read();


            if (msg != null)
            {
                /* Send and read in this library use bytes. So you have to choose your own encoding.
				 * The reason is that different Systems (Android, Arduino for example) use different encoding.
				 */
                string data = System.Text.ASCIIEncoding.ASCII.GetString(msg);

                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i] == '(')
                    {
                        line = "";
                    }
                    else if (data[i] == ')')
                    {
                        HandleData(line);
                    }
                    else
                    {
                        line += data[i];
                    }
                }
            }
        }
    }

    private void HandleData(string data)
    {
        Debug.Log("Received: " + data);
        string[] parts = data.Split(':');
        switch (parts[0])
        {
            case "0": // speed
                if (OnSpeedReceived != null) OnSpeedReceived(parts[1]);
                break;
            case "1": // distance
                if (OnDistanceReceived != null) OnDistanceReceived(parts[1]);
                break;
            case "2": // turn
                if (OnTurnReceived != null) OnTurnReceived(parts[1]);
                break;
            default:
                Debug.LogError("Unrecognized data code " + parts[0]);
                break;
        }
    }

    private IEnumerator RetryConnectingAfterDelay(float delay = 0.1f)
    {
        yield return new WaitForSeconds(delay);
        if (!isConnected())
            connect();
    }

    private void RetryConnection()
    {
        StartCoroutine(RetryConnectingAfterDelay());
    }

    private void Device_OnConnectionError(BluetoothDevice arg1, string arg2)
    {
        updateStatus("Disconnected");
        RetryConnection();
    }

    private void Device_OnDisconnected(BluetoothDevice obj)
    {
        updateStatus("Disconnected");
        RetryConnection();
    }

    private void Device_OnConnected(BluetoothDevice obj)
    {
        updateStatus("Connected");
    }
}