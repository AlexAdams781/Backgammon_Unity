using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarPanel : MonoBehaviour
{
    public GameObject controller;
    public int position = 0;

    public void SetCoords()
    {
        if (position == 26) this.transform.position = new Vector3(0.0f, 1.0f, -2.0f);
        else this.transform.position = new Vector3(0.0f, -1.0f, -2.0f);
    }
    public int GetPosition()
    {
        return position;
    }

    public void SetPosition(int x)
    {
        position = x;
    }

    private void OnMouseUp()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        int depth = controller.GetComponent<Game>().posMatrix[position];
        if (depth == 0) return;
        else if (depth > 0) controller.GetComponent<Game>().MoveChecker(position, 24, "white_checker");
        else controller.GetComponent<Game>().MoveChecker(position, 1, "black_checker");
    }
}
