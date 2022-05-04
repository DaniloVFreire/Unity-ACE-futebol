using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movimentar : MonoBehaviour
{
    private Transform positionBall;
    private int rodar;

    public bool spot = false; //booleana para saber se o jogador esta dentro do campo de visão
    public Transform target; //alvo que o inimigo vai perseguir, nesse caso o jogador
    public Transform inicioCP; //inicio do campo de visão 
    public Transform fimCP; //final do campo de visão

    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float steerSpeed = 100f;
    [SerializeField] float stamina = 1.0f;

    private void Start()
    {
        this.positionBall = GameObject.FindObjectOfType<Ball>().transform;
    }

    private void Update()
    {
        Raycasting();
        if (spot)
        {
            float moveAmount = moveSpeed * Time.deltaTime * stamina;
            this.transform.Translate(moveAmount, 0, 0);
        }
        else
        {
            float steerAmount = steerSpeed * Time.deltaTime;
            if (this.transform.position.y > this.positionBall.position.y)
            {
                steerAmount = steerAmount * -1;
            }
            this.transform.Rotate(0, 0, steerAmount);
        }
    }

    public void Raycasting()
    {
        Debug.DrawLine(inicioCP.position, fimCP.position, Color.green);
        this.spot = Physics2D.Linecast(inicioCP.position, fimCP.position, 1 << LayerMask.NameToLayer("Player"));
    }
}
