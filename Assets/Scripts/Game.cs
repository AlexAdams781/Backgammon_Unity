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
    public GameObject GameOverScreen;

    // Positions and team for each checker
    public GameObject[,] positions = new GameObject[28, 15];
    private GameObject[] playerWhite = new GameObject[15];
    private GameObject[] playerBlack = new GameObject[15];
    public string currentPlayer = "white";
    private bool gameOver = false;

    public int[] posMatrix = new int[28];

    public SortedSet<int> whitePos = new SortedSet<int>();
    public SortedSet<int> blackPos = new SortedSet<int>();

    public Stack<int> dice = new Stack<int>();
    public Stack<(int, int, bool)> undoDice = new Stack<(int, int, bool)>();

    public int maxMovesPossible = 0;

    public bool whiteBearoff = false;
    public bool blackBearoff = false;
    public string winner = "";

    // Start is called before the first frame update
    void Start()
    {
        playerWhite = new GameObject[]
        {
            Create("white", 24, 1, true), Create("white", 24, 2, true), Create("white", 13, 1, true),
            Create("white", 13, 2, true), Create("white", 13, 3, true), Create("white", 13, 4, true),
            Create("white", 13, 5, true), Create("white", 8, 1, true), Create("white", 8, 2, true),
            Create("white", 8, 3, true), Create("white", 6, 1, true), Create("white", 6, 2, true),
            Create("white", 6, 3, true), Create("white", 6, 4, true), Create("white", 6, 5, true),
        };
        playerBlack = new GameObject[]
        {
            Create("black", 24, 1, false), Create("black", 24, 2, false), Create("black", 13, 1, false),
            Create("black", 13, 2, false), Create("black", 13, 3, false), Create("black", 13, 4, false),
            Create("black", 13, 5, false), Create("black", 8, 1, false), Create("black", 8, 2, false),
            Create("black", 8, 3, false), Create("black", 6, 1, false), Create("black", 6, 2, false),
            Create("black", 6, 3, false), Create("black", 6, 4, false), Create("black", 6, 5, false),
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

    public void AddChecker(int position, int[] posMatrix_, SortedSet<int> posSet, string piece)
    {
        if (piece == "white")
        {
            if (position != 26) posSet.Add(position);
            posMatrix_[position] += 1;
  
        }
        else
        {
            if (position != 27) posSet.Add(position);
            posMatrix_[position] -= 1;
        }
        checkBearoff();
    }

    public void RemoveChecker(int position, int[] posMatrix_, SortedSet<int> posSet, string piece)
    {
        if (piece == "white")
        {
            if (posMatrix_[position] <= 1)
            {
                posSet.Remove(position);
            }
            posMatrix_[position] -= 1;
        }
        else
        {
            if (posMatrix_[position] >= -1)
            {
                posSet.Remove(position);
            }
            posMatrix_[position] += 1;
        }
        checkBearoff();
    }

    public bool PositionOnBoard(int x, int y)
    {
        return (x < 0 || y < 0 || x >= positions.GetLength(0) || y >= positions.GetLength(1));
    }

    public void ReactivatePiece(int pos, int newDep)
    {
        if ((pos == 0 || pos == 25) && newDep > 0) GetPosition(pos, newDep).GetComponent<Piece>().Activate();
        else if (newDep >= 5) GetPosition(pos, newDep).GetComponent<Piece>().Activate();
    }

    public void TakeTurn(int oldPos)
    {
        //Debug.Log("Take Turn");
        //Debug.Log(oldPos);
        int depth = posMatrix[oldPos];
        int newPos;
        if (depth == 0 || dice.Count == 0) return;
        else if (depth > 0 && currentPlayer == "white")
        {
            newPos = oldPos - dice.Peek();
            if (newPos > 0 && posMatrix[newPos] > -2 && (oldPos == 25 || posMatrix[25] == 0))
            {
                undoDice.Push((oldPos, oldPos - dice.Pop(), MoveChecker(oldPos, newPos, "white")));
            }
            else if ((newPos == 0 && whiteBearoff) || (newPos < 0 && whitePos.Max == oldPos))
            {
                dice.Pop();
                undoDice.Push((oldPos, 26, MoveChecker(oldPos, 26, "white")));
            }
            else return;
        }
        else if (depth < 0 && currentPlayer == "black")
        {
            //Debug.Log("here");
            newPos = oldPos + dice.Peek();
            //Debug.Log(newPos);
            if (newPos <= 24 && posMatrix[newPos] < 2 && (oldPos == 0 || posMatrix[0] == 0))
            {
                undoDice.Push((oldPos, oldPos + dice.Pop(), MoveChecker(oldPos, newPos, "black")));
            }
            else if ((newPos == 25 && blackBearoff) || (newPos > 25 && blackPos.Min == oldPos))
            {
                dice.Pop();
                undoDice.Push((oldPos, 27, MoveChecker(oldPos, 27, "black")));
            }
            else return;
        }
        UndoDice.SetActive(true);
        if (undoDice.Count == maxMovesPossible) Done.SetActive(true);
        if (currentPlayer == "white") PrintSet(whitePos);
        else PrintSet(blackPos);
        //Debug.Log("Undo Count " + undoDice.Count() + " maxMoves " + maxMovesPossible);
        checkGameOver();
    }

    public bool MoveChecker(int oldPos, int newPos, string piece)
    {
        //Debug.Log("move checker");
        //Debug.Log(oldPos);
        //Debug.Log(newPos);
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

        piScript.SetPosition(newPos);
        piScript.SetDepth(newDep + 1);
        piScript.name = piece;
        piScript.Activate();
        Debug.Log("move checker " + oldPos + " " + newPos + " " + oldDep);
        ReactivatePiece(oldPos, oldDep - 1);

        SetPositionEmpty(oldPos, oldDep, piece);
        SetPosition(obj, piece);
        PrintSet(whitePos);
        PrintSet(blackPos);
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

            if (oldPos == 0 && oldDep > 1) GetPosition(0, oldDep).GetComponentInChildren<TextMeshProUGUI>().color = new Color(255, 255, 255, 255);
        }
        Debug.Log("capture checker");
        ReactivatePiece(oldPos, oldDep - 1);
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

    public void PrintStack(Stack<int> S)
    {
        Stack<int> S_ = new Stack<int>(S.Reverse());
        Debug.Log("Dice printing . . .");
        Debug.Log("{");
        while (S_.Count() > 0)
        {
            Debug.Log(S_.Peek());
            S_.Pop();
        }
        Debug.Log("}");
    }

    public void PrintSet(SortedSet<int> S)
    {
        int[] S_ = S.ToArray();
        Debug.Log("Set printing . . .");
        Debug.Log("{");
        foreach (int i in S_)
        {
            Debug.Log(i);
        }
        Debug.Log("}");
    }

    public void ReverseDice(Stack<int> S)
    {
        int tmp1 = S.Pop();
        int tmp2 = S.Pop();
        S.Push(tmp1);
        S.Push(tmp2);
    }

    public (List<List<(int, int)>>, int) GetMoves(Stack<int> diceClone)
    {
        List<List<(int, int)>> movesList = new List<List<(int, int)>>();
        int maxCheckersMoved = 0;

        int[] posMatrix_ = (int[]) posMatrix.Clone();
        SortedSet<int> S;
        if (currentPlayer == "white") S = new SortedSet<int>(whitePos);
        else S = new SortedSet<int>(blackPos);

        bool tmpWhiteBearoff = whiteBearoff;
        bool tmpBlackBearoff = blackBearoff;

        void moveDFS(Stack< int> dice, List<(int, int)> moves)
        {

            if (dice.Count() == 0)
            {
                maxCheckersMoved = Mathf.Max(maxCheckersMoved, moves.Count);
                movesList.Add(moves);
                //PrintList(moves);
                return;
            }

            int diceVal = dice.Pop();
            bool captured = false;
            int newPos;
            SortedSet<int> S_pos = S;
            if (currentPlayer == "white" && posMatrix_[25] > 0)
            {
                S_pos = new SortedSet<int>();
                S_pos.Add(25);
            }
            else if (currentPlayer == "black" && posMatrix_[0] < 0)
            {
                //Debug.Log("change");
                S_pos = new SortedSet<int>();
                S_pos.Add(0);
            }
            //Debug.Log("bar length" + posMatrix_[0] + "hi" + posMatrix_[25]);

            foreach (int pos in new SortedSet<int>(S_pos))
            {
                if (currentPlayer == "white") newPos = pos - diceVal;
                else newPos = pos + diceVal;

                if (newPos < 0 || newPos >= 26)
                {
                    if (currentPlayer == "white" && (!whiteBearoff || whitePos.Max != pos)) continue;
                    else if (currentPlayer == "black" && (!blackBearoff || blackPos.Min != pos)) continue;
                }
                else if (currentPlayer == "white" && newPos == 0 && !whiteBearoff) continue;
                else if (currentPlayer == "black" && newPos == 26 && !blackBearoff) continue;

                if ((posMatrix_[newPos] < -1 && currentPlayer == "white") || (posMatrix_[newPos] > 1 && currentPlayer == "black")) continue;
                RemoveChecker(pos, posMatrix_, S, currentPlayer);
                if (Mathf.Abs(posMatrix_[newPos]) == -1)
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
            dice.Push(diceVal);
        }

        moveDFS(diceClone, new List<(int, int)>());
        whiteBearoff = tmpWhiteBearoff;
        blackBearoff = tmpBlackBearoff;
        return (movesList, maxCheckersMoved);
    }

    public void GetMaxMoves(Stack<int> diceClone)
    {
        int maxCheckersMoved = 0;
        int maxIntervalMove = 0;

        int[] posMatrix_ = (int[])posMatrix.Clone();
        SortedSet<int> S;
        if (currentPlayer == "white") S = new SortedSet<int>(whitePos);
        else S = new SortedSet<int>(blackPos);

        bool moveDFS(Stack<int> dice, int moves)
        {
            //Debug.Log("moves" + moves);
;           if (dice.Count() == 0)
            {
                //Debug.Log("checkers moved: " + moves);
                maxCheckersMoved = Mathf.Max(maxCheckersMoved, moves);
                return true;
            }

            int diceVal = dice.Pop();
            bool captured = false;
            int newPos;

            SortedSet<int> S_pos = S;
            if (currentPlayer == "white" && S.Contains(25))
            {
                S_pos = new SortedSet<int>();
                S_pos.Add(25);
            }
            else if (currentPlayer == "black" && S.Contains(0))
            {
                S_pos = new SortedSet<int>();
                S_pos.Add(0);
            }

            foreach (int pos in new SortedSet<int>(S_pos))
            {
                Debug.Log("diceval and pos" + diceVal + " " + pos);
                if (currentPlayer == "white") newPos = pos - diceVal;
                else newPos = pos + diceVal;
                if (newPos < 0 || newPos >= 26)
                {
                    if (currentPlayer == "white")
                    {
                        if (!whiteBearoff || whitePos.Max != pos) continue;
                        else newPos = 0;
                    }
                    else
                    {
                        if (!blackBearoff || blackPos.Min != pos) continue;
                        else newPos = 25;
                    }
                }
                else if (currentPlayer == "white" && newPos == 0 && !whiteBearoff) continue;
                else if (currentPlayer == "black" && newPos == 25 && !blackBearoff) continue;

                else if ((posMatrix_[newPos] < -1 && currentPlayer == "white") || (posMatrix_[newPos] > 1 && currentPlayer == "black")) continue;
                maxIntervalMove = Mathf.Max(maxIntervalMove, diceVal);
                RemoveChecker(pos, posMatrix_, S, currentPlayer);
                if (Mathf.Abs(posMatrix_[newPos]) == -1)
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

            dice.Push(diceVal);
            maxCheckersMoved = Mathf.Max(maxCheckersMoved, moves);
            return false;
        }
        // Stack<int> diceClone = new Stack<int>(dice.Reverse());
        Stack<int> diceClone_ = new Stack<int>(diceClone.Reverse());
        //PrintStack(diceClone_);
        moveDFS(diceClone, 0);
        ReverseDice(diceClone_);
        //Debug.Log("ZZZZZZZ");
        //PrintStack(diceClone_);
        if (dice.Count() > maxCheckersMoved) moveDFS(diceClone_, 0);
        maxMovesPossible = maxCheckersMoved;

        //Debug.Log("max moves possible: " + maxCheckersMoved);
    }

    public void checkBearoff()
    {
        int count = 0;
        if (currentPlayer == "white")
        {
            if (whitePos.Max <= 6) whiteBearoff = true;
            else whiteBearoff = false;
        }
        else
        {
            if (blackPos.Min > 18) blackBearoff = true;
            else blackBearoff = false;
        }
    }

    private string Capitalize(string s)
    {
        return s[0].ToString().ToUpper() + s.Substring(1);
    }

    public void checkGameOver()
    {
        if ((posMatrix[26] == 15) || (posMatrix[27] == -15))
        {
            Debug.Log("Game over");
            winner = Capitalize(currentPlayer);
            GameOverScreen.SetActive(true);
            GameOverScreen.GetComponentInChildren<TextMeshProUGUI>().text = "Game Over!\n" + winner + "Wins!";
        }
    }
}
