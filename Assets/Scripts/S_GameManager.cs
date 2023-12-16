using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Static instance of GameManager for singleton pattern
    public static GameManager Singleton { get; private set; }
    
    // Game Board Settings
    private int[,] _board = new int[7, 6]; // 7 columns, 6 rows
    private int _currentPlayer;

    [Header("UI")]
    [SerializeField] private Transform[] tokenHolders = null;
    [SerializeField] private Button[] columnButtons = null;
    [Space (5)]
    [SerializeField] private TMP_Text playerTurn = null;
    [SerializeField] private GameObject redoButton = null;

    [Header("Tokens")]
    [SerializeField] private GameObject redTokenPrefab = null;
    [SerializeField] private GameObject yellowTokenPrefab = null;
    [Space (5)]
    [SerializeField] private Color32 winTokenColor = Color.green;
    
    [Header("Debug")]
    [SerializeField] private bool printDebugLogs = false;
    
    // Token Values
    private float _tokenDropHeight = 6f;
    private List<Vector2Int> _winningTokensPositions = new List<Vector2Int>();
    
    // Game Values
    private bool _gameDone = false;
    private string _printColor;

    private void Awake()
    {
        // Ensure only one instance of GameManager exists
        if (Singleton == null)
            Singleton = this;
        else if (Singleton != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        // Hide redo button at game start
        if (redoButton) redoButton.SetActive(false);

        // Randomize starting player
        _currentPlayer = Random.Range(1, 3);

        // Initialize game
        InitializeBoard();
        SetPlayerTurnName();
    }

    private void InitializeBoard()
    {
        for (int x = 0; x < 7; x++)
        {
            for (int y = 0; y < 6; y++)
                _board[x, y] = 0;
        }
    }
    
    public void DropPiece(int column)
    {
        if (_gameDone || IsColumnFull(column)) return;

        // Choose the appropriate token based on the current player
        GameObject tokenToDrop = _currentPlayer == 1 ? redTokenPrefab : yellowTokenPrefab;

        for (int y = 0; y < 6; y++)
        {
            if (_board[column, y] == 0)
            {
                _board[column, y] = _currentPlayer;
                Vector3 spawnPosition = tokenHolders[column].position + new Vector3(0, _tokenDropHeight, 0);
                if (tokenToDrop) Instantiate(tokenToDrop, spawnPosition, Quaternion.identity, tokenHolders[column]);
                break;
            }
        }

        // Disable column button if column is full
        if (IsColumnFull(column)) columnButtons[column].interactable = false;

        // Check game status after the move
        ProcessGameStatus();
    }
    
    #region Player

    private void SwitchPlayer()
    {
        _currentPlayer = _currentPlayer == 1 ? 2 : 1;
        SetPlayerTurnName();
    }
    
    private void SetPlayerTurnName()
    {
        SetPlayerColor();
        PrintText($"TURN: <color={_printColor}>Player {_currentPlayer}</color>");
    }

    private void SetPlayerColor()
    {
        _printColor = _currentPlayer == 1 ? "red" : "yellow";
    }

    #endregion
    
    #region Check Board Status
    
    private void ProcessGameStatus()
    {
        if (CheckWinCondition())
        {
            SetPlayerColor();
            HighlightWinningTokens();
            
            PrintText($"GAME WON BY <color={_printColor}>PLAYER {_currentPlayer}</color>!");
            EndGame();
        }
        else if (IsBoardFull())
        {
            PrintText("IT'S A DRAW!");
            EndGame();
        }
        else
            SwitchPlayer();
    }
    
    private bool IsColumnFull(int column)
    {
        return _board[column, 5] != 0;
    }
    
    private bool IsBoardFull()
    {
        for (int x = 0; x < 7; x++)
        {
            for (int y = 0; y < 6; y++)
            {
                if (_board[x, y] == 0) return false;
            }
        }

        return true;
    }

    private bool CheckWinCondition()
    {
        // Clear previous winning positions
        _winningTokensPositions.Clear();

        // Check for winning conditions
        return CheckBoardForLineMatch();
    }
    
    private void EndGame()
    {
        foreach (Button button in columnButtons)
        {
            if (button) button.interactable = false;
        }

        if (redoButton) redoButton.SetActive(true);

        _gameDone = true;
    }
    
    private bool CheckBoardForLineMatch()
    {
        // Checking for winning conditions
        // Horizontal, Vertical, Diagonal (ascending and descending)

        // Horizontal check
        for (int y = 0; y < 6; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                if (CheckLineMatch(x, y, 1, 0)) return true;
            }
        }

        // Vertical check
        for (int x = 0; x < 7; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (CheckLineMatch(x, y, 0, 1)) return true;
            }
        }

        // Diagonal (ascending) check
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (CheckLineMatch(x, y, 1, 1)) return true;
            }
        }

        // Diagonal (descending) check
        for (int x = 0; x < 4; x++)
        {
            for (int y = 3; y < 6; y++)
            {
                if (CheckLineMatch(x, y, 1, -1)) return true;
            }
        }

        return false;
    }

    private bool CheckLineMatch(int startX, int startY, int xIncrement, int yIncrement)
    {
        for (int i = 0; i < 4; i++)
        {
            int x = startX + i * xIncrement;
            int y = startY + i * yIncrement;

            if (_board[x, y] != _currentPlayer) return false;
        }

        AddWinningPositions(startX, startY, xIncrement, yIncrement, 4);
        return true;
    }
    
    private void AddWinningPositions(int startX, int startY, int xIncrement, int yIncrement, int count)
    {
        for (int i = 0; i < count; i++)
            _winningTokensPositions.Add(new Vector2Int(startX + (i * xIncrement), startY + (i * yIncrement)));
    }

    private void HighlightWinningTokens()
    {
        foreach (var position in _winningTokensPositions)
        {
            Transform token = tokenHolders[position.x].GetChild(position.y);
            if (token)
            {
                Image tokenSprite = token.GetComponent<Image>();
                if (tokenSprite) tokenSprite.color = winTokenColor;
            }
        }
    }
    
    #endregion
    
    private void PrintText(string text)
    {
        if (playerTurn) playerTurn.text = text;
        if (printDebugLogs) Debug.Log(text);
    }
    
    public void LoadLevel(string levelName)
    {
        SceneManager.LoadSceneAsync(levelName);
    }
}