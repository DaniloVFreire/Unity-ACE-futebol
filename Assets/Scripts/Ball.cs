using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ball : MonoBehaviour
{
    private bool chute = false;
    private bool gol = false;
    private bool rotacionou = false;
    private float distance;

    private float forceKick = 6.0f;

    public GameManagerScript gameManager;
    public GridManagerScript gridmanager;

    private Transform playerClosest;
    private GameObject jogadorComABola;

    private char time = 'N';

    public Text ScoreA;
    private int ScoreNA;
    public Text ScoreB;
    private int ScoreNB;

    // Start is called before the first frame update
    void Start()
    {
        gridmanager = GameObject.FindObjectOfType<GridManagerScript>();
        gameManager = GameObject.FindGameObjectWithTag("gamemanager").GetComponent<GameManagerScript>();

        ScoreNA = 0;
        ScoreA.text = ScoreNA.ToString();
        ScoreNB = 0;
        ScoreB.text = ScoreNB.ToString();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J) && !chute && gameManager.matchon && !gol) // ...e apertar j 
        {
            playerClosest = gameManager.MenorDistancia(jogadorComABola.transform, false, time);
            chute = true;
        }

        if (chute)
        {
            this.Chutar();
        }
    }

    private void Chutar()
    {
        if (playerClosest != null) // existe o player
        {
            // 1 que e uma margem para pegar o player do lado
            float distanciaProMaisProximo = Vector3.Distance(jogadorComABola.transform.position, playerClosest.position);

            if (distanciaProMaisProximo <= forceKick)
            {
                this.transform.position = Vector2.MoveTowards(this.transform.position, playerClosest.position, 10 * Time.deltaTime);
            }
        }
        else // nao tem "opcoes"
        {
            // tocar pro gol
            if (jogadorComABola.GetComponent<PlayerA>() != null) // teamA
            {
                distance = Vector3.Distance(this.transform.position, gameManager.GolB.position);

                if (distance > forceKick)
                {
                    playerClosest = gameManager.MenorDistancia(jogadorComABola.transform, true, time);

                    float distanciaProMaisProximo = Vector3.Distance(jogadorComABola.transform.position, playerClosest.position);

                    if (distanciaProMaisProximo <= forceKick)
                    {
                        // ROTACAO
                        // pode fazer uma animacao rotacionando um pouco na update
                        jogadorComABola.transform.Rotate(new Vector3(0, 0, 180));
                        this.rotacionou = true;
                        this.transform.position = Vector2.MoveTowards(this.transform.position, playerClosest.position, 10 * Time.deltaTime);
                    }
                    else
                    {
                        // NAO CHUTA
                    }
                }
                else // chuta pro gol
                {
                    this.transform.position = Vector2.MoveTowards(this.transform.position, gameManager.GolB.position, 10 * Time.deltaTime);
                }

            }
            else if (jogadorComABola.GetComponent<PlayerB>() != null) // TeamB
            {
                distance = Vector3.Distance(this.transform.position, gameManager.GolA.position);

                if (distance > forceKick)
                {
                    playerClosest = gameManager.MenorDistancia(jogadorComABola.transform, true, time);

                    float distanciaProMaisProximo = Vector3.Distance(jogadorComABola.transform.position, playerClosest.position);

                    if (distanciaProMaisProximo <= forceKick)
                    {
                        // ROTACAO
                        // pode fazer uma animacao rotacionando um pouco na update
                        jogadorComABola.transform.Rotate(new Vector3(0, 0, 180));
                        this.rotacionou = true;
                        this.transform.position = Vector2.MoveTowards(this.transform.position, playerClosest.position, 10 * Time.deltaTime);
                    }
                    else
                    {
                        // NAO CHUTA
                    }
                }
                else // chuta pro gol
                {
                    this.transform.position = Vector2.MoveTowards(this.transform.position, gameManager.GolA.position, 10 * Time.deltaTime);
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // mudar para adicionar para colidir com player (seria pegar o prefab do player e adicionar tag nele)
        if (other.collider.CompareTag("PlayerA") || other.collider.CompareTag("PlayerB"))
        {
            if (other.collider.CompareTag("PlayerA"))
            {
                jogadorComABola = other.gameObject;
                time = 'A';
                chute = false;
                gridmanager.RestartTargetTile();
                gridmanager.UpdateToggleTiles();
                gridmanager.DesactiveTiles();
            }
            else if (other.collider.CompareTag("PlayerB"))
            {
                jogadorComABola = other.gameObject;
                time = 'B';
                chute = false;
                gridmanager.RestartTargetTile();
                gridmanager.UpdateToggleTiles();
                gridmanager.DesactiveTiles();
            }

            gameManager.InterceptedBall();
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (this.rotacionou)
        {
            if (other.collider.CompareTag("PlayerA")) // se era o player e o player tinha rotacionado
            {
                // voltar a rotacao
                StartCoroutine(EsperarSairDaCaixaColisao(other.transform));
            }
            else if (other.collider.CompareTag("PlayerB"))
            {
                StartCoroutine(EsperarSairDaCaixaColisao(other.transform));
            }
            this.rotacionou = false;
        }
    }

    private IEnumerator EsperarSairDaCaixaColisao(Transform colidiu)
    {
        yield return new WaitForSeconds(0.2f);
        colidiu.Rotate(new Vector3(0, 0, -180));
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("golA") || other.CompareTag("golB"))
        {
            chute = false;
            gol = true;

            if (other.CompareTag("golA"))
            {
                ScoreNB++;
                ScoreB.text = ScoreNB.ToString();
            }
            else if (other.CompareTag("golB"))
            {
                ScoreNA++;
                ScoreA.text = ScoreNA.ToString();
            }
        }
    }

    /*private void FixedUpdate()
    {
        // ir diminuindo quando bater em parede?
        //this.fisica.velocity = Vector2.zero;
        if (this.chute)
        {
            //this.fisica.velocity = this.vetorMovimento * this.forcaChute;
            this.fisica.AddForce(this.vetorMovimento * this.forcaChute, ForceMode2D.Force);
            this.chute = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D colisao)
    {
        // verificar se foi o jogador (pra nao colidir com os tiles)
        Vector2 diferencaX, diferencaY;
        diferencaX = new Vector2(this.transform.position.x - colisao.transform.position.x, 0);
        diferencaY = new Vector2(0, this.transform.position.y - colisao.transform.position.y);
        Vector2 vetorMovimentar = diferencaX + diferencaY;

        this.vetorMovimento = vetorMovimentar.normalized;
        this.chute = true;
    }*/
}
