using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class chess : MonoBehaviour
{
    // création d'une matrice de structures "plateau" de 8x8 cases
    public static structure.s_square[,] board = new structure.s_square[8, 8];

    // création d'un mouvement "mouvement précédent", actuellement vide
    public static structure.s_move lastMove = new structure.s_move(new structure.s_piece("Empty", "Empty", new Vector3(0, 0)), new structure.s_position(0, 0), new structure.s_position(0, 0));

    public static List<structure.s_piece> piecesList = new List<structure.s_piece>();

    public static List<GameObject> piecesListObjects = new List<GameObject>();

    public static List<(structure.s_piece, GameObject)> pairedList = new List<(structure.s_piece, GameObject)>();
    void Start()
    {
        // Remplissage du plateau
        for (int line = 0; line < 8; line++)
        {
            for (int column = 0; column < 8; column++)
            {
                // on remplit en fonction des lignes et colonnes du plateau
                if (line == 1)
                {
                    //                   |  une nouvelle case   | remplie |  par un nouveau  | pion |  noir
                    board[line, column] = new structure.s_square(false, new structure.s_piece("Pawn", "Black", new Vector3(column, -line, -1)));
                }
                else if (line == 6)
                {
                    board[line, column] = new structure.s_square(false, new structure.s_piece("Pawn", "White", new Vector3(column, -line, -1)));
                }
                else if (line == 0)
                {
                    board[line, column] = new structure.s_square(false, new structure.s_piece(GetPieceName(column), "Black", new Vector3(column, -line, -1)));
                }
                else if (line == 7)
                {
                    board[line, column] = new structure.s_square(false, new structure.s_piece(GetPieceName(column), "White", new Vector3(column, -line, -1)));
                }
                else
                {
                    board[line, column] = new structure.s_square(true, new structure.s_piece("Empty", "Empty", new Vector3(column, -line, -1))); // Cases vides
                }
            }
        }

        // on créé une liste de chaque piece en tant qu objet
        foreach (Transform child in transform)
        {
            piecesListObjects.Add(child.gameObject);
        }
        print(piecesListObjects.Count);

        // on créé une liste avec toutes les pieces en tant que classe
        foreach (structure.s_square square in board)
        {
            if (square.piece.name != "Empty")
            {
                piecesList.Add(square.piece);
            }
        }
        print(piecesList.Count);

        // Affichage pour vérifier le contenu du plateau
        printChessBoard(board);
        createPairedList();
        printPairedList();
    }

    public void createPairedList()
    {
        pairedList.Clear(); // Nettoyer la liste des anciens éléments

        // Séparer les pièces et objets par couleur
        var whitePieces = piecesList.Where(piece => piece.color == "White").OrderBy(piece => piece.name).ToList();
        var blackPieces = piecesList.Where(piece => piece.color == "Black").OrderBy(piece => piece.name).ToList();

        var whiteObjects = piecesListObjects.Where(go => go.name.Contains("White")).OrderBy(go => go.name).ToList();
        var blackObjects = piecesListObjects.Where(go => go.name.Contains("Black")).OrderBy(go => go.name).ToList();

        // Vérification des tailles des listes
        if (whitePieces.Count != whiteObjects.Count || blackPieces.Count != blackObjects.Count)
        {
            Debug.LogWarning("Le nombre de pièces et d'objets ne correspond pas pour une couleur !");
            return; // Si les tailles sont différentes, arrêter l'ajout
        }

        // Ajouter les pièces blanches avec les objets blancs
        for (int i = 0; i < whitePieces.Count; i++)
        {
            pairedList.Add((whitePieces[i], whiteObjects[i]));
        }

        // Ajouter les pièces noires avec les objets noirs
        for (int i = 0; i < blackPieces.Count; i++)
        {
            pairedList.Add((blackPieces[i], blackObjects[i]));
        }

        // Affichage du nombre d'éléments ajoutés
        Debug.Log($"Paired list created and sorted with {pairedList.Count} items.");
    }


    public void printPairedList()
    {
        foreach (var pair in pairedList)
        {
            structure.s_piece piece = pair.Item1; // Premier élément : s_piece
            GameObject gameObject = pair.Item2;  // Deuxième élément : GameObject

            // Affiche les informations dans la console
            Debug.Log($"Piece Name: {piece.name}, Color: {piece.color}, GameObject Name: {gameObject.name}");
        }
    }

    private string GetPieceName(int columnIndex)
    {
        switch (columnIndex)
        {
            case 0: return "Rook";
            case 1: return "Knight";
            case 2: return "Bishop";
            case 3: return "Queen";
            case 4: return "King";
            case 5: return "Bishop";
            case 6: return "Knight";
            case 7: return "Rook";
            default: return "Empty";
        }
    }

    public static void printChessBoard(structure.s_square[,] board)
    {
        // Affichage du plateau de jeu dans la console
        String board_string = "\n";
        for (int line = 0; line < 8; line++)
        {
            for (int column = 0; column < 8; column++)
            {
                board_string += "|";
                if (board[line, column].piece.name == "Queen")
                {
                    board_string += "Q";
                }
                else if (board[line, column].piece.name == "King")
                {
                    board_string += "K";
                }
                else if (board[line, column].piece.name == "Bishop")
                {
                    board_string += "B";
                }
                else if (board[line, column].piece.name == "Pawn")
                {
                    board_string += "P";
                }
                else if (board[line, column].piece.name == "Knight")
                {
                    board_string += "k";
                }
                else if (board[line, column].piece.name == "Rook")
                {
                    board_string += "R";
                }
                else if (board[line, column].piece.name == "Empty")
                {
                    board_string += "  ";
                }
            }
            board_string += "|\n-----------------\n";
        }
        print(board_string);
    }

    void Update()
    {
        foreach (var pair in pairedList)
        {
            structure.s_piece piece = pair.Item1; // Classe s_piece
            GameObject gameObject = pair.Item2;  // GameObject associé

            // Comparer les positions
            if (piece.position != gameObject.transform.position)
            {
                //Debug.Log($"Mismatch: Piece '{piece.name}' is not at the correct position.");

                // Mettre à jour la position du GameObject pour correspondre à celle de la pièce
                gameObject.transform.position = piece.position;

                //Debug.Log($"Updated position of GameObject '{gameObject.name}' to match '{piece.name}'.");
            }
        }
    }
}
