using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DescriptionPosition : MonoBehaviour
{
    RectTransform thisObject;
    RectTransform basisObject;

    private void Start()
    {
        thisObject = this.GetComponent<RectTransform>();

        basisObject = GameObject.Find("ChaosCards").GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 offset;

        if(Input.mousePosition.x > 220) { offset = new Vector3(-125, -100, 0); }
        else { offset = new Vector3(125, -100, 0); }

        Debug.Log(Input.mousePosition);

        Vector3 screenPosition = Input.mousePosition + offset;

        screenPosition.z = basisObject.position.z;

        thisObject.position = Camera.main.ScreenToWorldPoint(screenPosition);
    }

    public void ActiveObject(string name)
    {
        GetComponent<Image>().color = new Color(1,1,1,1);

        transform.Find("Description").gameObject.SetActive(true);
        transform.Find("Description").GetComponent<TextMeshProUGUI>().text = CheckCard(name);
    }

    public void DesactiveObject()
    {
        GetComponent<Image>().color = new Color(1, 1, 1, 0);
        transform.Find("Description").gameObject.SetActive(false);
    }

    string CheckCard(string name)
    {
        string text = "Error";

        switch (name)
        {
            case "Teleportation(Clone)":
                text = "Instantly move a piece to any empty tile.";
                break;
            case "Swap Positions(Clone)":
                text = "Swap the positions of two pieces.";
                break;
            case "Freeze(Clone)":
                text = "Freeze an enemy piece for one turn.";
                break;
            case "Tornado(Clone)":
                text = "Move all pieces in a 3x3 area randomly.";
                break;
            case "Trap Detector(Clone)":
                text = "Detect all traps within a 3x3 area.";
                break;
            case "Trap(Clone)":
                text = "Place a hidden trap on a tile.";
                break;
            case "Sacrifice(Clone)":
                text = "Sacrifice a piece to likely destroy a similar enemy.";
                break;
            case "Revenge(Clone)":
                text = "Capture an enemy piece that just captured yours.";
                break;
            case "Banish(Clone)":
                text = "Make invisible a piece (allay or enemy) from the board.";
                break;
            case "Reveal(Clone)":
                text = "Reveal traps and invisible pieces on the board.";
                break;
            case "Promote(Clone)":
                text = "Promote a piece to a more powerful piece.";
                break;
            case "Summon(Clone)":
                text = "Summon an extra pawn and 2 assassins on the board.";
                break;
        }

        return text;
    }
}
