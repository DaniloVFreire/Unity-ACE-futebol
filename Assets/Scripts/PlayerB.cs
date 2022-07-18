using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Classe responsável por gerenciar e representar um jogador do time B*/
public class PlayerB : MonoBehaviour
{
    public GameManagerScript gameManager;  //Acesso ao gamemanager e grid manager para fazer uso de métodos
    public GridManagerScript gridManager;  //E variáveis dessas classes
    private PlayerManagerScript playerManager;
    
    public bool withBallB = false;
    private bool chute = false;
    public Vector2 startPosition;
    public float velocity = 2f;
    public float kickStr = 3f;

    [SerializeField]
    private GameObject myBall;

    void Start()
    {
        // Obtendo o script desses Game Objects
        gameManager = GameObject.FindGameObjectWithTag("gamemanager").GetComponent<GameManagerScript>();
        gridManager = GameObject.FindGameObjectWithTag("gridmanager").GetComponent<GridManagerScript>();
        playerManager = GameObject.FindObjectOfType<PlayerManagerScript>();
        myBall = GameObject.FindGameObjectWithTag("ball");
    }

    void Update()
    {

    }

    //Ao clicar sobre um jogador do time B
    private void OnMouseDown()
    {
        //Se estivermos na fase de posicionamento remover o player do campo
        if (!gameManager.matchon)
        {
            DestroyPlayer();
        }
        else
        {
            gameManager.MarkingEnemy(this.transform);
        }
    }

    private void DestroyPlayer()
    {
        this.withBallB = false; // ninguém está com a bola
        this.myBall.transform.SetParent(null); // ela vira órfã

        playerManager.teamB.Remove(this);
        Destroy(gameObject);
        gridManager.GetTileAtPosition(transform.position).ToggleTrueVacantTile();
        gridManager.placeA = false;
        gridManager.placeB = true;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("ball"))
        {
            withBallB = true;
            myBall.transform.SetParent(this.transform); // a bola se torna o filho do jogador

            // player A, como ataca pra direita, a bola vai estar mais a direita do player
            myBall.transform.position = new Vector3(this.transform.position.x - 0.6f, this.transform.position.y, this.transform.position.z);
        }
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("ball"))
        {
            withBallB = false;
            myBall.transform.SetParent(null); //o pai vai comprar cigarro
        }
    }
}
