using System.Linq;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Menu de inicio (titulo, boton Iniciar juego, controles, nombre del autor) y boton de Game Over en espanol.
// Menu: Tools > Aplicar Menu de Inicio. Es idempotente.
public static class AplicarMenu
{
    const string ScenePath = "Assets/Escena/main.unity";
    static readonly Color Rojo = new Color(1f, 0.29f, 0.24f);

    [MenuItem("Tools/Aplicar Menu de Inicio")]
    public static void Aplicar()
    {
        if (EditorApplication.isPlaying) { Debug.LogError("[Menu] Sal del modo Play primero."); return; }

        Scene scene = SceneManager.GetActiveScene();
        if (scene.path != ScenePath) scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

        var goPop = Buscar("PopInicio");
        var goOver = Buscar("PopGameOver");
        var goCuenta = Buscar("CuentraAtras") ?? Buscar("CuentaAtras");
        if (goPop == null || goCuenta == null) { Debug.LogError("[Menu] Faltan PopInicio o CuentraAtras."); return; }
        var cuenta = goCuenta.GetComponent<CuentaAtras>();
        var fuente = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        var pop = goPop.transform;

        // ---- Game Over: boton en espanol ----
        if (goOver != null)
        {
            var b = goOver.transform.Find("Button");
            if (b != null) EstiloBoton(b, "REINICIAR", 38, fuente);
        }

        // ---- Menu de inicio ----
        // fondo: pantalla de titulo DEVIL WHEELS
        var fondo = Hijo(pop, "Fondo");
        Estirar(fondo);
        var imgFondo = Asegurar<Image>(fondo.gameObject);
        imgFondo.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Texturas/Logo.png");
        imgFondo.raycastTarget = false;
        fondo.SetAsFirstSibling();

        var bg = pop.Find("BG");
        if (bg != null) bg.gameObject.SetActive(false); // el fondo ya es opaco

        // panel de controles (el dibujo Help) abajo a la izquierda
        const float esc = 0.36f; // 1168 x 604 -> 420 x 217
        var help = pop.Find("Image");
        var posHelp = new Vector2(-410f, -190f);
        if (help != null) Colocar((RectTransform)help, posHelp, new Vector2(1168f * esc, 604f * esc));

        // el titulo "HOW TO PLAY" del dibujo esta en ingles: se tapa con "COMO JUGAR"
        var cubre = Hijo(pop, "CubreTitulo");
        Colocar((RectTransform)cubre, posHelp + new Vector2(-1f, 94f), new Vector2(120f, 28f));
        var imgCubre = Asegurar<Image>(cubre.gameObject);
        imgCubre.color = new Color(1f, 0.29f, 0.24f);
        imgCubre.raycastTarget = false;
        Texto(cubre, "Texto", "CÓMO JUGAR", 16, FontStyle.Bold, Vector2.zero, new Vector2(120f, 28f), fuente, false);

        Texto(pop, "TextoControles", "Mover: flechas IZQUIERDA / DERECHA o A / D", 17, FontStyle.Bold,
              new Vector2(-410f, -320f), new Vector2(430f, 30f), fuente, true);

        // boton Iniciar juego, a la derecha
        var boton = pop.Find("Button");
        if (boton != null)
        {
            Colocar((RectTransform)boton, new Vector2(400f, -120f), new Vector2(340f, 70f));
            EstiloBoton(boton, "INICIAR JUEGO", 32, fuente);
        }

        // nombre del autor, abajo en el centro
        Texto(pop, "Autor", "Joel Ocaño", 28, FontStyle.Bold, new Vector2(0f, -335f), new Vector2(360f, 40f), fuente, true);

        // logica
        var menu = Asegurar<MenuInicio>(goPop);
        menu.cuentaAtras = cuenta;
        EditorUtility.SetDirty(menu);
        var btn = boton != null ? boton.GetComponent<Button>() : null;
        if (btn != null && btn.onClick.GetPersistentEventCount() == 0)
            UnityEventTools.AddPersistentListener(btn.onClick, menu.Iniciar);

        goPop.SetActive(true);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[Menu] Listo: menu de inicio (titulo, controles, boton y nombre) y boton de Game Over en espanol.");
    }

    // ---------- utilidades ----------
    static void EstiloBoton(Transform boton, string texto, int tam, Font fuente)
    {
        var img = boton.GetComponent<Image>();
        if (img != null) { img.sprite = null; img.color = Rojo; }
        var t = boton.Find("Text");
        if (t == null) return;
        var txt = t.GetComponent<Text>();
        txt.text = texto;
        txt.font = fuente;
        txt.fontSize = tam;
        txt.fontStyle = FontStyle.Bold;
        txt.color = Color.white;
        txt.alignment = TextAnchor.MiddleCenter;
        Asegurar<Shadow>(t.gameObject);
    }

    static void Texto(Transform padre, string nombre, string contenido, int tam, FontStyle estilo,
                      Vector2 pos, Vector2 size, Font fuente, bool contorno)
    {
        var t = Hijo(padre, nombre);
        Colocar((RectTransform)t, pos, size);
        var txt = Asegurar<Text>(t.gameObject);
        txt.text = contenido;
        txt.font = fuente;
        txt.fontSize = tam;
        txt.fontStyle = estilo;
        txt.color = Color.white;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.horizontalOverflow = HorizontalWrapMode.Overflow;
        txt.verticalOverflow = VerticalWrapMode.Overflow;
        txt.raycastTarget = false;
        if (contorno)
        {
            var o = Asegurar<Outline>(t.gameObject);
            o.effectColor = new Color(0f, 0f, 0f, 0.9f);
            o.effectDistance = new Vector2(2f, -2f);
        }
    }

    static Transform Hijo(Transform padre, string nombre)
    {
        var t = padre.Find(nombre);
        if (t != null) return t;
        var go = new GameObject(nombre, typeof(RectTransform));
        go.transform.SetParent(padre, false);
        return go.transform;
    }

    static void Colocar(RectTransform rt, Vector2 pos, Vector2 size)
    {
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
    }

    static void Estirar(Transform t)
    {
        var rt = (RectTransform)t;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    static T Asegurar<T>(GameObject go) where T : Component
    {
        var c = go.GetComponent<T>();
        return c != null ? c : go.AddComponent<T>();
    }

    static GameObject Buscar(string nombre)
    {
        return Resources.FindObjectsOfTypeAll<GameObject>()
            .FirstOrDefault(g => g.name == nombre && g.scene.IsValid() && !EditorUtility.IsPersistent(g));
    }
}
