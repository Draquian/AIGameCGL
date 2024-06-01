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

    public GameObject[] pawnPrefab;

    public GameObject panelChaosCard;

    bool tpCard = false;
    bool spCard = false;
    bool frCard = false;
    bool trCard = false;
    bool saCard = false;
    bool prCard = false;
    bool reCard = false;
    bool baCard = false;
    bool deCard = false;

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
        if (trCard) GenerateTrap();
        if (saCard) SacrificeAbility();
        if (prCard) Promotion();
        if (reCard) AddRevenge();
        if (baCard) BanishPiece();
        if (deCard) TrapDetect();
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
            //"Teleportation", "Swap Positions", "Freeze", "Tornado"
            randomCard = commonCards[Random.Range(0, commonCards.Length)];
        }
        else if (raretyCard <= isRare)
        {
            //"Trap Detector", "Trap", "Sacrifice", "Revenge" 
            randomCard = rareCards[Random.Range(0, rareCards.Length)];
        }
        else if (raretyCard <= isEpic)
        {
            //"Banish", "Promote", "Reveal"
            randomCard = epicCards[Random.Range(0, epicCards.Length)];
        }
        else if (raretyCard <= isLegend)
        {
            //"Summon"
            randomCard = legendCards[Random.Range(0, legendCards.Length)];
        }

        abilityCards.Add(randomCard.name);

        GameObject button = Instantiate(randomCard);

        GameObject parentCards = GameObject.Find("CardsTable");

        button.transform.SetParent(parentCards.transform, false);

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
            case "Tornado":
                Tornado();
                break;
            case "Time Anchor":
                Debug.Log(card);    //desabilitado por ahora
                break;
            case "Trap Detector":
                deCard = true;
                break;
            case "Trap":
                trCard = true;
                break;
            case "Sacrifice":
                saCard = true;
                break;
            case "Revenge":
                reCard = true;
                break;
            case "Banish":
                baCard = true;
                break;
            case "Reveal":
                Reveal();
                break;
            case "Promote":
                prCard = true;
                break;
            case "Summon":
                Summon();
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

        if (CGM.resetGame == false && CGM.canPromote == false && CGM.endTurn == false && Input.GetMouseButtonUp(1))
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

        if (CGM.resetGame == false && CGM.canPromote == false && CGM.endTurn == false && Input.GetMouseButtonUp(1))
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

        if (CGM.resetGame == false && CGM.canPromote == false && CGM.endTurn == false && Input.GetMouseButtonUp(1))
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

    void Tornado()
    {
        GameObject tornado = new GameObject("A Big Tornado");

        int posX = Random.Range(1, 8);
        int posZ = Random.Range(1, 8);
        tornado.transform.position = new Vector3(posX, 1, posZ);

        tornado.AddComponent<BoxCollider>();
        tornado.GetComponent<BoxCollider>().isTrigger = true;

        tornado.AddComponent<TornadoManager>();
    }
    
    void GenerateTrap()
    {
        ChessGameManager CGM = FindObjectOfType<ChessGameManager>();

        if (CGM.resetGame == false && CGM.canPromote == false && CGM.endTurn == false && Input.GetMouseButtonUp(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Raycast to detect piece
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, CGM.pieceLayer))
            {
                GameObject hitPiece = hit.transform.gameObject;

                if (CGM.CanSelectPiece(hitPiece))
                {
                    if (GameObject.Find("All_Traps") == null) new GameObject("All_Traps");

                    GameObject trap = new GameObject("Its a trap");
                    trap.transform.SetParent(GameObject.Find("All_Traps").transform);

                    float posX = hitPiece.transform.position.x;
                    float posZ = hitPiece.transform.position.z;

                    trap.transform.position = new Vector3(posX, 1, posZ);

                    trap.AddComponent<BoxCollider>();
                    trap.GetComponent<BoxCollider>().isTrigger = true;

                    trap.layer = 8;

                    trap.AddComponent<Trap>();

                    CGM.IncrementTurn();
                    trCard = false;
                }
            }
        }
    }

    void Reveal()
    {
        Trap[] trapTile = FindObjectsOfType<Trap>();

        if (trapTile == null) return;

        StartCoroutine(RevealTrap(trapTile));

        GameObject allPieces = GameObject.Find("Pieces");

        foreach (Transform child in allPieces.transform)
        {
            foreach(Transform inChild in child)
            {
                Color tempColor = inChild.gameObject.GetComponent<MeshRenderer>().material.color;
                tempColor.a = 1;
                inChild.gameObject.GetComponent<MeshRenderer>().material.color = tempColor;
            }
        }
    }

    IEnumerator RevealTrap(Trap[] trapTile)
    {
        ChessGameManager CGM = FindObjectOfType<ChessGameManager>();

        List<GameObject> traps = new List<GameObject>();
        List<Color> trapsColor = new List<Color>();

        foreach (Trap tT in trapTile)
        {
            Collider[] colliders = Physics.OverlapSphere(tT.GetPos(), 0.5f, CGM.tileLayer);
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Tile"))
                {
                    traps.Add(collider.gameObject);
                    trapsColor.Add(collider.GetComponent<MeshRenderer>().material.color);
                    collider.GetComponent<MeshRenderer>().material.color = Color.green;
                }
            }
            yield return new WaitForSeconds(0.25f);
        }

        //yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < traps.Count; i++)
        {
            traps[i].GetComponent<MeshRenderer>().material.color = trapsColor[i];

            yield return new WaitForSeconds(0.1f);
        }
    }

    void SacrificeAbility()
    {
        //Sacrifice 1 piece to maybe kiil on similar random enemy piece

        ChessGameManager CGM = FindObjectOfType<ChessGameManager>();

        if (CGM.resetGame == false && CGM.canPromote == false && CGM.endTurn == false && Input.GetMouseButtonUp(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Raycast to detect piece
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, CGM.pieceLayer))
            {
                GameObject hitPiece = hit.transform.gameObject;

                if (CGM.CanSelectPiece(hitPiece) && hitPiece.name != "King(Clone)")
                {
                    GameObject enemyPieces = CGM.isWhiteTurn == true ? GameObject.Find("Black") : GameObject.Find("White");

                    List<GameObject> canDie = new List<GameObject>();

                    foreach (Transform child in enemyPieces.transform)
                    {
                        if (child.name == hitPiece.name) canDie.Add(child.gameObject);
                    }

                    int ranTarget = Random.Range(0, canDie.Count);

                    Color originalColor = canDie[ranTarget].GetComponent<MeshRenderer>().material.color;

                    canDie[ranTarget].GetComponent<MeshRenderer>().material.color = Color.red;

                    StartCoroutine(Sacrifacing(canDie[ranTarget], originalColor));

                    Destroy(hitPiece);

                    saCard = false;
                }
            }
        }
    }

    IEnumerator Sacrifacing(GameObject target, Color colorOG)
    {
        ChessGameManager CGM = FindObjectOfType<ChessGameManager>();
        yield return new WaitForSeconds(1f);

        int ranNum = Random.Range(0, 2);

        if (ranNum == 0)
        {
            target.GetComponent<MeshRenderer>().material.color = colorOG;
            Destroy(target);
        }
        else
        {
            target.GetComponent<MeshRenderer>().material.color = colorOG;
        }

        CGM.IncrementTurn();
    }

    void Promotion()
    {
        ChessGameManager CGM = FindObjectOfType<ChessGameManager>();

        if (CGM.resetGame == false && CGM.canPromote == false && CGM.endTurn == false && Input.GetMouseButtonUp(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Raycast to detect piece
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, CGM.pieceLayer))
            {
                GameObject hitPiece = hit.transform.gameObject;

                if (CGM.CanSelectPiece(hitPiece))
                {
                    CGM.pawnToPromote = hitPiece;

                    CGM.canPromote = true;
                    CGM.panelToPromote.SetActive(true);

                    CGM.IncrementTurn();

                    prCard = false;
                }
            }
        }
    }

    void AddRevenge()
    {
        ChessGameManager CGM = FindObjectOfType<ChessGameManager>();

        if (CGM.resetGame == false && CGM.canPromote == false && CGM.endTurn == false && Input.GetMouseButtonUp(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Raycast to detect piece
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, CGM.pieceLayer))
            {
                GameObject hitPiece = hit.transform.gameObject;

                if (CGM.CanSelectPiece(hitPiece))
                {
                    hitPiece.AddComponent<Revenge>();
                    reCard = false;
                }
            }
        }
    }

    void BanishPiece()
    {
        ChessGameManager CGM = FindObjectOfType<ChessGameManager>();

        if (CGM.resetGame == false && CGM.canPromote == false && CGM.endTurn == false && Input.GetMouseButtonUp(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Raycast to detect piece
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, CGM.pieceLayer))
            {
                GameObject hitPiece = hit.transform.gameObject;

                    float r = hitPiece.GetComponent<MeshRenderer>().material.color.r;
                    float g = hitPiece.GetComponent<MeshRenderer>().material.color.g;
                    float b = hitPiece.GetComponent<MeshRenderer>().material.color.b;
                    float a = hitPiece.GetComponent<MeshRenderer>().material.color.a;

                    while (hitPiece.GetComponent<MeshRenderer>().material.color.a > 0f)
                    {
                        a = a - 0.0001f;
                        hitPiece.GetComponent<MeshRenderer>().material.color = new Color(r, g, b, a);
                    }
                    baCard = false;          
            }
        }
    }

    void Summon()
    {
        if (this.GetComponent<MeshRenderer>().material.color.r >= 0.5f)
        {
            for (int i = 0; i < pawnPrefab.Length; i++)
            {
                if (i == 0 && this.transform.position.x != 0 && !IsPieceAtTile(new Vector3(this.transform.position.x - 1, this.transform.position.y, this.transform.position.z)))
                {
                    GameObject pawn = Instantiate(pawnPrefab[i], transform);
                    pawn.transform.position = new Vector3(this.transform.position.x - 1, this.transform.position.y, this.transform.position.z);

                    pawn.GetComponent<MeshRenderer>().material.color = Color.white;
                    pawn.transform.parent = GameObject.Find("White").transform;
                    pawn.transform.localScale = pawn.transform.localScale * 2;
                }
                else if (i == 1 && this.transform.position.x != 9 && !IsPieceAtTile(new Vector3(this.transform.position.x + 1, this.transform.position.y, this.transform.position.z)))
                {
                    GameObject pawn = Instantiate(pawnPrefab[i], transform);
                    pawn.transform.position = new Vector3(this.transform.position.x + 1, this.transform.position.y, this.transform.position.z);

                    pawn.GetComponent<MeshRenderer>().material.color = Color.white;
                    pawn.transform.parent = GameObject.Find("White").transform;
                    pawn.transform.localScale = pawn.transform.localScale * 2;
                }
                else if (i == 2 && this.transform.position.z != 0 && !IsPieceAtTile(new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - 1)))
                {
                    GameObject pawn = Instantiate(pawnPrefab[i], transform);
                    pawn.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - 1);

                    pawn.GetComponent<MeshRenderer>().material.color = Color.white;
                    pawn.transform.parent = GameObject.Find("White").transform;
                    pawn.transform.localScale = pawn.transform.localScale * 2;
                }
            }
        }
        else if (this.GetComponent<MeshRenderer>().material.color.r <= 0.49f)
        {
            for (int i = 0; i < pawnPrefab.Length; i++)
            {
                if (i == 0 && this.transform.position.x != 0 && !IsPieceAtTile(new Vector3(this.transform.position.x - 1, this.transform.position.y, this.transform.position.z)))
                {
                    GameObject pawn = Instantiate(pawnPrefab[i], transform);
                    pawn.transform.position = new Vector3(this.transform.position.x - 1, this.transform.position.y, this.transform.position.z);

                    pawn.GetComponent<MeshRenderer>().material.color = Color.black;
                    pawn.transform.parent = GameObject.Find("Black").transform;
                    pawn.transform.localScale = pawn.transform.localScale * 2;
                }
                else if (i == 1 && this.transform.position.x != 9 && !IsPieceAtTile(new Vector3(this.transform.position.x + 1, this.transform.position.y, this.transform.position.z)))
                {
                    GameObject pawn = Instantiate(pawnPrefab[i], transform);
                    pawn.transform.position = new Vector3(this.transform.position.x + 1, this.transform.position.y, this.transform.position.z);

                    pawn.GetComponent<MeshRenderer>().material.color = Color.black;
                    pawn.transform.parent = GameObject.Find("Black").transform;
                    pawn.transform.localScale = pawn.transform.localScale * 2;
                }
                else if (i == 2 && this.transform.position.z != 9 && !IsPieceAtTile(new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + 1)))
                {
                    GameObject pawn = Instantiate(pawnPrefab[i], transform);
                    pawn.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + 1);

                    pawn.GetComponent<MeshRenderer>().material.color = Color.black;
                    pawn.transform.parent = GameObject.Find("Black").transform;
                    pawn.transform.localScale = pawn.transform.localScale * 2;
                }
            }
        }
    }  

    bool IsPieceAtTile(Vector3 pos)
    {
        ChessGameManager CGM = FindObjectOfType<ChessGameManager>();

        Collider[] colliders = Physics.OverlapSphere(pos, 0.5f, CGM.pieceLayer);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Piece"))
            {
                return true;
            }
        }
        return false;
    }

    void TrapDetect()
    {
        List<Trap> allTraps = new List<Trap>();

        Trap[] trapTile = FindObjectsOfType<Trap>();

        ChessGameManager CGM = FindObjectOfType<ChessGameManager>();

        if (CGM.resetGame == false && CGM.canPromote == false && CGM.endTurn == false && Input.GetMouseButtonUp(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Raycast to detect tiles
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, CGM.tileLayer))
            {
                foreach (Trap tr in trapTile)
                {
                    float posX = tr.transform.position.x;
                    float posZ = tr.transform.position.z;

                    if((posX > hit.transform.position.x - 2) && (posX < hit.transform.position.x + 2) && (posZ > hit.transform.position.z - 2) && (posZ < hit.transform.position.z + 2))
                    {
                        allTraps.Add(tr);
                    }
                }

                deCard = false;

                StartCoroutine(RevealTrapDetected(allTraps));
            }
        }
    }

    GameObject IsATrap(Vector3 pos)
    {
        ChessGameManager CGM = FindObjectOfType<ChessGameManager>();

        Debug.Log(pos);

        Collider[] colliders = Physics.OverlapSphere(pos, 0.5f, CGM.trapLayer);
        foreach (Collider collider in colliders)
        {
            Debug.Log("Casi Gotcha");
            if (collider.gameObject.GetComponent<Trap>() != null)
            {
                Debug.Log("Gotcha");
                return collider.gameObject;
            }
        }
        return null;
    }

    IEnumerator RevealTrapDetected(List<Trap> trapTile)
    {
        ChessGameManager CGM = FindObjectOfType<ChessGameManager>();

        List<GameObject> traps = new List<GameObject>();
        List<Color> trapsColor = new List<Color>();

        foreach (Trap tT in trapTile)
        {
            Collider[] colliders = Physics.OverlapSphere(tT.GetPos(), 0.5f, CGM.tileLayer);
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Tile"))
                {
                    traps.Add(collider.gameObject);
                    trapsColor.Add(collider.GetComponent<MeshRenderer>().material.color);
                    collider.GetComponent<MeshRenderer>().material.color = Color.green;
                }
            }
            yield return new WaitForSeconds(0.25f);
        }

        //yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < traps.Count; i++)
        {
            traps[i].GetComponent<MeshRenderer>().material.color = trapsColor[i];

            yield return new WaitForSeconds(0.1f);
        }
    }
}
