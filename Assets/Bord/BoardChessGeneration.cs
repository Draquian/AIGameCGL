using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardChessGeneration : MonoBehaviour
{
    //"None White" is 0.
    //"None Black" is 1.
    //"Teleportation Gate" is 2.
    //"Charged Tile" is 3.
    //"Time Dilation Tile" is 4.
    /*{ 2, 1, 0, 3, 0, 1, 0, 2, 0, 1 },
      { 1, 0, 1, 0, 3, 0, 1, 0, 1, 0 },
      { 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
      { 1, 0, 2, 0, 1, 0, 1, 0, 1, 0 },
      { 0, 1, 0, 3, 0, 3, 0, 1, 0, 1 },
      { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0 },
      { 0, 1, 0, 3, 0, 3, 0, 1, 0, 1 },
      { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0 },
      { 0, 3, 0, 1, 0, 1, 0, 3, 0, 1 },
      { 2, 3, 1, 0, 1, 0, 3, 2, 1, 0 }*/

    // Prefabs for different tile types
    public GameObject[] tilePrefabs;

    // Prefabs for different chess pieces
    public GameObject[] piecePrefabs;

    // Size of the chessboard
    public int rows = 8;
    public int cols = 8;

    // Offset to position the chessboard
    public Vector3 offset = new Vector3(0, 0, 0);

    // Size of each tile
    public float tileSize = 1.0f;

    GameObject board;
    GameObject pieces;
    GameObject whitePieces;
    GameObject blackPieces;

    void Start()
    {
        GenerateChessboard();
    }

    public void ResetGame()
    {
        Destroy(board);
        Destroy(pieces);
        GenerateChessboard();
    }

    void GenerateChessboard()
    {
        board = new GameObject("Board");
        pieces = new GameObject("Pieces");
        whitePieces = new GameObject("White");
        blackPieces = new GameObject("Black");

        board.transform.parent = transform;
        pieces.transform.parent = transform;
        whitePieces.transform.parent = pieces.transform;
        blackPieces.transform.parent = pieces.transform;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                // Generate the tiles for he board
                GenerateTiles(row, col, board);

                // Generate chess pieces if the tile is on the starting position of the pieces
                if (row == 1 || row == 8)
                {
                    GeneratePawn(col, row, row == 1, whitePieces, blackPieces); // Generate pawns for both black and white
                }
                else if (row == 0 || row == 9)
                {
                    GeneratePieces(col, row, whitePieces, blackPieces); // Generate other pieces for both black and white
                }
            }
        }
    }

    void GenerateTiles(int row, int col, GameObject father)
    {
        // Determine the type of tile based on the array values
        int tileType = GetTileType(row, col);

        // Instantiate the corresponding tile prefab
        GameObject tilePrefab = tilePrefabs[tileType];
        GameObject tile = Instantiate(tilePrefab, transform);

        // Calculate the position of the tile
        Vector3 tilePosition = new Vector3(col * tileSize, 0, row * tileSize) + offset;
        tile.transform.position = tilePosition;

        //Set as child from a empty Object, just order
        tile.transform.parent = father.transform;
    }

    void GeneratePawn(int col, int row, bool isWhite, GameObject white, GameObject black)
    {
        // Pawns types for the 1 and 8 rows
        int[] pawnTypes = { 1, 0, 0, 2, 0, 0, 2, 0, 0, 1 }; // Assassin, Pawn * 2, SoulEater, Pawn * 2, SoulEater, Pawn * 2, Assassin

        // Determine the piece type based on the column
        int pawnType = pawnTypes[col];

        // Instantiate pawn prefab
        GameObject pawnPrefab = piecePrefabs[pawnType]; // Pawn is at index 0 in piecePrefabs array
        GameObject pawn = Instantiate(pawnPrefab, transform);

        // Calculate position for pawn
        Vector3 pawnPosition = new Vector3(col * tileSize, 0.5f, row * tileSize) + offset;
        pawn.transform.position = pawnPosition;

        // Set color of pawn
        MeshRenderer renderer = pawn.GetComponent<MeshRenderer>();
        renderer.material.color = isWhite ? Color.white : Color.black;

        if (row == 8) pawn.transform.parent = black.transform;
        else if (row == 1) pawn.transform.parent = white.transform;
    }

    void GeneratePieces(int col, int row, GameObject white, GameObject black)
    {
        // Piece types for the first and last rows
        int[] pieceTypes = { 3, 4, 5, 8, 7, 6, 9, 5, 4, 3 }; // Rook, Knight, Bishop, Chaos, King, Queen, Sacerdote, Bishop, Knight, Rook

        // Determine the piece type based on the column
        int pieceType = pieceTypes[col];

        // Adjust piece type if it's on the back row (row 9)
        if (row == 9)
        {
            // Invert the order of piece types for the black back row
            pieceType = pieceTypes[9 - col];
        }

        // Instantiate the corresponding piece prefab
        GameObject piecePrefab = piecePrefabs[pieceType];
        GameObject piece = Instantiate(piecePrefab, transform);

        // Calculate the position of the piece
        Vector3 piecePosition = new Vector3(col * tileSize, 0.5f, row * tileSize) + offset;
        piece.transform.position = piecePosition;

        // Set color of the piece
        MeshRenderer renderer = piece.GetComponent<MeshRenderer>();
        renderer.material.color = row == 0 ? Color.white : Color.black;

        if (row == 9)   piece.transform.parent = black.transform;
        else if (row == 0)  piece.transform.parent = white.transform;
    }

    int GetTileType(int row, int col)
    {
        // Example array representing the chessboard with terrain effects
        int[,] chessboard = new int[10, 10]
        {
            { 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0 },
            { 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0 },
            { 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0 },
            { 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0 },
            { 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0 }
        };

        // Return the value from the chessboard array for the corresponding row and column
        return chessboard[row, col];
    }
}
