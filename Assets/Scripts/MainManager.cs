using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    [System.Serializable]
    struct SaveData
    {
        public string playerName;
        public int score;
    }
    private SaveData HighScore;

    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public GameObject GameOverText;
    [SerializeField] private Text BestScoreText;

    private bool m_Started = false;
    private int m_Points;

    private bool m_GameOver = false;


    // Start is called before the first frame update
    void Start()
    {
        HighScore = GetBestScore();
        BestScoreText.text = $"Best Score : {HighScore.playerName} : {HighScore.score}";
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
        if (m_Points > HighScore.score)
        {
            HighScore.playerName = MenuManager.Instance.playerName;
            HighScore.score = m_Points;
            BestScoreText.text = $"Best Score : {HighScore.playerName} : {HighScore.score}";
            SaveBestScore();
        }
    }
    SaveData GetBestScore()
    {
        string saveDataPath = Application.persistentDataPath + "/SaveData.json";
        if (File.Exists(saveDataPath))
        {
            string saveData = File.ReadAllText(saveDataPath);
            SaveData sv = JsonUtility.FromJson<SaveData>(saveData);
            return sv;
        }
        else
        {
            SaveData sv = new SaveData() { playerName = "None", score = 0 };
            return sv;
        }
    }

    void SaveBestScore()
    {
        string saveData = JsonUtility.ToJson(HighScore);
        File.WriteAllText(Application.persistentDataPath + "/SaveData.json", saveData);
    }
}
