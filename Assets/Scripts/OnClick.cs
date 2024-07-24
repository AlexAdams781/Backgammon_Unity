using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class OnClick : MonoBehaviour
{
    public GameObject leftDice;
    public GameObject rightDice;
    public GameObject rollDice;
    public GameObject undoDice;
    public GameObject Done;

    public Sprite One, Two, Three, Four, Five, Six;
    public Sprite Black_One, Black_Two, Black_Three, Black_Four, Black_Five, Black_Six;

    public GameObject controller;

    public int firstDice = 0;

    public void ActivateDoubles(int val)
    {
        controller.GetComponent<Game>().dice.Push(val);
        controller.GetComponent<Game>().dice.Push(val);
    }

    public (Sprite, int) rng(string player)
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        int rand_val = Random.Range(1, 7);

        controller.GetComponent<Game>().dice.Push(rand_val);
        if (player == "white")
        {
            if (rand_val == 1) return (One, 1);
            else if (rand_val == 2) return (Two, 2);
            else if (rand_val == 3) return (Three, 3);
            else if (rand_val == 4) return (Four, 4);
            else if (rand_val == 5) return (Five, 5);
            else return (Six, 6);
        }
        else
        {
            if (rand_val == 1) return (Black_One, 1);
            else if (rand_val == 2) return (Black_Two, 2);
            else if (rand_val == 3) return (Black_Three, 3);
            else if (rand_val == 4) return (Black_Four, 4);
            else if (rand_val == 5) return (Black_Five, 5);
            else return (Black_Six, 6);
        }
    }

    public void RollClick()
    {
        int val1;
        int val2;

        controller = GameObject.FindGameObjectWithTag("GameController");
        leftDice.SetActive(true);
        rightDice.SetActive(true);
        (rightDice.GetComponent<Image>().sprite, val1) = rng(controller.GetComponent<Game>().currentPlayer);
        (leftDice.GetComponent<Image>().sprite, val2) = rng(controller.GetComponent<Game>().currentPlayer);

        if (val1 > val2) DiceClick();
        else if (val1 == val2) ActivateDoubles(val1);

        Stack<int> diceClone = new Stack<int>(controller.GetComponent<Game>().dice.Reverse());
        //Debug.Log("stop search");
        controller.GetComponent<Game>().GetMaxMoves(diceClone);

        rollDice.SetActive(false);
        if (controller.GetComponent<Game>().maxMovesPossible == 0) Done.SetActive(true);
        controller.GetComponent<Game>().PrintStack(controller.GetComponent<Game>().dice);
    }

    public void DiceClick()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        controller.GetComponent<Game>().ReverseDice(controller.GetComponent<Game>().dice);

        Sprite spriteTmp = leftDice.GetComponent<Image>().sprite;
        leftDice.GetComponent<Image>().sprite = rightDice.GetComponent<Image>().sprite;
        rightDice.GetComponent<Image>().sprite = spriteTmp;
    }

    public void UndoClick()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        (int oldPos, int newPos, bool isCapture) = controller.GetComponent<Game>().undoDice.Pop();
        string player = controller.GetComponent<Game>().currentPlayer;

        controller.GetComponent<Game>().MoveChecker(newPos, oldPos, player);
        if (isCapture && player == "white") controller.GetComponent<Game>().MoveChecker(0, newPos, "black");
        else if (isCapture && player == "black") controller.GetComponent<Game>().MoveChecker(25, newPos, "white");

        int diceVal = oldPos - newPos;
        if (newPos == 26) diceVal = oldPos;
        else if (newPos == 27) diceVal = 25 - oldPos;
        controller.GetComponent<Game>().dice.Push(Mathf.Abs(diceVal));
        //Debug.Log("pushed" + (diceVal) + "into stack.");
        controller.GetComponent<Game>().PrintStack(controller.GetComponent<Game>().dice);
        //Debug.Log("Undo Count " + controller.GetComponent<Game>().undoDice.Count() + " maxMoves " + controller.GetComponent<Game>().maxMovesPossible);

        Done.SetActive(false);
        if (controller.GetComponent<Game>().undoDice.Count == 0) undoDice.SetActive(false);
    }

    public void DoneClick()
    {
        if (controller.GetComponent<Game>().currentPlayer == "black") controller.GetComponent<Game>().currentPlayer = "white";
        else controller.GetComponent<Game>().currentPlayer = "black";
        Done.SetActive(false);
        undoDice.SetActive(false);
        rollDice.SetActive(true);
        leftDice.SetActive(false);
        rightDice.SetActive(false);
        controller.GetComponent<Game>().undoDice.Clear();
        controller.GetComponent<Game>().dice.Clear();
        controller.GetComponent<Game>().maxMovesPossible = 0;
    }
}
