using UnityEngine;

// Menu de inicio: muestra el titulo, los controles y el nombre del autor.
// Al pulsar "Iniciar juego" (o ENTER / ESPACIO) se cierra y empieza la cuenta atras.
public class MenuInicio : MonoBehaviour
{
	public CuentaAtras cuentaAtras;

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space))
			Iniciar();
	}

	public void Iniciar()
	{
		if (!gameObject.activeSelf) return;
		gameObject.SetActive(false);
		if (cuentaAtras != null) cuentaAtras.IniciarCuentaAtras();
	}
}
