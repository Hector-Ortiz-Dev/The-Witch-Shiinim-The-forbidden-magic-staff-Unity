using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particulas : MonoBehaviour
{
    //Enemigo
    //Planta
    //Fuego
    //Agua
    //Luz
    public List<ParticleSystem> hechizos;

    public void ataqueEnemigo()
    {
        hechizos[0].Play();
    }

    public void hechizoPlanta()
    {
        hechizos[1].Play();
    }
    public void hechizoFuego()
    {
        hechizos[2].Play();
    }
    public void hechizoAgua()
    {
        hechizos[3].Play();
    }
    public void hechizoLuz()
    {
        hechizos[4].Play();
    }
}