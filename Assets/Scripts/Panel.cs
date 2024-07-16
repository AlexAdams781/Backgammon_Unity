using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel : MonoBehaviour
{
    public GameObject controller;
    public int position = 0;

    public void SetCoords()
    {
        float x = position;
        float y = position;

        if ((x <= 18.0f && x >= 13.0f) || (x <= 6.0f && x >= 1.0f)) x -= 1.0f;
        x = ((Mathf.Abs(x - 12.0f) - 6.0f) * 0.694f);

        if (y > 12.0f) y = 2.5f;
        else y = -2.5f;

        this.transform.position = new Vector3(x, y, -2.0f);
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
        else if (depth > 0) controller.GetComponent<Game>().MoveChecker(position, position - 1, "white_checker");
        else controller.GetComponent<Game>().MoveChecker(position, position + 1, "black_checker");
    }
}
