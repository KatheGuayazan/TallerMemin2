# MemoEngineer - Documentación del Proyecto

## 📋 Descripción General

**MemoEngineer** es un juego interactivo desarrollado en Unity que integra un sistema completo de analíticas en Firebase Firestore. El juego combina mecánicas de recolección y esquive con un sistema de puntuación dinámico. Los datos de cada sesión se envían a Firebase al finalizar el juego, permitiendo análisis profundos del comportamiento del jugador y visualización en un dashboard web.

**Duración estimada de sesión:** 2-5 minutos  
**Plataforma:** Unity (PC/Editor)  
**Backend:** Firebase Firestore  
**Dashboard:** Aplicación web (Vue.js/React - a desarrollar)

---

## 🎮 Flujo del Juego

### 1. **Pantalla de Inicio**
- El jugador ingresa su nombre o alias
- Se crea una nueva sesión en Firestore con estado inicial
- Se genera automáticamente un ID de sesión único

### 2. **Pantalla de Instrucciones**
- Se muestra cómo jugar (recolectar Arduinos, esquivar Tóxicos)
- Se registra el tiempo total que el jugador tarda en leer las instrucciones
- Métrica de comportamiento no trivial: *Tiempo de lectura de instrucciones*

### 3. **Gameplay**
- El jugador controla un personaje que se mueve horizontalmente
- **Mecánica 1: Recolectar Arduinos (objetos verdes)**
  - +1 punto por cada Arduino recolectado
  - Objetivo: acumular la mayor cantidad posible
  
- **Mecánica 2: Esquivar Tóxicos (objetos rojos)**
  - +3 puntos por cada Tóxico esquivado
  - Mecánica de evasión que premia la precisión
  
- **Mecánica 3: Evitar Perder Arduinos (objetos oscuros)**
  - -1 punto si se recoge accidentalmente
  - Penalización por falta de precisión

- **Recolección silenciosa de analíticas:** Todos los eventos se registran en tiempo real sin interferir con la experiencia de juego

### 4. **Pantalla de Resultados**
- Muestra el puntaje final del jugador
- Posición en el ranking global (top 10)
- Desglose visual: Arduinos recogidos, Tóxicos esquivados, Arduinos perdidos
- **Acción final:** Se guardan todos los datos en Firebase al presionar "Finalizar"

---

## 🗄️ Requisitos Técnicos - Firebase

### Base de Datos

**Plataforma:** Firebase Firestore  
**Colección Principal:** `Sections` (sesiones de juego)

#### Estructura de Colección `Sections`

```
Sections/
├── {sessionId}
│   ├── Nombre (string): Alias del jugador
│   ├── HoraCreacion (Timestamp): Fecha/hora de creación del documento
│   ├── Comportamiento (map)
│   │   └── TiempoInstruccionesSeg (float): Segundos totales visualizando instrucciones
│   └── Estadistica (array)
│       └── [0]
│           ├── ArduinosRecolectados (int)
│           ├── ArduinosPerdidos (int)
│           ├── ToxicosEsquivados (int)
│           ├── HoraInicio (Timestamp)
│           └── HoraFinal (Timestamp)
```

#### Colección `highscores` (derivada de Sections)

Esta colección se alimenta mediante una función Cloud Function que procesa los datos al finalizar cada sesión:

```
highscores/
├── {playerId}
│   ├── Nombre (string): Nombre del jugador
│   ├── PuntajeFinal (int): Puntaje total (calidad máxima)
│   ├── FechaRegistro (Timestamp)
│   └── SessionId (string): Referencia al documento en Sections
```

---

## 📊 Esquema de Analíticas Detallado

### Justificación y Propósito de cada Métrica

#### **Métricas de Sesión (Requeridas)**

| Campo | Tipo | Ejemplo | Justificación |
|-------|------|---------|---------------|
| `Identificador Sesión` | String (ID de documento) | `a7f8k3n2m1p0` | Correlaciona todos los eventos de una partida y permite rastreo único |
| `Nombre/Alias Jugador` | String | `"Juan"` | Identificación del jugador para el ranking y análisis personal |
| `Fecha/Hora Inicio` | Timestamp | `2026-05-16T14:30:00Z` | Permite análisis de patrones temporales (horarios de juego) |
| `Duración Total Sesión` | Segundos (calculado) | `180` | Métrica de engagement; sesiones más largas = mayor interés |
| `Puntaje Final` | Integer | `250` | Métrica principal de rendimiento del jugador |

#### **Métricas Específicas de la Mecánica (al menos 4)**

1. **ArduinosRecolectados** (int)
   - **Ejemplo:** `15`
   - **Justificación:** Mide la capacidad del jugador para identificar y recolectar objetos objetivo. Correlaciona directamente con precisión visual y velocidad de reacción.
   - **Análisis:** Un jugador promedio recolecta 10-20, expertos 25+

2. **ToxicosEsquivados** (int)
   - **Ejemplo:** `12`
   - **Justificación:** Mide habilidad defensiva y anticipación. Los jugadores deben predecir trayectorias de objetos malos. Recibe multiplicador de 3x en puntos por dificultad.
   - **Análisis:** Requiere mayor skill que recolectar; indica experiencia con juegos de esquive

3. **ArduinosPerdidos** (int)
   - **Ejemplo:** `2`
   - **Justificación:** Indicador negativo de precisión. Mide cuántas veces el jugador cometió errores recogiendo objetos incorrectos. Útil para identificar problemas de UX/claridad visual.
   - **Análisis:** Jugadores expertos: 0-1, novatos: 3+

4. **TasaAciertoPorMinuto** (calculado: `(ArduinosRecolectados + ToxicosEsquivados) / DuracionEnMinutos`)
   - **Ejemplo:** `(15 + 12) / 3 = 9 aciertos/minuto`
   - **Justificación:** Normaliza el desempeño ajustando por duración. Permite comparar jugadores que juegan diferentes tiempos.
   - **Análisis:** Métrica de intensidad; 5-15 aciertos/min es promedio

5. **PrecisionDeRecoleccion** (calculado: `ArduinosRecolectados / (ArduinosRecolectados + ArduinosPerdidos) * 100%`)
   - **Ejemplo:** `15 / 17 * 100 = 88.2%`
   - **Justificación:** Mide exactitud en decisiones rápidas. Separar aciertos de errores ayuda a identificar problemas de discriminación visual o fatiga.
   - **Análisis:** Expertos: >90%, novatos: 70-85%

#### **Métrica de Comportamiento No Trivial**

**TiempoInstruccionesSeg** (float)
- **Ejemplo:** `45.5` segundos
- **Justificación:** Mide cuánto tiempo el jugador dedica a leer/comprender las instrucciones antes de jugar
- **Análisis Completo:**
  - **< 10 segundos:** Jugador experimentado o descuidado. Predice puntuaciones altas O muchos errores
  - **10-30 segundos:** Comportamiento típico. Dedicación promedio a entender reglas
  - **> 60 segundos:** Indica dificultad cognitiva, estrés, o revisiones múltiples
  - **Correlación posible:** Jugadores que invierten más tiempo en instrucciones suelen tener mejor precisión pero menor velocidad
  - **Uso:** Identificar si el tutorial es confuso (muchos regresos) o si hay perfiles de jugadores con diferentes estrategias de aprendizaje

---

## 🏗️ Arquitectura del Sistema

### Diagrama de Flujo de Datos

```
┌─────────────┐         ┌──────────────┐         ┌─────────────┐
│   Unity     │         │   Firebase   │         │  Dashboard  │
│  (Gameplay) │────────▶│  (Firestore) │◀────────│    (Web)    │
└─────────────┘         └──────────────┘         └─────────────┘
       │                       │                         │
       │ Eventos de juego      │ Documento guardado      │ Lectura
       │ (Real-time)           │ (Al finalizar)          │ (REST API)
       │                       │                         │
       ▼                       ▼                         ▼
  - Recolectar            Almacenamiento           Visualización
  - Esquivar              Persistente               - Ranking
  - Errores               del jugador               - Gráficas
```

### Componentes Principales

#### **Unity (Frontend del Juego)**

- **SessionCreator:** Genera nueva sesión y envía nombre del jugador
- **ScoreUI:** Actualiza visualización de puntuación en tiempo real
- **InstructionsAnalytics:** Rastrea tiempo de lectura de instrucciones
- **ScoreToFirestoreEvents:** Convierte eventos de juego a eventos de Firebase
- **PlayerMovement2D:** Control de personaje (recolección y esquive)

#### **Firebase (Backend de Datos)**

- **Firestore:** Base de datos NoSQL con colecciones estructuradas
- **Cloud Functions (opcional pero recomendado):**
  - Procesar datos al finalizar sesión
  - Generar documentos resumen
  - Enviar notificaciones
  - Limpiar datos expirados

#### **Dashboard Web (A Desarrollar)**

- Vue.js/React con Firebase Realtime Connection
- Consultas a Firestore en tiempo real
- Visualización de datos con Chart.js o similar

---

## 🎯 Especificaciones del Dashboard Web

### Requisitos Mínimos

El dashboard debe ser una **interfaz web completamente separada del juego** que se conecte a Firestore mediante API REST o SDK de Firebase para web.

#### **1. Tabla de Ranking (Highscores)**

```
┌─────────┬──────────┬──────────┐
│ Posición│  Nombre  │  Puntaje │
├─────────┼──────────┼──────────┤
│    1    │   Juan   │   320    │
│    2    │  María   │   280    │
│    3    │  Carlos  │   210    │
│   ...   │   ...    │   ...    │
└─────────┴──────────┴──────────┘
```

**Características:**
- Listado de top 10-20 jugadores ordenados por puntaje descendente
- Actualización automática cada 10 segundos
- Mostrar fecha de última actualización
- Opción de exportar a CSV

#### **2. Gráfica de Distribución de Puntajes**

```
Cantidad de Jugadores
          │
        8 │     ██
        7 │    ███
        6 │    ███
        5 │   █████
        4 │   █████
        3 │  ███████
        2 │ █████████
        1 │ █████████
        0 └─────────────────────────
          0-50  50-100  100-150  150+
                    Rango de Puntaje
```

**Características:**
- Histograma de rango de puntajes
- Mostrar distribución de habilidades
- Indicador de promedio global
- Filtro por fecha (últimas 24h, 7 días, todo)

#### **3. Visualización 1: Precisión de Recoleccion vs Velocidad**

```
Scatter Plot: Todos los jugadores
     Velocidad (aciertos/min)
            │
        15  │              ●
        12  │          ●       ●
         9  │      ●       ●
         6  │  ●           ●
         3  │
         0  └─────────────────────
            70%  80%  90%  100%
          Precisión de Recolección
```

**Características:**
- Cada punto = una sesión
- X: Precisión (0-100%)
- Y: Velocidad (aciertos/min)
- Colores por rango de puntaje
- Hover: mostrar nombre del jugador y puntaje

#### **4. Visualización 2: Impacto de Instrucciones en Rendimiento**

```
Gráfica de Línea: Correlación Tiempo-Instrucciones vs Puntaje
    Puntaje
        │
    300 │              ╱╲
        │            ╱    ╲
    200 │          ╱        ╲
        │        ╱            ╲
    100 │      ╱                ╲
        │    ╱                    ╲
      0 └────────────────────────────
        0s  15s  30s  45s  60s  90s
          Tiempo Instrucciones (seg)
```

**Características:**
- Mostrar si hay correlación entre tiempo de lectura y rendimiento
- Línea de tendencia
- Detectar "punto dulce" de tiempo óptimo
- Identificar jugadores que gastan demasiado tiempo

#### **5. Panel de Estadísticas Globales**

```
┌────────────────────┬────────────────────┐
│ Total de Sesiones  │   Puntaje Promedio │
│       145          │       187.4        │
├────────────────────┼────────────────────┤
│ Jugadores Únicos   │  Mayor Puntaje     │
│       23           │   520 (Juan)       │
└────────────────────┴────────────────────┘
```

### Funcionalidades Adicionales

- **Filtros:**
  - Por fecha (hoy, últimos 7 días, mes, todo)
  - Por rango de puntaje
  - Por nombre del jugador (búsqueda)

- **Actualizaciones:** Datos en tiempo real (WebSocket o polling cada 10 seg)

- **Responsive:** Funciona en desktop, tablet y mobile

---

## 💾 Guardado de Datos

### Política de Escritura en Firebase

✅ **Se escribe al finalizar la sesión:**
- Documento completo de `Sections` con todos los datos acumulados
- Todos los eventos de recolección/esquive se consolidan en estadística final

❌ **NO se escribe en tiempo real durante el juego:**
- Evita latencia y costos innecesarios
- Reduces load de base de datos
- Mejora fluidez del juego

### Datos Almacenados

```json
{
  "docId": "a7f8k3n2m1p0",
  "Nombre": "Juan",
  "Comportamiento": {
    "TiempoInstruccionesSeg": 35.5
  },
  "Estadistica": [
    {
      "ArduinosRecolectados": 15,
      "ArduinosPerdidos": 2,
      "ToxicosEsquivados": 12,
      "HoraInicio": "2026-05-16T14:30:00Z",
      "HoraFinal": "2026-05-16T14:33:22Z"
    }
  ]
}
```

---

## 🔧 Implementación Técnica

### Scripts de Analíticas (Unity)

| Script | Responsabilidad |
|--------|-----------------|
| `Firestore.cs` | Operaciones CRUD a Firestore |
| `SessionCreator.cs` | Inicializa sesión con nombre del jugador |
| `InstructionsAnalytics.cs` | Rastrea tiempo en pantalla de instrucciones |
| `ScoreToFirestoreEvents.cs` | Convierte eventos de juego a datos de Firebase |
| `ScoreUI.cs` | Actualiza UI con puntuación real |
| `FirestoreEvents.cs` | Sistema de eventos para comunicación |
| `FirestoreEventHandler.cs` | Gestiona eventos y llamadas async a Firestore |
| `PlayerMovement2D.cs` | Control del jugador (recolección/esquive) |

### Eventos Clave

```csharp
// Al recolectar Arduino
ScoreEvents.OnGoodCollected?.Invoke();

// Al esquivar Tóxico
ScoreEvents.OnToxicDodged?.Invoke();

// Al recoger accidentalmente objeto malo
ScoreEvents.OnBadCollected?.Invoke();

// Al finalizar la partida
ScoreEvents.FinishGame?.Invoke();

// Para actualizar tiempo de instrucciones
FirestoreEvents.OnUpdateInstructionTime?.Invoke(totalSeconds);

// Para guardar estadísticas
FirestoreEvents.OnSaveStatistics?.Invoke(
  recolectados, 
  perdidos, 
  esquivados, 
  horaInicio, 
  horaFinal
);
```

---

## 📹 Entregables Finales

### Video de Funcionamiento (REQUERIDO)

- **Duración:** 3 minutos
- **Contenido obligatorio:**
  1. Pantalla de inicio (ingreso de nombre)
  2. Pantalla de instrucciones (muestra timer)
  3. Gameplay (recolectar, esquivar, errores)
  4. Pantalla de resultados (puntaje y ranking)
  5. Dashboard web mostrando:
     - Ranking actualizado
     - Gráficas de distribución
     - Datos en tiempo real

### Documentación Entregada

- ✅ Este README.md con especificaciones completas
- ✅ Justificación de cada métrica de analítica
- ✅ Esquema de Firestore documentado
- ✅ Arquitectura del sistema

---

## 🚀 Próximos Pasos

1. **Verificar integración Firebase:** Confirmar que datos se guardan correctamente
2. **Desarrollar Dashboard Web:** Crear interfaz con Vue.js o React
3. **Optimizar Cloud Functions:** Procesar datos al finalizar sesión
4. **Testing:** Verificar recolección de datos en múltiples sesiones
5. **Video de demostración:** Grabar funcionamiento completo del sistema
6. **Ajustes finales:** Pulir UI/UX basado en feedback

---

## 📝 Notas Adicionales

- Todos los tiempos se almacenan en UTC (Firestore Timestamp)
- Los IDs de sesión se generan automáticamente por Firestore
- La colección `highscores` se usa para ranking optimizado (lectura frecuente)
- El dashboard debe actualizar máximo cada 10 segundos para evitar exceso de lecturas
- Implementar manejo de errores de red (desconexión de Firebase)
