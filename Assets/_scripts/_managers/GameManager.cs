using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public float ClayMoveAmt = 1;
    public float ClayJukeRadius = 1;

    public float worldRadius = 50f;
    public Dome dome;
    public BirdSpawner birdSpawner;
    public UIManager ui;
    //public DayNightManager day;
    public RadarManager radar;
    //public VirtualCameraShaker virtualCameraShaker;
    public Camera watchCamera;
    public HighScores highScore;

    public GameObject[] deathPrefabs;

    public Vector2 sensitivityBounds = new Vector2();

    public bool IsPlaying() { return _playing; }
    bool _playing;
    int _birdsKilled = 0;
    int _points = 0;
    float _seconds = 0;


    private void Awake()
    {
        Instance = this;
        radar = FindObjectOfType<RadarManager>();
        _playing = true;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.C))
            Cursor.lockState = Cursor.lockState == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;
#endif

        if (_playing)
            _seconds += Time.deltaTime; 
    }

    public void BirdDown(int points)
    {
        if (points > 0)
            AddPoints(points);
        _birdsKilled++;
        birdSpawner.aliveBirds--;
    }

    public void AddPoints(int points)
    {
        _points += points;
        ui.AddScore(points, _points);
    }

    public void EndGame()
    {
        if (dome.IsDead()) return;

        dome.Dead();
        _playing = false;

        // send score to high score site
        int total = _points * birdSpawner.ReachedWave();

        Debug.Log("TODO: high scores?");
        //if (HighScores.Instance.myTopScore == null || total > HighScores.Instance.myTopScore.score)
        //    HighScores.Instance.AddNewHighScore(PlayerPrefs.GetString("username"), total, Mathf.CeilToInt(_seconds));
        //else
        //    HighScores.Instance.lastScore = new HighScore(PlayerPrefs.GetString("username"), total);

        ui.settingsButton.SetActive(false);
        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(2f);
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(2f);

        //SceneManager.LoadScene("death");
        Instantiate(deathPrefabs[Random.Range(0, deathPrefabs.Length)]);
    }

    public void NextWaveInSeconds(float seconds) {
        StartCoroutine(NextWaveInSecondsCoroutine(seconds));
    }

    IEnumerator NextWaveInSecondsCoroutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        birdSpawner.IntroduceWave();
    }

    public void GiveBeaks()
    {
        int beaks = PlayerPrefs.GetInt("beaks", 0);

        // TODO: modify this amount by some kind of increasable upgrade

        PlayerPrefs.SetInt("beaks", beaks + _birdsKilled);
        ui.GiveBeaks(_birdsKilled);
    }
}
