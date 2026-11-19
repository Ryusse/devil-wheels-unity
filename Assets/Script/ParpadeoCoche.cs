using System.Collections;
using UnityEngine;

// Hace parpadear al coche mientras esta protegido despues de un choque.
public class ParpadeoCoche : MonoBehaviour
{
	public void Parpadear(float segundos)
	{
		StopAllCoroutines();
		StartCoroutine(Rutina(segundos));
	}

	IEnumerator Rutina(float segundos)
	{
		SpriteRenderer sr = GetComponent<SpriteRenderer>();
		if (sr == null) yield break;

		float t = 0f;
		while (t < segundos)
		{
			sr.enabled = !sr.enabled;
			yield return new WaitForSeconds(0.1f);
			t += 0.1f;
		}
		sr.enabled = true;
	}
}
