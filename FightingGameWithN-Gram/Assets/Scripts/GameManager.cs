using UnityEngine;

public enum GameState 
{
    Ready,
    Fight,
    Gameplay,
    RoundEnd,
    MatchEnd
}

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    private GameState _currentState = GameState.Ready;
    private float _stateTimer = 0f;

    // State durations
    [SerializeField] private float readyDuration = 3f;
    [SerializeField] private float fightDuration = 1f;
    [SerializeField] private float roundEndDuration = 1f;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject singletonObject = new GameObject("GameManager");
                _instance = singletonObject.AddComponent<GameManager>();
                DontDestroyOnLoad(singletonObject);
            }
            return _instance;
        }
    }

    public Character character1;
    public Character character2;
    public GameState CurrentState => _currentState;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Start the game sequence
        ChangeState(GameState.Ready);
    }

    private void Update()
    {
        _stateTimer -= Time.deltaTime;

        // Handle state transitions
        switch (_currentState)
        {
            case GameState.Ready:
                if (_stateTimer <= 0) ChangeState(GameState.Fight);
                break;

            case GameState.Fight:
                if (_stateTimer <= 0) ChangeState(GameState.Gameplay);
                break;

            case GameState.RoundEnd:
                if (_stateTimer <= 0) ChangeState(GameState.Ready);
                break;
                // Add other state transitions as needed
        }
    }

    public void ChangeState(GameState newState)
    {
        // Exit current state
        switch (_currentState)
        {
            case GameState.Ready:
                // Hide "Ready" UI
                break;
            case GameState.Fight:
                // Hide "Fight!" UI
                break;
        }

        // Enter new state
        _currentState = newState;

        UIManager.Instance.UpdateGameState(_currentState);

        switch (newState)
        {
            case GameState.Ready:
                _stateTimer = readyDuration;
                // Show "Ready" UI
                // Reset character positions
                ResetState();
                break;

            case GameState.Fight:
                _stateTimer = fightDuration;
                // Show "Fight!" UI
                break;

            case GameState.Gameplay:
                // Enable character controls
                EnableCharacterControls(true);
                break;

            case GameState.RoundEnd:
                _stateTimer = roundEndDuration;
                break;
        }

        Debug.Log($"GameState changed to: {newState}");
    }

    private void ResetState()
    {
        // Reset character positions, health, etc.
        if (character1 != null) character1.ResetState();
        if (character2 != null) character2.ResetState();
    }

    private void EnableCharacterControls(bool enable)
    {
        if (character1 != null) character1.SetIsReadyToPlay(enable);
        if (character2 != null) character2.SetIsReadyToPlay(enable);
    }

}
