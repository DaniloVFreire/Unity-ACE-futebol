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
    private Dictionary<Vector2, bool> _vactiles;  // Dict com mesma chave mas um bool de valor indicando
                                                  // se a tile já está ocupada

    // Variáveis de controle para saber que time posicionar em campo
    public bool placeA;   
    public bool placeB;
    public bool placeBall;


    private void Start()
    {
        GenerateGrid();
    }

    // funcao de geração da grid
    private void GenerateGrid()
    {
        _tiles = new Dictionary<Vector2, Tile>();  
        _vactiles = new Dictionary<Vector2, bool>();
      

        // ciclo de repetição para preencher o tabuleiro com altura e largura informada
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                // instanciando uma casa na coordenada atual do ciclo e definindo seu nome
                var spawnedTile = Instantiate(_tilePrefab, new Vector2(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile{x} {y}";

                // controle para definir a cor da fileira de casas
                var isOffset = (x % 2 == 0);
                spawnedTile.Init(isOffset);

                // adicionando elementos aos dicionários criados previamente
                _tiles[new Vector2(x, y)] = spawnedTile;
                _vactiles[new Vector2(x, y)] = true;
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

    // funcao para saber se uma tile está ocupada ou não
    public bool IsAVacantTile(Vector2 pos)
    {
        if (_vactiles.TryGetValue(pos, out var isvac))
        {
            return isvac;
        }
        return false;
    }

    //funcao para alterar o estado de uma tile entre ocupada e desocupada
    public void ToggleVacantTile(Vector2 pos)
    {
        if(_vactiles.TryGetValue(pos, out var isvac))
        {
            _vactiles[pos] = ! _vactiles[pos];
        }
    }
}
