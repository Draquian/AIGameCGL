using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChaosManager : MonoBehaviour
{
    public int turnsOnBoard = 0;
    private const int turnsPerCard = 3;
    public List<string> abilityCards = new List<string>();
    List<GameObject> deck = new List<GameObject>();

    float commonProbability = 0.4f;
    float rareProbability = 0.3f;
    float epicProbability = 0.2f;
    float legendaryProbability = 0.1f;

    public GameObject[] commonCards;
    public GameObject[] rareCards;
    public GameObject[] epicCards;
    public GameObject[] legendCards;

    public GameObject panelChaosCard;

    bool tpCard = false;
    bool spCard = false;
    bool frCard = false;

    void Start()
    {
        turnsOnBoard = 0;
        panelChaosCard = GameObject.Find("ChaosCards");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G)) GenerateAbilityCard();
        if (tpCard) TPCard();
        if (spCard) SwapPos();
        if (frCard) Freeze();
    }

    public void IncrementRound()
    {
        turnsOnBoard++;
        if (turnsOnBoard % turnsPerCard == 0 && deck.Count < 5)
        {
            GenerateAbilityCard();
        }
    }

    public void SelectedCard(GameObject card)
    {
        UseAbilityCard(card);
    }

    private void GenerateAbilityCard()
    {
        // Define the types of ability cards
        string[] possibleRarety= { "Common", "Rare", "Epic", "Legendary" };
        int provabilityRarety = possibleRarety.Length * 10;
        int raretyCard = Random.Range(0, provabilityRarety);

        float isCommon = provabilityRarety * commonProbability;
        float isRare = (provabilityRarety * rareProbability) + isCommon;
        float isEpic = (provabilityRarety * epicProbability) + isCommon + isRare;
        float isLegend = (provabilityRarety * legendaryProbability) + isCommon + isRare + isEpic;

        GameObject randomCard = null;

        if (raretyCard <= isCommon)
        {
            //"Teleportation", "Swap Positions", "Freeze", "Vortex", "Time Anchor"
            randomCard = commonCards[Random.Range(0, commonCards.Length)];
        }
        else if (raretyCard <= isRare)
        {
            //"Invisibility", "Trap", "Sacrifice", "Revenge" 
            randomCard = rareCards[Random.Range(0, rareCards.Length)];
        }
        else if (raretyCard <= isEpic)
        {
            //"Banish", "Promote", "Void Rift"
            randomCard = epicCards[Random.Range(0, epicCards.Length)];
        }
        else if (raretyCard <= isLegend)
        {
            //"Summon"
            randomCard = legendCards[Random.Range(0, legendCards.Length)];
        }

        abilityCards.Add(randomCard.name);

        GameObject button = Instantiate(randomCard);

        button.transform.SetParent(panelChaosCard.transform, false);

        button.GetComponent<Button>().onClick.AddListener(delegate { SelectedCard(button); });

        float posY = GetComponent<MeshRenderer>().material.color == Color.black ? -100.0f : 100.0f;

        float posX = -300 + (150 * deck.Count);

        button.transform.localPosition = new Vector3(posX, posY, 0.0f);
        deck.Add(button);

        if (turnsOnBoard % 2 == 0)
            SetInteraction(true);
        else
            SetInteraction(false);
    }

    public void SetInteraction(bool whiteTurn)
    {
        if (whiteTurn)
        {
            if (GetComponent<MeshRenderer>().material.color == Color.white)
                foreach (GameObject card in deck)
                    card.GetComponent<Button>().interactable = true;
            else
                foreach (GameObject card in deck)
                    card.GetComponent<Button>().interactable = false;        
        }
        else
        {
            if (GetComponent<MeshRenderer>().material.color == Color.white)
                foreach (GameObject card in deck)
                    card.GetComponent<Button>().interactable = false;
            else
                foreach (GameObject card in deck)
                    card.GetComponent<Button>().interactable = true;
        }

        foreach (GameObject card in deck)
        {
            if (card.GetComponent<Button>().interactable == true)
                card.transform.localScale = new Vector3(1, 1, 1);
            else
                card.transform.localScale = new Vector3(0.75f, 0.75f, 1);
        }
    }

    public List<string> GetAbilityCards()
    {
        return abilityCards;
    }

    public void UseAbilityCard(GameObject cardGO)
    {
        string[] splitArray = cardGO.name.Split(char.Parse("("));
        string card = splitArray[0];

        switch (card)
        {
            case "Teleportation":
                tpCard = true;
                break;
            case "Swap Positions":
                spCard = true;
                break;
            case "Freeze":
                frCard = true;
                break;
            case "Vortex":
                Debug.Log(card);
                break;
            case "Time Anchor":
                Debug.Log(card);
                break;
            case "Invisibility":
                Debug.Log(card);
                break;
            case "Trap":
                Debug.Log(card);
                break;
            case "Sacrifice":
                Debug.Log(card);
                break;
            case "Revenge":
                Debug.Log(card);
                break;
            case "Banish":
                Debug.Log(card);
                break;
            case "Void Rift":
                Debug.Log(card);
                break;
            case "Promote":
                Debug.Log(card);
                break;
            case "Summon":
                Debug.Log(card);
                break;
        }
        abilityCards.Remove(card);
        deck.Remove(cardGO);
        Destroy(cardGO);

        int i = 0;

        foreach (GameObject cardLeft in deck)
        {
            cardLeft.transform.localPosition = new Vector3(-300 + (150 * i), cardLeft.transform.localPosition.y, 0);
            i++;
        }
    }

    void TPCard()
    {
        ChessGameManager CGM = FindObjectOfType<ChessGameManager>();

        if (CGM.resetGame == false && CGM.canPromote == false && Input.GetMouseButtonUp(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Raycast to detect tiles
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, CGM.tileLayer) && CGM.selectedPiece != null)
            {
                // Move the selected piece
                CGM.MovePiece(hit.transform.gameObject, true);

                tpCard = false;
            }
        }
    }

    void SwapPos()
    {
        ChessGameManager CGM = FindObjectOfType<ChessGameManager>();

        if (CGM.resetGame == false && CGM.canPromote == false && Input.GetMouseButtonUp(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Raycast to detect piece
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, CGM.pieceLayer) && CGM.selectedPiece != null)
            {
                GameObject hitPiece = hit.transform.gameObject;

                if (CGM.CanSelectPiece(hitPiece))
                {
                    Vector3 aux = CGM.selectedPiece.transform.position;
                    CGM.selectedPiece.transform.position = hit.transform.position;
                    CGM.SelectPiece(hitPiece);
                    CGM.selectedPiece.transform.position = aux;

                    CGM.IncrementTurn();
                    spCard = false;
                }
            }
        }
    }

    void Freeze()
    { 
        ChessGameManager CGM = FindObjectOfType<ChessGameManager>();

        if (CGM.resetGame == false && CGM.canPromote == false && Input.GetMouseButtonUp(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Raycast to detect piece
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, CGM.pieceLayer))
            {
                hit.transform.gameObject.AddComponent(typeof(FreezePiece));
                CGM.IncrementTurn();
                frCard = false;
            }
        }
    }
}
