using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Classe responsável por gerenciar e representar um jogador do time A*/
public class PlayerA : MonoBehaviour
{
    public GameManagerScript gameManager;  //Acesso ao gamemanager e grid manager para fazer uso de métodos
    public GridManagerScript gridManager;  //E variáveis dessas classes
    private PlayerManagerScript playerManager; 
    public bool withBallA = false;
    public bool chute = false;
    public Transform nextPosition = null;
    public Vector2 startPosition;
    public float velocity = 2f;
    public float kickStr = 3f;

    

    [SerializeField]
    private GameObject myBall;
    private Rigidbody2D ballPhysic;
    public bool checkAlreadyPosition = false;

    //private float forcaChute = 1.0f;

    void Start()
    {
        // Obtendo o script desses Game Objects
        gridManager = GameObject.FindObjectOfType<GridManagerScript>();
        gameManager = GameObject.FindObjectOfType<GameManagerScript>();
        playerManager = GameObject.FindObjectOfType<PlayerManagerScript>();
        myBall = GameObject.FindGameObjectWithTag("ball");
        ballPhysic = myBall.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //Como os jogadores são separados, essa checagem era feito em todas, e como só um possuía a bola
        //no else, acabava sempre SetParent(null) quando tinha mais de um jogadpr em cena
        if (withBallA) // Se estiver com a bola...
        {
            if (Input.GetKeyDown(KeyCode.L)) //pressionado L
            {
                //mostro a area de chute em vez da area de movimento
                gameManager.ShowAreaShoot(this);
            }
        }
    }

    //Ao clicar sobre um jogador do time A
    private void OnMouseDown()
    {
        Debug.Log("player");
        //Se estivermos na fase de posicionamento remover o player do campo
        if (!gameManager.matchon)
        {
            DestroyPlayer();
        }
        else
        {
            // se o jogador selecionado não estiver na area de ação do toque de bola
            if (!gameManager.onAreaBallMoviment(this.transform))
            {
                //significa que mudei de opinião e vou mover outro jogador
                gameManager.ShowAreaMoviment(this);
            }
            else
            {
                gameManager.Move(gridManager.GetTileAtPosition(this.transform.position));
            }
        }
    }

    public void MovePlayerToTile()
    {
        Transform playerpos = this.transform;
        if (this.transform.position == nextPosition.position) //se cheguei ao meu destino, paro
        {
            gridManager.GetTileAtPosition(nextPosition.position).ToggleFalseVacantTile(); //ocupo a vaga

            nextPosition = null;
            checkAlreadyPosition = true;
            //positionPlayerToMove = null;
            //movePlayer = false;
        }
        else
        {
            checkAlreadyPosition = false;
            //enquanto não chegar ao destino continuo me movendo
            playerpos.position = Vector2.MoveTowards(playerpos.position, nextPosition.position, 10 * Time.deltaTime);
            
        }
    }

    // Funcao para destruir o game object do player e removê-lo do campo
    private void DestroyPlayer()
    {
        this.withBallA = false; // ninguém está com a bola
        this.myBall.transform.SetParent(null); // ela vira órfã

        playerManager.teamA.Remove(this);
        Destroy(gameObject);
        gridManager.GetTileAtPosition(transform.position).ToggleTrueVacantTile();
        gridManager.placeA = true;
        gridManager.placeB = false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("ball"))
        {
            withBallA = true;
            myBall.transform.SetParent(this.transform); // a bola se torna o filho do jogador

            // player A, como ataca pra direita, a bola vai estar mais a direita do player
            myBall.transform.position = new Vector3(this.transform.position.x + 0.6f, this.transform.position.y, this.transform.position.z);
        }
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("ball"))
        {
            withBallA = false;
            myBall.transform.SetParent(null); //o pai vai comprar cigarro
        }
    }
}
