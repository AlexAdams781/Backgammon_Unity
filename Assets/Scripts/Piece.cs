using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Piece : MonoBehaviour
{
    // References
    public GameObject controller;
    public GameObject piece;
    public GameObject canvas;
    public GameObject textUI;

    // Positions
    private int position = 0;
    private int depth = 0;

    // Variable to keep track of "black" or "white" player
    private string player;

    // References for all the sprites that the chesspiece can be
    public Sprite black_checker, white_checker;

    public void Start()
    {
        this.GetComponentInChildren<TextMeshProUGUI>().text = "";
    }

    public void Activate()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");

        // take instantiated location and adjust the transform
        SetCoords();

        switch (this.name)
        {
            case "black_checker": this.GetComponent<SpriteRenderer>().sprite = black_checker; player = "black";  break;
            case "white_checker": this.GetComponent<SpriteRenderer>().sprite = white_checker; player = "white";  break;
        }
    }

    public void SetCoords()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        float x = position;
        float y = position;

        if (position == 26.0f)
        {
            x = 0.0f;
            y = 1.0f;
            int quant = controller.GetComponent<Game>().posMatrix[26] + 1;
            Debug.Log("2666");
            if (quant > 1)
            {
                this.GetComponentInChildren<TextMeshProUGUI>().color = new Color(0, 0, 0, 255);
                this.GetComponentInChildren<TextMeshProUGUI>().text = quant.ToString();
            }
            else this.GetComponentInChildren<TextMeshProUGUI>().text = "";
        }
        else if (position == 27.0f)
        {
            x = 0.0f;
            y = -1.0f;
            int quant = 1 - controller.GetComponent<Game>().posMatrix[27];
            if (quant > 1)
            {
                Debug.Log("changed");
                controller.GetComponent<Game>().GetPosition(27, quant - 1).GetComponentInChildren<TextMeshProUGUI>().color = new Color(255, 255, 255, 0);
                this.GetComponentInChildren<TextMeshProUGUI>().color = new Color(255, 255, 255, 255);
                this.GetComponentInChildren<TextMeshProUGUI>().text = (quant).ToString();
            }
            else this.GetComponentInChildren<TextMeshProUGUI>().text = "";
        }
        else
        {
            this.GetComponentInChildren<TextMeshProUGUI>().text = "";
            if ((x <= 18.0f && x >= 13.0f) || (x <= 6.0f && x >= 1.0f)) x -= 1.0f;
            x = ((Mathf.Abs(x - 12.0f) - 6.0f) * 0.694f);
            if (y > 12.0f) y = 4.835f - ((float)depth) * 0.65f;
            else y = -4.835f + ((float)depth) * 0.65f;
        }

        this.transform.position = new Vector3(x, y, -1.0f);
    }

    public int GetPosition()
    {
        return position;
    }

    public int GetDepth()
    {
        return depth;
    }

    public void SetPosition(int x)
    {
        position = x;
    }

    public void SetDepth(int x)
    {
        depth = x;
    }

    private void OnMouseUp()
    {
        controller.GetComponent<Game>().test();
    }

}
