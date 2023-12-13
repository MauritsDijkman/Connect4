using System;
using UnityEngine;

public class Column : MonoBehaviour
{
    [Header("Info")]
    [SerializeField] private int columnNumber;

    public void DropPieceInColumn()
    {
        try
        {
            GameManager.Singleton.DropPiece(columnNumber);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Column | Error: {e}");
            throw;
        }
    }
}