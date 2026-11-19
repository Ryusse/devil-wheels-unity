# Herramientas de editor (`Assets/Editor`)

La escena `main.unity` está guardada en formato binario, así que no se puede editar a mano. Para dejar la configuración
**reproducible** hay cuatro herramientas en el menú **Tools** de Unity. Todas son *idempotentes*: se pueden ejecutar
varias veces sin duplicar nada.

> Ejecútalas con el editor **fuera del modo Play** y revisa la pestaña **Console** después.

| Menú | Archivo | Qué configura |
|---|---|---|
| **Tools › Aplicar Arcen S08-2** | `AplicarArcenS08.cs` | Agrega `ArcenCarreteras` a `ArcenR` y `ArcenD` del prefab `Calle0`, pone el tag `Coche` al carro, lo deja con collider sin Trigger y Rigidbody2D *Kinematic*, y vincula `MotorCarreteras` y `Carro` en las 6 calles. |
| **Tools › Aplicar Colisiones y Esfera** | `AplicarColisiones.cs` | Genera el sprite `Esfera.png`, crea el objeto `Esfera` dentro de `Calle0` y agrega sonido de choque a `CarroMalo`, `PowerUp`, `QuitaVida` y `Esfera`. |
| **Tools › Aplicar Vidas** | `AplicarVidas.cs` | Conecta `Vidas` y `PopGameOver`, convierte `CarroMalo`, `QuitaVida`, `Esfera` y los 14 edificios en obstáculos que quitan vida, y agrega la escena a *Build Settings*. |
| **Tools › Aplicar Menu de Inicio** | `AplicarMenu.cs` | Construye el menú de inicio (fondo, controles, botón y nombre) y pone el botón de Game Over en español. |

## Orden recomendado

Si partes de una escena limpia:

1. `Aplicar Arcen S08-2`
2. `Aplicar Colisiones y Esfera`
3. `Aplicar Vidas`
4. `Aplicar Menu de Inicio`

`Aplicar Vidas` necesita que exista la `Esfera` (paso 2) y `Aplicar Menu de Inicio` no funciona sin `CuentaAtras`
ya esperando al menú.

## Qué hacer si algo falla

- Un aviso `[Menu]`, `[Vidas]`, `[Arcen]` o `[Colisiones]` en la Console indica qué objeto no se encontró.
- Si el juego se queda quieto en el `1:30` al dar Play, falta ejecutar **Aplicar Menu de Inicio**: sin menú nadie
  lanza la cuenta atrás.
