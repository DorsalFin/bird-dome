using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EarthScene : MonoBehaviour
{
    public float delay;
    public GameObject earthObject;
    public ParticleSystem explodeParticlePrefab;
    public GameObject[] buttons;

    //public GameObject newBestPanel;
    //public HighScoreObject newBestScoreObject;
    //public GameObject notBestPanel;
    //public HighScoreObject notBestScoreObject;
    //public HighScoreObject notBestTopScoreObject;
	

	void Start ()
    {
#if UNITY_EDITOR
        Cursor.lockState = CursorLockMode.None;
#endif
        StartCoroutine("DestroyEarthInSeconds");
	}
	
	IEnumerator DestroyEarthInSeconds()
    {
        yield return new WaitForSeconds(delay);
        Instantiate(explodeParticlePrefab, earthObject.transform.position, Quaternion.identity);
        Destroy(earthObject);
        Camera.main.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(1);

        // send back to gamemanager to award beaks
        GameManager.Instance.GiveBeaks();

        //while (HighScores.Instance.uploadingNewHighScore || HighScores.Instance.fetchingTopScore)
        //    yield return null;

        //// was this a new high score?
        //bool newHighScore = HighScores.Instance.myTopScore == null || HighScores.Instance.lastScore == null || (HighScores.Instance.lastScore.score > HighScores.Instance.myTopScore.score);
        //if (newHighScore)
        //{
        //    HighScore hs = HighScores.Instance.myTopScore == null ? HighScores.Instance.lastScore : HighScores.Instance.myTopScore;
        //    newBestScoreObject.FillScore(hs.username, hs.score, hs.rank, true);
        //}
        //else
        //{
        //    notBestTopScoreObject.FillScore(HighScores.Instance.myTopScore.username, HighScores.Instance.myTopScore.score, HighScores.Instance.myTopScore.rank, true);
        //    notBestScoreObject.FillScore(HighScores.Instance.lastScore.username, HighScores.Instance.lastScore.score, HighScores.Instance.lastScore.rank, true);
        //}

        //newBestPanel.SetActive(newHighScore);
        //notBestPanel.SetActive(!newHighScore);

        float d = 0f;
        foreach (GameObject button in buttons)
        {
            button.SetActive(true);
            LeanTween.scale(button, Vector3.one * 1.05f, 1f).setEase(LeanTweenType.punch).setLoopType(LeanTweenType.linear).setDelay(d);
            d += 0.5f;
        }
    }

    public void LoadScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
}
