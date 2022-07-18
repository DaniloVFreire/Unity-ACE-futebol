using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Essa classe, simbolicamente representa o banco de reserva dos jogadores, mas na prática funciona
 como um botão que após clicado permite que o usuário selecione uma tile em campo para posicionar
um jogador correspondente ao banco clicado*/
public class TeamABench : MonoBehaviour
{
    public bool isClicked = false;        // booleano de controle
    public GridManagerScript gridManager; // referencia ao gridmanager para utilizar métodos dessa classe
    
    void Start()
    {
        // obtendo o componente script do gridmanager para utilizar alguns métodos e var contidos nele
        gridManager = GameObject.FindGameObjectWithTag("gridmanager").GetComponent<GridManagerScript>();
    }
    
    private void OnMouseDown()
    {
        isClicked = !(isClicked);  // ativando/desativando o booleano de controle
        //Debug.Log(isClicked); // debug message
        if (isClicked)
        {
            // desativando a permissão para posicionar um jogador do time B
            gridManager.placeB = false;
            //ativando/desativando a permissão para posicionar um jogador do time A
            gridManager.placeA = !(gridManager.placeA); 
            
            // Debug.Log(gridManager.placeA); // debug message

            gridManager.placeBall = false;
        }
    }
}
