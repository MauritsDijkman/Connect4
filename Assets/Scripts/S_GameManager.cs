using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Static instance of the class
    public static GameManager Singleton { get; private set; }

    private int[,] _board = new int[7, 6];  // 7 columns, 6 rows
    private int _currentPlayer;

    [Header("UI")]
    [SerializeField] private Transform[] tokenHolders = null;
    [SerializeField] private Button[] columnButtons = null;
    [SerializeField] private TMP_Text playerTurn = null;
    [SerializeField] private GameObject redoButton = null;

    [Header("Token")]
    [SerializeField] private GameObject redTokenPrefab = null;
    [SerializeField] private GameObject yellowTokenPrefab = null;

    private float _tokenDropHeight = 6f;
    private bool _gameDone = false;

    private string _printColor;

    private void Awake()
    {
        if (!Singleton)
            Singleton = this;
        else
        {
            if (Singleton != this)
                Destroy(this);
        }
    }

    private void Start()
    {
        if (redoButton.activeSelf) redoButton.SetActive(false);

        _currentPlayer = Random.Range(1, 3);
        
        // Initialize the board
        for (int x = 0; x < 7; x++)
        {
            for (int y = 0; y < 6; y++)
                _board[x, y] = 0;
        }

        SetPlayerTurnName();
    }

    public void DropPiece(int column)
    {
        if (_gameDone) return;

        GameObject tokenToDrop = _currentPlayer == 1 ? redTokenPrefab : yellowTokenPrefab;

        for (int y = 0; y < 6; y++)
        {
            if (_board[column, y] == 0)
            {
                _board[column, y] = _currentPlayer;
                Vector3 spawnPosition = tokenHolders[column].position + new Vector3(0, _tokenDropHeight, 0);
                Instantiate(tokenToDrop, spawnPosition, Quaternion.identity, tokenHolders[column]);
                break;
            }
        }

        if (CheckWinCondition())
        {
            SetPlayerColor();

            playerTurn.text = $"GAME WON BY <color={_printColor}>PLAYER {_currentPlayer}</color>!";
            Debug.Log($"GAME WON BY <color={_printColor}>PLAYER {_currentPlayer}</color>!");

            foreach (Button pButton in columnButtons)
                pButton.enabled = false;

            if (!redoButton.activeSelf) redoButton.SetActive(true);
        }
        else
            SwitchPlayer();
    }

    private void SwitchPlayer()
    {
        _currentPlayer = _currentPlayer == 1 ? 2 : 1;
        SetPlayerTurnName();
    }

    private bool CheckWinCondition()
    {
        // Horizontal check
        for (int y = 0; y < 6; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                if (_board[x, y] == _currentPlayer &&
                    _board[x + 1, y] == _currentPlayer &&
                    _board[x + 2, y] == _currentPlayer &&
                    _board[x + 3, y] == _currentPlayer)
                {
                    _gameDone = true;
                    return true;
                }
            }
        }

        // Vertical check
        for (int x = 0; x < 7; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (_board[x, y] == _currentPlayer &&
                    _board[x, y + 1] == _currentPlayer &&
                    _board[x, y + 2] == _currentPlayer &&
                    _board[x, y + 3] == _currentPlayer)
                {
                    _gameDone = true;
                    return true;
                }
            }
        }

        // Diagonal (ascending) check
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (_board[x, y] == _currentPlayer &&
                    _board[x + 1, y + 1] == _currentPlayer &&
                    _board[x + 2, y + 2] == _currentPlayer &&
                    _board[x + 3, y + 3] == _currentPlayer)
                {
                    _gameDone = true;
                    return true;
                }
            }
        }

        // Diagonal (descending) check
        for (int x = 0; x < 4; x++)
        {
            for (int y = 3; y < 6; y++)
            {
                if (_board[x, y] == _currentPlayer &&
                    _board[x + 1, y - 1] == _currentPlayer &&
                    _board[x + 2, y - 2] == _currentPlayer &&
                    _board[x + 3, y - 3] == _currentPlayer)
                {
                    _gameDone = true;
                    return true;
                }
            }
        }

        return false;
    }

    private void SetPlayerTurnName()
    {
        SetPlayerColor();

        playerTurn.text = $"TURN: <color={_printColor}>Player {_currentPlayer}</color>!";
        Debug.Log($"TURN: <color={_printColor}>Player {_currentPlayer}</color>!");
    }

    private void SetPlayerColor()
    {
        _printColor = _currentPlayer == 1 ? "red" : "yellow";
    }

    public void ReloadScene(string pLevelName)
    {
        SceneManager.LoadSceneAsync(pLevelName);
    }
}