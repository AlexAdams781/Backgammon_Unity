using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Game : MonoBehaviour
{
    public GameObject piece;
    public GameObject panel;
    public GameObject bar_panel;

    // Positions and team for each checker
    public GameObject[,] positions = new GameObject[28, 15];
    private GameObject[] playerWhite = new GameObject[15];
    private GameObject[] playerBlack = new GameObject[15];
    private string currentPlayer = "white";
    private bool gameOver = false;

    public int[] posMatrix = new int[28];

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("started");
        Debug.Log(posMatrix[27]);
        playerWhite = new GameObject[]
        {
            Create("white_checker", 24, 1, true), Create("white_checker", 24, 2, true), Create("white_checker", 13, 1, true),
            Create("white_checker", 13, 2, true), Create("white_checker", 13, 3, true), Create("white_checker", 13, 4, true),
            Create("white_checker", 13, 5, true), Create("white_checker", 8, 1, true), Create("white_checker", 8, 2, true),
            Create("white_checker", 8, 3, true), Create("white_checker", 6, 1, true), Create("white_checker", 6, 2, true),
            Create("white_checker", 6, 3, true), Create("white_checker", 6, 4, true), Create("white_checker", 6, 5, true),
        };
        playerBlack = new GameObject[]
        {
            Create("black_checker", 24, 1, false), Create("black_checker", 24, 2, false), Create("black_checker", 13, 1, false),
            Create("black_checker", 13, 2, false), Create("black_checker", 13, 3, false), Create("black_checker", 13, 4, false),
            Create("black_checker", 13, 5, false), Create("black_checker", 8, 1, false), Create("black_checker", 8, 2, false),
            Create("black_checker", 8, 3, false), Create("black_checker", 6, 1, false), Create("black_checker", 6, 2, false),
            Create("black_checker", 6, 3, false), Create("black_checker", 6, 4, false), Create("black_checker", 6, 5, false),
        };

        for (int i = 0; i < playerWhite.Length; i++)
        {
            SetPosition(playerBlack[i], "black_checker");
            SetPosition(playerWhite[i], "white_checker");
        }

        for (int i = 0; i < 24; i++) CreatePanel(i + 1);
        for (int i = 26; i < 28; i++) CreateBarPanel(i);
    }

    public GameObject Create(string name, int position, int depth, bool isWhite)
    {
        GameObject obj = Instantiate(piece, new Vector3(1, 0, -1), Quaternion.identity);
        Piece pi = obj.GetComponent<Piece>();
        pi.name = name;
        pi.SetDepth(depth);
        if (isWhite) pi.SetPosition(position);
        else pi.SetPosition(25 - position);
        pi.Activate();
        return obj;
    }

    public GameObject CreatePanel(int position)
    {
        GameObject obj = Instantiate(panel, new Vector3(1, 0, -1), Quaternion.identity);
        Panel p = obj.GetComponent<Panel>();
        p.SetPosition(position);
        p.SetCoords();
        //obj.SetActive(false);
        return obj;
    }

    public GameObject CreateBarPanel(int position)
    {
        GameObject obj = Instantiate(bar_panel, new Vector3(1, 0, -1), Quaternion.identity);
        BarPanel p = obj.GetComponent<BarPanel>();
        p.SetPosition(position);
        p.SetCoords();
        return obj;
    }

    public void SetPosition(GameObject obj, string player)
    {
        Piece pi = obj.GetComponent<Piece>();
        positions[pi.GetPosition(), pi.GetDepth() - 1] = obj;
        Debug.Log(pi.GetPosition());
        AddChecker(pi.GetPosition(), player);
    }

    public void SetPositionEmpty(int position, int depth, string player)
    {
        positions[position, depth - 1] = null;
        RemoveChecker(position, player);
    }

    public GameObject GetPosition(int position, int depth)
    {
        return positions[position, Mathf.Abs(depth)-1];
    }

    public int GetPosMatrix(int position, string player)
    {
        if (player == "white") return posMatrix[position];
        else return posMatrix[25 - position];
    }

    public void AddChecker(int position, string piece)
    {
        if (piece == "white_checker")
        {
            posMatrix[position] += 1;
        }
        else
        {
            posMatrix[position] -= 1;
        }
    }

    public void RemoveChecker(int position, string piece)
    {
        if (piece == "white_checker")
        {
            Debug.Log("here");
            posMatrix[position] -= 1;
        }
        else
        {
            posMatrix[position] += 1;
        }
    }

    public bool PositionOnBoard(int x, int y)
    {
        return (x < 0 || y < 0 || x >= positions.GetLength(0) || y >= positions.GetLength(1));
    }

    public void MoveChecker(int oldPos, int newPos, string piece)
    {
        int oldDep = posMatrix[oldPos];
        int newDep = posMatrix[newPos];

        if (oldDep * newDep < 0)
        {
            if (Mathf.Abs(newDep) == 1)
            {
                GameObject attacker = GetPosition(oldPos, Mathf.Abs(oldDep));
                GameObject victim = GetPosition(newPos, 1);
                CaptureChecker(attacker, victim, piece);
                return;
            }
            else return;
        }
        oldDep = Mathf.Abs(oldDep);
        newDep = Mathf.Abs(newDep);

        GameObject obj = GetPosition(oldPos, oldDep);
        if (obj == null) Debug.Log("Dereferencing null!!!");
        Piece piScript = obj.GetComponent<Piece>();

        if (piece == "white_checker") piScript.SetPosition(newPos);
        else piScript.SetPosition(newPos);
        piScript.SetDepth(newDep + 1);
        piScript.name = piece;
        piScript.Activate();

        SetPositionEmpty(oldPos, oldDep, piece);
        SetPosition(obj, piece);
    }

    public void CaptureChecker(GameObject attacker, GameObject victim, string piece)
    {
        Debug.Log("capture");
        Piece piAttacker = attacker.GetComponent<Piece>();
        Piece piVictim = victim.GetComponent<Piece>();
        int oldPos = piAttacker.GetPosition();
        int newPos = piVictim.GetPosition();
        int oldDep = piAttacker.GetDepth();

        if (piece == "white_checker")
        {
            piAttacker.SetPosition(newPos);
            piAttacker.SetDepth(1);
            piVictim.SetPosition(27);
            piVictim.SetDepth(-posMatrix[27]+1);
            piAttacker.Activate();
            piVictim.Activate();
            SetPositionEmpty(newPos, 1, "black_checker");
            SetPositionEmpty(oldPos, oldDep, "white_checker");
            SetPosition(attacker, "white_checker");
            Debug.Log("made it");
            SetPosition(victim, "black_checker");
        }
        else
        {
            piAttacker.SetPosition(newPos);
            piAttacker.SetDepth(1);
            piVictim.SetPosition(26);
            piVictim.SetDepth(posMatrix[26]+1);
            piAttacker.Activate();
            piVictim.Activate();
            SetPositionEmpty(newPos, 1, "white_checker");
            SetPositionEmpty(oldPos, oldDep, "black_checker");
            SetPosition(attacker, "black_checker");
            Debug.Log("made it");
            SetPosition(victim, "white_checker");

            if (oldPos == 27) GetPosition(27, oldDep - 1).GetComponentInChildren<TextMeshProUGUI>().color = new Color(255, 255, 255, 255);
        }
    }

    public int test()
    {
        return 1;
    }
}
