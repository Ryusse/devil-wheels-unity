using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Aplica el PDF "Semana S08_s2" (arcen de la carretera) a main.unity y al prefab Calle0.
// Menu: Tools > Aplicar Arcen S08-2. Es idempotente.
public static class AplicarArcenS08
{
    const string ScenePath = "Assets/Escena/main.unity";
    const string PrefabPath = "Assets/Prefabs/Calle0.prefab";

    [MenuItem("Tools/Aplicar Arcen S08-2")]
    public static void Aplicar()
    {
        if (EditorApplication.isPlaying) { Debug.LogError("[Arcen] Sal del modo Play primero."); return; }

        // Tag "Coche" (paso 6)
        if (!UnityEditorInternal.InternalEditorUtility.tags.Contains("Coche"))
            UnityEditorInternal.InternalEditorUtility.AddTag("Coche");

        // Pasos 1, 2 y 5: script ArcenCarreteras en ArcenR y ArcenD, aplicado al prefab Calle0
        var raiz = PrefabUtility.LoadPrefabContents(PrefabPath);
        int enPrefab = 0;
        foreach (var nombre in new[] { "Colliders/ArcenR", "Colliders/ArcenD" })
        {
            var t = raiz.transform.Find(nombre);
            if (t == null) { Debug.LogWarning("[Arcen] El prefab no tiene " + nombre); continue; }
            if (t.GetComponent<ArcenCarreteras>() == null) { t.gameObject.AddComponent<ArcenCarreteras>(); enPrefab++; }
        }
        PrefabUtility.SaveAsPrefabAsset(raiz, PrefabPath);
        PrefabUtility.UnloadPrefabContents(raiz);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Scene scene = SceneManager.GetActiveScene();
        if (scene.path != ScenePath)
            scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

        var motor = Buscar("MotorCarreteras");
        var carro = Buscar("Carro");
        if (motor == null || carro == null) { Debug.LogError("[Arcen] Faltan MotorCarreteras o Carro en la escena."); return; }

        // Pasos 6 y 8: Carro con tag Coche, collider sin Trigger, Rigidbody2D Kinematic con Full Kinematic Contacts
        carro.tag = "Coche";
        var bc = carro.GetComponent<BoxCollider2D>();
        if (bc != null) bc.isTrigger = false;
        var rb = carro.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = true;
            rb.useFullKinematicContacts = true;
        }
        EditorUtility.SetDirty(carro);

        // Pasos 3, 4 y 9: vincular MotorCarreteras y Carro en ArcenR/ArcenD de las 6 calles (Calle0 + ContenedorCalles)
        var motorScript = motor.GetComponent<MotorCarreteras>();
        int vinculados = 0;
        foreach (var a in Resources.FindObjectsOfTypeAll<ArcenCarreteras>()
                     .Where(x => x.gameObject.scene.IsValid() && !EditorUtility.IsPersistent(x)))
        {
            var so = new SerializedObject(a);
            so.FindProperty("motorCarretera").objectReferenceValue = motor;
            so.FindProperty("motorCarreterasScript").objectReferenceValue = motorScript;
            so.FindProperty("coche").objectReferenceValue = carro;
            so.ApplyModifiedProperties();
            vinculados++;
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("[Arcen] Listo. Script agregado al prefab en " + enPrefab + " objeto(s); ArcenCarreteras vinculados en la escena: " + vinculados + " (se esperan 12 = 6 calles x ArcenR y ArcenD).");
    }

    static GameObject Buscar(string nombre)
    {
        return Resources.FindObjectsOfTypeAll<GameObject>()
            .FirstOrDefault(g => g.name == nombre && g.scene.IsValid() && !EditorUtility.IsPersistent(g));
    }
}
