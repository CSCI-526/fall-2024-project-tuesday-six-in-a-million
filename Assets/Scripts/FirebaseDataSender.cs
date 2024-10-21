using UnityEngine;
using System.Collections;
using System.Text;
using UnityEngine.Networking;

public class FirebaseDataSender : MonoBehaviour
{
    private string databaseURL = "https://test-32dcb-default-rtdb.firebaseio.com/";

    public static FirebaseDataSender Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SendGameResult(bool isWin, int finalWave, float totalGameTime)
    {   
        // create data object
        GameResultData data = new GameResultData
        {
            totalGameTime = totalGameTime,
            result = isWin ? "Win" : "Lose",
            finalWave = finalWave,
            timestamp = GetTimestamp()
        };

        //  Serialize the data object as JSON
        string json = JsonUtility.ToJson(data);

        // Starting a concatenation to send data
        StartCoroutine(PostDataToFirebase(json));
    }

    private IEnumerator PostDataToFirebase(string json)
    {
        // Construct the request URL with a unique node 
        string url = databaseURL + "gameResults.json";

        // create UnityWebRequest,  use POST 
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        // Setting the request header
        request.SetRequestHeader("Content-Type", "application/json");

        // Send a request and wait for a response
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Data sent to Firebase successfully.");
        }
        else
        {
            Debug.LogError("Error sending data to Firebase: " + request.error);
        }
    }

    private string GetTimestamp()
    {
        return System.DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
    }
}

[System.Serializable]
public class GameResultData
{
    public float totalGameTime;
    public string result;
    public int finalWave;
    public string timestamp;
}
