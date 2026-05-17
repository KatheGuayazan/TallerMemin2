# Checklist de Implementación - MemoEngineer

## ✅ Checklist Completo del Proyecto

Este documento es una guía paso a paso para verificar que todos los requisitos estén implementados correctamente antes de grabar el video de demostración (3 minutos).

---

## 🎮 Parte 1: Juego Unity (MemoEngineer)

### Funcionalidad Base

- [ ] **Pantalla de Inicio**
  - [ ] Campo de texto para ingresar nombre del jugador
  - [ ] Botón "Comenzar" que inicia la sesión
  - [ ] Validación: Si está vacío, usar "Player" como nombre
  - [ ] Se genera ID de sesión único en Firestore

- [ ] **Pantalla de Instrucciones**
  - [ ] Texto claro explicando las mecánicas
  - [ ] Botón "Entendido" para continuar
  - [ ] Cronómetro registrando tiempo de visualización
  - [ ] Tiempo se guarda en `Comportamiento.TiempoInstruccionesSeg`

- [ ] **Gameplay**
  - [ ] Jugador se mueve horizontalmente con teclado/joystick
  - [ ] Límites visuales en pantalla (Gizmos verdes)
  - [ ] Aparecen objetos que caen (Arduino, Tóxicos, Malos)
  - [ ] Recolectar Arduino = +1 punto visible en UI
  - [ ] Esquivar Tóxico = +3 puntos visible en UI
  - [ ] Recoger objeto malo = -1 punto visible en UI
  - [ ] UI actualiza en tiempo real (sin lag)

- [ ] **Pantalla de Resultados**
  - [ ] Muestra puntaje final
  - [ ] Desglose: Arduinos recogidos, Tóxicos esquivados, Arduinos perdidos
  - [ ] Muestra ranking top 5-10 desde Firebase
  - [ ] Posición del jugador actual en ranking
  - [ ] Botón "Finalizar" que guarda datos en Firestore

### Sistema de Analíticas (Firebase)

- [ ] **Recolección de Datos**
  - [ ] ArduinosRecolectados se cuenta correctamente
  - [ ] ToxicosEsquivados se cuenta correctamente (×3)
  - [ ] ArduinosPerdidos se cuenta correctamente (-1)
  - [ ] HoraInicio se registra con Timestamp correcto
  - [ ] HoraFinal se registra con Timestamp correcto
  - [ ] TiempoInstruccionesSeg se calcula correctamente

- [ ] **Guardado en Firestore**
  - [ ] Documento en Sections/{sessionId} existe después de jugar
  - [ ] Campo "Nombre" contiene el nombre del jugador
  - [ ] Array "Estadistica" contiene entrada con todos los campos
  - [ ] Campo "Comportamiento.TiempoInstruccionesSeg" tiene valor correcto
  - [ ] NO hay escrituras durante el gameplay (solo al finalizar)

- [ ] **Estructura de Datos**
  ```json
  Sections/{sessionId} = {
    "Nombre": "Juan",
    "HoraCreacion": Timestamp,
    "Comportamiento": {
      "TiempoInstruccionesSeg": 35.5
    },
    "Estadistica": [{
      "ArduinosRecolectados": 15,
      "ArduinosPerdidos": 2,
      "ToxicosEsquivados": 12,
      "HoraInicio": Timestamp,
      "HoraFinal": Timestamp
    }]
  }
  ```

### Validación Técnica

- [ ] Todos los scripts están en `Assets/MemoEngineer/Scripts/`
- [ ] No hay errores en la consola de Unity durante juego
- [ ] Conexión a Firebase funciona (verificar logs)
- [ ] IDs de sesión se generan automáticamente
- [ ] Eventos de juego se disparan correctamente
- [ ] Los datos se guardan después de 5-10 segundos del fin de partida

---

## 🌐 Parte 2: Dashboard Web

### Configuración Inicial

- [ ] Proyecto web creado (Vue.js o React)
- [ ] Firebase SDK configurado correctamente
- [ ] Variables de entorno (.env) configuradas
- [ ] `FirestoreService.js` implementado completamente
- [ ] Conexión a Firestore probada (sin errores de autenticación)

### Componentes Principales

- [ ] **Ranking (Highscores)**
  - [ ] Tabla muestra top 20 jugadores
  - [ ] Ordenada por puntaje descendente (mayor primero)
  - [ ] Muestra: Posición, Nombre, Puntaje, Fecha
  - [ ] Medallas (🥇🥈🥉) para top 3
  - [ ] Botón "Actualizar" funciona
  - [ ] Se actualiza automáticamente cada 30 seg
  - [ ] Responde correctamente en mobile

- [ ] **Distribución de Puntajes**
  - [ ] Histograma muestra 5 rangos (0-50, 50-100, 100-150, 150-200, 200+)
  - [ ] Barras tienen colores diferentes
  - [ ] Cuenta correctamente sesiones por rango
  - [ ] Incluye tarjetas de promedio y máximo
  - [ ] Responsive en todos los tamaños

- [ ] **Precisión vs Velocidad**
  - [ ] Scatter plot con puntos para cada sesión
  - [ ] Eje X: Precisión (0-100%)
  - [ ] Eje Y: Velocidad (aciertos/minuto)
  - [ ] Hover muestra nombre del jugador y puntaje
  - [ ] Colores indican rango de puntaje
  - [ ] Actualiza cuando hay nuevas sesiones

- [ ] **Impacto de Instrucciones**
  - [ ] Línea/scatter plot mostrando correlación
  - [ ] Eje X: Tiempo de instrucciones (segundos)
  - [ ] Eje Y: Puntaje final
  - [ ] Identifica punto óptimo de tiempo
  - [ ] Muestra si relación es positiva/negativa/neutra

- [ ] **Estadísticas Globales**
  - [ ] Total de sesiones (actualiza en tiempo real)
  - [ ] Jugadores únicos
  - [ ] Puntaje promedio
  - [ ] Puntaje más alto
  - [ ] Nombre del mejor jugador

### Funcionalidades

- [ ] Datos se actualizan cada 30 segundos sin refrescar página
- [ ] Funciona correctamente en desktop (1920x1080)
- [ ] Funciona correctamente en tablet (768x1024)
- [ ] Funciona correctamente en mobile (375x667)
- [ ] Sin errores de console (F12)
- [ ] Manejo de errores si Firebase no responde
- [ ] Loading states mientras se cargan datos
- [ ] Formato de fechas correcto (ej: "Hace 2 minutos")

### Estilos

- [ ] Interfaz limpia y profesional
- [ ] Colores consistentes (tema claro u oscuro)
- [ ] Tipografía legible
- [ ] Espaciado adecuado
- [ ] Botones con hover effects
- [ ] Gráficas bien dimensionadas
- [ ] Sin elementos rotos o desalineados

---

## 📹 Parte 3: Preparación del Video (3 Minutos)

### Requisitos del Video

**IMPORTANTE:** Si no se entrega el video, todos los items de funcionamiento valen 0.

#### Estructura del Video (Recomendado)

```
0:00 - 0:30   → Pantalla de inicio + Ingreso de nombre
0:30 - 1:00   → Pantalla de instrucciones (mostrar timer)
1:00 - 1:45   → Gameplay (recolectar, esquivar, errores)
1:45 - 2:15   → Pantalla de resultados
2:15 - 2:45   → Dashboard web mostrando datos nuevos
2:45 - 3:00   → Resumen y cierre
```

### Preparación Técnica

- [ ] PC conectado a buen internet
- [ ] Cámara/grabador en HD (mínimo 1280x720)
- [ ] Audio claro sin ruido de fondo
- [ ] Micrófono funcional (o subtítulos)
- [ ] Herramienta de grabación (OBS, Screenity, etc.)
- [ ] Script escrito para narración (opcional pero recomendado)

### Contenido Obligatorio en Video

#### Escena 1: Inicio
- [ ] Mostrar cómo ingresar nombre
- [ ] Presionar "Comenzar"
- [ ] Narrar: "Sistema crea sesión en Firebase"
- [ ] Mostrar ID de sesión en consola (opcional)

#### Escena 2: Instrucciones
- [ ] Mostrar pantalla de instrucciones
- [ ] Aguantar ~30 segundos leyendo (para demostrar timer)
- [ ] Narrar: "Se registra tiempo de lectura"
- [ ] Presionar "Entendido"

#### Escena 3: Gameplay
- [ ] Demostrar recolectar Arduinos (mínimo 5) ✅
- [ ] Demostrar esquivar Tóxicos (mínimo 3) ✅
- [ ] Cometer al menos 1 error (recoger objeto malo) ❌
- [ ] Mostrar puntuación en tiempo real
- [ ] Narrar cada acción
- [ ] Jugar durante ~45 segundos

#### Escena 4: Resultados
- [ ] Mostrar puntaje final
- [ ] Mostrar desglose (Arduinos, Tóxicos, Perdidos)
- [ ] Mostrar ranking top 5 desde Firebase
- [ ] Presionar "Guardar"/"Finalizar"
- [ ] Narrar: "Datos guardados en Firestore"

#### Escena 5: Dashboard Web
- [ ] Abrir dashboard en navegador
- [ ] Mostrar ranking con el nuevo jugador
- [ ] Mostrar gráfica de distribución
- [ ] Mostrar gráfica precisión vs velocidad (nuevo punto)
- [ ] Mostrar gráfica de instrucciones
- [ ] Mostrar estadísticas globales actualizadas
- [ ] Narrar: "Datos en tiempo real desde Firestore"

### Calidad de Grabación

- [ ] Resolución mínimo 1280x720
- [ ] FPS: 30 o superior
- [ ] Audio: Claro, sin ruido de fondo
- [ ] Velocidad de reproducción: 1x (no acelerado)
- [ ] Sin cortes abruptos entre escenas
- [ ] Narración clara y profesional

### Edición (Opcional pero Recomendado)

- [ ] Agregar títulos de escenas (Inicio, Gameplay, Dashboard)
- [ ] Agregar overlays con métricas importantes
- [ ] Añadir transiciones suaves
- [ ] Sincronizar audio con video
- [ ] Normalizar niveles de audio
- [ ] Exportar en MP4 (H.264, AAC)

---

## 🔍 Parte 4: Verificación de Métricas

Antes de grabar, verificar que estas métricas se calculen correctamente:

### Métricas Base (Se Almacenan)

- [ ] **ArduinosRecolectados**: Contador de +1 por Arduino recogido
  - Verificar: Debe ser > 0 después de una sesión
  
- [ ] **ToxicosEsquivados**: Contador de +3 por Tóxico esquivado
  - Verificar: Multiplicador de 3 se aplica correctamente
  
- [ ] **ArduinosPerdidos**: Contador de -1 por error
  - Verificar: Se resta del puntaje
  
- [ ] **TiempoInstruccionesSeg**: Cronómetro en pantalla instrucciones
  - Verificar: > 0 y corresponde a tiempo real

- [ ] **HoraInicio / HoraFinal**: Timestamps de partida
  - Verificar: HoraFinal > HoraInicio

### Métricas Derivadas (Dashboard Calcula)

- [ ] **Puntaje Final** = Arduinos + (Tóxicos × 3) - Perdidos
  - Ejemplo: 15 + (12 × 3) - 2 = 49
  
- [ ] **Duración** = (HoraFinal - HoraInicio) en segundos
  - Ejemplo: 157 segundos = 2:37
  
- [ ] **Tasa de Aciertos/Min** = (Arduinos + Tóxicos) / (Duración/60)
  - Ejemplo: 27 / 2.6 = 10.3 aciertos/min
  
- [ ] **Precisión** = Arduinos / (Arduinos + Perdidos) × 100%
  - Ejemplo: 15 / 17 × 100 = 88.2%

---

## 📋 Parte 5: Documentación Entregable

- [ ] **README.md** - Documento principal del proyecto
  - [ ] Descripción general
  - [ ] Flujo del juego
  - [ ] Requisitos técnicos Firebase
  - [ ] Esquema de analíticas detallado
  - [ ] Especificaciones del dashboard
  - [ ] Políticas de guardado

- [ ] **TECHNICAL_SPEC.md** - Especificación técnica
  - [ ] Esquema Firestore JSON
  - [ ] Métricas calculadas con fórmulas
  - [ ] Flujo de datos completo
  - [ ] Reglas de seguridad Firestore
  - [ ] Cloud Functions (opcional)

- [ ] **DASHBOARD_GUIDE.md** - Guía del dashboard web
  - [ ] Estructura de proyecto
  - [ ] Configuración inicial
  - [ ] Componentes principales
  - [ ] Integración Firebase
  - [ ] Ejemplos de código

- [ ] **Este Checklist** - Verificación de implementación
  - [ ] Disponible para revisar antes de entregar

---

## 🎯 Puntos Críticos para No Olvidar

### Qué Hace Valer 0 Puntos en Funcionamiento

❌ **NO** entregar video (CANCELATORIA)
❌ **NO** guardar datos en Firebase
❌ **NO** mostrar puntaje en tiempo real durante juego
❌ **NO** actualizar dashboard con datos nuevos
❌ **NO** implementar todas las 4 métricas específicas
❌ **NO** implementar métrica de comportamiento no trivial

### Qué da Puntos Extra

✅ Gráficas adicionales más allá del mínimo
✅ Análisis adicionales en dashboard
✅ Exportar datos a CSV/JSON
✅ Validaciones y manejo de errores robusto
✅ UI/UX excepcional
✅ Video con buena edición y narración
✅ Implementar modo multijugador o competitivo

---

## 🚀 Proceso Final (Últimas 2 Horas Antes de Entregar)

### Cronograma Sugerido

**1.5 horas antes:**
- [ ] Hacer test completo del juego (jugar 3 sesiones)
- [ ] Verificar que datos se guardan en Firestore
- [ ] Probar dashboard carga datos correctamente

**1 hora antes:**
- [ ] Preparar ambiente de grabación
- [ ] Revisar script del video
- [ ] Hacer test de grabación (30 segundos)

**30 min antes:**
- [ ] Grabación final
- [ ] Revisar video (buscar cortes, problemas)
- [ ] Exportar en formato correcto

**15 min antes:**
- [ ] Empacar toda documentación
- [ ] Crear carpeta de entrega con estructura clara
- [ ] Verificar que todo está incluido

**Al Finalizar:**
```
Entrega/
├── README.md
├── TECHNICAL_SPEC.md
├── DASHBOARD_GUIDE.md
├── MemoEngineer_Demo_Video.mp4
└── Documentacion_Completa/
    ├── Requisitos_Tecnicos.pdf
    ├── Esquema_Firebase.pdf
    └── Metricas_Explicadas.pdf
```

---

## 📞 Puntos de Contacto / Troubleshooting

Si algo no funciona:

| Problema | Solución |
|----------|----------|
| Firebase no conecta | Verificar credenciales en .env / Reglas de Firestore |
| Datos no se guardan | Verificar eventos se disparan / Console de Firebase |
| Dashboard vacío | Verificar colección "highscores" existe / Firestore permisos |
| Video no se ve bien | Verificar resolución / Codificador / Velocidad conexión |
| Gráficas sin datos | Esperar 30 seg + Verificar datos en Firestore |

---

## ✨ Resumen Final

Para una entrega exitosa:

1. ✅ **Juego funciona:** Inicio → Instrucciones → Gameplay → Resultados
2. ✅ **Firebase almacena:** Todos los datos de una sesión correctamente
3. ✅ **Dashboard muestra:** Ranking, gráficas, estadísticas en tiempo real
4. ✅ **Video demuestra:** Todo el flujo en 3 minutos
5. ✅ **Documentación:** Explica cada métrica y decisión de diseño

**Estado: Listo para Presentación** ✅

---

*Última actualización: 2026-05-16*
*Versión: 1.0*
