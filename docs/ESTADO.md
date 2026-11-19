# Estado del proyecto

## Hecho

- [x] Carretera infinita con 5 variantes de tramo (guía S08, sesión 1).
- [x] Cuenta atrás 3-2-1-GO, cronómetro, metros recorridos y sonidos (sesión 1).
- [x] Movimiento del carro con flechas y `A`/`D`, con inclinación (sesión 1).
- [x] Arcén que reduce la velocidad (sesión 2).
- [x] Sonido en cada choque.
- [x] Esfera como obstáculo con colisión.
- [x] Sistema de **3 vidas**; cada choque quita una; el carro se recoloca junto al obstáculo y se protege 2 s.
- [x] Edificios y casas como obstáculos.
- [x] Game Over con metros recorridos y botón **REINICIAR** en español.
- [x] Menú de inicio con título, controles, botón **INICIAR JUEGO** y nombre del autor.
- [x] Migración del proyecto de Unity 5.6.2 a Unity 6.
- [x] Compilación para **WebGL** y publicación en Vercel: https://devil-wheels.vercel.app (ver [PUBLICACION.md](PUBLICACION.md)).

## Pendiente / limitaciones

- [ ] **`PowerUp` no suma tiempo todavía.** Hoy solo suena (`MasDIez`). Para sumar 10 segundos falta el script
      `PowerUP.cs` de las siguientes sesiones.
- [ ] `CarroMalo` solo quita una vida; la ralentización original (`CocheMalo.cs`, velocidad 3) no está instalada.
- [ ] Al llegar el tiempo a 0 la carretera se detiene, pero no aparece el panel de Game Over (código comentado
      en `Cronometro`).
- [ ] Los colliders de borde (`LimiteR`, `LimiteD`) no generan sonido: el carro es *Kinematic* y no recibe
      colisión con colliders estáticos.
- [ ] Pantallazo y video demostrativo: fuera del alcance de este repositorio por ahora.

## Decisiones tomadas

| Decisión | Motivo |
|---|---|
| Los scripts del curso de sesiones posteriores (`Scripts-2`) **no** se instalaron completos. | Cambiaban `CuentaAtras`, `MotorCarreteras` y `Cronometro` y rompían la escena actual (exigían un panel de Game Over sin vincular y cargaban una escena `Level1` inexistente). |
| `CuentaAtras` espera al menú. | Es lo que indica el propio script del curso (`//AQUI OCULTAR O APAGAR LA FUNCION PARA QUE ESPERE EL BOTON PLAY`). |
| Tras un choque el carro se recoloca en un carril libre en lugar de reiniciar la partida. | Replica la dinámica de otros juegos de carreras: se pierde una vida y se sigue desde el mismo punto. |
| Los edificios cuentan como obstáculo con un collider al 80 % de su dibujo. | Evita que la sombra del sprite cuente como golpe. |
| El Game Over usa un botón rojo con texto en español. | La imagen original del botón traía el texto en inglés dibujado ("PLAY AGAIN INSERT COIN"). |
| Se mantiene `Active Input Handling = Input Manager (Old)`. | Los scripts del curso usan `Input.GetAxis`. |

## Historial de versiones de Unity

`5.6.2f1` (original, 2018) → `6000.6.9f1` (actual). Los paquetes de servicios heredados (`Analytics`, `In App Purchasing`)
se retiraron del manifiesto porque no se necesitan y no se pueden descargar sin sesión del Package Manager.
