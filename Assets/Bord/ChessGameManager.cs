using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
 TODO: 
        -Make the art
        -Add AI to cast the game as an Epic War

Maybe Todo:
        - Enroque
        - Anpasant
 */

public class ChessGameManager : MonoBehaviour
{
    public LayerMask pieceLayer;
    public LayerMask tileLayer;
    public LayerMask trapLayer;

    public GameObject selectedPiece;
    public bool isWhiteTurn = true; // White starts first
    public bool endTurn = false;

    private Color colorPieceSelected;

    public List<PriestManager.CapturedPieceInfo> capturedPiecesWhite = new List<PriestManager.CapturedPieceInfo>();
    public List<PriestManager.CapturedPieceInfo> capturedPiecesBlack = new List<PriestManager.CapturedPieceInfo>();

    public bool resetGame = false;

    public bool canPromote = false;
    public GameObject panelToPromote;
    public GameObject pawnToPromote;

    public GameObject losePanel;
    float alphaPanel;

    public TextMeshProUGUI loseSide;
    public TextMeshProUGUI winSide;

    public TextMeshProUGUI normalText1;
    public TextMeshProUGUI normalText3;

    public GameObject whiteCamera;
    public GameObject blackCamera;
    public GameObject light;

    void Update()
    {
        if (!resetGame && !canPromote && Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Raycast to detect pieces
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, pieceLayer))
            {
                // Select the piece
                GameObject hitPiece = hit.transform.gameObject;
                Vector3 temporalPos = hit.transform.position;
                temporalPos.y = 3;
                light.transform.position = temporalPos;
                if (CanSelectPiece(hitPiece))
                {
                    SelectPiece(hitPiece);
                }
            }
        }

        if(!resetGame && !endTurn && !canPromote && Input.GetMouseButtonUp(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Raycast to detect tiles
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, tileLayer) && selectedPiece != null)
            {
                // Move the selected piece
                MovePiece(hit.transform.gameObject);
                Vector3 temporalPos = hit.transform.position;
                temporalPos.y = 3;
                light.transform.position = temporalPos;
            }
        }
        
        if(Input.GetKeyDown(KeyCode.D))
        {
            Destroy(selectedPiece.GetComponent<PriestManager>());
        }
        if(Input.GetKeyUp(KeyCode.E))
        {
            EndGame("white");
        }
        if(Input.GetKeyUp(KeyCode.W))
        {
            EndGame("black");
        }
        
        if(Input.GetKeyDown(KeyCode.R))
        {
            BoardChessGeneration BCG = FindObjectOfType<BoardChessGeneration>();
            BCG.ResetGame();
            losePanel.SetActive(false);

            GameObject deck = GameObject.Find("CardsTable");

            foreach (Transform child in deck.transform)
            {
                Destroy(child.gameObject);
            }

            Destroy(GameObject.Find("All_Traps"));

            isWhiteTurn = true;
            resetGame = false;
        }

        if (resetGame && Input.anyKey)
        {
            BoardChessGeneration BCG = FindObjectOfType<BoardChessGeneration>();
            BCG.ResetGame();
            losePanel.SetActive(false);

            GameObject deck = GameObject.Find("CardsTable");

            foreach (Transform child in deck.transform)
            {
                Destroy(child.gameObject);
            }

            Destroy(GameObject.Find("All_Traps"));

            isWhiteTurn = true;
            resetGame = false;
        }

        if(resetGame)
        { 
            alphaPanel += Time.deltaTime;

            Color auxColor = losePanel.GetComponent<Image>().color;
            auxColor.a = alphaPanel;
            losePanel.GetComponent<Image>().color = auxColor;
        }
    }

    public bool CanSelectPiece(GameObject piece)
    {
        if (piece.CompareTag("Piece") && piece.GetComponent<FreezePiece>() == null)
        {
            Color pieceColor = piece.GetComponent<MeshRenderer>().material.color;
            if ((isWhiteTurn && pieceColor.r >= 0.5f) || (!isWhiteTurn && pieceColor.r <= 0.49f))
            {
                return true;
            }
        }
        return false;
    }

    public void SelectPiece(GameObject piece)
    {
        // Deselect previous piece if any
        if (selectedPiece != null)
        {
            selectedPiece.GetComponent<MeshRenderer>().material.color = colorPieceSelected;
        }

        // Highlight selected piece
        selectedPiece = piece;
        colorPieceSelected = selectedPiece.GetComponent<MeshRenderer>().material.color;
        selectedPiece.GetComponent<MeshRenderer>().material.color = Color.yellow;
    }

    public void MovePiece(GameObject tile, bool usignCard = false)
    {
        // Check if the destination tile contains a piece
        GameObject pieceAtTile = GetPieceAtTile(tile);
        GameObject pieceAtPiece = AdjacentToDeath(tile);

        // Check if the selected piece can move to the destination tile (add your movement logic here)
        if (!usignCard)
            switch(selectedPiece.name)
            {
                case "Pawn(Clone)":
                    if (!PawnMovement(tile)) return;
                    break;
                case "Tower(Clone)":
                    if (!MoveRook(tile)) return;
                    break;
                case "Bishop(Clone)":
                    if (!MoveBishop(tile)) return;
                    break;            
                case "Queen(Clone)":
                    if (!MoveQueen(tile)) return;
                    break;
                case "King(Clone)":
                    if (!MoveKing(tile)) return;
                    break;
                case "Knight(Clone)":
                    if (!MoveKnight(tile)) return;
                    break;
                case "SoulEater(Clone)":
                    if (!MoveSoulEater(tile)) return;
                    break;
                case "Assassin(Clone)":
                    if (!MoveAssassin(tile)) return;
                    break;
                case "Priest(Clone)":
                    if (!MovePriest(tile)) return;
                    break;
                case "Chaos(Clone)":
                    if (!IsChaosMoveValid(selectedPiece, tile)) return;
                    break;
                default:
                    return;
            }
        else
            switch(selectedPiece.name)
            {
                case "Pawn(Clone)":
                    CheckForPromotion(tile);
                    pawnToPromote = selectedPiece;
                    break;
                case "Priest(Clone)":
                    if (isWhiteTurn)
                    { 
                        if (pieceAtPiece == null)
                            if (pieceAtTile == null && selectedPiece.GetComponent<PriestManager>().CanRevive(selectedPiece, tile, capturedPiecesWhite))
                                selectedPiece.GetComponent<PriestManager>().RevivePiece(tile, capturedPiecesWhite);
                    }
                    else
                    { 
                        if (pieceAtPiece == null)
                                if (pieceAtTile == null && selectedPiece.GetComponent<PriestManager>().CanRevive(selectedPiece, tile, capturedPiecesBlack))
                                    selectedPiece.GetComponent<PriestManager>().RevivePiece(tile, capturedPiecesBlack);
                    }
                    break;
                default:
                    break;
            }

        if (pieceAtTile != null)
        {
            // If there's a piece at the tile, check if it's an opponent's piece
            Color pieceColor = pieceAtTile.GetComponent<MeshRenderer>().material.color;
            if ((isWhiteTurn && pieceColor == Color.black) || (!isWhiteTurn && pieceColor == Color.white))
            {
                if(pieceAtTile.name == "King(Clone)")
                {
                    if (isWhiteTurn) EndGame("white");
                    else EndGame("black");

                    pieceAtTile.SetActive(false); //dont destroy, they can revive
                }
                else if(selectedPiece.name != "Priest(Clone)")
                {
                    // Capture the opponent's piece
                    //Destroy(pieceAtTile);
                    if (!isWhiteTurn)
                        capturedPiecesWhite.Add(new PriestManager.CapturedPieceInfo(pieceAtTile, tile.transform.position, 0));
                    else
                        capturedPiecesBlack.Add(new PriestManager.CapturedPieceInfo(pieceAtTile, tile.transform.position, 0));

                    pieceAtTile.SetActive(false); //dont destroy, they can revive

                    if (pieceAtTile.GetComponent<Revenge>() != null) pieceAtTile.GetComponent<Revenge>().TakeRevenge(selectedPiece);
                }
            }
            else
            {
                // If it's own piece, do not move
                return;
            }
        }

        // Move the piece to the destination tile
        selectedPiece.transform.position = new Vector3(tile.transform.position.x, 1f, tile.transform.position.z);

        IncrementTurn();
    }

    void EndGame(string winner)
    {
        resetGame = true;

        losePanel.SetActive(true);

        Color txtColor = winner == "black" ? new Vector4(1, 1, 1, 0) : new Vector4(0, 0, 0, 0);

        losePanel.GetComponent<Image>().color = txtColor;
        alphaPanel = 0;

        string txtL = winner == "white" ? "WHITE" : "BLACK";
        string txtW = winner == "white" ? "BLACK" : "WHITE";

        Color txtC = winner == "white" ? new Vector4(1, 1, 1, 1) : new Vector4(0, 0, 0, 1);

        loseSide.text = txtL;
        winSide.text = txtW;

        normalText1.color = txtC;
        normalText3.color = txtC;
        loseSide.color = txtC;
        winSide.color = txtC;
    }

    public GameObject GetPieceAtTile(GameObject tile)
    {
        Collider[] colliders = Physics.OverlapSphere(tile.transform.position, 0.5f, pieceLayer);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Piece"))
            {
                return collider.gameObject;
            }
        }
        return null;
    }

    bool PawnMovement(GameObject tile)
    {
        // Check if the destination tile is forward one square
        float forwardDistance = isWhiteTurn ? tile.transform.position.z - selectedPiece.transform.position.z : selectedPiece.transform.position.z - tile.transform.position.z;
        float lateralDistance = isWhiteTurn ? tile.transform.position.x - selectedPiece.transform.position.x : selectedPiece.transform.position.x - tile.transform.position.x;

        GameObject isAPiece = GetPieceAtTile(tile);

        if (((isWhiteTurn && tile.transform.position.z == 3f) || (!isWhiteTurn && tile.transform.position.z == 6f)) && (Mathf.Approximately(forwardDistance, 1f) || Mathf.Approximately(forwardDistance, 2f)) && Mathf.Approximately(lateralDistance, 0f))
        {
            return true;
        }

        if (Mathf.Approximately(forwardDistance, 1f) && Mathf.Approximately(lateralDistance, 0f))
        {
            // Check for promotion
            CheckForPromotion(tile);

            pawnToPromote = selectedPiece;

            //valid move
            return true;
 
        }
        else if (Mathf.Approximately(forwardDistance, 1f) && (Mathf.Approximately(lateralDistance, -1f) || Mathf.Approximately(lateralDistance, 1f)) && GetPieceAtTile(tile) != null)
        {
            // Check for promotion
            CheckForPromotion(tile);

            pawnToPromote = selectedPiece;

            //valid move
            return true;
        }
        else
        {
            // Invalid move, do not move the pawn
            return false;
        }
    }

    void CheckForPromotion(GameObject Tile)
    {
        if ((isWhiteTurn && Tile.transform.position.z == 9f) || (!isWhiteTurn && Tile.transform.position.z == 0f))
        {
            // Promote the pawn
            // (You can implement this part according to your game rules, e.g., showing a promotion UI)
            canPromote = true;
            panelToPromote.SetActive(true);
        }
    }

    public void GetPromoteTo(GameObject newPiece)
    {
        PromoteTo(pawnToPromote, newPiece.name);
    }

    void PromoteTo(GameObject pawn, string newPiece)
    {
        GameObject promotionTo = null;

        BoardChessGeneration BCG = FindObjectOfType<BoardChessGeneration>();

        switch (newPiece)
        {
            case "Tower":
                promotionTo = Instantiate(BCG.piecePrefabs[3]);
                promotionTo.transform.position = new Vector3(pawn.transform.position.x, pawn.transform.position.y, pawn.transform.position.z);
                break;
            case "Bishop":
                promotionTo = Instantiate(BCG.piecePrefabs[5]);
                promotionTo.transform.position = new Vector3(pawn.transform.position.x, pawn.transform.position.y, pawn.transform.position.z);
                break;
            case "Queen":
                promotionTo = Instantiate(BCG.piecePrefabs[6]);
                promotionTo.transform.position = new Vector3(pawn.transform.position.x, pawn.transform.position.y, pawn.transform.position.z);
                break;
            case "Knight":
                promotionTo = Instantiate(BCG.piecePrefabs[4]);
                promotionTo.transform.position = new Vector3(pawn.transform.position.x, pawn.transform.position.y, pawn.transform.position.z);
                break;
            case "SoulEater":
                promotionTo = Instantiate(BCG.piecePrefabs[2]);
                promotionTo.transform.position = new Vector3(pawn.transform.position.x, pawn.transform.position.y, pawn.transform.position.z);
                break;
            case "Assassin":
                promotionTo = Instantiate(BCG.piecePrefabs[1]);
                promotionTo.transform.position = new Vector3(pawn.transform.position.x, pawn.transform.position.y, pawn.transform.position.z);
                break;
            case "Priest":
                promotionTo = Instantiate(BCG.piecePrefabs[9]);
                promotionTo.transform.position = new Vector3(pawn.transform.position.x, pawn.transform.position.y, pawn.transform.position.z);
                break;
            case "Chaos":
                promotionTo = Instantiate(BCG.piecePrefabs[8]);
                promotionTo.transform.position = new Vector3(pawn.transform.position.x, pawn.transform.position.y, pawn.transform.position.z);
                break;
            default:
                return;
        }

        if(promotionTo != null)
        {
            promotionTo.GetComponent<MeshRenderer>().material.color = pawn.GetComponent<MeshRenderer>().material.color;
        }

        Destroy(pawn);
        panelToPromote.SetActive(false);
        canPromote = false;
    }

    bool MoveRook(GameObject tile)
    {
        // Check if the destination tile is in the same row or column
        if (IsInSameRow(tile) || IsInSameColumn(tile))
        {
            // Check if the path between the rook and the destination tile is clear
            if (IsPathClear(selectedPiece, tile))
            {
                //valid move
                return true;
            }

            //Invalid move
            return false;
        }

        //Invalid move
        return false;
    }

    bool IsInSameRow(GameObject tile)
    {
        return Mathf.Approximately(tile.transform.position.z, selectedPiece.transform.position.z);
    }

    bool IsInSameColumn(GameObject tile)
    {
        return Mathf.Approximately(tile.transform.position.x, selectedPiece.transform.position.x);
    }

    bool IsPathClear(GameObject startTile, GameObject endTile)
    {
        // Check if there are no pieces between start and end tile
        Vector3 direction = (endTile.transform.position - startTile.transform.position).normalized;
        float distance = Vector3.Distance(startTile.transform.position, endTile.transform.position);

        RaycastHit[] hits = Physics.RaycastAll(startTile.transform.position, direction, distance);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Piece") && hit.collider.gameObject != selectedPiece)
            {
                return false;
            }
        }

        return true;
    }

    GameObject PieceInPath(GameObject startTile, GameObject endTile)
    {
        // Check if there are no pieces between start and end tile
        Vector3 direction = (endTile.transform.position - startTile.transform.position).normalized;
        float distance = Vector3.Distance(startTile.transform.position, endTile.transform.position);

        RaycastHit[] hits = Physics.RaycastAll(startTile.transform.position, direction, distance);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Piece") && hit.collider.gameObject != selectedPiece)
            {
                return hit.collider.gameObject;
            }
        }

        return null;
    }

    bool MoveBishop(GameObject tile)
    {
        // Check if the destination tile is on a diagonal path
        if (IsOnDiagonal(selectedPiece, tile))
        {
            // Check if the path between the bishop and the destination tile is clear
            if (IsPathClear(selectedPiece, tile))
            {
                //valid move
                return true;
            }

            //Invalid move
            return false;
        }

        //Invalid move
        return false;
    }

    bool IsOnDiagonal(GameObject startTile, GameObject endTile)
    {
        float xOffset = Mathf.Abs(endTile.transform.position.x - startTile.transform.position.x);
        float yOffset = Mathf.Abs(endTile.transform.position.z - startTile.transform.position.z);

        return Mathf.Approximately(xOffset, yOffset);
    }

    bool MoveQueen(GameObject tile)
    {
        // Check if the destination tile is on a horizontal, vertical, or diagonal path
        if (IsInSameColumn(tile) || IsInSameRow(tile) || IsOnDiagonal(selectedPiece, tile))
        {
            // Check if the path between the queen and the destination tile is clear
            if (IsPathClear(selectedPiece, tile))
            {
                //valid move
                return true;
            }

            //Invalid move
            return false;
        }

        //Invalid move
        return false;
    }

    bool MoveKing(GameObject tile)
    {
        // Check if the destination tile is adjacent to the king's current position
        if (IsAdjacent(selectedPiece, tile))
        {           
            //valid move
            return true;
        }

        //Invalid move
        return false;
    }

    bool IsAdjacent(GameObject startTile, GameObject endTile)
    {
        // Calculate the absolute difference in x and z coordinates between the start and end tiles
        float xOffset = Mathf.Abs(endTile.transform.position.x - startTile.transform.position.x);
        float zOffset = Mathf.Abs(endTile.transform.position.z - startTile.transform.position.z);

        // Check if the destination tile is adjacent to the start tile (one square away)
        return (xOffset <= 1 && zOffset <= 1);
    }

    bool MoveKnight(GameObject tile)
    {
        // Check if the destination tile is a valid knight move
        if (IsKnightMove(selectedPiece, tile))
        {
            //valid move
            return true;
        }

        //Invalid move
        return false;
    }

    bool IsKnightMove(GameObject startTile, GameObject endTile)
    {
        // Calculate the absolute difference in x and z coordinates between the start and end tiles
        float xOffset = Mathf.Abs(endTile.transform.position.x - startTile.transform.position.x);
        float zOffset = Mathf.Abs(endTile.transform.position.z - startTile.transform.position.z);

        // Check if the move is an "L" shape
        return (xOffset == 2 && zOffset == 1) || (xOffset == 1 && zOffset == 2);
    }

    bool MoveSoulEater(GameObject tile)
    {
        // Check if the destination tile is a valid Soul Eater move
        if (IsSoulEaterMove(selectedPiece, tile) || selectedPiece.GetComponent<SoulEaterManager>().ValidPieceAbilities())
        {
            // Capture the piece if present
            GameObject pieceAtTile = GetPieceAtTile(tile);

            if(selectedPiece.GetComponent<PriestManager>() != null)
            {
                GameObject pieceAtPiece = AdjacentToDeath(tile);

                if (isWhiteTurn)
                { 
                    if (pieceAtPiece == null)
                        if (pieceAtTile == null && selectedPiece.GetComponent<PriestManager>().CanRevive(selectedPiece, tile, capturedPiecesWhite))
                        {
                            selectedPiece.GetComponent<PriestManager>().RevivePiece(tile, capturedPiecesWhite);
                            Destroy(selectedPiece.GetComponent<PriestManager>());
                        }
                }
                else
                { 
                    if (pieceAtPiece == null)
                        if (pieceAtTile == null && selectedPiece.GetComponent<PriestManager>().CanRevive(selectedPiece, tile, capturedPiecesBlack))
                        {
                            selectedPiece.GetComponent<PriestManager>().RevivePiece(tile, capturedPiecesBlack);
                            Destroy(selectedPiece.GetComponent<PriestManager>());
                        }
                }  
            }

            if (pieceAtTile != null && selectedPiece.GetComponent<SoulEaterManager>() != null && selectedPiece != pieceAtTile)
            {
                // Soul Eater copies abilities
                selectedPiece.GetComponent<SoulEaterManager>().CopyPieceAbilities(pieceAtTile);

                if (selectedPiece.GetComponent<PriestManager>() != null)
                    Destroy(selectedPiece.GetComponent<PriestManager>());

                if (pieceAtTile.name == "Priest(Clone)")
                    selectedPiece.gameObject.AddComponent(typeof(PriestManager));
            }

            //valid move
            return true;
        }

        //Invalid move
        return false;
    }

    bool IsSoulEaterMove(GameObject startTile, GameObject endTile)
    {
        float xOffset = Mathf.Abs(endTile.transform.position.x - startTile.transform.position.x);
        float zOffset = Mathf.Abs(endTile.transform.position.z - startTile.transform.position.z);

        // Move like a king or knight
        return (xOffset <= 1 && zOffset <= 1) || (xOffset == 2 && zOffset == 1) || (xOffset == 1 && zOffset == 2);
    }

    bool MoveAssassin(GameObject tile)
    {
        // Check if the destination tile is a valid Soul Eater move
        if (IsAssassinMove(selectedPiece, tile))
        {
            // Capture the piece if present
            GameObject pieceAtTile = GetPieceAtTile(tile);

            if(pieceAtTile != null && selectedPiece.GetComponent<AssassinManager>().GetTarget() == pieceAtTile) return true;
            else if(pieceAtTile != null && selectedPiece.GetComponent<AssassinManager>().GetTarget() != pieceAtTile) return false;

            if (!IsPathClear(selectedPiece, tile))
            {
                GameObject aux = PieceInPath(selectedPiece, tile);

                selectedPiece.GetComponent<AssassinManager>().SetTarget(aux);
            }

            //valid move
            return true;
        }

        //Invalid move
        return false;
    }

    bool IsAssassinMove(GameObject start, GameObject end)
    {
        float xOffset = Mathf.Abs(start.transform.position.x - end.transform.position.x);
        float zOffset = Mathf.Abs(start.transform.position.z - end.transform.position.z);

        // Move like a king but up to 2 squares
        return (xOffset <= 2 && zOffset <= 2);
    }

   bool MovePriest(GameObject tile)
    {
        // Check if the destination tile is adjacent to the king's current position
        if (IsAdjacent(selectedPiece, tile))
        {
            GameObject pieceAtTile = GetPieceAtTile(tile);
            GameObject pieceAtPiece = AdjacentToDeath(tile);

            if (pieceAtTile != null)
            {
                return false;
            }

            if(isWhiteTurn)
            { 
                if(pieceAtPiece == null)
                    if (pieceAtTile == null && selectedPiece.GetComponent<PriestManager>().CanRevive(selectedPiece, tile, capturedPiecesWhite))
                        selectedPiece.GetComponent<PriestManager>().RevivePiece(tile, capturedPiecesWhite);
            }
            else
            { 
                if (pieceAtPiece == null)
                    if (pieceAtTile == null && selectedPiece.GetComponent<PriestManager>().CanRevive(selectedPiece, tile, capturedPiecesBlack))
                        selectedPiece.GetComponent<PriestManager>().RevivePiece(tile, capturedPiecesBlack);
            }

            //valid move
            return true;
        }

        //Invalid move
        return false;
    }

    public void IncrementTurn()
    {
        if(selectedPiece != null)
        {
            // Deselect the piece
            selectedPiece.GetComponent<MeshRenderer>().material.color = colorPieceSelected;
            selectedPiece = null;
        }

        endTurn = true;
        StartCoroutine(EndTurn());
    }

    GameObject AdjacentToDeath(GameObject startTile)
    {
        foreach (PriestManager.CapturedPieceInfo capturedPieceInfo in capturedPiecesWhite)
        {
            // Calculate the absolute difference in x and z coordinates between the start and end tiles
            float xOffset = Mathf.Abs(capturedPieceInfo.piece.transform.position.x - startTile.transform.position.x);
            float zOffset = Mathf.Abs(capturedPieceInfo.piece.transform.position.z - startTile.transform.position.z);

            // Check if the destination tile is adjacent to the start tile (one square away)
            if (xOffset <= 1 && zOffset <= 1)
            {
                GameObject pieceAux = GetPieceAtTile(capturedPieceInfo.piece);
                return pieceAux;
            }
        }

        foreach (PriestManager.CapturedPieceInfo capturedPieceInfo in capturedPiecesBlack)
        {
            // Calculate the absolute difference in x and z coordinates between the start and end tiles
            float xOffset = Mathf.Abs(capturedPieceInfo.piece.transform.position.x - startTile.transform.position.x);
            float zOffset = Mathf.Abs(capturedPieceInfo.piece.transform.position.z - startTile.transform.position.z);

            // Check if the destination tile is adjacent to the start tile (one square away)
            if (xOffset <= 1 && zOffset <= 1)
            {
                GameObject pieceAux = GetPieceAtTile(capturedPieceInfo.piece);
                return pieceAux;
            }
        }

        return null;
    }

    bool IsChaosMoveValid(GameObject piece, GameObject targetTile)
    {
        Vector3 piecePos = piece.transform.position;
        Vector3 tilePos = targetTile.transform.position;

        // Move exactly 2 squares horizontally or vertically
        if ((Mathf.Abs(piecePos.x - tilePos.x) == 2 && piecePos.z == tilePos.z) ||
            (Mathf.Abs(piecePos.z - tilePos.z) == 2 && piecePos.x == tilePos.x))
        {
            return true;
        }

        // Move exactly 2 squares diagonally
        if (Mathf.Abs(piecePos.x - tilePos.x) == 2 && Mathf.Abs(piecePos.z - tilePos.z) == 2)
        {
            return true;
        }

        return false;
    }

    IEnumerator EndTurn()
    {
        yield return new WaitForSeconds(0.25f);

        // Switch turn to the other player
        isWhiteTurn = !isWhiteTurn;
        whiteCamera.SetActive(isWhiteTurn);
        blackCamera.SetActive(!isWhiteTurn);

        foreach (PriestManager.CapturedPieceInfo capturedPieceInfo in capturedPiecesWhite)
        {
            capturedPieceInfo.turnsSinceCapture++;
        }

        foreach (PriestManager.CapturedPieceInfo capturedPieceInfo in capturedPiecesBlack)
        {
            capturedPieceInfo.turnsSinceCapture++;
        }

        ChaosManager[] chaosPieces = FindObjectsOfType<ChaosManager>();
        foreach (ChaosManager chaos in chaosPieces)
        {
            chaos.SetInteraction(isWhiteTurn);
            chaos.IncrementRound();
        }

        FreezePiece[] freezedPieces = FindObjectsOfType<FreezePiece>();
        foreach (FreezePiece fP in freezedPieces)
        {
            fP.IncrementTurn();
        }

        endTurn = false;
    }
}
