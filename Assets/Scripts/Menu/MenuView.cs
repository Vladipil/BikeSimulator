using System.Collections.Generic;
using UnityEngine;

public class MenuView : MonoBehaviour
{
    public SceneController sceneController;
    public CanvasGroup[] Tracks;

    private Dictionary<int, string> trackNames = new Dictionary<int, string>()
    {
        { 0, "Urban Scene" },
        { 1, "Off-Road Scene" },
    };

    private int currentTrackIdx;
    private int newTrackIdx;

    private float vel;
    private float fadeTime = 0.1f;
    private float epsilon = 0.01f;

    void Start()
    {
        currentTrackIdx = newTrackIdx = 0;
        for (int i = 0; i < Tracks.Length; i++)
        {
            Tracks[i].gameObject.SetActive(false);
        }
        Tracks[currentTrackIdx].gameObject.SetActive(true);
    }

    void Update()
    {
        if (newTrackIdx != currentTrackIdx)
        {
            if (1 - Tracks[newTrackIdx].alpha < epsilon)
            {
                Tracks[newTrackIdx].alpha = 1;
                Tracks[currentTrackIdx].alpha = 0;
                Tracks[currentTrackIdx].gameObject.SetActive(false);
                currentTrackIdx = newTrackIdx;
            }
            else
            {
                float a = Mathf.SmoothDamp(Tracks[newTrackIdx].alpha, 1, ref vel, fadeTime);
                Tracks[newTrackIdx].alpha = a;
                Tracks[currentTrackIdx].alpha = 1 - a;
            }
        }
    }

    private void SelectTrack(int index)
    {
        if (newTrackIdx == currentTrackIdx)
        {
            newTrackIdx = index;
            Tracks[index].alpha = 0;
            Tracks[index].gameObject.SetActive(true);
        }
    }


    public void PreviousButton()
    {
        SelectTrack(currentTrackIdx == 0 ? Tracks.Length - 1 : currentTrackIdx - 1);
    }

    public void NextButton()
    {
        SelectTrack((currentTrackIdx + 1) % Tracks.Length);
    }

    public void StartGame()
    {
        sceneController.ChangeScene(trackNames[newTrackIdx]);
    }
}
