using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagerScript : MonoBehaviour
{
    [SerializeField] public int maxPlayers = 4; // número max de jogadores em cada time
    public List<PlayerA> teamA = new List<PlayerA>(); // Lista com os jogadores do time A
    public List<PlayerB> teamB = new List<PlayerB>(); // Lista com os jogadores do time B
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public PlayerA GetPlayerAatPosition(Transform reqposition)
    {
        foreach (PlayerA item in (List<PlayerA>)teamA)
        {
            if (item.transform.position.x == reqposition.position.x)
            {
                if (item.transform.position.y == reqposition.position.y)
                {
                    return item;
                }
            }

        }

        return null;

    }

    public PlayerB GetPlayerBatPosition(Transform reqposition)
    {
        foreach (PlayerB item in (List<PlayerB>)teamB)
        {
            if (item.transform.position.x == reqposition.position.x)
            {
                if (item.transform.position.y == reqposition.position.y)
                {
                    return item;
                }
            }

        }

        return null;

    }

    public void ReturnEveryoneToStartPosition()
    {
        foreach (PlayerA player in teamA)
        {
            player.transform.position = player.startPosition;
        }
    }

    public bool CheckMaxPlayersNumberInField()
    {
        if(teamA.Count == maxPlayers && teamB.Count == maxPlayers)
        {
            return true;
        }
        return false;
    }

    public void ToggleCurrentPlayersPosition(GridManagerScript gridmanager)
    {
        foreach (PlayerA player in (List<PlayerA>)teamA)
        {
            if (player.nextPosition != null)
            {
                gridmanager.GetTileAtPosition(player.transform.position).ToggleFalseVacantTile();
            }

        }
    }

    public bool CheckAndMovePlayersTillDestiny()
    {
        bool checkPos = true;
        foreach (PlayerA player in (List<PlayerA>)teamA)
        {
            if (player.nextPosition != null)
            {
                checkPos = checkPos && player.checkAlreadyPosition;
                player.MovePlayerToTile();
                //runningTurn = true;
            }


        }
        return checkPos;
    }

    public bool CheckMaxPlayersAInField()
    {
        if (teamA.Count == maxPlayers)
        {
            return true;
        }
        return false;
    }

    public void DefineStartPositions()
    {
        foreach(PlayerA playerA in teamA)
        {
            playerA.startPosition = playerA.transform.position;
        }
        foreach (PlayerA playerB in teamA)
        {
            playerB.startPosition = playerB.transform.position;
        }
    }

    public bool CheckMaxPlayersBInField()
    {
        if (teamB.Count == maxPlayers)
        {
            return true;
        }
        return false;
    }

    public void AddNewPlayerInTeamA(GameObject newPlayerA)
    {
        teamA.Add(newPlayerA.GetComponent<PlayerA>()); // Adicionando o novo player na lista
    }

    public void AddNewPlayerInTeamB(GameObject newPlayerB)
    {
        teamB.Add(newPlayerB.GetComponent<PlayerB>()); // Adicionando o novo player na lista
    }

    public void RotatePlayer(PlayerA player)
    {
        player.transform.Rotate(new Vector3(0, 0, 180));
        StartCoroutine(WaitRotatePlayer(player));
    }

    private IEnumerator WaitRotatePlayer(PlayerA player)
    {
        yield return new WaitForSeconds(0.1f);
        player.transform.Rotate(new Vector3(0, 0, 180));
    }
}
