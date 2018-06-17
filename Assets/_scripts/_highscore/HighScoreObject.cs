using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreObject : MonoBehaviour
{
    public Text usernameText;
    public Text scoreText;
    public Text rankText;
    public GameObject mineObj;


    private void Awake() {
        if (mineObj) LeanTween.rotateZ(mineObj, -1f, 1f).setLoopPingPong().setEaseInOutSine();
    }

    public void FillScore(string username, int score, int rank, bool mine)
    {
        usernameText.text = username;
        scoreText.text = score.ToString();
        if (rankText) rankText.text = "#" + (rank + 1).ToString();
        if (mineObj) mineObj.SetActive(mine);
    }
}
