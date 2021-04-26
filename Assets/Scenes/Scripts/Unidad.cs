using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum listaTipo
    {
        jugador,
        agua,
        fuego,
        planta,
        tierra,
        oscuridad,
        luz

    }
public class Unidad : MonoBehaviour
{
    [Header("Info")]
    //nombre de la entidad
    public string nombre;
    //Stats
    [SerializeField]
    public listaTipo tipo;
    [Header("Stats")]
    public int nivel;
    public int exp; //Experiencia actual
    public int expSig {get; private set;} //Experiencia necesaria para el siguiente nivel
    [Space(10)]
    public int vidaMax;
    public int vidaActual {get; private set;}
    public int atk;
    public int def;
    public int vel;

    //Buffs a usar en el panel de resultados
    [HideInInspector]
    public int buffVida;
    [HideInInspector]
    public int buffAtk;
    [HideInInspector]
    public int buffDef;
    [HideInInspector]
    public int buffVel;
    [Space(20)]
    //Aqui se guarda el Animator Controler
    public Animator anim;

    void Awake()
    {
        vidaActual = vidaMax;
    }
        void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void configExpSig()
    {
        float buff;
        Debug.Log("Calculando exp necesaria para el sig nivel.");
        buff = Mathf.Pow(nivel + 1, 3);
        buff *= 4;
        buff /= 5;
        Debug.Log("Buff: " + buff);
        expSig = (int) buff;
        Debug.Log("Nivel: " + nivel + "\n" + "ExpSig. " + expSig);
    }

    //Funcion que se ejecuta al momento de atacar
    public bool RecibeDanio(int dmg)
    {
        Debug.Log("Reduciendo vida de la entidad");
        vidaActual -= dmg;

        //Si esta muerto es true
        Debug.Log("Si la vida es 0 o menos se muere");
        if (vidaActual <= 0)
        {
            vidaActual = 0;
            return true;
        }
        else
            return false;
    }

   public void nuevosStats()
   {
       Debug.Log("Se ejecuta la funcion Nuevos Stats");
       Debug.Log("Guardando valores en buffs");
       buffVida = Random.Range(5,10);
       buffAtk = Random.Range(5,10);
       buffDef = Random.Range(5,10);
       buffVel = Random.Range(5,10);
       Debug.Log("Buff vida = " + buffVida);
       Debug.Log("Buff akt = " + buffAtk);
       Debug.Log("Buff def = " + buffDef);
       Debug.Log("Buff vel = " + buffVel);
   }

    public bool subirNv() //Regresa false si ya no puede subir de nivel y true si puede seguir subiendo
    {
        Debug.Log("Entrando a la funcion subir de nivel");
        nivel ++;
        exp -= expSig;
        configExpSig(); //Calcula la exp necesaria para el siguiente nivel
        
        //Aumenta de manera aleatoria cada stat   
        Debug.Log("Sumando buffs a los stats base");
        vidaMax += buffVida;
        atk += buffAtk;
        def += buffDef;
        vel += buffVel;

        vidaActual = vidaMax; //Si sube de nivel su salud se recupera por completo

        if(exp >= expSig)
            return true; //Puede subir otro nivel
        else
            return false; //Ya no puede subir otro nivel
    }
}
