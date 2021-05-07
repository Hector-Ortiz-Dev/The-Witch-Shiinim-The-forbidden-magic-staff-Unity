using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum BattleState
{
    INICIO,
    ELEGIRHECHIZO,
    TURNOJUGADOR,
    TURNOENEMIGO,
    GANA,
    PIERDE,
    RESULTADOS,
    SIGNIVEL,
    FINAL
}

public class SistemaBatalla : MonoBehaviour
{
    public GameObject jugadorPrefab;
    public List<GameObject> enemigoPrefab;

    public Transform spawnJugador;
    public Transform spawnEnemigo;

    Unidad entiWitch;
    Unidad enemigoUnidad;

    public Text mensajeTexto;

    public BatallaHUD jugadorHUD;
    public BatallaHUD enemigoHUD;
    public ResultadosHUD resultadoHUD;

    public Arduino arduino;
    float angleX;
    float angleY;

    public BattleState estado;
    Quaternion rotInicial;

    int fase;
    string tipoAtk;

    void Start()
    {
        estado = BattleState.INICIO;
        StartCoroutine(ConfigBatalla());
        rotInicial = entiWitch.transform.rotation;
        fase = 0;
    }

    void Update()
    {
        jugadorHUD.llenarExp(entiWitch);
        jugadorHUD.llenarVida(entiWitch);
        enemigoHUD.llenarVida(enemigoUnidad);

        angleX = arduino.leerX();
        angleY = arduino.leerY();
        Debug.Log("Angulo X: " + angleX);
        Debug.Log("Angulo Y: " + angleY);
        if (estado == BattleState.ELEGIRHECHIZO)
        {
            if (angleX <= 70 && angleX >= 45)
            {
                BotonAtkArriba();
            }
            else if (angleX <= -45 && angleX >= -70)
            {
                BotonAtkAbajo();
            }
            else if (angleY <= 70 && angleY >= 45)
            {
                BotonAtkDer();
            }
            else if (angleY <= -45 && angleY >= -70)
            {
                BotonAtkIzq();
            }
        }
    }

    IEnumerator ConfigBatalla()
    {
        Debug.Log("Configurando Batalla");
        Debug.Log("Fase: " + fase);
        if (fase == 9)
            SceneManager.LoadScene("Victoria");

        if (estado == BattleState.INICIO)
        {
            GameObject jugadorGo = Instantiate(jugadorPrefab, spawnJugador); //Instanciar un personaje del jugador al spawn
            entiWitch = jugadorGo.GetComponent<Unidad>();   //Obtener el componente Unidad del jugador
        }

        GameObject enemigoGo = Instantiate(enemigoPrefab[fase], spawnEnemigo);    //Instanciar un enemigo al spawn
        enemigoUnidad = enemigoGo.GetComponent<Unidad>(); //Obtener el componente Unidad

        mensajeTexto.text = "Ha aparecido " + "<b><color=#b40404>" + enemigoUnidad.nombre + "</color></b>" + ".";

        entiWitch.configExpSig();

        jugadorHUD.configHUD(entiWitch);
        Debug.Log("Configurando HUD de jugador");

        enemigoHUD.configHUD(enemigoUnidad);
        Debug.Log("Configurando HUD de enemigo");

        resultadoHUD.configResultado(entiWitch.nombre, entiWitch.nivel, entiWitch.vidaMax, entiWitch.atk, entiWitch.def, entiWitch.vel);

        yield return new WaitForSeconds(2f);
        Debug.Log("Esperar 2 segundos");

        Debug.Log("Escondiendo panel de resultaos");
        resultadoHUD.mostrar(false);

        estado = BattleState.ELEGIRHECHIZO;
        Debug.Log("Estado cambiado a Elegir Hechizo");
        ElegirHechizo();
    }

    IEnumerator AtaqueJugador()
    {
        Debug.Log("Esperando 4 segundos reales");
        yield return new WaitForSecondsRealtime(4);

        //Recibe danio y verifica si esta muerto
        Debug.Log("Verificando si el enemigo esta muerto.");
        bool estaMuerto = enemigoUnidad.RecibeDanioJ(entiWitch.atk, tipoAtk, enemigoUnidad.tipo.ToString());

        Debug.Log("Actualizando HUD del enemigo");
        enemigoHUD.actVida(enemigoUnidad.vidaActual, enemigoUnidad.vidaMax);
        mensajeTexto.text = "Haz atacado a " + "<b><color=#b40404>" + enemigoUnidad.nombre + "</color></b>";

        Debug.Log("Esperando 2 seg");
        yield return new WaitForSeconds(2f);

        if (estaMuerto)
        {
            Debug.Log("EL enemigo murio");
            //Fin de la batalla
            StartCoroutine(MuereEnemigo());
            estado = BattleState.RESULTADOS;
            Debug.Log("Se ha actualizado el estado a Resultados");
            FinBatalla();
        }
        else
        {
            estado = BattleState.TURNOENEMIGO;
            Debug.Log("Se ha actualizado el estado a Turno del enemigo");
            StartCoroutine(TurnoEnemigo());
        }   //Turno del enemigo
    }

    IEnumerator MuereEnemigo()
    {
        Debug.Log("Ejecutando funcion Muere Enemigo");
        Destroy(enemigoUnidad.gameObject);
        yield return new WaitForSeconds(2f);
    }

    public void FinBatalla()
    {
        Debug.Log("Ejecutando la funcion Fin de la batalla");
        if (estado == BattleState.RESULTADOS)
        {
            mensajeTexto.text = "<b>Haz ganado la batalla!</b>";
            StartCoroutine(Resultados());
        }
        else if (estado == BattleState.PIERDE)
        {
            entiWitch.anim.SetTrigger(Animator.StringToHash("Muerte"));
            mensajeTexto.text = "<b>Haz sido derrotado.</b>";
            StartCoroutine(GameOver());
        }
    }

    public void Gana()
    {
        Debug.Log("Ejecutando funcion de Gana y aumentando variable fase.");
        fase ++;
        StartCoroutine(ConfigBatalla());
    }

    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("Game Over");
    }

    IEnumerator TurnoEnemigo()
    {
        Debug.Log("Se ha ejecutado la funcion Turno del Enemigo");
        entiWitch.transform.rotation = rotInicial;
        mensajeTexto.text = "<b><color=#b40404>" + enemigoUnidad.nombre + "</color></b> ataca!";

        Debug.Log("Esperando 1 segundo");
        yield return new WaitForSeconds(1f);

        Debug.Log("Reduciendo vida del jugador");
        bool estaMuerto = entiWitch.RecibeDanioE(enemigoUnidad.atk);
        arduino.vibrarOn();

        Debug.Log("Actualizando HUD del jugador");
        jugadorHUD.actVida(entiWitch.vidaActual, entiWitch.vidaMax);

        Debug.Log("Esperando 1 segundo");
        yield return new WaitForSeconds(1f);

        if (estaMuerto)
        {
            Debug.Log("Actualizando estado a Pierde");
            estado = BattleState.PIERDE;
            FinBatalla();
        }
        else
        {
            Debug.Log("Actualizando estado a Elegir hechizo");
            estado = BattleState.ELEGIRHECHIZO;
            ElegirHechizo();
        }
    }

    void ElegirHechizo()
    {
        Debug.Log("Funcion elegir hechizo ejecutada");
        mensajeTexto.text = "<b>Realiza un hechizo</b>";
    }

    public void BotonAtkArriba() //Ataque de Luz
    {
        tipoAtk = "luz";
         if (estado != BattleState.ELEGIRHECHIZO)
            return;
        
        arduino.amarilloOn();
        mensajeTexto.text = "Has realizado un <b><color=#f4be8a>hechizo de luz</color></b>";

        Debug.Log("Comienza atk arriba");
        estado = BattleState.TURNOJUGADOR;
        Debug.Log("Estado de batalla cambiado a Turno Jugador");

        Debug.Log("Ejecutando animacion");
        entiWitch.anim.SetTrigger(Animator.StringToHash("Atk arriba"));
        Debug.Log("Rotacion reestablecida");
        entiWitch.transform.rotation = rotInicial;

        StartCoroutine(AtaqueJugador());
    }
    public void BotonAtkAbajo() //Ataque de Agua
    {
        tipoAtk = "agua";
        if (estado != BattleState.ELEGIRHECHIZO)
            return;

        arduino.blancoOn();
        mensajeTexto.text = "Has realizado un <b><color=#5b5bff>hechizo de agua</color></b>";

        Debug.Log("Comienza atk abajo");
        estado = BattleState.TURNOJUGADOR;
        Debug.Log("Estado de batalla cambiado a Turno Jugador");

        Debug.Log("Ejecutando animacion");
        entiWitch.anim.SetTrigger(Animator.StringToHash("Atk abajo"));
        Debug.Log("Rotacion reestablecida");
        entiWitch.transform.rotation = rotInicial;

        StartCoroutine(AtaqueJugador());
    }
    public void BotonAtkIzq() //Ataque de planta
    {
        tipoAtk = "planta";
         if (estado != BattleState.ELEGIRHECHIZO)
             return;

        arduino.verdeOn();
        mensajeTexto.text = "Has realizado un <b><color=#008000>hechizo de planta</color></b>";

        Debug.Log("Comienza atk izquierda");
        estado = BattleState.TURNOJUGADOR;
        Debug.Log("Estado de batalla cambiado a Turno Jugador");

        Debug.Log("Ejecutando animacion");
        entiWitch.anim.SetTrigger(Animator.StringToHash("Atk izquierda"));
        Debug.Log("Rotacion reestablecida");
        entiWitch.transform.rotation = rotInicial;

        StartCoroutine(AtaqueJugador());
    }
    public void BotonAtkDer() //Ataque de fuego
    {  
        tipoAtk = "fuego";
        if (estado != BattleState.ELEGIRHECHIZO)
           return;

        arduino.rojoOn();
        mensajeTexto.text = "Has realizado un <b><color=#ff3232>hechizo de fuego</color></b>";

        Debug.Log("Comienza atk derecha");
        estado = BattleState.TURNOJUGADOR;
        Debug.Log("Estado de batalla cambiado a Turno Jugador");

        Debug.Log("Ejecutando animacion");
        entiWitch.anim.SetTrigger(Animator.StringToHash("Atk derecha"));
        Debug.Log("Rotacion reestablecida");
        entiWitch.transform.rotation = rotInicial;

        StartCoroutine(AtaqueJugador());
    }

    IEnumerator Resultados()
    {
        bool subeNivel;
        Debug.Log("Se ha ejectuado la funcion Resultados");
        yield return new WaitForSecondsRealtime(2);
        mensajeTexto.text = "Haz obtenido <b>" + enemigoUnidad.exp + "</b> puntos de experiencia.";

        Debug.Log("Verificando si subio de nivel");
        entiWitch.exp += enemigoUnidad.exp; //Se suma la exp al jugador sin importar que pueda subir de nivel

        if (entiWitch.exp >= entiWitch.expSig)
        {
            subeNivel = true;
            jugadorHUD.actExp(entiWitch.exp, entiWitch.expSig);
        }
        else
        {
            Debug.Log("No subio de nivel. Se ha sumando la exp ganada y actualizado en la interfaz.");
            subeNivel = false;
            jugadorHUD.actExp(entiWitch.exp, entiWitch.expSig); //Se actualizo la exp en pantalla
            yield return new WaitForSecondsRealtime(2);
        }

        while (subeNivel == true) //Se ejecuta meintras pueda seguir subiendo de nivelS
        {
            yield return new WaitForSecondsRealtime(1);

            Debug.Log("Mostrando mensaje de subio nivel.");
            mensajeTexto.text = "Has subido al nivel <b>" + (entiWitch.nivel + 1) + "</b>.";
            yield return new WaitForSeconds(1);

            resultadoHUD.mostrar(true); //Se muestra panel de resultados sin cambiar
            yield return new WaitForSeconds(2);

            entiWitch.nuevosStats(); //Se obtien los buffs
            resultadoHUD.actualizar(entiWitch.nombre, entiWitch.nivel, entiWitch.vidaMax, entiWitch.atk, entiWitch.def, entiWitch.vel, entiWitch.buffVida, entiWitch.buffAtk, entiWitch.buffDef, entiWitch.buffVel);

            yield return new WaitForSeconds(2);

            Debug.Log("Entrado al while de subir nivel.");
            subeNivel = entiWitch.subirNv(); //Aqui se sube el nivel y los stats
            jugadorHUD.actExp(entiWitch.exp, entiWitch.expSig); //Se actualiza la exp
            resultadoHUD.configResultado(entiWitch.nombre, entiWitch.nivel, entiWitch.vidaMax, entiWitch.atk, entiWitch.def, entiWitch.vel); //Los nuevos datos se sobre escriben en el panel
            yield return new WaitForSecondsRealtime(3);

            jugadorHUD.configHUD(entiWitch);
            resultadoHUD.mostrar(false); //Se esconde el panel de resultados
            yield return new WaitForSeconds(1);
        }
        Debug.Log("Terminando funcion resultados.");
        estado = BattleState.SIGNIVEL;
        Gana();
    }
}