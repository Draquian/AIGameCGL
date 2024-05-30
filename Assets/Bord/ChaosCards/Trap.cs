using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    bool isPiece = false;

    private void OnTriggerEnter(Collider collision)
    {
        ChessGameManager CGM = FindObjectOfType<ChessGameManager>();

        if (collision.CompareTag("Piece") && isPiece)
        {
            GameObject pieceAtTile = CGM.GetPieceAtTile(this.gameObject);

            if (pieceAtTile.name != "King(Clone)")
            {
                // Capture the opponent's piece
                if (CGM.isWhiteTurn == false)
                    CGM.capturedPiecesWhite.Add(new PriestManager.CapturedPieceInfo(pieceAtTile, transform.position, 0));
                else
                    CGM.capturedPiecesBlack.Add(new PriestManager.CapturedPieceInfo(pieceAtTile, transform.position, 0));

                pieceAtTile.SetActive(false); //dont destroy, they can revive
                Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Piece"))
        {
            isPiece = true;
        }
    }

    public Vector3 GetPos()
    {
        return this.transform.position;
    }
}
