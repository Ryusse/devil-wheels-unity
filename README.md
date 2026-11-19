# Devil Wheels — juego de carreras 2D en Unity

Juego de carreras en vista cenital (2D) hecho con **Unity**. Conduces un carro por una carretera infinita,
esquivas obstáculos, cuidas tus **3 vidas** y corres contra un cronómetro mientras sumas metros recorridos.

> Proyecto académico del curso de **Mecánicas de Juego** (guías de la Semana S08).
> **Autor:** Joel Ocaño

---

## Cómo se juega

1. En el **menú de inicio** pulsa **INICIAR JUEGO** (o `ENTER` / `ESPACIO`).
2. Aparece la cuenta atrás **3 · 2 · 1 · GO** y empieza la carrera.
3. Mueve el carro de lado a lado y esquiva los obstáculos.
4. Si te quedas sin vidas, aparece **Game Over** con los metros que recorriste. Pulsa **REINICIAR** para volver a jugar.

### Controles

| Acción | Teclas |
|---|---|
| Mover a la izquierda | `←` o `A` |
| Mover a la derecha | `→` o `D` |
| Empezar desde el menú | `ENTER`, `ESPACIO` o clic en el botón |

### Reglas

- Empiezas con **3 vidas** (los 3 carritos arriba a la izquierda).
- **Cada choque quita una vida**: contra otros carros, la esfera, el obstáculo de la botella y los edificios.
  Tras el choque el juego **continúa donde estabas**: el carro se coloca en el carril libre más cercano al lado del
  obstáculo y queda protegido 2 segundos (parpadea).
- Con **0 vidas** termina la partida.
- El **arcén** (franja gris clara a los lados de la calzada) reduce la velocidad de la carretera y cambia el sonido del motor.
- El panel superior derecho muestra el **tiempo** y los **metros** recorridos.
- Cada choque suena (`ChoqueCoche`) y la pérdida de vida tiene su propio sonido.

Más detalle en [docs/MECANICAS.md](docs/MECANICAS.md).

---

## Requisitos y cómo abrirlo

- **Unity 6** — versión usada: `6000.6.9f1` (ver `ProjectSettings/ProjectVersion.txt`).
- El proyecto nació en **Unity 5.6.2f1** (2018) y fue migrado a Unity 6.
- Paquete `Unity UI` (`com.unity.ugui`) — viene en `Packages/manifest.json`.

Pasos:

1. Clona el repositorio.
2. En **Unity Hub** → *Add* → *Add project from disk* y elige la carpeta del repositorio.
3. Ábrelo con Unity 6 (`6000.x`). La primera vez tardará en importar.
4. Abre la escena `Assets/Escena/main.unity` y pulsa **Play**.

> La primera apertura en Unity 6 muestra avisos amarillos del tipo *"Serialized files [version 17] before 2019.1 are
> deprecated"*. Son inofensivos: vienen de archivos de configuración del proyecto original.

---

## Estructura del proyecto

```
Assets/
├── Escena/main.unity        Escena única del juego (carretera, HUD, menú y Game Over)
├── Prefabs/Calle0.prefab    Tramo de calle: asfalto, arcenes, edificios y obstáculos
├── Script/                  Código del juego (C#)
├── Editor/                  Herramientas de editor que configuran la escena (menú Tools)
├── Sonidos/                 Música y efectos
├── Texturas/                Sprites: calles, edificios, obstáculos, interfaz y título
└── MobileDependencyResolver/ Resto del proyecto original (no se usa en el juego)
docs/                        Documentación adicional
```

## Documentación

| Documento | Contenido |
|---|---|
| [docs/MECANICAS.md](docs/MECANICAS.md) | Mecánicas del juego explicadas con detalle |
| [docs/SCRIPTS.md](docs/SCRIPTS.md) | Qué hace cada script y cómo se conectan |
| [docs/HERRAMIENTAS_EDITOR.md](docs/HERRAMIENTAS_EDITOR.md) | Menús `Tools` que configuran la escena |
| [docs/ESTADO.md](docs/ESTADO.md) | Qué está hecho, qué falta y decisiones tomadas |

## Créditos

- **Autor / desarrollo:** Joel Ocaño.
- **Guías y scripts base:** material de clase del curso *Mecánicas de Juego* (Semana S08), UTP.
- **Arte y sonidos:** recursos proporcionados con el material del curso, incluidos aquí solo con fines educativos.
  Si vas a reutilizarlos, confirma antes sus condiciones de uso.
