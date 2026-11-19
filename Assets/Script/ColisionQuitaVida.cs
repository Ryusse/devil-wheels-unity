using UnityEngine;

// Cuando el carro (tag "Coche") choca con este GameObject:
//  - pierde una vida
//  - el juego NO se reinicia: el carro se recoloca en el carril libre mas lejano al obstaculo,
//    justo al lado de donde choco, y queda protegido unos segundos (parpadea).
// Con 0 vidas no recoloca: el script Vidas muestra el Game Over.
public class ColisionQuitaVida : MonoBehaviour
{
	public Vidas vidasScript;
	public string tagObjetivo = "Coche";
	public float segundosInvulnerable = 2f;
	// posiciones X (mundo) de los carriles de la carretera
	public float[] carriles = { -1.4f, 0f, 1.4f };

	// compartido por todos los obstaculos: mientras dure, no se pierden mas vidas
	static float invulnerableHasta = -100f;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	static void Reiniciar()
	{
		invulnerableHasta = -100f;
	}

	void Start()
	{
		if (vidasScript == null)
		{
			GameObject go = GameObject.Find("Vidas");
			if (go != null) vidasScript = go.GetComponent<Vidas>();
		}
	}

	void OnCollisionEnter2D(Collision2D cInfo)
	{
		Golpe(cInfo.gameObject);
	}

	void OnTriggerEnter2D(Collider2D cInfo)
	{
		Golpe(cInfo.gameObject);
	}

	void Golpe(GameObject otro)
	{
		if (!otro.CompareTag(tagObjetivo) || vidasScript == null) return;
		if (vidasScript.contadorVidas <= 0 || Time.time < invulnerableHasta) return;

		invulnerableHasta = Time.time + segundosInvulnerable;
		vidasScript.contadorVidas = vidasScript.contadorVidas - 1;
		vidasScript.ImagenMenosVida();

		// si aun quedan vidas, sigue la partida al lado del obstaculo
		if (vidasScript.contadorVidas > 0)
		{
			RecolocarCoche(otro);
			ParpadeoCoche p = otro.GetComponent<ParpadeoCoche>();
			if (p != null) p.Parpadear(segundosInvulnerable);
		}
	}

	void RecolocarCoche(GameObject coche)
	{
		// el movimiento lateral lo controla el padre (ControladorCarro)
		Transform mover = coche.transform.parent != null ? coche.transform.parent : coche.transform;

		Collider2D col = GetComponent<Collider2D>();
		float centroX = col != null ? col.bounds.center.x : transform.position.x;
		float medio = col != null ? col.bounds.extents.x : 0f;

		// carril con mas espacio libre respecto al obstaculo
		float mejor = carriles[0];
		float mejorDist = -1f;
		foreach (float c in carriles)
		{
			float d = Mathf.Max(0f, Mathf.Abs(c - centroX) - medio);
			if (d > mejorDist + 0.001f || (Mathf.Abs(d - mejorDist) <= 0.001f && Mathf.Abs(c) < Mathf.Abs(mejor)))
			{
				mejorDist = d;
				mejor = c;
			}
		}
		mover.position = new Vector3(mejor, mover.position.y, mover.position.z);
	}
}
