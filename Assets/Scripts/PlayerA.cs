using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Classe respons�vel por gerenciar e representar um jogador do time A*/
public class PlayerA : MonoBehaviour
{
    public GameManagerScript gameManager;  //Acesso ao gamemanager e grid manager para fazer uso de m�todos
    public GridManagerScript gridManager;  //E vari�veis dessas classes
    public bool withBall = false;          //Diz o player tem a bola
    
    [SerializeField]
    private GameObject myBall;

    //private float forcaChute = 1.0f;

    void Start()
    {
        // Obtendo o script desses Game Objects
        gameManager = GameObject.FindGameObjectWithTag("gamemanager").GetComponent<GameManagerScript>();
        gridManager = GameObject.FindGameObjectWithTag("gridmanager").GetComponent<GridManagerScript>();
        myBall = GameObject.FindGameObjectWithTag("ball");
    }

    void Update()
    {
        //Como os jogadores s�o separados, essa checagem era feito em todas, e como s� um possu�a a bola
        //no else, acabava sempre SetParent(null) quando tinha mais de um jogadpr em cena
        if (withBall) // Se estiver com a bola...
        {
            myBall.transform.SetParent(this.transform); // a bola se torna o filho do jogador
        }
        //Agora, antes de setar null, procura-se em toda a cena por um jogador com a bola
        //se nenhum for encontrado, a bola n�o est� na posse de ningu�m
        else if (!gameManager.FindBall()) // se a bola nao estiver com ninguem
        {
            myBall.transform.SetParent(null); //o pai vai comprar cigarro
        }
    }

    //Ao clicar sobre um jogador do time A
    private void OnMouseDown()
    {
        //Se estivermos na fase de posicionamento remover o player do campo
        if (! gameManager.matchon)
        {
            DestroyPlayer();
        }
    }

    // Funcao para destruir o game object do player e remov�-lo do campo
    private void DestroyPlayer()
    {
        gameManager.imWithBall.Remove(withBall); //parametro refere-se a true ou false, se ele esta ou n�o com a bola

        this.withBall = false; // ningu�m est� com a bola
        this.myBall.transform.SetParent(null); // ela vira �rf�

        gameManager.teamA.Remove(gameObject);
        Destroy(gameObject);
        gridManager.ToggleVacantTile(transform.position);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("ball"))
        {
            // chama a fun��o que sincroniza as listas quando o jogador pega a bola
            gameManager.AdicionarNaListaBool(gameObject, true);
            withBall = true;

            // player A, como ataca pra direita, a bola vai estar mais a direita do player
            myBall.transform.position = new Vector3(this.transform.position.x + 0.6f, this.transform.position.y, this.transform.position.z);
        }
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("ball"))
        {
            //chama a fun��o que sincroniza as listas quando a bola sai do jogador
            gameManager.AdicionarNaListaBool(gameObject, false);
            withBall = false;
        }
    }
}
