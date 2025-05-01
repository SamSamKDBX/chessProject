using UnityEngine;

// script pour bouger un pion en avant en touchant n'importe quelle piece
public class ClickDetection2D : MonoBehaviour
{
    void Update()
    {
        // Vérifie si le bouton gauche de la souris est enfoncé
        if (Input.GetMouseButtonDown(0))
        {
            // Crée un rayon depuis la position de la souris
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            // Vérifie si un objet a été touché par le rayon
            if (hit.collider != null)
            {
                // Si l'objet cliqué est celui-ci
                if (hit.collider.gameObject == gameObject)
                {
                    // Code à exécuter lorsqu'on clique sur l'objet
                    // attendre que le joueur clique sur une autre case

                    Debug.Log("L'objet " + this.name + "a été cliqué");
 
                    /* // position du pion test
                    structure.s_position pos = new structure.s_position(6, 4);
                    // position cible test
                    structure.s_position target = new structure.s_position(5, 4);
                    // mouvement de test
                    structure.s_move move = new structure.s_move(chess.board[pos.line, pos.column].piece, pos, target);
                    legalMove.movePiece(move, chess.board);
                    chess.printChessBoard(chess.board); */
                }
            }
        }
    }
}
