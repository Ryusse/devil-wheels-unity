using System.IO;
using UnityEditor;
using UnityEngine;

// Entregables: sonido en cada colision + colision con la esfera.
// Menu: Tools > Aplicar Colisiones y Esfera. Es idempotente. Edita el prefab Calle0
// (las 6 calles de la escena lo heredan).
public static class AplicarColisiones
{
    const string PrefabPath = "Assets/Prefabs/Calle0.prefab";
    const string EsferaPng = "Assets/Texturas/Esfera.png";

    [MenuItem("Tools/Aplicar Colisiones y Esfera")]
    public static void Aplicar()
    {
        if (EditorApplication.isPlaying) { Debug.LogError("[Colisiones] Sal del modo Play primero."); return; }

        var esfera = CrearSpriteEsfera();
        var choque = Clip("ChoqueCoche.aif");
        var quitaVida = Clip("QuitaVida.aif");
        var masDiez = Clip("MasDIez.aif");

        var raiz = PrefabUtility.LoadPrefabContents(PrefabPath);

        PonerSonido(raiz.transform, "CarroMalo", choque);
        PonerSonido(raiz.transform, "PowerUp", masDiez);
        PonerSonido(raiz.transform, "QuitaVida", quitaVida);

        // Esfera: hija de Calle0, con la escala compensada del padre (1.1 x 40.32) para quedar sin deformar
        var t = raiz.transform.Find("Esfera");
        if (t == null)
        {
            var go = new GameObject("Esfera");
            go.transform.SetParent(raiz.transform, false);
            t = go.transform;
        }
        t.localPosition = new Vector3(-0.3f, -0.17f, 0f);
        t.localScale = new Vector3(1f / raiz.transform.localScale.x, 1f / raiz.transform.localScale.y, 1f);

        var sr = Asegurar<SpriteRenderer>(t.gameObject);
        sr.sprite = esfera;
        sr.sortingOrder = 1;

        var cc = Asegurar<CircleCollider2D>(t.gameObject);
        cc.isTrigger = false;
        cc.radius = esfera != null ? esfera.bounds.extents.x : 0.6f;

        var rb = Asegurar<Rigidbody2D>(t.gameObject);
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        var sc = Asegurar<SonidoColision>(t.gameObject);
        sc.sonido = choque;
        sc.tagObjetivo = "Coche";

        PrefabUtility.SaveAsPrefabAsset(raiz, PrefabPath);
        PrefabUtility.UnloadPrefabContents(raiz);
        AssetDatabase.SaveAssets();
        Debug.Log("[Colisiones] Listo: sonido en CarroMalo, PowerUp, QuitaVida y Esfera (nueva) dentro del prefab Calle0. La escena lo hereda.");
    }

    static void PonerSonido(Transform raiz, string nombre, AudioClip clip)
    {
        var t = raiz.Find(nombre);
        if (t == null) { Debug.LogWarning("[Colisiones] El prefab no tiene " + nombre); return; }
        var sc = Asegurar<SonidoColision>(t.gameObject);
        sc.sonido = clip;
        sc.tagObjetivo = "Coche";
        if (clip == null) Debug.LogWarning("[Colisiones] Falta el clip de " + nombre);
    }

    static AudioClip Clip(string archivo)
    {
        var c = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sonidos/" + archivo);
        if (c == null) Debug.LogWarning("[Colisiones] No encontre Assets/Sonidos/" + archivo);
        return c;
    }

    static T Asegurar<T>(GameObject go) where T : Component
    {
        var c = go.GetComponent<T>();
        return c != null ? c : go.AddComponent<T>();
    }

    // Dibuja una esfera con sombreado y la importa como Sprite
    static Sprite CrearSpriteEsfera()
    {
        if (!File.Exists(EsferaPng))
        {
            const int n = 128;
            var tex = new Texture2D(n, n, TextureFormat.RGBA32, false);
            var baseColor = new Color(0.90f, 0.25f, 0.10f);
            var luz = new Vector2(-0.35f, 0.4f);
            for (int y = 0; y < n; y++)
                for (int x = 0; x < n; x++)
                {
                    var p = new Vector2((x + 0.5f) / n * 2f - 1f, (y + 0.5f) / n * 2f - 1f);
                    float d = p.magnitude;
                    float alfa = Mathf.Clamp01((1f - d) * n * 0.5f);
                    float brillo = Mathf.Clamp01(1.0f - (p - luz).magnitude * 0.75f);
                    var c = Color.Lerp(baseColor * 0.45f, Color.Lerp(baseColor, Color.white, 0.55f), brillo);
                    c.a = alfa;
                    tex.SetPixel(x, y, c);
                }
            File.WriteAllBytes(EsferaPng, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
            AssetDatabase.ImportAsset(EsferaPng);
        }
        var ti = (TextureImporter)AssetImporter.GetAtPath(EsferaPng);
        if (ti != null && ti.textureType != TextureImporterType.Sprite)
        {
            ti.textureType = TextureImporterType.Sprite;
            ti.spriteImportMode = SpriteImportMode.Single;
            ti.spritePixelsPerUnit = 100;
            ti.alphaIsTransparency = true;
            ti.SaveAndReimport();
        }
        return AssetDatabase.LoadAssetAtPath<Sprite>(EsferaPng);
    }
}
