using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Essa classe é responsável por fazer a transição posicionamento-partida, bem como gerir a partida
 será nessa classe que chamaremos a função da IA*/
public class GameManagerScript : MonoBehaviour
{
    [SerializeField] public int maxPlayers = 4; // número max de jogadores em cada time
    [SerializeField] public Color _matchColor;  // cor transparente das tiles no decorrer da partida
    
    public bool matchon;                        // booleano que controla se a partida começou ou não
    private int turno = 0;                      // turno da partida
    private int quantFilhosJogador = 1;

    private float mDistance = 99999f; //Variavel de comparação
    private float distance = 0f; // Armazenará a distância para o player mais próximo, aprenas comparação
    private Transform mPosition = null; // guarda a posição do player mais próximo, começa null para tratar erros

    public List<GameObject> teamA = new List<GameObject>(); // Lista com os jogadores do time A
    public List<GameObject> teamB = new List<GameObject>(); // Lista com os jogadores do time B

    // Lista com ambos os jogadores, se estão ou não com a bola, sincronizado com a lista teamA
    public List<bool> imWithBall = new List<bool>();

    public SpriteRenderer _nrenderer;           // componente reder que será usado adiante pra modificar
                                                // a cor das tiles
   

    // Update is called once per frame
    void Update()
    {
        // se o time A estiver todo em campo, e o espaço houver sido apertado
        if (Input.GetKeyDown(KeyCode.Space) && teamA.Count >= maxPlayers)
        {
            matchon = true;
            Match();
            // Debug.Log("Match started"); // debug message
        }

        
    }

    //func responsável pela partida, gestão de turnos e execução da IA 
    private void Match()
    {
        // se for o turno 0 fazemos o grid ficar transparente
        if (turno == 0)
        {
            InvisibleGrid();
            for (int i = 0; i < teamA.Count; i++)
            {
                //Debug.Log(teamA[i].transform.position);

                // debug, checando se tinha mais de 1 filho
                // pois todos já tem 1, o trinagulo na frente
                /*if (teamA[i].transform.childCount > quantFilhosJogador)
                {
                    Debug.Log("com a bola");
                }*/
            }
            turno += 1;
        }

        // a partir do primeiro turno executamos a IA para fazermos a partida
        else
        {
            //fazer o algoritmo da IA rodar aqui
            turno += 1;
        }        

    }

    public bool FindBall()
    {
        //procurando se alguém tem a bola em cena
        for (int i = 0; i < imWithBall.Count; i++)
        {
            if (imWithBall[i])
            {
                return true;
            }
        } 
        return false;
    }

    //Acha a menor distancia entre o player com a bola e um jogador do time, retorna a posição dele
    public Transform MenorDistancia(Transform playerPosition)
    {
        for (int i = 0; i < teamA.Count; i++)
        {
            if (!imWithBall[i]) // não checo o player com a bola
            {
                if (teamA[i].transform.position.x >= playerPosition.position.x) // so pode passar para frente
                {
                    distance = Vector3.Distance(playerPosition.position, teamA[i].transform.position); // pega a distancia

                    if (distance < mDistance)
                    {
                        //Debug.Log(teamA[i].transform.position);
                        mDistance = distance;
                        mPosition = teamA[i].transform;
                    }
                }
            }
        }

        mDistance = 99999f; //reinicia a menor distancia

        return mPosition;
    }

    //Sincroniza as listas Imwithball e teamA sempre que a bola é passada
    //recebe do script PalyerA o jogador que colidiu com a bola, recebeu o passe
    //e o valor true para adicionar à lista
    //ou o jogador e o valor false para o jogador que tocou a bola
    public void AdicionarNaListaBool(GameObject player, bool withBall)
    {
        for (int i = 0; i < teamA.Count; i++) // procura na lista teamA para sincronizar
        {
            if (teamA[i] == player)
            {
                imWithBall[i] = withBall;
            }
        }
    }

    //essa func deixa nossas tiles do grid transparentes pra o campo ser exibido em foco principal
    //porém ainda mantendo a estrutura tabuleiro 
    private void InvisibleGrid() 
    {
        // obtendo um array com todas as tiles que compoem nosso grid
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("tile");
        // Debug.Log("Match started " + tiles.Length); //debug message

        // loop que percorre o array e modifica a cor de cada tile
        for (int i = 0; i < tiles.Length; i++)
        {
            _nrenderer = tiles[i].GetComponent<SpriteRenderer>();
            _nrenderer.color = _matchColor;
            // Debug.Log(tiles[i].transform.position); //debug message 
        }
    } 
}
