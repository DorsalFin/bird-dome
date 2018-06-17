using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScore
{
    public string username;
    public int score;
    public int rank;


    public HighScore(string username, int score, int rank = 0)
    {
        this.username = username;
        this.score = score;
        this.rank = rank;
    }
}

public class HighScores : MonoBehaviour
{
    const string privateCode = "cF9vu5IW60eDhFiLGJ6DVQV9Vd9XCLAEyVLOVOh_4JmQ";
    const string publicCode = "5b123d68191a8a0bcc390445";
    const string webUrl = "http://dreamlo.com/lb/";

    public static HighScores Instance;
    public int highScoresToFetch;
    [HideInInspector] public HighScore myTopScore = null;
    [HideInInspector] public HighScore lastScore = null;
    [HideInInspector] public bool fetchingTopScore = false;
    [HideInInspector] public bool uploadingNewHighScore = false;
    HighScore[] _highScoresList;


    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(transform);

        // TEST ONLY
        //AddNewHighScore("pocket", 268, 214);
        //AddNewHighScore("mckrindle", 224, 15);
        //AddNewHighScore("log roll", 101, 156);
        //AddNewHighScore("breast", 86, 29);
        //
    }

    public void AddNewHighScore(string username, int score, int seconds) {
        StartCoroutine(UploadNewHighScore(username, score, seconds));
    }

    IEnumerator UploadNewHighScore(string username, int score, int seconds)
    {
        uploadingNewHighScore = true;
        lastScore = null;
        WWW www = new WWW(webUrl + privateCode + "/add/" + WWW.EscapeURL(username) + "/" + score + "/" + seconds + "/" + WWW.EscapeURL(SystemInfo.deviceUniqueIdentifier));
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            Debug.Log("uploaded high score : " + username + "," + score);
            SetTopScoreForUsername(username);
        }
        else
            Debug.Log("error uploading score : " + www.error);

        uploadingNewHighScore = false;
    }

    public void GetHighScores(int count) {
        StartCoroutine("DownloadHighScores");
    }

    IEnumerator DownloadHighScores()
    {
        WWW www = new WWW(webUrl + publicCode + "/pipe/" + highScoresToFetch);
        yield return www;

        if (string.IsNullOrEmpty(www.error))
            FormatHighScores(www.text);
        else
            Debug.Log("error downloading scores : " + www.error);
    }

    void FormatHighScores(string textStream)
    {
        string[] entries = textStream.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        _highScoresList = new HighScore[entries.Length];

        for (int i = 0; i < entries.Length; i++)
        {
            string[] entryInfo = entries[i].Split(new char[] { '|' });
            string username = entryInfo[0];
            int score = int.Parse(entryInfo[1]);
            int rank = int.Parse(entryInfo[entryInfo.Length - 1]);
            _highScoresList[i] = new HighScore(username, score, rank);
        }

        Menu.Instance.HighScoresReceived(_highScoresList);
    }

    public void CheckUsername(string username) {
        StartCoroutine(CheckDataForUsername(username));
    }

    IEnumerator CheckDataForUsername(string username)
    {
        WWW www = new WWW(webUrl + publicCode + "/pipe-get/" + username);
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            Debug.Log("successful response, www text is : " + www.text);
            if (www.text == string.Empty)
                Menu.Instance.UsernameResponse(true);
            else
            {
                // check if this username was created on this device
                string[] entryInfo = www.text.Split(new char[] { '|' });
                string deviceId = entryInfo[3];
                Menu.Instance.UsernameResponse(deviceId == WWW.EscapeURL(SystemInfo.deviceUniqueIdentifier));
            }
        }
        else
            Debug.Log("error checking username : " + www.error);
    }

    public void SetTopScoreForUsername(string username) {
        StartCoroutine(FetchScoreForusername(username));
    }

    IEnumerator FetchScoreForusername(string username)
    {
        fetchingTopScore = true;
        myTopScore = null;
        WWW www = new WWW(webUrl + publicCode + "/pipe-get/" + username);
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            Debug.Log("successful response, www text is : " + www.text);
            if (www.text == string.Empty)
                myTopScore = null;
            else
            {
                // check if this username was created on this device
                string[] entryInfo = www.text.Split(new char[] { '|' });
                int score = int.Parse(entryInfo[1]);
                int rank = int.Parse(entryInfo[entryInfo.Length - 1]);
                myTopScore = new HighScore(username, score, rank);
            }
        }
        else
        {
            Debug.Log("error checking username : " + www.error);
            myTopScore = null;
        }
        fetchingTopScore = false;
    }
}
