using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Static instance of the class
    public static GameManager Singleton { get; private set; }

    private int[,] _board = new int[7, 6];      // 7 columns, 6 rows
    private int _currentPlayer = 1;             // Player 1 starts

    [Header("UI")]
    [SerializeField] private Transform[] tokenHolders = null;
    [SerializeField] private Button[] columnButtons = null;
    [SerializeField] private TMP_Text playerTurn = null;

    [Header("Token")]
    [SerializeField] private GameObject redTokenPrefab = null;
    [SerializeField] private GameObject yellowTokenPrefab = null;

    private float _tokenDropHeight = 6f;
    private bool _gameDone = false;

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
                GameObject token = Instantiate(tokenToDrop, spawnPosition, Quaternion.identity, tokenHolders[column]);
                break;
            }
        }

        if (CheckWinCondition())
        {
            playerTurn.text = $"GAME WON BY PLAYER {_currentPlayer}!";
            Debug.Log($"GAME WON BY PLAYER {_currentPlayer}!");

            foreach (Button pButton in columnButtons)
                pButton.enabled = false;
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
        playerTurn.text = $"TURN: Player {_currentPlayer}";
        Debug.Log($"TURN: Player {_currentPlayer}");
    }
}