using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriestManager : MonoBehaviour
{
    public bool CanRevive(GameObject start, GameObject end, List<CapturedPieceInfo> capturedPieces)
    {
        foreach (CapturedPieceInfo capturedPieceInfo in capturedPieces)
        {
            if (capturedPieceInfo.turnsSinceCapture <= 4)
            {
                Vector3 capturedPosition = capturedPieceInfo.position;
                float xOffset = Mathf.Abs(end.transform.position.x - capturedPosition.x);
                float zOffset = Mathf.Abs(end.transform.position.z - capturedPosition.z);

                if (xOffset <= 1 && zOffset <= 1)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void RevivePiece(GameObject end, List<CapturedPieceInfo> capturedPieces)
    {
        foreach (CapturedPieceInfo capturedPieceInfo in capturedPieces)
        {
            if (capturedPieceInfo.turnsSinceCapture <= 4)
            {
                Vector3 capturedPosition = capturedPieceInfo.position;

                GameObject pieceToRevive = capturedPieceInfo.piece;
                capturedPieces.Remove(capturedPieceInfo);

                pieceToRevive.transform.position = new Vector3(capturedPosition.x, 0.5f, capturedPosition.z);
                pieceToRevive.SetActive(true);

                break;
            }
        }
    }

    public class CapturedPieceInfo
    {
        public GameObject piece;
        public Vector3 position;
        public int turnsSinceCapture;

        public CapturedPieceInfo(GameObject piece, Vector3 position, int turnsSinceCapture)
        {
            this.piece = piece;
            this.position = position;
            this.turnsSinceCapture = turnsSinceCapture;
        }
    }
}
