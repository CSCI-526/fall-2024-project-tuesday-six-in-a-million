using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSwitcher : MonoBehaviour
{   
    public string name;
    // 跳转到指定场景
    public void LoadSceneByName()
    {   
        Debug.Log("Attempting to load scene: ");
        SceneManager.LoadScene("Level 1");
    }

    // 或通过场景索引加载
    public void LoadSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
