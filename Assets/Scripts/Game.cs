using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class Game : MonoBehaviour
{
    public GameObject piece;
    public GameObject panel;
    public GameObject bar_panel;
    public GameObject RollDice;
    public GameObject UndoDice;
    public GameObject Done;

    // Positions and team for each checker
    public GameObject[,] positions = new GameObject[28, 15];
    private GameObject[] playerWhite = new GameObject[15];
    private GameObject[] playerBlack = new GameObject[15];
    public string currentPlayer = "white";
    private bool gameOver = false;

    public int[] posMatrix = new int[28];

    public HashSet<int> whitePos = new HashSet<int>();
    public HashSet<int> blackPos = new HashSet<int>();

    public Stack<int> dice = new Stack<int>();
    public Stack<(int, int, bool)> undoDice = new Stack<(int, int, bool)>();

    public int maxMovesPossible = 0;

    // Start is called before the first frame update
    void Start()
    {
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
            SetPosition(playerBlack[i], "black");
            SetPosition(playerWhite[i], "white");
        }

        for (int i = 0; i < 24; i++) CreatePanel(i + 1);
        CreateBarPanel(0);
        CreateBarPanel(25);

        whitePos.Add(24);
        whitePos.Add(13);
        whitePos.Add(8);
        whitePos.Add(6);
        blackPos.Add(19);
        blackPos.Add(17);
        blackPos.Add(12);
        blackPos.Add(1);
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
        if (player == "white") AddChecker(pi.GetPosition(), posMatrix, whitePos, player);
        else AddChecker(pi.GetPosition(), posMatrix, blackPos, player);
    }

    public void SetPositionEmpty(int position, int depth, string player)
    {
        positions[position, depth - 1] = null;
        if (player == "white") RemoveChecker(position, posMatrix, whitePos, player);
        else RemoveChecker(position, posMatrix, blackPos, player);
    }

    public GameObject GetPosition(int position, int depth)
    {
        return positions[position, Mathf.Abs(depth) - 1];
    }

    public int GetPosMatrix(int position, string player)
    {
        if (player == "white") return posMatrix[position];
        else return posMatrix[25 - position];
    }

    public void AddChecker(int position, int[] posMatrix_, HashSet<int> posSet, string piece)
    {
        if (piece == "white")
        {
            posSet.Add(position);
            posMatrix_[position] += 1;
        }
        else
        {
            posSet.Add(position);
            posMatrix_[position] -= 1;
        }
    }

    public void RemoveChecker(int position, int[] posMatrix_, HashSet<int> posSet, string piece)
    {
        if (piece == "white")
        {
            if (posMatrix_[position] <= 1) posSet.Remove(position);
            posMatrix_[position] -= 1;
        }
        else
        {
            if (posMatrix_[position] >= -1)  posSet.Remove(position);
            posMatrix_[position] += 1;
        }
    }

    public bool PositionOnBoard(int x, int y)
    {
        return (x < 0 || y < 0 || x >= positions.GetLength(0) || y >= positions.GetLength(1));
    }

    public void TakeTurn(int oldPos)
    {
        int depth = posMatrix[oldPos];
        int newPos;
        int newDep;
        if (depth == 0 || dice.Count == 0) return;
        else if (depth > 0 && currentPlayer == "white")
        {
            newPos = oldPos - dice.Peek();
            newDep = posMatrix[newPos];
            if (newPos > 0 && newDep > -2 && (oldPos == 25 || posMatrix[25] == 0)) undoDice.Push((oldPos, oldPos - dice.Pop(), MoveChecker(oldPos, newPos, "white")));
            else return;
        }
        else if (currentPlayer == "black")
        {
            newPos = oldPos + dice.Peek();
            newDep = posMatrix[newPos];
            if (newPos <= 24 && newDep < 2 && (oldPos == 0 || posMatrix[0] == 0)) undoDice.Push((oldPos, oldPos + dice.Pop(), MoveChecker(oldPos, newPos, "black")));
            else return;
        }
        UndoDice.SetActive(true);
        Debug.Log("maxmovesss");
        Debug.Log(maxMovesPossible);
        Debug.Log(undoDice.Count);
        if (undoDice.Count == maxMovesPossible) Done.SetActive(true);
    }

    public bool MoveChecker(int oldPos, int newPos, string piece)
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
                return true;
            }
        }
        oldDep = Mathf.Abs(oldDep);
        newDep = Mathf.Abs(newDep);

        GameObject obj = GetPosition(oldPos, oldDep);
        if (obj == null) Debug.Log("Dereferencing null!!!");
        Piece piScript = obj.GetComponent<Piece>();

        if (piece == "white") piScript.SetPosition(newPos);
        else piScript.SetPosition(newPos);
        piScript.SetDepth(newDep + 1);
        piScript.name = piece;
        piScript.Activate();

        SetPositionEmpty(oldPos, oldDep, piece);
        SetPosition(obj, piece);
        return false;
    }

    public void CaptureChecker(GameObject attacker, GameObject victim, string piece)
    {
        Piece piAttacker = attacker.GetComponent<Piece>();
        Piece piVictim = victim.GetComponent<Piece>();
        int oldPos = piAttacker.GetPosition();
        int newPos = piVictim.GetPosition();
        int oldDep = piAttacker.GetDepth();

        if (piece == "white")
        {
            piAttacker.SetPosition(newPos);
            piAttacker.SetDepth(1);
            piVictim.SetPosition(0);
            piVictim.SetDepth(-posMatrix[0] + 1);
            piAttacker.Activate();
            piVictim.Activate();
            SetPositionEmpty(newPos, 1, "black");
            SetPositionEmpty(oldPos, oldDep, "white");
            SetPosition(attacker, "white");
            SetPosition(victim, "black");
        }
        else
        {
            piAttacker.SetPosition(newPos);
            piAttacker.SetDepth(1);
            piVictim.SetPosition(25);
            piVictim.SetDepth(posMatrix[25] + 1);
            piAttacker.Activate();
            piVictim.Activate();
            SetPositionEmpty(newPos, 1, "white");
            SetPositionEmpty(oldPos, oldDep, "black");
            SetPosition(attacker, "black");
            SetPosition(victim, "white");

            if (oldPos == 0) GetPosition(0, oldDep - 1).GetComponentInChildren<TextMeshProUGUI>().color = new Color(255, 255, 255, 255);
        }
    }

    public int test()
    {
        return 1;
    }

    public void PrintList(List<(int, int)> L)
    {
        Debug.Log("{");
        foreach ((int start, int end) k in L)
        {
            Debug.Log(k.start);
            Debug.Log(k.end);
        }
        Debug.Log("}");
    }

    public void ReverseDice(Stack<int> S)
    {
        Debug.Log("size of");
        Debug.Log(S.Count());
        int tmp1 = S.Pop();
        int tmp2 = S.Pop();
        S.Push(tmp1);
        S.Push(tmp2);
    }

    public (List<List<(int, int)>>, int) GetMoves(Stack<int> diceClone)
    {
        Debug.Log("get Moves started");
        List<List<(int, int)>> movesList = new List<List<(int, int)>>();
        int maxCheckersMoved = 0;

        int[] posMatrix_ = (int[]) posMatrix.Clone();
        HashSet<int> S;
        if (currentPlayer == "white") S = new HashSet<int>(whitePos);
        else
        {
            S = new HashSet<int>(blackPos);
            for (int i = 0; i < posMatrix_.Length; i++) posMatrix_[i] *= -1;
        }

        void moveDFS(Stack< int> dice, List<(int, int)> moves)
        {
            Debug.Log("move DFS started");
         
            if (diceClone.Count() == 0)
            {
                Debug.Log("empty dice");
                maxCheckersMoved = Mathf.Max(maxCheckersMoved, moves.Count);
                movesList.Add(moves);
                PrintList(moves);
                return;
            }
            else Debug.Log(dice.Peek());

            int diceVal = diceClone.Pop();
            bool captured = false;
            int newPos;
            HashSet<int> S_pos = S;
            if (currentPlayer == "white" && posMatrix_[25] > 0)
            {
                S_pos = new HashSet<int>();
                S_pos.Add(25);
            }
            else if (currentPlayer == "black" && posMatrix_[0] < 0)
            {
                S_pos = new HashSet<int>();
                S_pos.Add(0);
            }

            foreach (int pos in new HashSet<int>(S_pos))
            {
                Debug.Log(pos);
                if (currentPlayer == "white") newPos = pos - diceVal;
                else newPos = pos + diceVal;

                if (newPos < 0 || newPos >= 26) continue;

                if (posMatrix_[newPos] < -1) continue;
                RemoveChecker(pos, posMatrix_, S, currentPlayer);
                if (posMatrix_[newPos] == -1)
                {
                    AddChecker(newPos, posMatrix_, S, currentPlayer);
                    captured = true;
                }
                moves.Add((pos, newPos));
                AddChecker(newPos, posMatrix_, S, currentPlayer);

                moveDFS(dice, moves);

                AddChecker(pos, posMatrix_, S,currentPlayer);
                moves.RemoveAt(moves.Count - 1);
                RemoveChecker(newPos, posMatrix_, S,currentPlayer);
                if (captured) captured = !captured;
            }
            diceClone.Push(diceVal);
        }

        moveDFS(diceClone, new List<(int, int)>());
        Debug.Log("max Checkers Moved");
        Debug.Log(maxCheckersMoved);
        return (movesList, maxCheckersMoved);
    }

    public (int, int) GetMaxMoves(Stack<int> diceClone)
    {
        Debug.Log("get Moves started");
        int maxCheckersMoved = 0;
        int maxIntervalMove = 0;

        int[] posMatrix_ = (int[])posMatrix.Clone();
        HashSet<int> S;
        if (currentPlayer == "white") S = new HashSet<int>(whitePos);
        else
        {
            S = new HashSet<int>(blackPos);
            for (int i = 0; i < posMatrix_.Length; i++) posMatrix_[i] *= -1;
        }

        bool moveDFS(Stack<int> dice, int moves)
        {
            Debug.Log("move DFS started");

            if (diceClone.Count() == 0)
            {
                maxCheckersMoved = Mathf.Max(maxCheckersMoved, moves);
                return true;
            }

            int diceVal = diceClone.Pop();
            bool captured = false;
            int newPos;

            HashSet<int> S_pos = S;
            if (currentPlayer == "white" && posMatrix_[25] > 0)
            {
                S_pos = new HashSet<int>();
                S_pos.Add(25);
            }
            else if (currentPlayer == "black" && posMatrix_[0] < 0)
            {
                S_pos = new HashSet<int>();
                S_pos.Add(0);
            }

            foreach (int pos in new HashSet<int>(S_pos))
            {
                Debug.Log(pos);
                if (currentPlayer == "white") newPos = pos - diceVal;
                else newPos = pos + diceVal;

                if (newPos < 0 || newPos >= 26) continue;

                if (posMatrix_[newPos] < -1) continue;
                maxIntervalMove = Mathf.Max(maxIntervalMove, diceVal);
                RemoveChecker(pos, posMatrix_, S, currentPlayer);
                if (posMatrix_[newPos] == -1)
                {
                    AddChecker(newPos, posMatrix_, S, currentPlayer);
                    captured = true;
                }
                AddChecker(newPos, posMatrix_, S, currentPlayer);

                if (moveDFS(dice, moves+1)) return true;

                AddChecker(pos, posMatrix_, S, currentPlayer);
                RemoveChecker(newPos, posMatrix_, S, currentPlayer);
                if (captured) captured = !captured;
            }

            diceClone.Push(diceVal);
            return false;
        }
        // Stack<int> diceClone = new Stack<int>(dice.Reverse());
        Stack<int> diceClone_ = new Stack<int>(diceClone.Reverse());
        moveDFS(diceClone, 0);
        ReverseDice(diceClone_);
        if (dice.Count() > maxCheckersMoved) moveDFS(diceClone_, 0);

        Debug.Log("max Checkers Moved");
        Debug.Log(maxCheckersMoved);
        maxMovesPossible = maxCheckersMoved;
        return (maxCheckersMoved, maxIntervalMove);
    }
}
