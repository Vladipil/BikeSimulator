using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public OVRScreenFade screenFade;

    private string sceneToLoad;


    public void LoadMenu()
    {
        ChangeScene("Menu Scene");
    }

    public void ChangeScene(string name)
    {
        sceneToLoad = name;
        screenFade.FadeOut(() => SceneManager.LoadScene(sceneToLoad));
    }
}
