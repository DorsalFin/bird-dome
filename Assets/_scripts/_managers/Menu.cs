using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class Menu : MonoBehaviour
{
    public static Menu Instance;

    public GameObject usernamePanel;
    public InputField usernameInput;
    public GameObject usernameEntryPanel;
    public GameObject usernameLoadingPanel;
    public GameObject usernameTakenObject;
    public RectTransform usernameGuideText;

    public GameObject mainMenuPanel;
    public GameObject[] buttons;
    public Text usernameText;

    public GameObject highScorePanel;
    public GameObject highScoreLoadingPanel;
    public GameObject highScoreLoadedPanel;
    public HighScoreObject[] highScoreObjects;
    public GameObject myScorePanel;
    public HighScoreObject myScore;


    private void Awake()
    {
        Instance = this;

        if (HighScores.Instance == null)
            Instantiate(Resources.Load("HighScores"));
    }

    private void Start()
    {
        string username = PlayerPrefs.GetString("username", string.Empty);
        if (username == string.Empty)
            SetPanel(usernamePanel);
        else
        {
            usernameInput.text = username;
            usernameText.text = usernameInput.text;
            SetPanel(mainMenuPanel);
            HighScores.Instance.SetTopScoreForUsername(username);
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            float delay = i * 0.25f;
            LeanTween.scale(buttons[i], Vector3.one * 1.05f, 1f).setEase(LeanTweenType.punch).setLoopType(LeanTweenType.linear).setDelay(delay);
        }
        LeanTween.rotateZ(usernameTakenObject, 2f, 1f).setLoopPingPong().setEaseInOutSine();
    }

    public void UsernameSubmit()
    {
        Regex r = new Regex("^[a-zA-Z0-9]*$");
        if ((usernameInput.text.Length >= 3 && usernameInput.text.Length <= 12) && r.IsMatch(usernameInput.text))
        {
            HighScores.Instance.CheckUsername(usernameInput.text);
            usernameLoadingPanel.SetActive(true);
            usernameEntryPanel.SetActive(false);
        }
        else
        {
            usernameGuideText.GetComponent<Text>().color = new Color(228f / 255f, 58f / 255f, 47f / 255f);
            LeanTween.colorText(usernameGuideText, Color.white, 2f).setEaseInSine();
        }
    }

    public void SetPanel(GameObject panel)
    {
        usernamePanel.SetActive(panel == usernamePanel);
        mainMenuPanel.SetActive(panel == mainMenuPanel);
        highScorePanel.SetActive(panel == highScorePanel);

        if (usernamePanel.activeSelf)
        {
            usernameEntryPanel.SetActive(true);
            usernameLoadingPanel.SetActive(false);
            usernameTakenObject.SetActive(false);
        }

        if (!highScorePanel.activeSelf)
            StopCoroutine("WaitForTopScore");
    }

    public void EnterDomeButtonPressed() {
        SceneManager.LoadScene("bird attack");
    }

    public void UsernameResponse(bool okay)
    {
        if (okay)
        {
            PlayerPrefs.SetString("username", usernameInput.text);
            usernameText.text = usernameInput.text;
            SetPanel(mainMenuPanel);
            HighScores.Instance.SetTopScoreForUsername(usernameInput.text);
        }
        else
        {
            usernameLoadingPanel.SetActive(false);
            usernameEntryPanel.SetActive(true);
            usernameTakenObject.SetActive(true);
        }
    }

    public void HighScoresButtonPressed()
    {
        highScoreLoadingPanel.SetActive(true);
        highScoreLoadedPanel.SetActive(false);
        SetPanel(highScorePanel);

        HighScores.Instance.GetHighScores(highScoreObjects.Length);
    }

    public void HighScoresReceived(HighScore[] highScoresList)
    {
        // may have pressed back and left whilst loading
        if (!highScoreLoadingPanel.activeInHierarchy)
            return;

        bool mineFound = false;
        for (int i = 0; i < highScoreObjects.Length; i++)
        {
            bool active = i < highScoresList.Length;
            highScoreObjects[i].gameObject.SetActive(active);
            if (active)
            {
                bool mine = highScoresList[i].username == PlayerPrefs.GetString("username");
                highScoreObjects[i].FillScore(highScoresList[i].username, highScoresList[i].score, highScoresList[i].rank, mine);
                if (mine) mineFound = true;
            }
        }

        if (!mineFound)
        {
            //if we're not in the top 3, fetch our top score
            StartCoroutine("WaitForTopScore");
        }
        else
        {
            myScorePanel.SetActive(false);
            highScoreLoadedPanel.SetActive(true);
            highScoreLoadingPanel.SetActive(false);
        }
    }

    IEnumerator WaitForTopScore()
    {
        while (HighScores.Instance.fetchingTopScore)
            yield return null;

        if (HighScores.Instance.myTopScore != null)
        {
            myScore.FillScore(HighScores.Instance.myTopScore.username, HighScores.Instance.myTopScore.score, HighScores.Instance.myTopScore.rank, true);
            myScorePanel.SetActive(true);
            highScoreLoadedPanel.SetActive(true);
            highScoreLoadingPanel.SetActive(false);
        }
        else
        {
            myScorePanel.SetActive(false);
            highScoreLoadedPanel.SetActive(true);
            highScoreLoadingPanel.SetActive(false);
        }
    }
}
