using System.Linq;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Conecta el sistema de vidas: chocar con CarroMalo, Esfera o QuitaVida resta una vida;
// con 0 vidas sale el Game Over. Menu: Tools > Aplicar Vidas. Es idempotente.
public static class AplicarVidas
{
    const string ScenePath = "Assets/Escena/main.unity";
    const string PrefabPath = "Assets/Prefabs/Calle0.prefab";

    [MenuItem("Tools/Aplicar Vidas")]
    public static void Aplicar()
    {
        if (EditorApplication.isPlaying) { Debug.LogError("[Vidas] Sal del modo Play primero."); return; }

        // ---- Prefab Calle0: quien quita vida ----
        var choque = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sonidos/ChoqueCoche.aif");
        if (choque == null) Debug.LogWarning("[Vidas] No encontre Assets/Sonidos/ChoqueCoche.aif");
        var raiz = PrefabUtility.LoadPrefabContents(PrefabPath);
        var quitaVida = raiz.transform.Find("QuitaVida");
        if (quitaVida != null)
        {
            Asegurar<QuitaVidas>(quitaVida.gameObject);
            // sonido del choque; ademas, el script Vidas reproduce QuitaVida.aif al perder la vida
            PonerChoque(quitaVida.gameObject, choque);
        }
        else Debug.LogWarning("[Vidas] El prefab no tiene QuitaVida.");
        foreach (var n in new[] { "CarroMalo", "Esfera" })
        {
            var t = raiz.transform.Find(n);
            if (t == null) { Debug.LogWarning("[Vidas] El prefab no tiene " + n + (n == "Esfera" ? " (ejecuta antes Tools > Aplicar Colisiones y Esfera)." : ".")); continue; }
            Asegurar<ColisionQuitaVida>(t.gameObject);
            PonerChoque(t.gameObject, choque);
        }
        // Edificios y casas (ElementosCuidad): tambien son obstaculos. Collider en Trigger + Rigidbody2D
        // Kinematic (se mueven con la calle) y el mismo script que quita vida.
        var elementos = raiz.transform.Find("ElementosCuidad");
        int edificios = 0;
        if (elementos != null)
        {
            foreach (Transform e in elementos)
            {
                var sr = e.GetComponent<SpriteRenderer>();
                if (sr == null || sr.sprite == null) continue;
                var bc = Asegurar<BoxCollider2D>(e.gameObject);
                bc.isTrigger = true;
                bc.offset = sr.sprite.bounds.center;
                bc.size = sr.sprite.bounds.size * 0.8f; // un poco mas chico que el dibujo, para no pegar por la sombra
                var rb = Asegurar<Rigidbody2D>(e.gameObject);
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.gravityScale = 0f;
                Asegurar<ColisionQuitaVida>(e.gameObject);
                PonerChoque(e.gameObject, choque);
                edificios++;
            }
        }
        else Debug.LogWarning("[Vidas] El prefab no tiene ElementosCuidad.");

        PrefabUtility.SaveAsPrefabAsset(raiz, PrefabPath);
        PrefabUtility.UnloadPrefabContents(raiz);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[Vidas] Edificios convertidos en obstaculo: " + edificios + " (se esperan 14).");

        // ---- Escena ----
        Scene scene = SceneManager.GetActiveScene();
        if (scene.path != ScenePath) scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

        var goVidas = Buscar("Vidas");
        var goMotor = Buscar("MotorCarreteras");
        var goCrono = Buscar("Cronometro");
        var goPop = Buscar("PopGameOver");
        var carro = Buscar("Carro");
        var musica = Buscar("MusicaJuego");
        var fundido = Buscar("Fundido");
        var menos1 = Buscar("Menos1");
        if (goVidas == null || goMotor == null || goCrono == null || goPop == null || carro == null)
        { Debug.LogError("[Vidas] Faltan objetos en la escena."); return; }

        var motor = goMotor.GetComponent<MotorCarreteras>();
        var crono = goCrono.GetComponent<Cronometro>();
        Asegurar<ParpadeoCoche>(carro); // parpadeo del carro tras un choque

        // PopGameOver (panel de fin de juego)
        var pop = Asegurar<PopGameOver>(goPop);
        pop.BGpop = HijoComp<Image>(goPop, "BG");
        pop.imgPop = HijoComp<Image>(goPop, "Image");
        pop.botonReiniciar = HijoComp<Button>(goPop, "Button");
        pop.metrosRecorridos = HijoComp<Text>(goPop, "Text");
        pop.popGameOverGO = goPop;
        pop.imagenFundido = fundido != null ? fundido.GetComponent<Image>() : null;
        pop.cronometroScript = crono;
        pop.musicaJuego = musica;
        pop.musicaGameOver = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sonidos/Latin Industries.mp3");
        pop.Coche = carro;
        if (pop.botonReiniciar != null && pop.botonReiniciar.onClick.GetPersistentEventCount() == 0)
            UnityEventTools.AddPersistentListener(pop.botonReiniciar.onClick, pop.ReiniciarJuego);
        EditorUtility.SetDirty(pop);

        // Vidas
        var vidas = Asegurar<Vidas>(goVidas);
        vidas.vidas = new[] { Buscar("Vida1"), Buscar("Vida2"), Buscar("Vida3") };
        vidas.contadorVidas = 3;
        vidas.popGameOverGO = goPop;
        vidas.popGameOverScript = pop;
        vidas.motorCarretera = goMotor;
        vidas.motorCarreterasScript = motor;
        vidas.cronometroScript = crono;
        vidas.menosVida = menos1 != null ? menos1.GetComponent<Image>() : null;
        var audio = goVidas.GetComponent<AudioSource>();
        if (audio != null)
        {
            audio.clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sonidos/QuitaVida.aif");
            audio.playOnAwake = false;
        }
        EditorUtility.SetDirty(vidas);

        // Escena main en Build Settings (el boton Reiniciar la recarga)
        if (!EditorBuildSettings.scenes.Any(s => s.path == ScenePath))
            EditorBuildSettings.scenes = EditorBuildSettings.scenes.Concat(new[] { new EditorBuildSettingsScene(ScenePath, true) }).ToArray();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();

        if (vidas.vidas.Any(v => v == null) || pop.BGpop == null || pop.imgPop == null || pop.botonReiniciar == null || pop.metrosRecorridos == null || vidas.menosVida == null)
            Debug.LogWarning("[Vidas] Alguna referencia (Vida1-3, BG, Image, Button, Text o Menos1) quedo vacia. Revisalo en el Inspector.");
        Debug.Log("[Vidas] Listo. Chocar con CarroMalo, Esfera o QuitaVida resta una vida; con 0 aparece Game Over.");
    }

    static void PonerChoque(GameObject go, AudioClip clip)
    {
        var s = Asegurar<SonidoColision>(go);
        s.sonido = clip;
        s.tagObjetivo = "Coche";
    }

    static T HijoComp<T>(GameObject padre, string nombre) where T : Component
    {
        var t = padre.transform.Find(nombre);
        return t != null ? t.GetComponent<T>() : null;
    }

    static T Asegurar<T>(GameObject go) where T : Component
    {
        var c = go.GetComponent<T>();
        return c != null ? c : go.AddComponent<T>();
    }

    static GameObject Buscar(string nombre)
    {
        var go = Resources.FindObjectsOfTypeAll<GameObject>()
            .FirstOrDefault(g => g.name == nombre && g.scene.IsValid() && !EditorUtility.IsPersistent(g));
        if (go == null) Debug.LogWarning("[Vidas] No se encontro '" + nombre + "'.");
        return go;
    }
}
