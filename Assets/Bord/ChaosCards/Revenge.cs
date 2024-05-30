using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Revenge : MonoBehaviour
{
    public void TakeRevenge(GameObject otherPiece)
    {
        ChessGameManager CGM = FindObjectOfType<ChessGameManager>();

        if (CGM.isWhiteTurn)
            CGM.capturedPiecesWhite.Add(new PriestManager.CapturedPieceInfo(otherPiece, transform.position, 0));
        else
            CGM.capturedPiecesBlack.Add(new PriestManager.CapturedPieceInfo(otherPiece, transform.position, 0));

        otherPiece.SetActive(false); //dont destroy, they can revive
    }

    //maybe destroy after X turns
}
