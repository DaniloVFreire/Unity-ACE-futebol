using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Essa classe é responsável por fazer a transição posicionamento-partida, bem como gerir a partida
 será nessa classe que chamaremos a função da IA*/
public class GameManagerScript : MonoBehaviour
{
    [SerializeField] public Color _matchColor;  // cor transparente das tiles no decorrer da partida

    public bool matchon;                        // booleano que controla se a partida começou ou não
    private int turno = 0;                      // turno da partida
    private bool nextTurn = false;
    bool shootControl = false;

    private float mDistance;
    private bool runningTurn;
    private float distance = 0f;
    private Transform mPosition;

    [SerializeField] public Transform GolA;
    [SerializeField] public Transform GolB;

    private Ball ball;
    private GridManagerScript gridmanager;
    private PlayerManagerScript playermanager;

    private bool chute = false;
    private bool checkPos = false;

    private bool rotatePlayer = false;

    //variaveis para selecionar o tile origem e o tile destino
    private PlayerA positionPlayerToMove = null;
    public Transform tileToShoot = null;
    public PlayerA playerShoot = null;

    //variaveis para saber se é um movimento ou um chute
    private bool moveBall;


    private void Start()
    {
        ball = GameObject.FindObjectOfType<Ball>();
        gridmanager = GameObject.FindObjectOfType<GridManagerScript>();
        playermanager = GameObject.FindObjectOfType<PlayerManagerScript>();
    }


    // Update is called once per frame
    void Update()
    {
        HighlightTileToShoot();
        
        // se o time A estiver todo em campo, e o espaço houver sido apertado
        if (Input.GetKeyDown(KeyCode.Space) && playermanager.CheckMaxPlayersNumberInField() && !matchon)
        {
            matchon = true;
            playermanager.DefineStartPositions();
            Match();
            // Debug.Log("Match started"); // debug message
        }

        if (moveBall && nextTurn)
        {
            if (tileToShoot.transform.position.x < playerShoot.transform.position.x && !rotatePlayer) // ta atras, rotaciona
            {
                playermanager.RotatePlayer(playerShoot);
                rotatePlayer = true;
            }
            
            MoveBallToTile();
        }

        if (Input.GetKeyDown(KeyCode.Y)) //debug
        {
            gridmanager.VisibleGrid();
        }

        if (Input.GetKeyDown(KeyCode.N) && matchon)
        {
            nextTurn = true;
            playermanager.ToggleCurrentPlayersPosition(gridmanager);
            checkPos = playermanager.CheckAndMovePlayersTillDestiny();

            // Debug.Log("Match started"); // debug message
        }

        if (nextTurn && !checkPos)
        {
            checkPos = playermanager.CheckAndMovePlayersTillDestiny();
            runningTurn = true;
        }


        if(nextTurn && checkPos)
        {
            if (!moveBall)
            {
                EndTurn();
            }

            StopPlayers();
        }

            /*
            if (Input.GetKeyDown(KeyCode.J) && !chute && matchon && !ball.gol) // ...e apertar j 
            {
                playerClosest = gameManager.MenorDistancia(jogadorComABola.transform, false, time);
                chute = true;
            }

            if (chute)
            {
                this.Chutar();
            }*/
    }

    //func responsável pela partida, gestão de turnos e execução da IA 
    private void Match()
    {
        // se for o turno 0 fazemos o grid ficar transparente
        if (turno == 0)
        {
            gridmanager.InvisibleGrid();
            /*for (int i = 0; i < teamA.Count; i++)
            {
                //Debug.Log(teamA[i].transform.position);

                // debug, checando se tinha mais de 1 filho
                // pois todos já tem 1, o trinagulo na frente
                /*if (teamA[i].transform.childCount > quantFilhosJogador)
                {
                    Debug.Log("com a bola");
                }
            }*/
            
        }

        // a partir do primeiro turno executamos a IA para fazermos a partida
        else
        {
            //fazer o algoritmo da IA rodar aqui
            
        }

    }

    private void EndTurn()
    {
        nextTurn = false;
        if (runningTurn)
        {
            gridmanager.RestartTargetTile();


            turno++;
            Debug.Log(turno);

            //playermanager.ToggleCurrentPlayersPosition(gridmanager);
            gridmanager.UpdateToggleTiles();
            gridmanager.DesactiveTiles();


            runningTurn = false;
        }
    }

    private void StopPlayers()
    {
        checkPos = false;
        foreach (PlayerA item in (List<PlayerA>)playermanager.teamA)
        {

            item.checkAlreadyPosition = false;
            item.nextPosition = null;

        }
    }

    public void InterceptedBall()
    {
        foreach (PlayerA player in (List<PlayerA>)playermanager.teamA)
        {
            if (player.nextPosition != null) // se nao chegou ao destino
            {
                // mover para a tile mais proxima
                float x = player.transform.position.x;
                float y = player.transform.position.y;

                if (x % 1 != 0) // numero quebrado, ta fora de uma tile
                {
                    x = Mathf.Round(x);
                }
                if (y != (int)y) // numero quebrado, ta fora de uma tile
                {
                    y = Mathf.Round(y);
                }

                Tile newTile = gridmanager.GetTileAtPosition(new Vector2(x, y));
                player.nextPosition = newTile.transform;
            }
        }

        //EndTurn();
        //StopPlayers();
    }

    private void HighlightTileToShoot()
    {
        if (tileToShoot != null)
        {

            tileToShoot.GetComponent<SpriteRenderer>().enabled = true;
            tileToShoot.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.1f, 0.8f);
            foreach(Tile tile in gridmanager.tilesTarget)
            {
                if (tile.transform == tileToShoot)
                {
                    tileToShoot.GetComponent<SpriteRenderer>().color = new Color(0.9f, 0.5f, 0.2f, 0.7f);
                }
            }
            
        }
        
    }

    //NOVO
    public Transform MenorDistancia(Transform playerPosition, bool tocarPraTras, char time)
    {
        mPosition = null;
        mDistance = 99999f;

        for (int i = 0; i < playermanager.maxPlayers; i++) // Ainda não tem expulsão no jogo, se tiver, mudar para team.Count
        {
            if (time == 'A')
            {
                if (!playermanager.teamA[i].withBallA)
                {
                    if (!tocarPraTras)
                    {
                        if (playermanager.teamA[i].transform.position.x >= playerPosition.position.x) // so pode passar para frente
                        {
                            distance = Vector3.Distance(playerPosition.position, playermanager.teamA[i].transform.position);

                            if (distance < mDistance)
                            {
                                //Debug.Log(teamA[i].transform.position);
                                mDistance = distance;
                                mPosition = playermanager.teamA[i].transform;
                            }
                        }
                    }
                    else
                    {
                        distance = Vector3.Distance(playerPosition.position, playermanager.teamA[i].transform.position);

                        if (distance < mDistance)
                        {
                            //Debug.Log(teamA[i].transform.position);
                            mDistance = distance;
                            mPosition = playermanager.teamA[i].transform;
                        }
                    }
                }
            }
            else if (time == 'B')
            {
                if (!playermanager.teamB[i].withBallB)
                {
                    if (!tocarPraTras)
                    {
                        if (playermanager.teamB[i].transform.position.x <= playerPosition.position.x) // so pode passar para frente
                        {
                            distance = Vector3.Distance(playerPosition.position, playermanager.teamB[i].transform.position);

                            if (distance < mDistance)
                            {
                                //Debug.Log(teamB[i].transform.position);
                                mDistance = distance;
                                mPosition = playermanager.teamB[i].transform;
                            }
                        }
                    }
                    else
                    {
                        distance = Vector3.Distance(playerPosition.position, playermanager.teamB[i].transform.position);

                        if (distance < mDistance)
                        {
                            //Debug.Log(teamB[i].transform.position);
                            mDistance = distance;
                            mPosition = playermanager.teamB[i].transform;
                        }
                    }
                }
            }
        }

        return mPosition;
    }

    //essa func deixa nossas tiles do grid transparentes pra o campo ser exibido em foco principal
    //porém ainda mantendo a estrutura tabuleiro 
    


    public void Move(Tile tile)
    {
        //gridmanager.UpdateToggleTiles();
        //se um jogador selecionou uma area de ação para se mover
        if (positionPlayerToMove != null && tile.IsAVacantTile())
        {
            //no prox update vou para o destino e desocupo a vaga

            gridmanager.GetTileAtPosition(positionPlayerToMove.transform.position).ToggleTrueVacantTile();
            positionPlayerToMove.nextPosition = tile.transform;
            gridmanager.UpdateTargetTile();
            
            /*tile.GetComponent<SpriteRenderer>().enabled = true;
            tile.GetComponent<SpriteRenderer>().color = new Color(0.9f, 0.1f, 0.1f, 0.5f);
            gridmanager.tilesActives.Add(tile);*/
            //this.movePlayer = true;

            //gridmanager.ToggleVacantTile(tile.transform.position);
        }
        if (playerShoot != null)
        {
            if (gridmanager.IsInAreaShoot(tile))
            {
                MoveBall(tile.transform); // se um jogador selecionou uma area de ação para chutar
            }
        }
    }

    public void MoveBall(Transform tile)
    {
        if (shootControl && (tile.transform.position != playerShoot.transform.position))
        {
            //no prox update chuto a bola para o destino
            if (playerShoot.nextPosition != null)
            {
                if (playerShoot.nextPosition.position != tile.position)
                {
                    this.tileToShoot = tile;
                    shootControl = false;
                    this.moveBall = true;
                }
                else
                {
                    this.tileToShoot = null;
                    shootControl = false;
                    this.moveBall = false;
                }

            }
            else
            {
                this.tileToShoot = tile;
                shootControl = false;
                this.moveBall = true;
            }
        }
    }

    

    private void MoveBallToTile()
    {
        // se chegou no tile certo ou colidiu com alguem, a bola para
        if (ball.transform.position == tileToShoot.position || (ball.transform.parent != null && ball.transform.parent.transform != playerShoot.transform))
        {
            tileToShoot.GetComponent<SpriteRenderer>().enabled = false;
            tileToShoot = null;
            playerShoot = null;
            moveBall = false;
            rotatePlayer = false;
            gridmanager.RestartTargetTile();
            gridmanager.UpdateToggleTiles();
            gridmanager.DesactiveTiles();
        }
        else
        {
            //var temp = tileToShoot.position;
            //temp.x = 0;
            // Deflate it's x and z coordinate
            //var lookRotation = Quaternion.LookRotation(temp, new Vector3(1, 0, 1));
            //playerShoot.transform.rotation = Quaternion.Slerp(playerShoot.transform.rotation, lookRotation, 2 * Time.deltaTime);

            //enquanto não chegar ao destino continuo me movendo
            ball.transform.position = Vector2.MoveTowards(ball.transform.position, tileToShoot.position, 10 * Time.deltaTime);
        }
    }

    public void ShowAreaMoviment(PlayerA player)
    {
        //tratamento de erro para não mostrar a area de chute e de movimento ao mesmo tempo
        /*if (playerShoot != null)
        {
            playerShoot = null;
        }*/

        //recebe o player que vai se mover e ativa a area de ação
        positionPlayerToMove = player;
        this.DesactiveArea();
        gridmanager.ActiveMoveNeighbors(player.transform.position, 2); // representa a velocidade do jogador
    }

    public void ShowAreaShoot(PlayerA player)
    {
        //tratamento de erro para não mostrar a area de chute e de movimento ao mesmo tempo
        if (positionPlayerToMove != null)
        {
            positionPlayerToMove = null;
        }

        //recebe o player que vai chutar e ativa a area de ação
        playerShoot = player;
        shootControl = true;
        this.DesactiveArea();
        gridmanager.ActiveShootNeighbors(player.transform.position, 3, playerShoot.transform); // representa a força do chute
    }

    public bool onAreaBallMoviment(Transform playerPosition) // tratamento de erro
    {
        if (gridmanager.IsActiveTile(playerPosition.position))
        {
            Tile tile = gridmanager.GetTileAtPosition(playerPosition.position);
            tile.Move();

            return true;
        }
        return false;
    }

    public void DesactiveArea() //desativa a area de ação
    {
        if (gridmanager.tilesActives.Count != 0)
        {
            gridmanager.DesactiveTiles();
        }
    }

    public void MarkingEnemy(Transform playerPosition)
    {
        if (gridmanager.IsActiveTile(playerPosition.position))
        {
            Tile tile = gridmanager.GetTileAtPosition(playerPosition.position);
            tile.Move();
        }
    }

    public void Mark(Transform tile)
    {
        float x = tile.position.x;
        float y = tile.position.y;

        if (positionPlayerToMove.transform.position.x < x)
        {
            x = x - 1;
        }
        else if (positionPlayerToMove.transform.position.x > x)
        {
            x = x + 1;
        }

        if (positionPlayerToMove.transform.position.y < y)
        {
            y = y - 1;
        }
        else if (positionPlayerToMove.transform.position.y > y)
        {
            y = y + 1;
        }

        Tile t = gridmanager.GetTileAtPosition(new Vector2(x, y));
        Debug.Log(t.transform.position);
    }
}
