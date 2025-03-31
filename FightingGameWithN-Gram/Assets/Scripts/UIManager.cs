using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    // State UI
    [Header("State UI")]
    [SerializeField] private GameObject _readyUI;
    [SerializeField] private GameObject _fightUI;
    [SerializeField] private GameObject _gameplayUI;
    [SerializeField] private GameObject _matchEndUI;

    // Player Health
    [Header("Player Health")]
    [SerializeField] private HealthBar _player1Health;
    [SerializeField] private HealthBar _player2Health;

    // Game Info
    [Header("Game Info")]
    [SerializeField] private TMP_Text _winnerText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // State Management
    public void UpdateGameState(GameState state)
    {
        _readyUI.SetActive(state == GameState.Ready);
        _fightUI.SetActive(state == GameState.Fight);
        //_gameplayUI.SetActive(state == GameState.Gameplay);
        _matchEndUI.SetActive(state == GameState.MatchEnd);
    }

    // Health Updates
    public void UpdatePlayerHealth(int playerID, int currentHealth)
    {
        if (playerID == 0) _player1Health.SetHealth(currentHealth);
        else _player2Health.SetHealth(currentHealth);
    }

    public void UpdateWinnerText(int winnerID) 
    {
        if (winnerID == 0)
        {
            _winnerText.text = "Player 1 Win";
        }
        else 
        {
            _winnerText.text = "Player 2 Win";
        }
    }
}