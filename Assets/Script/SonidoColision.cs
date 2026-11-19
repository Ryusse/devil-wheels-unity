using UnityEngine;

// Reproduce un sonido cuando el objeto con el tag indicado (por defecto "Coche")
// choca con este GameObject o entra en su trigger.
public class SonidoColision : MonoBehaviour
{
	public AudioClip sonido;
	public string tagObjetivo = "Coche";
	[Range(0f, 1f)] public float volumen = 1f;

	void OnCollisionEnter2D(Collision2D cInfo)
	{
		Sonar(cInfo.gameObject);
	}

	void OnTriggerEnter2D(Collider2D cInfo)
	{
		Sonar(cInfo.gameObject);
	}

	void Sonar(GameObject otro)
	{
		if (sonido == null || !otro.CompareTag(tagObjetivo)) return;
		Vector3 punto = Camera.main != null ? Camera.main.transform.position : transform.position;
		AudioSource.PlayClipAtPoint(sonido, punto, volumen);
	}
}
