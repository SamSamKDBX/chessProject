using UnityEngine;

[ExecuteInEditMode] // Permet l'exécution du script dans l'éditeur
public class ChessBoardInitializer : MonoBehaviour
{
    // Pièces blanches
    public GameObject blackSquare;
    public GameObject whiteSquare;

    [ContextMenu("réinitialiser les cases du plateau")] // Ajoute une option contextuelle
    public void PlacePieces()
    {
        // Nettoyer les anciens objets pour éviter des duplications
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }

        Vector2 startingPosition = new Vector2(0, 0); // Position de la case en bas à droite

        // Placer les pions (blancs et noirs)
        char lettre = 'a';
        for (int x = 0; x < 8; x++)
        {
            lettre = 'a';
            for (int y = 0; y < 8; y++) {
                if (x % 2 == 0 && y % 2 == 0 || x % 2 != 0 && y % 2 != 0) {
                    GameObject square = Instantiate(whiteSquare, new Vector3(startingPosition.x + y, startingPosition.y - x, 0), Quaternion.identity, transform);
                    square.name = lettre + $"{8 - x}";
                }
                else {
                    GameObject square = Instantiate(blackSquare, new Vector3(startingPosition.x + y, startingPosition.y - x, 0), Quaternion.identity, transform);
                    square.name = lettre + $"{8 - x}";
                }
                lettre++;
            }     
        }

        foreach (Transform child in transform) {
            child.gameObject.tag = "Square";
        }
    }
}
