using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text currentTimeText;
    public Text bestTimeText;

    float _timer;


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
            _timer += Time.deltaTime;
            currentTimeText.text = _timer.ToString("00:00");
        }
    }

    public void GameEnded()
    {
        float thisSeconds = _timer;
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
