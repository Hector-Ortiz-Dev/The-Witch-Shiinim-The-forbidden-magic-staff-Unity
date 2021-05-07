using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatallaHUD : MonoBehaviour
{
    public Text txtNombre;
    public Text numVida;
    public Text numNivel;
    public Text numExp;
    public Image imgTipo;
    public List<Sprite> Tipos;
    //Nuevas
    public Image imgVidaC;
    public Image imgExpBar;
    float velocidad;

    public void configHUD(Unidad unidad)
    {
        txtNombre.text = unidad.nombre;

        numVida.text = "" + unidad.vidaActual;

        numNivel.text = "Nv." + unidad.nivel;

        switch(unidad.tipo)
        {
            case listaTipo.jugador:
                Debug.Log("Colocando sprite 0");
                imgTipo.sprite = Tipos[0];
                break;

            case listaTipo.agua:
                Debug.Log("Colocando sprite 1");
                imgTipo.sprite = Tipos[1];
                break;

            case listaTipo.fuego:
                Debug.Log("Colocando sprite 2");
                imgTipo.sprite = Tipos[2];
                break;

            case listaTipo.planta:
                Debug.Log("Colocando sprite 3");
                imgTipo.sprite = Tipos[3];
                break;

            case listaTipo.tierra:
                Debug.Log("Colocando sprite 4");
                imgTipo.sprite = Tipos[4];
                break;

            case listaTipo.oscuridad:
                Debug.Log("Colocando sprite 5");
                imgTipo.sprite = Tipos[5];
                break;

            case listaTipo.luz:
                Debug.Log("Colocando sprite 6");
                imgTipo.sprite = Tipos[6];
                break;
        }

        //Solo el jugador muestra su exp
        if(unidad.tipo == 0)
        {
            numExp.text = "Exp. " + unidad.exp + " / " + unidad.expSig;
            if (unidad.exp != 0)
            {
            float buff = (float) unidad.exp / unidad.expSig;
            imgExpBar.fillAmount = buff;
            Debug.Log(unidad.exp + " / " + unidad.expSig + " = " + buff);
            }
            else
            imgExpBar.fillAmount = 0;
        }

        //El enemigo y el jugador muestran su vida
        if(unidad.vidaActual != 0)
            {
                float buff = (float) unidad.vidaActual / unidad.vidaMax;
                buff = 0.677f * buff;
                buff += 0.216f;
                imgVidaC.fillAmount = buff;
            }
            else
            imgVidaC.fillAmount = 0.215f;
     }
     
    public void llenarExp(Unidad unidad)
    {
        velocidad = 3f * Time.deltaTime;

        if(unidad.tipo == 0)
        {
            velocidad = 3f * Time.deltaTime;
            numExp.text = "Exp. " + unidad.exp + " / " + unidad.expSig;
            if (unidad.exp != 0)
            {
            float buff = (float) unidad.exp / unidad.expSig;
            imgExpBar.fillAmount = Mathf.Lerp(imgExpBar.fillAmount, buff, velocidad);
            }
            else
            imgExpBar.fillAmount = 0;
        }
    }

    public void llenarVida(Unidad unidad)
    {
        velocidad = 3f * Time.deltaTime;
        if(unidad.vidaActual != 0)
            {
                float buff = (float) unidad.vidaActual / unidad.vidaMax;
                buff = 0.677f * buff;
                buff += 0.216f;
                imgVidaC.fillAmount = Mathf.Lerp(imgVidaC.fillAmount, buff, velocidad);
            }
            else
            imgVidaC.fillAmount = 0.215f;
    }

    public void actVida(int vidaActual, int vidaMax)
    {
        numVida.text = "" + vidaActual;
    }

    public void actExp(int exp, int expSig)
    {
        Debug.Log("Actualizando experiencia");
        numExp.text = "Exp. " + exp + " / " + expSig;
    }
}
