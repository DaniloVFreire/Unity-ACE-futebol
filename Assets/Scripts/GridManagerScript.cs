using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Essa classe concerne a disposição do tabuleiro utilizado tanto no posicionamento quanto na partida em si*/
public class GridManagerScript : MonoBehaviour
{
    [SerializeField] private int _width, _height; // altura e largura do tabuleiro, atualmente 16 x 9
    [SerializeField] private Tile _tilePrefab;    // prefab de uma casa do tabuleiro
    [SerializeField] private Transform _cam;      // camera principal para posicioná-la corretamente
    private Dictionary<Vector2, Tile> _tiles;     // Dict com chave posição da tile e valor a tile em si
    // private Dictionary<Vector2, bool> _vactiles;  // Dict com mesma chave mas um bool de valor indicando
                                                  // se a tile já está ocupada

    // Variáveis de controle para saber que time posicionar em campo
    public bool placeA;   
    public bool placeB;
    public bool placeBall;


    public List<Tile> tilesActives = new List<Tile>();
    public List<Tile> tilesTarget = new List<Tile>();

    public SpriteRenderer _nrenderer;

    private PlayerManagerScript playermanager;



    private void Start()
    {
        
        playermanager = GameObject.FindObjectOfType<PlayerManagerScript>();

        GenerateGrid();
    }

    // funcao de geração da grid
    private void GenerateGrid()
    {
        _tiles = new Dictionary<Vector2, Tile>();  
        
      

        // ciclo de repetição para preencher o tabuleiro com altura e largura informada
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                // instanciando uma casa na coordenada atual do ciclo e definindo seu nome
                var spawnedTile = Instantiate(_tilePrefab, new Vector2(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile{x} {y}";
                spawnedTile.isVacated = true;
                // controle para definir a cor da fileira de casas
                var isOffset = (x % 2 == 0);
                spawnedTile.Init(isOffset);

                // adicionando elementos aos dicionários criados previamente
                _tiles[new Vector2(x, y)] = spawnedTile;

            }
        }
        // posicionando a camera
        _cam.transform.position = new Vector3((float)_width / 2 - 0.5f, (float)_height / 2 - 0.5f, -1);
    }

    //funcao para obter uma tile através de sua posição
    public Tile GetTileAtPosition(Vector2 pos)
    {
        if(_tiles.TryGetValue(pos, out var tile))
        {
            return tile;
        }

        return null;

    }



    public void UpdateToggleTiles()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                Tile currentTile = _tiles[new Vector2(i, j)];
                if (playermanager.GetPlayerAatPosition(currentTile.transform) != null || playermanager.GetPlayerBatPosition(currentTile.transform) != null)
                {
                    currentTile.ToggleFalseVacantTile();
                }
              
                else
                {
                    currentTile.ToggleTrueVacantTile();
                }
                
            }
            //_nrenderer.color = _matchColor;

            // Debug.Log(tiles[i].transform.position); //debug message 
        }

        foreach (Tile tile in tilesTarget)
        {
            tile.ToggleFalseVacantTile();
        }

    }

    //ativa a area de ação
    public void ActiveMoveNeighbors(Vector2 pos, float velocidade)
    {
        float qnt = Mathf.Round(velocidade);
        
        for (int x = (int)(pos.x - qnt); x <= pos.x + qnt; x++)
        {
            for (int y = (int)(pos.y + qnt); y >= pos.y - qnt; y--)
            {
                Vector2 tile = new Vector2(x, y);
                if (0 <= tile.x && tile.x < _width && tile.y >=0 &&tile.y < _height)
                {
                    Tile t = GetTileAtPosition(tile);
                    if (t.IsAVacantTile() || playermanager.GetPlayerBatPosition(t.transform) != null) // é vago ou é um b
                    {
                        if (!tilesTarget.Contains(_tiles[tile]))
                        {
                            _tiles[tile].GetComponent<SpriteRenderer>().enabled = true;

                            _tiles[tile].GetComponent<SpriteRenderer>().color = new Color(0.9f, 0.1f, 0.1f, 0.5f);
                        }
                        else
                        {
                            _tiles[tile].GetComponent<SpriteRenderer>().enabled = false;
                        }


                        tilesActives.Add(_tiles[tile]);
                    }
                }
            }
        }
    }


    public void ActiveShootNeighbors(Vector2 pos, float velocidade, Transform playerShoot)
    {
        float qnt = Mathf.Round(velocidade);

        for (int x = (int)(pos.x - qnt); x <= pos.x + qnt; x++)
        {
            for (int y = (int)(pos.y + qnt); y >= pos.y - qnt; y--)
            {
                Vector2 tile = new Vector2(x, y);
                if (0 <= tile.x && tile.x < _width && tile.y >= 0 && tile.y < _height)
                {
       
                    if (tile != (Vector2) playerShoot.transform.position)
                    {
                        _tiles[tile].GetComponent<SpriteRenderer>().enabled = true;

                        _tiles[tile].GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.1f, 0.8f);

                        tilesActives.Add(_tiles[tile]);
                    }
                }
                    
            }
        }
    }

    /*
    public bool IsInAreaShoot(Vector2 pos, float velocidade, Vector2 tileDestiny)
    {
        float qnt = Mathf.Round(velocidade);

        for (int x = (int)(pos.x - qnt); x <= pos.x + qnt; x++)
        {
            for (int y = (int)(pos.y + qnt); y >= pos.y - qnt; y--)
            {
                Vector2 tile = new Vector2(x, y);
                if (0 <= tile.x && tile.x < _width && tile.y >= 0 && tile.y < _height)
                {
                    if (tile == tileDestiny)
                    {
                        return true;
                    }
                }

            }
        }
        return false;
    }*/

    public bool IsInAreaShoot(Tile tile)
    {
        return tilesActives.Contains(tile);
    }

    //desativa os tiles da area de ação
    public void DesactiveTiles()
    {
        foreach (Tile tile in tilesActives)
        {
            tile.GetComponent<SpriteRenderer>().enabled = false;
        }
        
        tilesActives.Clear();
        UpdateTargetTile();
    }

    //verifica se o tile selecionado esta ativo
    public bool IsActiveTile(Vector2 pos)
    {
        if (_tiles.TryGetValue(pos, out var isvac))
        {
            //Debug.Log(_tiles[pos].GetComponent<SpriteRenderer>().enabled);
           
            return _tiles[pos].GetComponent<SpriteRenderer>().enabled;

        }
        return false;
    }

    public void UpdateTargetTile()
    {
        RestartTargetTile();
        foreach (PlayerA item in playermanager.teamA)
        {
            if (item.nextPosition != null)
            {
                tilesTarget.Add(GetTileAtPosition(item.nextPosition.position));
            }
        }

        if (tilesTarget.Count > 0)
        {
            foreach (Tile item in tilesTarget)
            {
                item.GetComponent<SpriteRenderer>().enabled = true;
                item.GetComponent<SpriteRenderer>().color = new Color(0.9f, 0.2f, 0.2f, 0.7f);
                
                
            }
        }
    }
    public void RestartTargetTile()
    {
        foreach (Tile item in tilesTarget)
        {
            item.GetComponent<SpriteRenderer>().enabled = false;
        }
        tilesTarget.Clear();
    }

    public void InvisibleGrid()
    {

        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {

                _nrenderer = _tiles[new Vector2 (i, j)].GetComponent<SpriteRenderer>();
                _nrenderer.enabled = false;
            }
            //_nrenderer.color = _matchColor;

            // Debug.Log(tiles[i].transform.position); //debug message 
        }
    }

    // se clicar y, mostra a grid do jogo
    public void VisibleGrid()
    {

        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {

                _nrenderer = _tiles[new Vector2(i, j)].GetComponent<SpriteRenderer>();
                _nrenderer.enabled = !_nrenderer.enabled; ;
            }
            //_nrenderer.color = _matchColor;

            // Debug.Log(tiles[i].transform.position); //debug message 
        }
        
    }

}
