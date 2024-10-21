using UnityEngine;
using UnityEngine.Analytics;
using System.Collections.Generic;

public class GameDataRecorder : MonoBehaviour
{
    private float startTime;
    private bool gameEnded = false;

    void Start()
    {
        startTime = Time.time;
    }

    public void RecordGameResult(bool isWin, int finalWave)
    {
        if (gameEnded)
            return;

        gameEnded = true;
        float totalGameTime = Time.time - startTime;
        string result = isWin ? "Win" : "Lose";

        // 创建事件数据
        Dictionary<string, object> eventData = new Dictionary<string, object>
        {
            { "totalGameTime", totalGameTime },
            { "result", result },
            { "finalWave", finalWave }
        };

        // 发送自定义事件
        Analytics.CustomEvent("gameOver", eventData);

        Debug.Log("Game data sent to Unity Analytics: " + eventData);
    }
}
