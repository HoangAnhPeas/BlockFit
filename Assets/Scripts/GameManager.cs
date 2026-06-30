using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int Score { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public void AddScore(int amount)
    {
        Score += amount;

        Debug.Log("Score : " + Score);
    }
}