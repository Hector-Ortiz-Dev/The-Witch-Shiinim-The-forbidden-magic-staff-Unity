using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Victoria : MonoBehaviour
{
    public void Continuar()
    {
        SceneManager.LoadScene("Menu Principal");
    }
}