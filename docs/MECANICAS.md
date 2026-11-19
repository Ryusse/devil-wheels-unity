# Mecánicas del juego

## Carretera infinita

- El carro **no avanza**: lo que se mueve es la carretera. `MotorCarreteras` desplaza hacia abajo el objeto padre
  de todas las calles.
- Cuando un tramo sale por abajo (toca el collider `LimiteCarreteras`, tag `LimiteCalles`), se destruye y se crea
  uno nuevo arriba. El tramo nuevo se elige al azar entre **5 variantes** (`Calle0`–`Calle4`).
- Velocidades (unidades por segundo):

| Situación | Velocidad | Método |
|---|---|---|
| Carretera | 15 | `SpeedCarretera()` |
| Arcén | 5 | `SpeedArcen()` |
| Coche malo (sin usar por ahora) | 3 | `SpeedCocheMalo()` |
| Detenido | 0 | `SpeedStop()` |

## Inicio de partida

1. Al abrir la escena se muestra el **menú de inicio** y la carretera permanece quieta.
2. Al pulsar *INICIAR JUEGO* se lanza la **cuenta atrás**: suena `InicioFX`, aparecen 3, 2, 1 y GO.
3. En el GO se activan el **cronómetro**, el movimiento de la carretera, la **música** y el **motor** del carro.

## Control del carro

- Movimiento lateral con `←` `→` o `A` `D`, velocidad **10**.
- El carro se **inclina hasta 30°** al girar.
- Si se sale de los límites de pantalla, vuelve a la posición inicial.

## Arcén

Dos colliders trigger por tramo (`ArcenR` y `ArcenD`) cubren las franjas grises a los lados de la calzada.
Al entrar el carro: la velocidad baja a 5 y el motor suena más grave (pitch 1). Al salir vuelve a 15 y pitch 1,6.

## Vidas y choques

- **3 vidas**, representadas por 3 carritos en la esquina superior izquierda.
- Fuentes de daño: `CarroMalo`, `QuitaVida` (botella), `Esfera` y los **14 edificios** de cada tramo.
- Al chocar:
  1. Se pierde una vida y aparece el icono **☠ −1**.
  2. Suena el golpe (`ChoqueCoche`) y el sonido de vida perdida (`QuitaVida`).
  3. Si quedan vidas, el carro se recoloca en el **carril libre más alejado del obstáculo** (X = −1,4 / 0 / 1,4)
     y parpadea 2 segundos durante los cuales no puede perder más vidas.
  4. Con 0 vidas, la partida se detiene y aparece **Game Over**.
- `PowerUp` (comida) está pensado para **sumar 10 segundos** (icono ♥ +10). Ver [ESTADO.md](ESTADO.md).

## Cronómetro y distancia

- Cuenta atrás desde **30 s** (campo `tiempo` de `Cronometro`).
- La distancia en metros crece cada frame con `velocidad × tiempo` y se muestra en pantalla.
- Al llegar a 0 la partida se marca como terminada y la carretera se detiene.

## Game Over

Muestra el rótulo, el texto *"Has recorrido X mts"*, cambia la música y ofrece el botón **REINICIAR**,
que recarga la escena `main`.
