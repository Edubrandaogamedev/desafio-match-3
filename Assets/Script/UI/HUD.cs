using System;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] 
    private GameHandler _gameHandler;
    [SerializeField] 
    private TextMeshProUGUI _scoreText;

    private void Awake()
    {
        _gameHandler.OnScoreUpdated += UpdateScore;
        UpdateScore(0);
    }

    private void OnDestroy()
    {
        _gameHandler.OnScoreUpdated -= UpdateScore;
    }

    private void UpdateScore(int value)
    {
        _scoreText.text = $"Score: {value}";
    }
}
