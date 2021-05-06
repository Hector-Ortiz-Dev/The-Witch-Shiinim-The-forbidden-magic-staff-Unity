using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public Animator fade;
    public void Jugar()
    {
        SceneManager.LoadScene("Batalla");
    }

    public void Salir()
    {
        Application.Quit();
    }
}