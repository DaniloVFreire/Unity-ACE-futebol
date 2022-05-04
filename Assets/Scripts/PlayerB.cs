using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Classe responsável por gerenciar e representar um jogador do time B*/
public class PlayerB : MonoBehaviour
{
    public GameManagerScript gameManager;  //Acesso ao gamemanager e grid manager para fazer uso de métodos
    public GridManagerScript gridManager;  //E variáveis dessas classes
    
    void Start()
    {
        // Obtendo o script desses Game Objects
        gameManager = GameObject.FindGameObjectWithTag("gamemanager").GetComponent<GameManagerScript>();
        gridManager = GameObject.FindGameObjectWithTag("gridmanager").GetComponent<GridManagerScript>();
    }

    //Ao clicar sobre um jogador do time B
    private void OnMouseDown()
    {
        //Se estivermos na fase de posicionamento remover o player do campo
        if (!gameManager.matchon)
        {
            DestroyPlayer();
        }
    }

    private void DestroyPlayer()
    {
        gameManager.teamB.Remove(gameObject);
        Destroy(gameObject);
        gridManager.ToggleVacantTile(transform.position);
    }
}
