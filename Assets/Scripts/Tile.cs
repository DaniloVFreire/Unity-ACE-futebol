using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] public Color _baseColor, _offsetColor;  // Cores dos Tiles
    [SerializeField] private SpriteRenderer _renderer;       // Componente Renderer da Tile
    [SerializeField] private GameObject _highlight;          // Highlight Tile
    [SerializeField] private GameObject _playerA;            // Team A Player prefab para spawnar na tile
    [SerializeField] private GameObject _playerB;            // Team B Player prefab para spawnar na tile

    public GameManagerScript gameManager;  //Acesso ao gamemanager e grid manager para fazer uso de métodos
    public GridManagerScript gridManager;  //E variáveis dessas classes



    private void Start()
    {
        // Obtendo o script desses Game Objects
        gridManager = GameObject.FindGameObjectWithTag("gridmanager").GetComponent<GridManagerScript>();
        gameManager = GameObject.FindGameObjectWithTag("gamemanager").GetComponent<GameManagerScript>();
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
        InstatiatePlayer();     // Instanciar um jogador nela
    }

    // Função que gera um jogador na tile selecionada
    public void InstatiatePlayer()
    {
        // Se tiver clicado no banco do time A e a partida não tiver começado
        if (gridManager.placeA && !(gameManager.matchon))
        {
            // Se a tile está vaga e não estiver todos os jogadores do time A dispostos em campo
            if (gridManager.IsAVacantTile(transform.position) && gameManager.teamA.Count < gameManager.maxPlayers)
            {
                GameObject newPlayerA;
                // Instanciando o jogador novo na posição da tile escolhida
                newPlayerA = Instantiate(_playerA, transform.position, _playerA.transform.rotation);
                gridManager.ToggleVacantTile(transform.position); // Mudando a tile para ocupada
                gameManager.teamA.Add(newPlayerA); // Adicionando o novo player na lista

                if (newPlayerA.transform.childCount > 1)
                {
                    gameManager.imWithBall.Add(true);
                }
                else
                {
                    gameManager.imWithBall.Add(false);
                }
            }
        }

        // Se tiver clicado no banco do time B e a partida não tiver começado
        else if (gridManager.placeB && !(gameManager.matchon))
        {
            // Se a tile está vaga e não estiver todos os jogadores do time B dispostos em campo
            if (gridManager.IsAVacantTile(transform.position) && gameManager.teamB.Count < gameManager.maxPlayers)
            {
                GameObject newPlayerB;
                // Instanciando o jogador novo na posição da tile escolhida
                newPlayerB = Instantiate(_playerB, transform.position, _playerB.transform.rotation);
                gridManager.ToggleVacantTile(transform.position); // Mudando a tile para ocupada
                gameManager.teamB.Add(newPlayerB); // Adicionando o novo player na lista
                
            }
        }

    }

}
