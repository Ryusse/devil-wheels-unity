# Publicación en la web

El juego se compila a **WebGL** desde Unity y se despliega como sitio estático en **Vercel**.

**Versión en línea:** https://devil-wheels.vercel.app

---

## 1. Requisitos

- Unity `6000.6.9f1` con el módulo **Web Build Support** instalado
  (Unity Hub → *Installs* → ⚙ → *Add modules* → *Web Build Support*, ~1 GB de descarga).
- Cuenta en Vercel y Node.js (para usar la CLI de Vercel).

## 2. Compilar para WebGL

1. Abre el proyecto y entra en **File → Build Profiles**.
2. Elige la plataforma **Web** y pulsa **Switch Platform** (reimporta los assets, tarda unos minutos).
3. Abre **Player Settings → Publishing Settings** y pon **Compression Format = Disabled**.
   Así el sitio funciona en cualquier hosting estático sin configurar cabeceras.
4. Comprueba que `Assets/Escena/main` esté marcada en *Scenes in Build*.
5. Pulsa **Build** y elige una carpeta nueva, por ejemplo `build/` (ya está en `.gitignore`).

Resultado: `index.html`, `TemplateData/` y `Build/` (`build.data`, `build.wasm`, `build.framework.js`, `build.loader.js`).
Pesa unos 41 MB sin comprimir.

> **Probar en local:** abrir `index.html` con doble clic no funciona (el navegador bloquea los archivos locales).
> Usa *Build And Run* o, desde la carpeta de la build, `python -m http.server 8000` y abre `http://localhost:8000`.

## 3. Problemas conocidos

### La build falla con `0x800711C7` («Una directiva de Control de aplicaciones bloqueó este archivo»)

En Windows 11, **Smart App Control** bloquea las DLL sin firma digital de `BuildPipeline` de Unity, así que la compilación
termina con `Build completed with a result of 'Failed'`.

- Solución: desactivar *Smart App Control* en **Seguridad de Windows → Control de aplicaciones y navegador →
  Configuración de Smart App Control**.
- Aviso: una vez desactivado **no se puede volver a activar** sin reinstalar o restablecer Windows.
- Alternativa: compilar en otro equipo.

### El audio no suena al cargar

Los navegadores solo permiten audio después de una interacción del usuario. En este juego ya se cumple: el sonido
empieza al pulsar **INICIAR JUEGO**.

## 4. Desplegar en Vercel

Con la carpeta de la build lista:

```bash
cd build
npx vercel@latest login          # una sola vez: autoriza el dispositivo en el navegador
npx vercel@latest deploy --prod --yes
```

La primera vez crea el proyecto (toma el nombre de la carpeta) y devuelve la URL de producción.
Para actualizar el juego: recompilar en Unity y repetir `deploy --prod`.

Notas:

- **Vercel Drop** (`vercel.com/drop`) también sirve para sitios estáticos (acepta carpeta o `.zip`), pero es más
  incómodo con archivos grandes: `build.wasm` pesa unos 26 MB.
- Vercel sirve el `.wasm` con el tipo MIME correcto y lo comprime con Brotli, por lo que en línea la descarga baja
  de 41 MB a unos 18 MB.
- Comparte siempre el dominio de producción (`devil-wheels.vercel.app`). Las URL internas de cada despliegue
  pueden pedir inicio de sesión en Vercel.
- Para cambiar el título de la pestaña, edita `<title>` en el `index.html` generado antes de desplegar.
  Si se regenera la build, hay que repetirlo.

## 5. Alternativas de hosting

| Opción | Nota |
|---|---|
| GitHub Pages | Gratis; el repositorio debe ser público. Publica el contenido de la build en una rama `gh-pages`. |
| itch.io | Pensado para juegos: sube la build en `.zip` marcando *This file will be played in the browser*. |
