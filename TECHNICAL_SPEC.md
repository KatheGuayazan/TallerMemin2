# Especificación Técnica - MemoEngineer Analytics

## 📋 Índice
1. [Esquema Firestore Completo](#esquema-firestore-completo)
2. [Métricas Calculadas](#métricas-calculadas)
3. [Flujo de Datos](#flujo-de-datos)
4. [Reglas de Seguridad](#reglas-de-seguridad)
5. [Cloud Functions](#cloud-functions)
6. [Integración Firebase Web](#integración-firebase-web)

---

## Esquema Firestore Completo

### Colección: `Sections`

Almacena todas las sesiones de juego con datos completos de analíticas.

#### Estructura JSON Completa

```json
{
  "Sections": {
    "a7f8k3n2m1p0": {
      "Nombre": "Juan",
      "HoraCreacion": Timestamp(2026, 5, 16, 14, 30, 0),
      "Comportamiento": {
        "TiempoInstruccionesSeg": 35.5
      },
      "Estadistica": [
        {
          "ArduinosRecolectados": 15,
          "ArduinosPerdidos": 2,
          "ToxicosEsquivados": 12,
          "HoraInicio": Timestamp(2026, 5, 16, 14, 30, 45),
          "HoraFinal": Timestamp(2026, 5, 16, 14, 33, 22)
        }
      ]
    },
    "b9h2j4l5n6o1": {
      "Nombre": "María",
      "HoraCreacion": Timestamp(2026, 5, 16, 15, 00, 0),
      "Comportamiento": {
        "TiempoInstruccionesSeg": 28.2
      },
      "Estadistica": [
        {
          "ArduinosRecolectados": 22,
          "ArduinosPerdidos": 0,
          "ToxicosEsquivados": 18,
          "HoraInicio": Timestamp(2026, 5, 16, 15, 00, 30),
          "HoraFinal": Timestamp(2026, 5, 16, 15, 04, 15)
        }
      ]
    }
  }
}
```

#### Descripción de Campos

| Campo | Tipo | Requerido | Descripción | Ejemplo |
|-------|------|-----------|-------------|---------|
| **Nombre** | String | Sí | Alias o nombre del jugador | `"Juan"` |
| **HoraCreacion** | Timestamp | Sí | Momento en que se crea el documento | `2026-05-16T14:30:00Z` |
| **Comportamiento.TiempoInstruccionesSeg** | Float | Sí | Segundos totales leyendo instrucciones | `35.5` |
| **Estadistica** | Array | Sí | Array de objetos con datos de sesión | Ver abajo |
| **Estadistica[].ArduinosRecolectados** | Integer | Sí | Cantidad de objetos buenos recolectados | `15` |
| **Estadistica[].ArduinosPerdidos** | Integer | Sí | Cantidad de objetos malos recolectados | `2` |
| **Estadistica[].ToxicosEsquivados** | Integer | Sí | Cantidad de peligros esquivados | `12` |
| **Estadistica[].HoraInicio** | Timestamp | Sí | Momento de inicio de partida | `2026-05-16T14:30:45Z` |
| **Estadistica[].HoraFinal** | Timestamp | Sí | Momento de finalización de partida | `2026-05-16T14:33:22Z` |

### Colección: `highscores` (Derivada)

Se genera mediante Cloud Function al finalizar cada sesión. Optimizada para lecturas frecuentes del ranking.

```json
{
  "highscores": {
    "doc_juan_001": {
      "Nombre": "Juan",
      "PuntajeFinal": 250,
      "FechaRegistro": Timestamp(2026, 5, 16, 14, 33, 22),
      "SessionId": "a7f8k3n2m1p0",
      "ArduinosRecolectados": 15,
      "ArduinosPerdidos": 2,
      "ToxicosEsquivados": 12,
      "DuracionSegundos": 157
    },
    "doc_maria_001": {
      "Nombre": "María",
      "PuntajeFinal": 280,
      "FechaRegistro": Timestamp(2026, 5, 16, 15, 4, 15),
      "SessionId": "b9h2j4l5n6o1",
      "ArduinosRecolectados": 22,
      "ArduinosPerdidos": 0,
      "ToxicosEsquivados": 18,
      "DuracionSegundos": 225
    }
  }
}
```

---

## Métricas Calculadas

### Métricas Base (Almacenadas Directamente)

Estas métricas se recopilan durante el juego y se guardan directamente en Firestore:

| Métrica | Fórmula | Almacenaje | Actualización |
|---------|---------|-----------|-----------------|
| ArduinosRecolectados | Contador directo | Campo en Estadistica | Al finalizar |
| ArduinosPerdidos | Contador directo | Campo en Estadistica | Al finalizar |
| ToxicosEsquivados | Contador directo | Campo en Estadistica | Al finalizar |
| TiempoInstruccionesSeg | Cronómetro en pantalla | Campo en Comportamiento | Al avanzar |
| DuracionSesion | HoraFinal - HoraInicio | Calculada en lectura | En tiempo real |

### Métricas Derivadas (Calculadas en Lectura)

Se calculan cuando se leen datos de Firestore para análisis y dashboard:

#### 1. **Puntaje Final**
```
PuntajeFinal = ArduinosRecolectados 
             + (ToxicosEsquivados × 3) 
             - (ArduinosPerdidos × 1)
```

**Ejemplo:**
```
PuntajeFinal = 15 + (12 × 3) - (2 × 1)
             = 15 + 36 - 2
             = 49
```

**Justificación:**
- Recolectar Arduino = 1 punto (base)
- Esquivar Tóxico = 3 puntos (mayor dificultad)
- Perder Arduino = -1 punto (penalización)

#### 2. **Duración de Sesión (en segundos)**
```
DuracionSegundos = (HoraFinal - HoraInicio) / 1000
```

**Ejemplo:**
```
HoraFinal = 2026-05-16T14:33:22Z
HoraInicio = 2026-05-16T14:30:45Z
DuracionSegundos = 157 segundos (2 minutos 37 segundos)
```

#### 3. **Tasa de Aciertos por Minuto**
```
TasaAciertosMinuto = (ArduinosRecolectados + ToxicosEsquivados) / (DuracionSegundos / 60)
```

**Ejemplo:**
```
TasaAciertos = (15 + 12) / (157 / 60)
             = 27 / 2.616
             = 10.3 aciertos/minuto
```

**Rangos de referencia:**
- **0-5:** Jugador muy lento o principiante
- **5-10:** Promedio
- **10-15:** Rápido/Experimentado
- **15+:** Experto/Velocista

#### 4. **Precisión de Recolección**
```
PrecisionRecoleccion = ArduinosRecolectados / (ArduinosRecolectados + ArduinosPerdidos) × 100
```

**Ejemplo:**
```
Precision = 15 / (15 + 2) × 100
          = 15 / 17 × 100
          = 88.2%
```

**Rangos de referencia:**
- **< 70%:** Jugador distraído o muy rápido
- **70-85%:** Promedio (con algunos errores)
- **85-95%:** Buena precisión
- **> 95%:** Experto/Muy cuidadoso

#### 5. **Índice de Eficiencia**
```
IndiceEficiencia = (PuntajeFinal / DuracionSegundos) × 60
                 = Puntos por minuto
```

**Ejemplo:**
```
Eficiencia = (49 / 157) × 60
           = 0.312 × 60
           = 18.7 puntos/minuto
```

**Uso:** Normaliza el rendimiento eliminando efecto del tiempo jugado.

#### 6. **Correlación Instrucciones-Rendimiento**
```
RendimientoNormalizado = PuntajeFinal / PromedioGlobal
TiempoNormalizado = TiempoInstruccionesSeg / PromedioTiempoInstrucciones
CoeficienteCorrelacion = CORR(RendimientoNormalizado, TiempoNormalizado)
```

**Interpretación:**
- **Correlación > 0.7:** Fuerte relación positiva (más tiempo = mejor desempeño)
- **Correlación 0.3-0.7:** Relación moderada
- **Correlación < 0.1:** Sin relación (tiempo no afecta rendimiento)
- **Correlación negativa:** Relación inversa (más tiempo = peor desempeño)

---

## Flujo de Datos

### 1. Inicialización de Sesión

```
┌─────────────┐
│   Jugador   │
│  Ingresa    │
│   Nombre    │
└──────┬──────┘
       │
       ▼
┌──────────────────────────────┐
│ SessionCreator.CreateSession()│
│ - Valida nombre (default: "Player")
│ - Llama firestore.CreateSection(name)
└──────────┬───────────────────┘
           │
           ▼
┌────────────────────────────────────────┐
│ Firestore.CreateSection(nombreJugador) │
│ - Genera ID único de documento         │
│ - Crea estructura inicial              │
│ - Retorna documentId                   │
└──────────┬─────────────────────────────┘
           │
           ▼
┌─────────────────────────────┐
│ Documento guardado en:      │
│ Sections/{sessionId}        │
└─────────────────────────────┘
```

### 2. Recopilación de Analíticas Durante Gameplay

```
Evento de Juego                  Manejador
───────────────                  ──────────
Jugador recoge Arduino  ──▶ ScoreEvents.OnGoodCollected?.Invoke()
                             ↓
                        ScoreToFirestoreEvents.OnGoodCollected()
                             ↓
                        collectedArduinos++

Jugador esquiva Tóxico  ──▶ ScoreEvents.OnToxicDodged?.Invoke()
                             ↓
                        ScoreToFirestoreEvents.OnToxicDodged()
                             ↓
                        dodgedToxics += 3

Jugador recoge malo     ──▶ ScoreEvents.OnBadCollected?.Invoke()
                             ↓
                        ScoreToFirestoreEvents.OnBadCollected()
                             ↓
                        lostArduinos++

(Todos estos datos se guardan en variables locales, NO en Firebase aún)
```

### 3. Lectura de Instrucciones (Comportamiento)

```
┌──────────────────────────────────┐
│ InstructionsAnalytics            │
│ StartViewingInstructions()        │
│ - startTime = Time.time           │
└────┬─────────────────────────────┘
     │ (Usuario lee instrucciones por ~35 seg)
     ▼
┌──────────────────────────────────┐
│ InstructionsAnalytics            │
│ StopViewingInstructions()         │
│ - duration = Time.time - startTime│
│ - totalTime += duration           │
│ - Invoca FirestoreEvents          │
└────┬─────────────────────────────┘
     │
     ▼
┌────────────────────────────────┐
│ FirestoreEventHandler          │
│ OnUpdateInstructionTime(35.5)   │
│ - Llama updateTiempoInstrucciones
└────┬─────────────────────────────┘
     │
     ▼
┌─────────────────────────────────────┐
│ Firestore.UpdateTiempoInstrucciones │
│ - Updates campo Comportamiento      │
└─────────────────────────────────────┘
```

### 4. Guardado Final de Estadísticas

```
┌──────────────────────────┐
│ Pantalla de Resultados   │
│ Jugador ve puntuación    │
│ Presiona "Guardar"       │
└────┬─────────────────────┘
     │
     ▼
┌─────────────────────────┐
│ ScoreEvents.FinishGame  │
│ Invoca evento           │
└────┬────────────────────┘
     │
     ▼
┌──────────────────────────────────────┐
│ ScoreToFirestoreEvents.OnFinishGame()│
│ - Recopila variables locales:        │
│   * collectedArduinos = 15           │
│   * lostArduinos = 2                 │
│   * dodgedToxics = 12                │
│   * sessionStartTime (Timestamp)     │
│   * HoraFinal (Timestamp.Now())      │
└────┬─────────────────────────────────┘
     │
     ▼
┌──────────────────────────────────────┐
│ FirestoreEvents.OnSaveStatistics     │
│ Invoca con 5 parámetros              │
└────┬─────────────────────────────────┘
     │
     ▼
┌───────────────────────────────────────┐
│ FirestoreEventHandler.OnSaveStatistics│
│ - Obtiene documentId del session      │
│ - Llama firestore.AppendStatisticsEntry
└────┬──────────────────────────────────┘
     │
     ▼
┌──────────────────────────────────────────┐
│ Firestore.AppendStatisticsEntry()        │
│ - Crea objeto estadística:               │
│   {                                      │
│     "ArduinosRecolectados": 15,          │
│     "ArduinosPerdidos": 2,               │
│     "ToxicosEsquivados": 12,             │
│     "HoraInicio": Timestamp(...),        │
│     "HoraFinal": Timestamp(...)          │
│   }                                      │
│ - Usa FieldValue.ArrayUnion()            │
│ - Guarda en Sections/{id}/Estadistica    │
└─────────────────────────────────────────┘
```

---

## Reglas de Seguridad

### Firestore Security Rules

```javascript
rules_version = '2';
service cloud.firestore {
  match /databases/{database}/documents {
    
    // Cualquiera puede leer Sections (datos públicos)
    match /Sections/{document=**} {
      allow read: if true;
      allow create: if request.auth.uid != null;  // Solo autenticados pueden crear
      allow update: if request.auth.uid != null;  // Solo autenticados pueden actualizar
      allow delete: if false;                      // Nadie puede borrar
    }
    
    // Cualquiera puede leer highscores (datos públicos)
    match /highscores/{document=**} {
      allow read: if true;
      allow create: if false;  // Solo Cloud Functions crean
      allow update: if false;  // Solo Cloud Functions actualizan
      allow delete: if false;
    }
  }
}
```

---

## Cloud Functions (Recomendado)

### Función: `procesarFinSesion`

Se ejecuta cuando se escribe en Firestore (trigger: onCreate en Sections)

```javascript
const functions = require('firebase-functions');
const admin = require('firebase-admin');

exports.procesarFinSesion = functions.firestore
  .document('Sections/{docId}')
  .onCreate(async (snap, context) => {
    const data = snap.data();
    const docId = context.params.docId;

    // Calcular puntaje final
    let estadistica = data.Estadistica[0]; // Última entrada
    const puntajeFinal = 
      estadistica.ArduinosRecolectados 
      + (estadistica.ToxicosEsquivados * 3) 
      - estadistica.ArduinosPerdidos;

    // Calcular duración
    const duracion = 
      (estadistica.HoraFinal.toMillis() - 
       estadistica.HoraInicio.toMillis()) / 1000;

    // Crear documento en highscores
    await admin.firestore()
      .collection('highscores')
      .add({
        Nombre: data.Nombre,
        PuntajeFinal: puntajeFinal,
        FechaRegistro: admin.firestore.Timestamp.now(),
        SessionId: docId,
        ArduinosRecolectados: estadistica.ArduinosRecolectados,
        ArduinosPerdidos: estadistica.ArduinosPerdidos,
        ToxicosEsquivados: estadistica.ToxicosEsquivados,
        DuracionSegundos: duracion
      });

    console.log(`Sesión ${docId} procesada. Puntaje: ${puntajeFinal}`);
  });
```

---

## Integración Firebase Web (Dashboard)

### Configuración Firebase para Web

```javascript
// firebaseConfig.js
import { initializeApp } from 'firebase/app';
import { getFirestore } from 'firebase/firestore';

const firebaseConfig = {
  apiKey: "YOUR_API_KEY",
  authDomain: "your-project.firebaseapp.com",
  projectId: "your-project",
  storageBucket: "your-project.appspot.com",
  messagingSenderId: "YOUR_SENDER_ID",
  appId: "YOUR_APP_ID"
};

const app = initializeApp(firebaseConfig);
export const db = getFirestore(app);
```

### Consultas para Dashboard

#### 1. Obtener Ranking (Top 20)

```javascript
import { collection, query, orderBy, limit, getDocs } from 'firebase/firestore';

async function getRanking() {
  const q = query(
    collection(db, 'highscores'),
    orderBy('PuntajeFinal', 'desc'),
    limit(20)
  );
  const snapshot = await getDocs(q);
  return snapshot.docs.map(doc => doc.data());
}
```

#### 2. Obtener Distribución de Puntajes

```javascript
async function getScoreDistribution() {
  const q = query(collection(db, 'highscores'));
  const snapshot = await getDocs(q);
  
  const distribution = {
    '0-50': 0,
    '50-100': 0,
    '100-150': 0,
    '150+': 0
  };
  
  snapshot.docs.forEach(doc => {
    const score = doc.data().PuntajeFinal;
    if (score < 50) distribution['0-50']++;
    else if (score < 100) distribution['50-100']++;
    else if (score < 150) distribution['100-150']++;
    else distribution['150+']++;
  });
  
  return distribution;
}
```

#### 3. Obtener Datos Detallados de Sesión

```javascript
async function getSessionDetails(sessionId) {
  const docRef = doc(db, 'Sections', sessionId);
  const docSnap = await getDoc(docRef);
  
  if (docSnap.exists()) {
    const data = docSnap.data();
    const stats = data.Estadistica[0];
    
    // Calcular métricas derivadas
    const duracion = 
      (stats.HoraFinal.toMillis() - stats.HoraInicio.toMillis()) / 1000;
    const tasaAciertos = 
      (stats.ArduinosRecolectados + stats.ToxicosEsquivados) / (duracion / 60);
    const precision = 
      (stats.ArduinosRecolectados / 
       (stats.ArduinosRecolectados + stats.ArduinosPerdidos)) * 100;
    
    return {
      nombre: data.Nombre,
      puntajeFinal: stats.ArduinosRecolectados + (stats.ToxicosEsquivados * 3) - stats.ArduinosPerdidos,
      duracion,
      tasaAciertos,
      precision,
      tiempoInstrucciones: data.Comportamiento.TiempoInstruccionesSeg,
      ...stats
    };
  }
}
```

---

## Ejemplo Completo: Flujo de Datos de Una Sesión

### Sesión de Ejemplo: "Juan" juega

#### Paso 1: Inicio
```
- Jugador escribe: "Juan"
- SessionCreator.CreateSession()
- Firestore crea documento:

Sections/a7f8k3n2m1p0 = {
  "Nombre": "Juan",
  "HoraCreacion": Timestamp(2026-05-16 14:30:00),
  "Comportamiento": { "TiempoInstruccionesSeg": 0 },
  "Estadistica": []
}
```

#### Paso 2: Lee Instrucciones (35 seg)
```
- InstructionsAnalytics.StartViewingInstructions()
- [35 segundos pasan...]
- InstructionsAnalytics.StopViewingInstructions()
- Firestore actualiza:

Sections/a7f8k3n2m1p0 = {
  ...
  "Comportamiento": { "TiempoInstruccionesSeg": 35.5 }
}
```

#### Paso 3: Gameplay (2 min 37 seg)
```
Eventos:
- OnGoodCollected() ×15
- OnToxicDodged() ×12
- OnBadCollected() ×2

Variables locales:
- collectedArduinos = 15
- dodgedToxics = 12
- lostArduinos = 2
- sessionStartTime = 14:30:45
```

#### Paso 4: Finalizar
```
- ScoreEvents.FinishGame?.Invoke()
- Firestore añade entrada a Estadistica:

Sections/a7f8k3n2m1p0 = {
  ...
  "Estadistica": [
    {
      "ArduinosRecolectados": 15,
      "ArduinosPerdidos": 2,
      "ToxicosEsquivados": 12,
      "HoraInicio": Timestamp(2026-05-16 14:30:45),
      "HoraFinal": Timestamp(2026-05-16 14:33:22)
    }
  ]
}
```

#### Paso 5: Cloud Function Procesa
```
- Calcula: PuntajeFinal = 15 + (12 × 3) - 2 = 49
- Calcula: DuracionSegundos = 157
- Crea documento en highscores:

highscores/{autoId} = {
  "Nombre": "Juan",
  "PuntajeFinal": 49,
  "FechaRegistro": Timestamp(2026-05-16 14:33:22),
  "SessionId": "a7f8k3n2m1p0",
  "ArduinosRecolectados": 15,
  "ArduinosPerdidos": 2,
  "ToxicosEsquivados": 12,
  "DuracionSegundos": 157
}
```

#### Paso 6: Dashboard Lee Datos
```
- Dashboard consulta: collection("highscores").orderBy("PuntajeFinal", "desc")
- Obtiene ranking actualizado con Juan en posición N
- Muestra gráficas con nuevos datos
- Actualiza cada 10 segundos
```

---

## Notas Importantes

1. **Campos requeridos:** Todos los campos marcados como "Sí" son obligatorios
2. **Timestamps:** Siempre usar `Timestamp.GetCurrentTimestamp()` en Unity
3. **Arrays:** Usar `FieldValue.ArrayUnion()` para agregación segura
4. **Índices:** Firestore crea automáticamente índices compuestos para queries complejas
5. **Limites de lectura:** Cada query cuenta como lectura (max 1M/día en plan gratuito)
