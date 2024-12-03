using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelSelector : MonoBehaviour
{
    public void LoadLevelByName(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void LoadLevelByIndex(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }
}
