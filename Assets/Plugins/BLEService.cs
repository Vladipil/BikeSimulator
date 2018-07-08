using UnityEngine;

public interface IBLECallback
{
    void OnDidUpdateState();
    void OnDidConnect();
    void OnDidDisconnect();
    void OnDidReceiveWriteRequests(string base64String);
}

public class BLEService
{
    const string JAVA_CLASS_NAME = "com.bluetooth.le.BLEService";

    public static void CreateServiceCentral()
    {
#if !UNITY_EDITOR && UNITY_ANDROID 
         using (AndroidJavaClass plugin = new AndroidJavaClass(JAVA_CLASS_NAME))
         {
             plugin.CallStatic("createServiceCentral");
         }
#endif
    }

    public static void StartService(string uuidString)
    {
#if !UNITY_EDITOR && UNITY_ANDROID 
         using (AndroidJavaClass plugin = new AndroidJavaClass(JAVA_CLASS_NAME))
         {
             plugin.CallStatic("start", uuidString);
         }
#endif
    }

    public static void PauseService(bool isPause)
    {
#if !UNITY_EDITOR && UNITY_ANDROID 
         using (AndroidJavaClass plugin = new AndroidJavaClass(JAVA_CLASS_NAME))
         {
             plugin.CallStatic("pause", isPause);
         }
#endif
    }

    public static void StopService()
    {
#if !UNITY_EDITOR && UNITY_ANDROID 
         using (AndroidJavaClass plugin = new AndroidJavaClass(JAVA_CLASS_NAME))
         {
             plugin.CallStatic("stop");
         }
#endif
    }

    public static void Write(byte[] data, int length, bool withResponse)
    {
#if !UNITY_EDITOR && UNITY_ANDROID 
         using (AndroidJavaClass plugin = new AndroidJavaClass(JAVA_CLASS_NAME))
         {
             plugin.CallStatic("write", data);
         }
#endif
    }
}
