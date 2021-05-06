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
    //Prefabs enemigo;
    //public GameObject enemigoPrefab;

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

    void Start()
    {
        estado = BattleState.INICIO;
        StartCoroutine(ConfigBatalla());
        rotInicial = entiWitch.transform.rotation;
        fase = 1;
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
        GameObject jugadorGo = Instantiate(jugadorPrefab, spawnJugador);
        entiWitch = jugadorGo.GetComponent<Unidad>();

        GameObject enemigoGo = Instantiate(enemigoPrefab[0], spawnEnemigo);    //Instanciar un enemigo al spawn

        enemigoUnidad = enemigoGo.GetComponent<Unidad>(); //Obtener el componente Unidad

        mensajeTexto.text = "Ha aparecido " + "<b>" + enemigoUnidad.nombre + "</b>" + ".";

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
        bool estaMuerto = enemigoUnidad.RecibeDanio(entiWitch.atk);

        Debug.Log("Actualizando HUD del enemigo");
        enemigoHUD.actVida(enemigoUnidad.vidaActual, enemigoUnidad.vidaMax);
        mensajeTexto.text = "Haz atacado a " + enemigoUnidad.nombre;

        Debug.Log("Esperando 2 seg");
        yield return new WaitForSeconds(2f);

        if (estaMuerto)
        {
            Debug.Log("EL enemigo murio");
            //Fin de la batalla
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

    public void FinBatalla()
    {
        Debug.Log("Ejecutando la funcion Fin de la batalla");
        if (estado == BattleState.RESULTADOS)
        {
            mensajeTexto.text = "Haz ganado la batalla!";
            StartCoroutine(Resultados());
            Gana();
        }
        else if (estado == BattleState.PIERDE)
        {
            entiWitch.anim.SetTrigger(Animator.StringToHash("Muerte"));
            mensajeTexto.text = "Haz sido derrotado.";
            StartCoroutine(GameOver());
        }
    }

    public void Gana()
    {
        fase ++;
        ConfigBatalla();
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
        mensajeTexto.text = enemigoUnidad.nombre + " ataca!";

        Debug.Log("Esperando 1 segundo");
        yield return new WaitForSeconds(1f);

        Debug.Log("Reduciendo vida del jugador");
        bool estaMuerto = entiWitch.RecibeDanio(enemigoUnidad.atk);
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
        mensajeTexto.text = "Escoje un hechizo";
    }

    public void BotonAtkArriba() //Ataque de Luz
    {
         if (estado != BattleState.ELEGIRHECHIZO)
            return;

        arduino.amarilloOn();
        mensajeTexto.text = "Has realizado un ataque de luz";

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
        if (estado != BattleState.ELEGIRHECHIZO)
            return;

        arduino.blancoOn();
        mensajeTexto.text = "Has realizado un ataque de agua";

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
         if (estado != BattleState.ELEGIRHECHIZO)
             return;

        arduino.verdeOn();
        mensajeTexto.text = "Has realizado un ataque de planta";

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
        if (estado != BattleState.ELEGIRHECHIZO)
           return;

        arduino.rojoOn();
        mensajeTexto.text = "Has realizado un ataque de fuego";

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
        mensajeTexto.text = "Haz obtenido " + enemigoUnidad.exp + " puntos de experiencia.";

        Debug.Log("Verificando si subio de nivel");
        entiWitch.exp += enemigoUnidad.exp; //Se suma la exp al jugador sin importar que pueda subir de nivel

        if (enemigoUnidad.exp >= entiWitch.expSig)
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
            mensajeTexto.text = "Has subido al nivel " + (entiWitch.nivel + 1) + ".";
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
    }
}