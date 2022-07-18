using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] public Color _baseColor, _offsetColor;  // Cores dos Tiles
    [SerializeField] private SpriteRenderer _renderer;       // Componente Renderer da Tile
    [SerializeField] private GameObject _highlight;          // Highlight Tile
    [SerializeField] private GameObject _redhighlight;          // Highlight Tile
    [SerializeField] private GameObject _playerA;            // Team A Player prefab para spawnar na tile
    [SerializeField] private GameObject _playerB;            // Team B Player prefab para spawnar na tile
    public bool isVacated = true;

    public GameManagerScript gameManager;  //Acesso ao gamemanager e grid manager para fazer uso de m�todos
    public GridManagerScript gridManager;  //E vari�veis dessas classes
    public PlayerManagerScript playerManager;  //E vari�veis dessas classes



    private void Start()
    {
        // Obtendo o script desses Game Objects
        gridManager = GameObject.FindObjectOfType<GridManagerScript>();
        gameManager = GameObject.FindObjectOfType<GameManagerScript>();
        playerManager = GameObject.FindObjectOfType<PlayerManagerScript>();
    }
    public void Init(bool isOffset)
    {
        _renderer.color = isOffset ? _offsetColor : _baseColor; // Definindo a cor da tile
    }

    // Mouse sobre a tile
    private void OnMouseEnter()
    {
        _highlight.SetActive(true);  // Display o highlight na casa 
    }

    // Mouse deixa a tile
    private void OnMouseExit()
    {
        _highlight.SetActive(false);  // Sai o highlight na casa
    }


    // Ao clicar na casa
    private void OnMouseDown()
    {
        if (!gameManager.matchon)
        {
            InstatiatePlayer();     // Instanciar um jogador nela
        }
        else
        {
            //jogador selecionou o tile destino
            Debug.Log("tile");
            PlayerA playerAatTile = playerManager.GetPlayerAatPosition(this.transform);
            if (playerAatTile != null) // eu cliquei num player
            {
                // se o jogador selecionado n�o estiver na area de a��o do toque de bola
                if (!gameManager.onAreaBallMoviment(this.transform))
                {
                    //significa que mudei de opini�o e vou mover outro jogador
                    gameManager.ShowAreaMoviment(playerAatTile);
                }
                else
                {
                    gameManager.Move(gridManager.GetTileAtPosition(this.transform.position));
                }
            }
            else // cliquei num tile
            {
                this.Move();
            }

        }
    }

    public void Move()
    {
        // se a tile estiver na area permitida de movimento, posso selecion�-la
        if (gridManager.IsActiveTile(transform.position))
        {
            //se a tile estiver vaga, posso chutar ou ir para l�
            if (this.IsAVacantTile())
            {
                
                gameManager.Move(this);
                

            }
            else
            {
                // posso chutar ou marcar o jogador
                if (playerManager.GetPlayerAatPosition(this.transform) != null)
                {
                    gameManager.MoveBall(this.transform);
                }
                else if (playerManager.GetPlayerBatPosition(this.transform) != null)
                {
                    gameManager.Mark(this.transform);
                }
            }
        }
        //desativo a area permitida depois que seleciono uma tile
        //ou trocando de op��o
        gameManager.DesactiveArea();
    }

    public void ToggleTrueVacantTile()
    {
        isVacated = true;
    }

    public void ToggleFalseVacantTile()
    {
        isVacated = false;
    }

    public bool IsAVacantTile()
    {
        return isVacated;
    }

    // Fun��o que gera um jogador na tile selecionada
    public void InstatiatePlayer()
    {
        // Se tiver clicado no banco do time A e a partida n�o tiver come�ado
        if (gridManager.placeA && !(gameManager.matchon))
        {
            // Se a tile est� vaga e n�o estiver todos os jogadores do time A dispostos em campo
            if (this.IsAVacantTile() && !playerManager.CheckMaxPlayersAInField())
            {
                GameObject newPlayerA;
                // Instanciando o jogador novo na posi��o da tile escolhida
                newPlayerA = Instantiate(_playerA, transform.position, _playerA.transform.rotation);
                
                this.ToggleFalseVacantTile(); // Mudando a tile para ocupada
                //playerManager.AddNewPlayerInTeamA(newPlayerA);
                playerManager.teamA.Add(newPlayerA.GetComponent<PlayerA>()); // Adicionando o novo player na lista
                
            }
        }

        // Se tiver clicado no banco do time B e a partida n�o tiver come�ado
        else if (gridManager.placeB && !(gameManager.matchon))
        {
            // Se a tile est� vaga e n�o estiver todos os jogadores do time B dispostos em campo
            if (this.IsAVacantTile() && !playerManager.CheckMaxPlayersBInField())
            {
                GameObject newPlayerB;
                // Instanciando o jogador novo na posi��o da tile escolhida
                newPlayerB = Instantiate(_playerB, transform.position, _playerB.transform.rotation);
                this.ToggleFalseVacantTile(); // Mudando a tile para ocupada
                playerManager.AddNewPlayerInTeamB(newPlayerB);
                //teamB.Add(newPlayerB.GetComponent<PlayerB>()); // Adicionando o novo player na lista
                
            }
        }

    }

}
