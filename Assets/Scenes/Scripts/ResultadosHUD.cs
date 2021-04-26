using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultadosHUD : MonoBehaviour
{
  public GameObject panel;
  public Text txtResultado;
  Vector3 iniPos;

  void start()
  {
      iniPos = panel.transform.position;
  }
  public void configResultado(string nombre, int nivel, int vida, int atk, int def, int vel) //Se ponen los valores iniciales de Shiinim al comenzar la pelea
  {
      txtResultado.text = nombre + "  Nv." + nivel + "\n \n" 
      + "Vida: " + vida + "\n" 
      + "Ataque: " + atk + "\n" 
      + "Defensa: " + def + "\n" 
      + "Velocidad: " + vel;
  }

  public void mostrar(bool visible) //El panel de resultados se hace visible
  {
    panel.SetActive(visible);
  }

  public void actualizar(string nombre, int nivel, int vida, int atk, int def, int vel, int buffVida, int buffAtk, int buffDef, int buffVel) //Se muestra la info actual y lo que se le va a sumar
  {
    txtResultado.text = nombre + "  Nv." + nivel + "\t<color=#f3d352>+ 1</color>" + "\n \n" 
      + "Vida: " + vida + "\t<color=#f3d352>+ " + buffVida + "</color>\n" 
      + "Ataque: " + atk + "\t<color=#f3d352>+ " + buffAtk + "</color>\n" 
      + "Defensa: " + def + "\t<color=#f3d352>+ " + buffDef + "</color>\n" 
      + "Velocidad: " + vel + "\t<color=#f3d352>+ " + buffVel + "</color>";
  }
}