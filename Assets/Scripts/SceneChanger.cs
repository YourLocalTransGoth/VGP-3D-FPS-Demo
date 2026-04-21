using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    private const string SceneName = "Level1";

    public void ChangeScene()
    {
        SceneManager.LoadScene(SceneName);
    }
}
