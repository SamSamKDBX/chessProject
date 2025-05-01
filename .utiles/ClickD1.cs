using UnityEngine;

// script pour faire bouger une pièce en meme temps que la souris
public class ClickD1 : MonoBehaviour
{
    private GameObject selectedPiece = null; // Stocke la première pièce sélectionnée
    public LayerMask pieceLayer; // Assigner un LayerMask pour filtrer les clics

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Clic gauche de la souris
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, pieceLayer))
            {
                GameObject clickedPiece = hit.collider.gameObject;

                if (selectedPiece == null)
                {
                    // Sélectionner la première pièce
                    selectedPiece = clickedPiece;
                    Debug.Log($"Première pièce sélectionnée : {selectedPiece.name}");
                }
                else
                {
                    // Tenter de déplacer la pièce vers la nouvelle position
                    TryMovePiece(selectedPiece, clickedPiece.transform.position);
                    selectedPiece = null; // Réinitialiser la sélection
                }
            }
        }
    }

    void TryMovePiece(GameObject piece, Vector3 newPosition)
    {
        // on affiche le plateau de jeu
        chess.printChessBoard(chess.board);
        
        // Ici, tu peux ajouter des vérifications pour savoir si le mouvement est valide
        Debug.Log($"{piece.name} tente de se déplacer vers {newPosition}");

        structure.s_position pos = new structure.s_position((int)piece.transform.position.x, (int)piece.transform.position.z);
        // position cible test
        structure.s_position target = new structure.s_position((int)newPosition.x, (int)newPosition.z);
        // mouvement de test
        structure.s_move move = new structure.s_move(chess.board[pos.line, pos.column].piece, pos, target);
        legalMove.movePiece(move, chess.board);
        chess.printChessBoard(chess.board);
    }
}

