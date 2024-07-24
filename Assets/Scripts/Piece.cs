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
    public Sprite black_checker, white_checker, black_checker_home, white_checker_home;

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
            case "black": this.GetComponent<SpriteRenderer>().sprite = black_checker; player = "black";  break;
            case "white": this.GetComponent<SpriteRenderer>().sprite = white_checker; player = "white";  break;
        }
    }

    public void SetCoords()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        float x = position;
        float y = position;
        GameObject obj;
        Debug.Log("psotiion depth");
        Debug.Log(position);
        Debug.Log(depth);

        if (position == 26)
        {
            this.GetComponent<SpriteRenderer>().sprite = white_checker_home;
            x = 5.215f;
            y = -4.5f + (float)depth * 0.2f;
        }
        else if (position == 27)
        {
            this.GetComponent<SpriteRenderer>().sprite = black_checker_home;
            x = 5.215f;
            y = 4.5f - (float)depth * 0.2f;
        }
        else if (position == 25)
        {
            this.GetComponent<SpriteRenderer>().sprite = white_checker;
            x = 0.0f;
            y = 1.0f;
            if (depth > 1)
            {
                obj = controller.GetComponent<Game>().GetPosition(position, 1);
                obj.GetComponentInChildren<TextMeshProUGUI>().text = depth.ToString();
            }
            else if (depth == 1 && controller.GetComponent<Game>().GetPosition(position, 1))
            {
                obj = controller.GetComponent<Game>().GetPosition(position, 1);
                obj.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
        }
        else if (position == 0)
        {
            this.GetComponent<SpriteRenderer>().sprite = black_checker;
            x = 0.0f;
            y = -1.0f;
            if (depth > 1)
            {
                obj = controller.GetComponent<Game>().GetPosition(position, 1);
                obj.GetComponentInChildren<TextMeshProUGUI>().text = depth.ToString();
                obj.GetComponentInChildren<TextMeshProUGUI>().color = new Color(0, 0, 0, 255);
            }
            else if (depth == 1 && controller.GetComponent<Game>().GetPosition(position, 1))
            {
                obj = controller.GetComponent<Game>().GetPosition(position, 1);
                obj.GetComponentInChildren<TextMeshProUGUI>().text = "";
                obj.GetComponentInChildren<TextMeshProUGUI>().color = new Color(0, 0, 0, 255);
            }
        }
        else
        {
            if (this.GetComponent<SpriteRenderer>().sprite == black_checker_home) this.GetComponent<SpriteRenderer>().sprite = black_checker;
            else if (this.GetComponent<SpriteRenderer>().sprite == white_checker_home) this.GetComponent<SpriteRenderer>().sprite = white_checker;
            this.GetComponentInChildren<TextMeshProUGUI>().text = "";
            if ((x <= 18.0f && x >= 13.0f) || (x <= 6.0f && x >= 1.0f)) x -= 1.0f;
            x = ((Mathf.Abs(x - 12.0f) - 6.0f) * 0.694f);
            if (y > 12.0f) y = 4.835f - ((float)Mathf.Min(depth, 5)) * 0.65f;
            else y = -4.835f + ((float)Mathf.Min(depth, 5)) * 0.65f;

            if (depth > 5)
            {
                Debug.Log("here a");
                obj = controller.GetComponent<Game>().GetPosition(position, 5);
                obj.GetComponentInChildren<TextMeshProUGUI>().text = depth.ToString();
                if (player == "black") obj.GetComponentInChildren<TextMeshProUGUI>().color = new Color(255, 255, 255, 255);
            }
            
            else if (depth == 5 && controller.GetComponent<Game>().GetPosition(position, 5))
            {
                Debug.Log("here b");
                obj = controller.GetComponent<Game>().GetPosition(position, 5);
                obj.GetComponentInChildren<TextMeshProUGUI>().text = "";
                if (player == "black") obj.GetComponentInChildren<TextMeshProUGUI>().color = new Color(255, 255, 255, 255);
            }
            
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
