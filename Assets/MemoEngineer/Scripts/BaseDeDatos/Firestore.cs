using Firebase.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Firestore : MonoBehaviour
{
    #region VARIABLES

    private FirebaseFirestore db;

    #endregion

    #region UNITY METHODS

    private void Awake()
    {
        db = FirebaseFirestore.DefaultInstance;
    }

    #endregion

    #region CREATE SECTION

    public async Task<string> CreateSection(string nombreJugador)
    {
        DocumentReference docRef =
            db.Collection("Sections").Document();

        Dictionary<string, object> estadistica =
            new Dictionary<string, object>()
        {
            { "ArduinosRecolectados", 0 },
            { "ArduinosPerdidos", 0 },
            { "ToxicosEsquivados", 0 },

            { "HoraInicio", Timestamp.GetCurrentTimestamp() },
            { "HoraFinal", Timestamp.GetCurrentTimestamp() },

            { "Historico", new List<object>() }
        };

        Dictionary<string, object> data =
            new Dictionary<string, object>()
        {
            { "Nombre", nombreJugador },

            {
                "Comportamiento",
                new Dictionary<string, object>()
                {
                    { "TiempoInstruccionesSeg", 0f }
                }
            },

            { "Estadistica", estadistica }
        };

        await docRef.SetAsync(data);

        Debug.Log("Nueva sesión creada");
        Debug.Log("ID Documento: " + docRef.Id);

        return docRef.Id;
    }

    #endregion

    #region UPDATE NOMBRE

    public async Task UpdateNombre(
        string documentID,
        string nuevoNombre)
    {
        DocumentReference docRef =
            db.Collection("Sections")
            .Document(documentID);

        await docRef.UpdateAsync(
            "Nombre",
            nuevoNombre
        );

        Debug.Log("Nombre actualizado");
    }

    #endregion

    #region UPDATE COMPORTAMIENTO

    public async Task UpdateTiempoInstrucciones(
        string documentID,
        float tiempo)
    {
        DocumentReference docRef =
            db.Collection("Sections")
            .Document(documentID);

        await docRef.UpdateAsync(
            "Comportamiento.TiempoInstruccionesSeg",
            tiempo
        );

        Debug.Log("TiempoInstrucciones actualizado");
    }

    #endregion

    #region APPEND STATISTICS ENTRY


    public async Task AppendStatisticsEntry(
        string documentID,
        int recolectados,
        int perdidos,
        int toxicosEsquivados,
        Timestamp horaInicio,
        Timestamp horaFinal)
    {
        DocumentReference docRef =
            db.Collection("Sections")
            .Document(documentID);

        Dictionary<string, object> entry =
            new Dictionary<string, object>()
        {
            { "ArduinosRecolectados", recolectados },
            { "ArduinosPerdidos", perdidos },
            { "ToxicosEsquivados", toxicosEsquivados },

            { "HoraInicio", horaInicio },
            { "HoraFinal", horaFinal }
        };

        // Usar SetAsync con merge para crear el documento si no existe
        await docRef.SetAsync(
            new Dictionary<string, object>()
            {
                { "Estadistica", FieldValue.ArrayUnion(entry) }
            },
            SetOptions.MergeAll
        );

        Debug.Log("Nueva estadística añadida");
    }
    #endregion

    #region READ ALL SCORES

    public async Task<List<PlayerScoreData>> GetAllScores()
    {
        QuerySnapshot snapshot =
            await db.Collection("Sections")
            .GetSnapshotAsync();

        List<PlayerScoreData> scores =
            new List<PlayerScoreData>();

        foreach (DocumentSnapshot document in snapshot.Documents)
        {
            Dictionary<string, object> data =
                document.ToDictionary();

            string playerName = "Unknown";

            if (data.ContainsKey("Nombre"))
            {
                playerName = data["Nombre"].ToString();
            }

            int totalScore = 0;

            // =========================
            // ESTADISTICA ARRAY
            // =========================

            if (data.ContainsKey("Estadistica"))
            {
                List<object> estadisticas =
                    data["Estadistica"] as List<object>;

                if (estadisticas != null)
                {
                    foreach (object item in estadisticas)
                    {
                        Dictionary<string, object> stats =
                            item as Dictionary<string, object>;

                        if (stats == null) continue;

                        int recolectados = 0;
                        int perdidos = 0;
                        int esquivados = 0;

                        if (stats.ContainsKey("ArduinosRecolectados"))
                        {
                            recolectados =
                                System.Convert.ToInt32(
                                    stats["ArduinosRecolectados"]);
                        }

                        if (stats.ContainsKey("ArduinosPerdidos"))
                        {
                            perdidos =
                                System.Convert.ToInt32(
                                    stats["ArduinosPerdidos"]);
                        }

                        if (stats.ContainsKey("ToxicosEsquivados"))
                        {
                            esquivados =
                                System.Convert.ToInt32(
                                    stats["ToxicosEsquivados"]);
                        }

                        totalScore +=
                            recolectados +
                            esquivados -
                            perdidos;
                    }
                }
            }

            PlayerScoreData scoreData =
                new PlayerScoreData()
                {
                    playerName = playerName,
                    score = totalScore
                };

            scores.Add(scoreData);
        }

        // =========================
        // ORDER DESCENDING
        // =========================

        scores.Sort((a, b) => b.score.CompareTo(a.score));

        return scores;
    }

    #endregion

}