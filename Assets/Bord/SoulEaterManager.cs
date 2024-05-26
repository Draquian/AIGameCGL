using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulEaterManager : MonoBehaviour
{
    string lastSoul = "None";

    public void CopyPieceAbilities(GameObject capturedPiece)
    {
        string pieceName = capturedPiece.name;

        switch (pieceName)
        {
            case "Pawn(Clone)":
                // Add pawn abilities
                lastSoul = "Pawn";
                break;
            case "Tower(Clone)":
                // Add rook abilities
                lastSoul = "Rook";
                break;
            case "Bishop(Clone)":
                // Add bishop abilities
                lastSoul = "Bishop";
                break;
            case "Queen(Clone)":
                // Add queen abilities
                lastSoul = "Queen";
                break;
            case "King(Clone)":
                // Add king abilities
                lastSoul = "King";
                break;
            case "Knight(Clone)":
                // Add knight abilities
                lastSoul = "Knight";
                break;
            case "Assassin(Clone)":
                // Add assassin abilities
                lastSoul = "Assassin";
                break;
            case "Priest(Clone)":
                // Add priest abilities
                lastSoul = "Priest";
                break;
            case "Chaos(Clone)":
                // Add chaos abilities
                lastSoul = "Chaos";
                break;
                // Add other pieces if necessary
        }

        // Additional abilities
        //GainSoulShield();
    }

    public bool ValidPieceAbilities()
    {
        switch (lastSoul)
        {
            case "Pawn":
                // Add pawn abilities
                lastSoul = "None";
                return true;
            case "Rook":
                // Add rook abilities
                lastSoul = "None";
                return true;
            case "Bishop":
                // Add bishop abilities
                lastSoul = "None";
                return true;
            case "Queen":
                // Add queen abilities
                lastSoul = "None";
                return true;
            case "King":
                // Add king abilities
                lastSoul = "None";
                return true;
            case "Knight":
                // Add knight abilities
                lastSoul = "None";
                return true;
            case "Assassin":
                // Add assassin abilities
                lastSoul = "None";
                return true;
            case "Priest":
                // Add priest abilities
                lastSoul = "None";
                return true;
            case "Chaos":
                // Add chaos abilities
                lastSoul = "None";
                return true;
            // Add other pieces if necessary
            case "None":
                return false;
        }
        return false;
    }
}
