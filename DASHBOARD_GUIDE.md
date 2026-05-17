# Guía de Implementación - Dashboard Web MemoEngineer

## 📋 Tabla de Contenidos
1. [Estructura del Proyecto](#estructura-del-proyecto)
2. [Configuración Inicial](#configuración-inicial)
3. [Componentes Principales](#componentes-principales)
4. [Integración Firebase](#integración-firebase)
5. [Visualizaciones y Gráficas](#visualizaciones-y-gráficas)
6. [Actualización en Tiempo Real](#actualización-en-tiempo-real)
7. [Ejemplos de Código](#ejemplos-de-código)

---

## Estructura del Proyecto

### Opción 1: Vue.js + Vite (Recomendado)

```
dashboard-web/
├── public/
│   └── index.html
├── src/
│   ├── components/
│   │   ├── Ranking.vue
│   │   ├── ScoreDistribution.vue
│   │   ├── PrecisionVsVelocity.vue
│   │   ├── InstructionsImpact.vue
│   │   └── GlobalStats.vue
│   ├── views/
│   │   ├── Dashboard.vue
│   │   └── SessionDetail.vue
│   ├── services/
│   │   └── FirestoreService.js
│   ├── App.vue
│   └── main.js
├── package.json
├── vite.config.js
└── .env.example
```

### Opción 2: React + Vite

```
dashboard-web/
├── public/
├── src/
│   ├── components/
│   │   ├── Ranking.jsx
│   │   ├── ScoreDistribution.jsx
│   │   ├── PrecisionVsVelocity.jsx
│   │   ├── InstructionsImpact.jsx
│   │   └── GlobalStats.jsx
│   ├── pages/
│   │   ├── Dashboard.jsx
│   │   └── SessionDetail.jsx
│   ├── services/
│   │   └── FirestoreService.js
│   ├── App.jsx
│   └── main.jsx
├── package.json
├── vite.config.js
└── .env.example
```

---

## Configuración Inicial

### 1. Instalación de Dependencias

#### Para Vue.js

```bash
npm create vite@latest dashboard-web -- --template vue
cd dashboard-web
npm install
npm install firebase chart.js vue-chartjs date-fns
npm install -D tailwindcss postcss autoprefixer
npx tailwindcss init -p
```

#### Para React

```bash
npm create vite@latest dashboard-web -- --template react
cd dashboard-web
npm install
npm install firebase recharts chart.js react-chartjs-2 date-fns axios
npm install -D tailwindcss postcss autoprefixer
npx tailwindcss init -p
```

### 2. Archivo .env

```
VITE_FIREBASE_API_KEY=your-api-key
VITE_FIREBASE_AUTH_DOMAIN=your-project.firebaseapp.com
VITE_FIREBASE_PROJECT_ID=your-project-id
VITE_FIREBASE_STORAGE_BUCKET=your-project.appspot.com
VITE_FIREBASE_MESSAGING_SENDER_ID=your-sender-id
VITE_FIREBASE_APP_ID=your-app-id
VITE_FIREBASE_MEASUREMENT_ID=your-measurement-id
```

### 3. Archivo de Configuración Firebase

#### `src/services/FirestoreService.js`

```javascript
import { initializeApp } from 'firebase/app';
import { 
  getFirestore, 
  collection, 
  query, 
  orderBy, 
  limit, 
  getDocs,
  doc,
  getDoc,
  onSnapshot,
  where
} from 'firebase/firestore';

const firebaseConfig = {
  apiKey: import.meta.env.VITE_FIREBASE_API_KEY,
  authDomain: import.meta.env.VITE_FIREBASE_AUTH_DOMAIN,
  projectId: import.meta.env.VITE_FIREBASE_PROJECT_ID,
  storageBucket: import.meta.env.VITE_FIREBASE_STORAGE_BUCKET,
  messagingSenderId: import.meta.env.VITE_FIREBASE_MESSAGING_SENDER_ID,
  appId: import.meta.env.VITE_FIREBASE_APP_ID,
};

const app = initializeApp(firebaseConfig);
const db = getFirestore(app);

export const firestoreService = {
  // Obtener ranking (top 20)
  async getRanking(limit = 20) {
    try {
      const q = query(
        collection(db, 'highscores'),
        orderBy('PuntajeFinal', 'desc'),
        limit(limit)
      );
      const snapshot = await getDocs(q);
      return snapshot.docs.map((doc, index) => ({
        id: doc.id,
        posicion: index + 1,
        ...doc.data()
      }));
    } catch (error) {
      console.error('Error fetching ranking:', error);
      return [];
    }
  },

  // Obtener todas las sesiones para análisis
  async getAllSessions() {
    try {
      const q = query(collection(db, 'Sections'));
      const snapshot = await getDocs(q);
      return snapshot.docs.map(doc => ({
        id: doc.id,
        ...doc.data()
      }));
    } catch (error) {
      console.error('Error fetching sessions:', error);
      return [];
    }
  },

  // Obtener detalles de sesión específica
  async getSessionDetails(sessionId) {
    try {
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
        const puntajeFinal = 
          stats.ArduinosRecolectados + (stats.ToxicosEsquivados * 3) - stats.ArduinosPerdidos;
        
        return {
          nombre: data.Nombre,
          puntajeFinal,
          duracion,
          tasaAciertos: tasaAciertos.toFixed(2),
          precision: precision.toFixed(2),
          tiempoInstrucciones: data.Comportamiento.TiempoInstruccionesSeg,
          ...stats
        };
      }
      return null;
    } catch (error) {
      console.error('Error fetching session details:', error);
      return null;
    }
  },

  // Obtener distribución de puntajes
  async getScoreDistribution() {
    try {
      const sessions = await this.getAllSessions();
      const distribution = {
        '0-50': 0,
        '50-100': 0,
        '100-150': 0,
        '150-200': 0,
        '200+': 0
      };
      
      sessions.forEach(session => {
        if (session.Estadistica && session.Estadistica.length > 0) {
          const stats = session.Estadistica[0];
          const score = 
            stats.ArduinosRecolectados + 
            (stats.ToxicosEsquivados * 3) - 
            stats.ArduinosPerdidos;
          
          if (score < 50) distribution['0-50']++;
          else if (score < 100) distribution['50-100']++;
          else if (score < 150) distribution['100-150']++;
          else if (score < 200) distribution['150-200']++;
          else distribution['200+']++;
        }
      });
      
      return distribution;
    } catch (error) {
      console.error('Error fetching score distribution:', error);
      return {};
    }
  },

  // Obtener datos para gráfica Precisión vs Velocidad
  async getPrecisionVsVelocityData() {
    try {
      const sessions = await this.getAllSessions();
      return sessions.map(session => {
        if (!session.Estadistica || session.Estadistica.length === 0) {
          return null;
        }
        
        const stats = session.Estadistica[0];
        const duracion = 
          (stats.HoraFinal.toMillis() - stats.HoraInicio.toMillis()) / 1000;
        const precision = 
          (stats.ArduinosRecolectados / 
           (stats.ArduinosRecolectados + stats.ArduinosPerdidos)) * 100;
        const velocidad = 
          (stats.ArduinosRecolectados + stats.ToxicosEsquivados) / (duracion / 60);
        const score = 
          stats.ArduinosRecolectados + (stats.ToxicosEsquivados * 3) - stats.ArduinosPerdidos;
        
        return {
          nombre: session.Nombre,
          precision: parseFloat(precision.toFixed(2)),
          velocidad: parseFloat(velocidad.toFixed(2)),
          score
        };
      }).filter(item => item !== null);
    } catch (error) {
      console.error('Error fetching precision vs velocity data:', error);
      return [];
    }
  },

  // Obtener correlación entre tiempo de instrucciones y rendimiento
  async getInstructionsImpactData() {
    try {
      const sessions = await this.getAllSessions();
      return sessions.map(session => {
        if (!session.Estadistica || session.Estadistica.length === 0) {
          return null;
        }
        
        const stats = session.Estadistica[0];
        const score = 
          stats.ArduinosRecolectados + (stats.ToxicosEsquivados * 3) - stats.ArduinosPerdidos;
        
        return {
          nombre: session.Nombre,
          tiempoInstrucciones: session.Comportamiento?.TiempoInstruccionesSeg || 0,
          puntaje: score
        };
      }).filter(item => item !== null)
        .sort((a, b) => a.tiempoInstrucciones - b.tiempoInstrucciones);
    } catch (error) {
      console.error('Error fetching instructions impact data:', error);
      return [];
    }
  },

  // Obtener estadísticas globales
  async getGlobalStats() {
    try {
      const sessions = await this.getAllSessions();
      
      if (sessions.length === 0) {
        return {
          totalSessions: 0,
          uniquePlayers: 0,
          averageScore: 0,
          highestScore: 0,
          topPlayer: null
        };
      }
      
      let totalScore = 0;
      let maxScore = 0;
      let topPlayer = null;
      const players = new Set();
      
      sessions.forEach(session => {
        players.add(session.Nombre);
        
        if (session.Estadistica && session.Estadistica.length > 0) {
          const stats = session.Estadistica[0];
          const score = 
            stats.ArduinosRecolectados + (stats.ToxicosEsquivados * 3) - stats.ArduinosPerdidos;
          
          totalScore += score;
          
          if (score > maxScore) {
            maxScore = score;
            topPlayer = session.Nombre;
          }
        }
      });
      
      return {
        totalSessions: sessions.length,
        uniquePlayers: players.size,
        averageScore: (totalScore / sessions.length).toFixed(2),
        highestScore: maxScore,
        topPlayer
      };
    } catch (error) {
      console.error('Error fetching global stats:', error);
      return {};
    }
  },

  // Suscribirse a cambios en tiempo real (Ranking)
  subscribeToRanking(callback, limit = 20) {
    const q = query(
      collection(db, 'highscores'),
      orderBy('PuntajeFinal', 'desc'),
      limit(limit)
    );
    
    return onSnapshot(q, (snapshot) => {
      const data = snapshot.docs.map((doc, index) => ({
        id: doc.id,
        posicion: index + 1,
        ...doc.data()
      }));
      callback(data);
    }, (error) => {
      console.error('Error in realtime subscription:', error);
    });
  }
};

export default db;
```

---

## Componentes Principales

### Componente 1: Ranking (Vue)

#### `src/components/Ranking.vue`

```vue
<template>
  <div class="bg-white rounded-lg shadow-lg p-6">
    <h2 class="text-2xl font-bold mb-4">🏆 Ranking Global</h2>
    
    <div class="overflow-x-auto">
      <table class="w-full text-sm">
        <thead>
          <tr class="border-b-2 border-gray-200">
            <th class="text-left py-3 px-4">Posición</th>
            <th class="text-left py-3 px-4">Jugador</th>
            <th class="text-right py-3 px-4">Puntaje</th>
            <th class="text-right py-3 px-4">Fecha</th>
          </tr>
        </thead>
        <tbody>
          <tr 
            v-for="(player, index) in ranking"
            :key="player.id"
            :class="[
              'border-b border-gray-100 hover:bg-gray-50 transition',
              index === 0 ? 'bg-yellow-50' : index === 1 ? 'bg-gray-50' : index === 2 ? 'bg-orange-50' : ''
            ]"
          >
            <td class="py-3 px-4 font-bold">
              <span v-if="index === 0">🥇</span>
              <span v-else-if="index === 1">🥈</span>
              <span v-else-if="index === 2">🥉</span>
              <span v-else>{{ player.posicion }}</span>
            </td>
            <td class="py-3 px-4">{{ player.Nombre }}</td>
            <td class="py-3 px-4 text-right font-semibold">{{ player.PuntajeFinal }}</td>
            <td class="py-3 px-4 text-right text-gray-500 text-xs">
              {{ formatDate(player.FechaRegistro) }}
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    
    <div class="mt-4 flex justify-between items-center">
      <button 
        @click="refreshRanking"
        class="bg-blue-500 hover:bg-blue-600 text-white px-4 py-2 rounded transition"
      >
        🔄 Actualizar
      </button>
      <span class="text-gray-500 text-sm">
        Actualizado: {{ lastUpdate }}
      </span>
    </div>
  </div>
</template>

<script>
import { onMounted, ref } from 'vue';
import { firestoreService } from '../services/FirestoreService';
import { formatDistanceToNow } from 'date-fns';
import { es } from 'date-fns/locale';

export default {
  name: 'Ranking',
  setup() {
    const ranking = ref([]);
    const lastUpdate = ref('Nunca');
    
    const loadRanking = async () => {
      ranking.value = await firestoreService.getRanking(20);
      lastUpdate.value = 'Hace unos segundos';
    };
    
    const refreshRanking = () => {
      loadRanking();
    };
    
    const formatDate = (timestamp) => {
      if (!timestamp) return '-';
      const date = timestamp.toDate();
      return formatDistanceToNow(date, { addSuffix: true, locale: es });
    };
    
    onMounted(() => {
      loadRanking();
      
      // Actualizar cada 30 segundos
      setInterval(loadRanking, 30000);
    });
    
    return {
      ranking,
      lastUpdate,
      refreshRanking,
      formatDate
    };
  }
};
</script>
```

### Componente 2: Distribución de Puntajes

#### `src/components/ScoreDistribution.vue`

```vue
<template>
  <div class="bg-white rounded-lg shadow-lg p-6">
    <h2 class="text-2xl font-bold mb-4">📊 Distribución de Puntajes</h2>
    
    <BarChart
      :chartData="chartData"
      :options="chartOptions"
    />
    
    <div class="mt-4 grid grid-cols-2 gap-4">
      <div class="bg-blue-50 p-4 rounded">
        <p class="text-gray-600 text-sm">Puntaje Promedio</p>
        <p class="text-2xl font-bold text-blue-600">{{ averageScore }}</p>
      </div>
      <div class="bg-purple-50 p-4 rounded">
        <p class="text-gray-600 text-sm">Puntaje Máximo</p>
        <p class="text-2xl font-bold text-purple-600">{{ maxScore }}</p>
      </div>
    </div>
  </div>
</template>

<script>
import { onMounted, ref, reactive } from 'vue';
import { BarChart, useBarChart } from 'vue-chartjs';
import { Chart as ChartJS, CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend } from 'chart.js';
import { firestoreService } from '../services/FirestoreService';

ChartJS.register(CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend);

export default {
  name: 'ScoreDistribution',
  components: {
    BarChart
  },
  setup() {
    const distribution = ref({});
    const averageScore = ref(0);
    const maxScore = ref(0);
    
    const chartData = reactive({
      labels: ['0-50', '50-100', '100-150', '150-200', '200+'],
      datasets: [
        {
          label: 'Cantidad de Sesiones',
          data: [0, 0, 0, 0, 0],
          backgroundColor: [
            'rgba(255, 99, 132, 0.7)',
            'rgba(255, 193, 7, 0.7)',
            'rgba(76, 175, 80, 0.7)',
            'rgba(33, 150, 243, 0.7)',
            'rgba(156, 39, 176, 0.7)'
          ]
        }
      ]
    });
    
    const chartOptions = {
      responsive: true,
      plugins: {
        legend: {
          display: false
        }
      },
      scales: {
        y: {
          beginAtZero: true
        }
      }
    };
    
    const loadDistribution = async () => {
      const dist = await firestoreService.getScoreDistribution();
      distribution.value = dist;
      
      chartData.datasets[0].data = [
        dist['0-50'] || 0,
        dist['50-100'] || 0,
        dist['100-150'] || 0,
        dist['150-200'] || 0,
        dist['200+'] || 0
      ];
    };
    
    onMounted(() => {
      loadDistribution();
      setInterval(loadDistribution, 30000);
    });
    
    return {
      chartData,
      chartOptions,
      distribution,
      averageScore,
      maxScore
    };
  }
};
</script>
```

### Componente 3: Precisión vs Velocidad

#### `src/components/PrecisionVsVelocity.vue`

```vue
<template>
  <div class="bg-white rounded-lg shadow-lg p-6">
    <h2 class="text-2xl font-bold mb-4">⚡ Precisión vs Velocidad</h2>
    
    <ScatterChart
      :chartData="chartData"
      :options="chartOptions"
    />
    
    <div class="mt-4 text-sm text-gray-600">
      <p>Cada punto representa una sesión. El color indica el puntaje final.</p>
      <p>Eje X: Precisión de Recolección (%) | Eje Y: Velocidad (aciertos/minuto)</p>
    </div>
  </div>
</template>

<script setup>
import { onMounted, ref, reactive } from 'vue';
import { firestoreService } from '../services/FirestoreService';

const chartData = reactive({
  datasets: [
    {
      label: 'Sesiones de Juego',
      data: [],
      backgroundColor: 'rgba(76, 175, 80, 0.6)',
      borderColor: 'rgba(76, 175, 80, 1)',
      borderWidth: 2,
      pointRadius: 6,
      pointHoverRadius: 8
    }
  ]
});

const chartOptions = {
  responsive: true,
  plugins: {
    tooltip: {
      callbacks: {
        label: function(context) {
          const point = context.raw;
          return `${point.nombre}: ${point.precision.toFixed(1)}% - ${point.velocidad.toFixed(2)} aciertos/min - Puntaje: ${point.score}`;
        }
      }
    }
  },
  scales: {
    x: {
      title: {
        display: true,
        text: 'Precisión (%)'
      },
      min: 0,
      max: 100
    },
    y: {
      title: {
        display: true,
        text: 'Velocidad (aciertos/min)'
      },
      beginAtZero: true
    }
  }
};

const loadData = async () => {
  const data = await firestoreService.getPrecisionVsVelocityData();
  chartData.datasets[0].data = data.map(item => ({
    x: item.precision,
    y: item.velocidad,
    nombre: item.nombre,
    precision: item.precision,
    velocidad: item.velocidad,
    score: item.score
  }));
};

onMounted(() => {
  loadData();
  setInterval(loadData, 30000);
});
</script>
```

---

## Visualizaciones y Gráficas

### Librerías Recomendadas

1. **Chart.js** - Gráficas simples y confiables
   ```bash
   npm install chart.js vue-chartjs
   ```

2. **Recharts** (React) - Componentes react-first
   ```bash
   npm install recharts
   ```

3. **Plotly.js** - Gráficas interactivas complejas
   ```bash
   npm install react-plotly.js plotly.js
   ```

### Tipos de Gráficas Necesarias

```
1. Tabla (Ranking)
   └─ HTML Table nativo

2. Histograma (Distribución)
   └─ Chart.js BarChart

3. Scatter Plot (Precisión vs Velocidad)
   └─ Chart.js ScatterChart

4. Línea (Instrucciones Impact)
   └─ Chart.js LineChart

5. Tarjetas (Estadísticas Globales)
   └─ HTML div con Tailwind CSS
```

---

## Actualización en Tiempo Real

### Opción 1: Polling (Simpler)

```javascript
// Actualizar cada 30 segundos
setInterval(async () => {
  const data = await firestoreService.getRanking();
  updateUI(data);
}, 30000);
```

### Opción 2: Real-time Listeners (Recomendado)

```javascript
// Suscribirse a cambios en tiempo real
const unsubscribe = firestoreService.subscribeToRanking((data) => {
  updateUI(data);
});

// Desuscribirse cuando se desmonta el componente
onUnmounted(() => {
  unsubscribe();
});
```

### Opción 3: WebSocket (Opcional)

```javascript
// Usar Firebase Realtime Database en lugar de Firestore
// O implementar un servidor WebSocket personalizado
const socket = io('https://your-server.com');

socket.on('rankingUpdated', (data) => {
  updateUI(data);
});
```

---

## Ejemplos de Código

### Página Principal del Dashboard

```vue
<template>
  <div class="min-h-screen bg-gray-100 p-8">
    <div class="max-w-7xl mx-auto">
      
      <!-- Header -->
      <div class="mb-8">
        <h1 class="text-4xl font-bold text-gray-800">🎮 MemoEngineer Dashboard</h1>
        <p class="text-gray-600 mt-2">Análisis en tiempo real de sesiones de juego</p>
      </div>
      
      <!-- Estadísticas Globales -->
      <GlobalStats />
      
      <!-- Grid Principal -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-8 mt-8">
        <!-- Ranking -->
        <div class="lg:col-span-2">
          <Ranking />
        </div>
        
        <!-- Distribución -->
        <ScoreDistribution />
        
        <!-- Precisión vs Velocidad -->
        <PrecisionVsVelocity />
        
        <!-- Impacto de Instrucciones -->
        <div class="lg:col-span-2">
          <InstructionsImpact />
        </div>
      </div>
      
    </div>
  </div>
</template>

<script>
import Ranking from '../components/Ranking.vue';
import ScoreDistribution from '../components/ScoreDistribution.vue';
import PrecisionVsVelocity from '../components/PrecisionVsVelocity.vue';
import InstructionsImpact from '../components/InstructionsImpact.vue';
import GlobalStats from '../components/GlobalStats.vue';

export default {
  components: {
    Ranking,
    ScoreDistribution,
    PrecisionVsVelocity,
    InstructionsImpact,
    GlobalStats
  }
};
</script>
```

### Configuración Tailwind CSS

```javascript
// tailwind.config.js
export default {
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        primary: '#3B82F6',
        success: '#10B981',
        warning: '#F59E0B',
        danger: '#EF4444',
      }
    },
  },
  plugins: [],
}
```

---

## Checklist de Implementación

- [ ] Proyecto creado (Vue.js o React)
- [ ] Firebase configurado con .env
- [ ] FirestoreService.js implementado
- [ ] Componente Ranking funcional
- [ ] Componente ScoreDistribution funcional
- [ ] Componente PrecisionVsVelocity funcional
- [ ] Componente InstructionsImpact funcional
- [ ] Componente GlobalStats funcional
- [ ] Actualización automática cada 30 seg
- [ ] Responsive en mobile/tablet
- [ ] Estilos con Tailwind CSS
- [ ] Manejo de errores y loading states
- [ ] Documentado y listo para deployment

---

## Deployment

### Opción 1: Firebase Hosting

```bash
npm install -g firebase-tools
firebase login
firebase init hosting
npm run build
firebase deploy
```

### Opción 2: Vercel

```bash
npm install -g vercel
vercel
```

### Opción 3: Netlify

```bash
npm install -g netlify-cli
netlify deploy --prod --dir=dist
```

---

## Notas Finales

- Todas las consultas a Firestore cuentan como lecturas
- Usar índices compuestos para queries complejas
- Cache los datos localmente cuando sea posible
- Implementar error boundaries y fallbacks
- Testing: usar Vitest + Testing Library
