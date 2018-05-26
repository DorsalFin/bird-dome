using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text currentTimeText;
    public Text bestTimeText;

    float _totalSeconds;


    private void Awake()
    {
        float bestSeconds = PlayerPrefs.GetFloat("BestTime", 0f);
        if (bestSeconds > 0f)
            bestTimeText.text = SecondsToString(bestSeconds);
    }

    private void Update()
    {
        if (GameManager.Instance.IsPlaying())
        {
            _totalSeconds += Time.deltaTime;
            currentTimeText.text = SecondsToString(_totalSeconds);
        }
    }

    public void GameEnded()
    {
        float thisSeconds = _totalSeconds;
        float bestSeconds = PlayerPrefs.GetFloat("BestTime", Mathf.Infinity);
        if (thisSeconds < bestSeconds)
        {
            bestTimeText.text = SecondsToString(thisSeconds);
            PlayerPrefs.SetFloat("BestTime", thisSeconds);
        }
    }

    string SecondsToString(float totalSeconds)
    {
        int seconds = Mathf.RoundToInt(totalSeconds % 60);
        int minutes = Mathf.RoundToInt(totalSeconds / 60);
        return minutes.ToString("00") + ":" + seconds.ToString("00");
    }
}
