using UnityEngine;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour
{
    public Button level1Button;
    public Button level2Button;
    public Button level3Button;
    public Button quitButton;

    void Start()
    {
        // Set the button texts
        SetButtonText(level1Button, "Level 1");
        SetButtonText(level2Button, "Level 2");
        SetButtonText(level3Button, "Level 3");
        SetButtonText(quitButton, "Quit");

        // Set up button click listeners if needed
        level1Button.onClick.AddListener(() => LoadLevel("Level1"));
        level2Button.onClick.AddListener(() => LoadLevel("Level2"));
        level3Button.onClick.AddListener(() => LoadLevel("Level3"));
        quitButton.onClick.AddListener(QuitGame);
    }

    void SetButtonText(Button button, string text)
    {
        Text buttonText = button.GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            buttonText.text = text;
        }
        else
        {
            Debug.LogWarning("Text component not found in button: " + button.name);
        }
    }

    void LoadLevel(string levelName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(levelName);
    }

    void QuitGame()
    {
        Application.Quit();
    }
}
