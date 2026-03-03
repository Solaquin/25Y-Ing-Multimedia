using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ProfesorIA : MonoBehaviour
{
    //Referencia al jugador para calcular distancia, Distancia en la que el profesor detecta al jugador 
    public Transform jugador;
    public float distanciaDeteccion = 15f;

    //puntos que recorre mientras patrulla 
    public Transform[] puntosPatrulla;
    private int indiceActual;

    //Agente que permite moverse sobre el NavMesh
    private NavMeshAgent agente;

    public enum Estado
    {
        Patrullando,
        Persiguiendo
    }

    public Estado estadoActual;

    public enum TipoProfesor
    {
        SI,
        PM,
        PAIM,
        TD
    }

    public TipoProfesor tipo;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        estadoActual = Estado.Patrullando;
        IrSiguientePunto();

        Renderer rend = GetComponent<Renderer>();


        //Configuracion de velocidad y color segun el tipo de profesor 
        switch (tipo)
        {
            case TipoProfesor.SI:
                agente.speed = 2f;
                rend.material.color = Color.blue;
                break;

            case TipoProfesor.PM:
                agente.speed = 3f;
                rend.material.color = Color.red;
                break;

            case TipoProfesor.PAIM:
                agente.speed = 1.5f;
                rend.material.color = Color.green;
                break;

            case TipoProfesor.TD:
                agente.speed = 2.5f;
                rend.material.color = Color.yellow;
                break;
        }
    }
    void Update()
    {
        //Calcula la distancia al jugador
        float distancia = Vector3.Distance(transform.position, jugador.position);
        
        //Cambia a modo persecucion si el jugador esta cerca 
        if(distancia < distanciaDeteccion)
        {
            estadoActual = Estado.Persiguiendo;
        }
        //Si esta persiguiendo, se dirige al jugador
        if(estadoActual== Estado.Persiguiendo)
        {
            agente.destination = jugador.position;
            return;
        }
        //Si llega a un punto de patrulla, va al siguiente 
        if(!agente.pathPending && agente.remainingDistance < 0.5f)
        {
            IrSiguientePunto();
        }
        NavMeshHit hit;
        if (NavMesh.SamplePosition(jugador.position, out hit, 1.0f, NavMesh.AllAreas))
        {
            Debug.Log("Jugador está en NavMesh");
        }
        else
        {
            Debug.Log("Jugador NO está en NavMesh");
        }
        Debug.Log("Distancia al jugador: " + distancia);
    }

    //Envia al profesor al siguiente punto de patrulla 
    void IrSiguientePunto()
    {
        if (puntosPatrulla.Length == 0) return; 
        {
            agente.destination = puntosPatrulla[indiceActual].position;
            indiceActual = (indiceActual + 1) % puntosPatrulla.Length;
        }
    }
}
