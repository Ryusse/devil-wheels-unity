# Scripts (`Assets/Script`)

Los scripts marcados como **curso** vienen del material de clase; los marcados como **propio** los agregó el autor.

| Script | Origen | Dónde está | Qué hace |
|---|---|---|---|
| `MotorCarreteras` | curso | GameObject `MotorCarreteras` | Mueve la carretera, crea tramos nuevos al azar y guarda las velocidades. |
| `LimiteCarreteras` | curso | GameObject `LimiteCarreteras` | Destruye el tramo que sale de pantalla y pide uno nuevo. |
| `ControladorCoche` | curso | `ControladorCarro` | Movimiento lateral, inclinación del carro y reposición si se sale. |
| `CuentaAtras` | curso | `CuentraAtras` | Cuenta atrás 3-2-1-GO. Arranca música y motor. **Espera al menú** para iniciar. |
| `Cronometro` | curso | `Cronometro` | Tiempo restante, metros recorridos e icono +10. |
| `ArcenCarreteras` | curso | `ArcenR` / `ArcenD` (dentro de `Calle0`) | Reduce la velocidad mientras el carro está sobre el arcén. |
| `Vidas` | curso | `Vidas` | Controla el contador de vidas, los iconos y dispara el Game Over. |
| `QuitaVidas` | curso | `QuitaVida` (dentro de `Calle0`) | Quita una vida al tocarla y desaparece. |
| `PopGameOver` | curso | `PopGameOver` | Panel de fin de juego, metros recorridos y reinicio. Recarga `main`. |
| `ColisionQuitaVida` | propio | `CarroMalo`, `Esfera`, edificios | Quita una vida, recoloca el carro en un carril libre y da 2 s de protección. |
| `ParpadeoCoche` | propio | `Carro` | Hace parpadear el carro mientras está protegido. |
| `SonidoColision` | propio | Obstáculos y `PowerUp` | Reproduce un sonido cuando el carro choca o entra en el trigger. |
| `MenuInicio` | propio | `PopInicio` | Cierra el menú y lanza la cuenta atrás (botón, `ENTER` o `ESPACIO`). |

## Cómo se conectan

```
MenuInicio ──► CuentaAtras ──► Cronometro.cronometroEncendido
                    │
                    └────────► MotorCarreteras.cuentaRegresivaTermino  (la carretera empieza a moverse)

ArcenCarreteras ─► MotorCarreteras (SpeedArcen / SpeedCarretera)

ColisionQuitaVida ─┐
QuitaVidas ────────┼─► Vidas ──► PopGameOver (con 0 vidas)
                   └─► ParpadeoCoche (invulnerabilidad)

MotorCarreteras ◄─► LimiteCarreteras (genera y destruye tramos)
```

## Tags usados

| Tag | Quién lo usa |
|---|---|
| `Coche` | El carro. Lo comprueban `ArcenCarreteras`, `QuitaVidas`, `ColisionQuitaVida` y `SonidoColision`. |
| `LimiteCalles` | `Pieza1` de cada tramo. Lo comprueba `LimiteCarreteras`. |

## Notas

- Los scripts del curso se mantienen sin cambios salvo dos ajustes: `CuentaAtras` ya no arranca solo (espera al menú)
  y `PopGameOver` recarga la escena `main` (la guía original usaba `Level1`, que no existe aquí).
- Los scripts usan el **Input Manager clásico** (`Input.GetAxis`). El proyecto está configurado con
  `Active Input Handling = Input Manager (Old)`.
