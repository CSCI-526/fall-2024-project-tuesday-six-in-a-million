using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Reset : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetGame()
    {
        StartCoroutine(ResetGameCoroutine());
    }
     private IEnumerator ResetGameCoroutine()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
        yield return null; // 等待一幀，確保加載穩定
        Time.timeScale = 1; // 重置遊戲時間
    }
}
