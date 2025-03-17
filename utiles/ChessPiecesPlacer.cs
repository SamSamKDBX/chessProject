using UnityEngine;

[ExecuteInEditMode] // Permet l'exécution du script dans l'éditeur
public class ChessPiecePlacer : MonoBehaviour
{
    // Pièces blanches
    public GameObject whitePawnPrefab;
    public GameObject whiteRookPrefab;
    public GameObject whiteKnightPrefab;
    public GameObject whiteBishopPrefab;
    public GameObject whiteQueenPrefab;
    public GameObject whiteKingPrefab;

    // Pièces noires
    public GameObject blackPawnPrefab;
    public GameObject blackRookPrefab;
    public GameObject blackKnightPrefab;
    public GameObject blackBishopPrefab;
    public GameObject blackQueenPrefab;
    public GameObject blackKingPrefab;

    [ContextMenu("Place Pieces on Chessboard")] // Ajoute une option contextuelle
    public void PlacePieces()
    {
        // Nettoyer les anciens objets pour éviter des duplications
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }

        float squareSize = 1f; // Taille d'une case
        Vector2 startingPosition = new Vector2(7, -7); // Position de la case en bas à droite

        // Placer les pions (blancs et noirs)
        for (int x = 0; x < 8; x++)
        {
            // Pions blancs sur la ligne 1 (y = -6)
            GameObject whitePawn = Instantiate(whitePawnPrefab, new Vector3(startingPosition.x - x * squareSize, startingPosition.y + 1 * squareSize, 0), Quaternion.identity, transform);
            whitePawn.name = $"WhitePawn_{x + 1}";

            // Pions noirs sur la ligne 6 (y = 0)
            GameObject blackPawn = Instantiate(blackPawnPrefab, new Vector3(startingPosition.x - x * squareSize, startingPosition.y + 6 * squareSize, 0), Quaternion.identity, transform);
            blackPawn.name = $"BlackPawn_{x + 1}";
        }

        // Placer les pièces principales (tour, cavalier, fou, reine, roi)
        // Blancs (ligne 0)
        GameObject whiteRook1 = Instantiate(whiteRookPrefab, new Vector3(startingPosition.x - 0 * squareSize, startingPosition.y + 0 * squareSize, 0), Quaternion.identity, transform);
        whiteRook1.name = "WhiteRook_1";
        
        GameObject whiteKnight1 = Instantiate(whiteKnightPrefab, new Vector3(startingPosition.x - 1 * squareSize, startingPosition.y + 0 * squareSize, 0), Quaternion.identity, transform);
        whiteKnight1.name = "WhiteKnight_1";
        
        GameObject whiteBishop1 = Instantiate(whiteBishopPrefab, new Vector3(startingPosition.x - 2 * squareSize, startingPosition.y + 0 * squareSize, 0), Quaternion.identity, transform);
        whiteBishop1.name = "WhiteBishop_1";
        
        GameObject whiteKing = Instantiate(whiteKingPrefab, new Vector3(startingPosition.x - 3 * squareSize, startingPosition.y + 0 * squareSize, 0), Quaternion.identity, transform);
        whiteKing.name = "WhiteKing";

        GameObject whiteQueen = Instantiate(whiteQueenPrefab, new Vector3(startingPosition.x - 4 * squareSize, startingPosition.y + 0 * squareSize, 0), Quaternion.identity, transform);
        whiteQueen.name = "WhiteQueen";
        
        GameObject whiteBishop2 = Instantiate(whiteBishopPrefab, new Vector3(startingPosition.x - 5 * squareSize, startingPosition.y + 0 * squareSize, 0), Quaternion.identity, transform);
        whiteBishop2.name = "WhiteBishop_2";
        
        GameObject whiteKnight2 = Instantiate(whiteKnightPrefab, new Vector3(startingPosition.x - 6 * squareSize, startingPosition.y + 0 * squareSize, 0), Quaternion.identity, transform);
        whiteKnight2.name = "WhiteKnight_2";
        
        GameObject whiteRook2 = Instantiate(whiteRookPrefab, new Vector3(startingPosition.x - 7 * squareSize, startingPosition.y + 0 * squareSize, 0), Quaternion.identity, transform);
        whiteRook2.name = "WhiteRook_2";

        // Noirs (ligne 7)
        GameObject blackRook1 = Instantiate(blackRookPrefab, new Vector3(startingPosition.x - 0 * squareSize, startingPosition.y + 7 * squareSize, 0), Quaternion.identity, transform);
        blackRook1.name = "BlackRook_1";
        
        GameObject blackKnight1 = Instantiate(blackKnightPrefab, new Vector3(startingPosition.x - 1 * squareSize, startingPosition.y + 7 * squareSize, 0), Quaternion.identity, transform);
        blackKnight1.name = "BlackKnight_1";
        
        GameObject blackBishop1 = Instantiate(blackBishopPrefab, new Vector3(startingPosition.x - 2 * squareSize, startingPosition.y + 7 * squareSize, 0), Quaternion.identity, transform);
        blackBishop1.name = "BlackBishop_1";
        
        GameObject blackKing = Instantiate(blackKingPrefab, new Vector3(startingPosition.x - 3 * squareSize, startingPosition.y + 7 * squareSize, 0), Quaternion.identity, transform);
        blackKing.name = "BlackKing";
        
        GameObject blackQueen = Instantiate(blackQueenPrefab, new Vector3(startingPosition.x - 4 * squareSize, startingPosition.y + 7 * squareSize, 0), Quaternion.identity, transform);
        blackQueen.name = "BlackQueen";
        
        GameObject blackBishop2 = Instantiate(blackBishopPrefab, new Vector3(startingPosition.x - 5 * squareSize, startingPosition.y + 7 * squareSize, 0), Quaternion.identity, transform);
        blackBishop2.name = "BlackBishop_2";
        
        GameObject blackKnight2 = Instantiate(blackKnightPrefab, new Vector3(startingPosition.x - 6 * squareSize, startingPosition.y + 7 * squareSize, 0), Quaternion.identity, transform);
        blackKnight2.name = "BlackKnight_2";
        
        GameObject blackRook2 = Instantiate(blackRookPrefab, new Vector3(startingPosition.x - 7 * squareSize, startingPosition.y + 7 * squareSize, 0), Quaternion.identity, transform);
        blackRook2.name = "BlackRook_2";
    }
}
