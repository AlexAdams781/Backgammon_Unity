using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarPanel : MonoBehaviour
{
    public GameObject controller;
    public int position = 0;

    public void SetCoords()
    {
        if (position == 25) this.transform.position = new Vector3(0.0f, 1.0f, -2.0f);
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
        Debug.Log("Bar");
        controller = GameObject.FindGameObjectWithTag("GameController");
        controller.GetComponent<Game>().TakeTurn(position);
    }
}
