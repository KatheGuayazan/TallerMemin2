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
                    { "NumeroSecciones", 0 },
                    { "TiempoInstrucciones", 0f }
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

    public async Task UpdateNumeroSecciones(
        string documentID,
        int value)
    {
        DocumentReference docRef =
            db.Collection("Sections")
            .Document(documentID);

        await docRef.UpdateAsync(
            "Comportamiento.NumeroSecciones",
            value
        );

        Debug.Log("NumeroSecciones actualizado");
    }

    public async Task UpdateTiempoInstrucciones(
        string documentID,
        float tiempo)
    {
        DocumentReference docRef =
            db.Collection("Sections")
            .Document(documentID);

        await docRef.UpdateAsync(
            "Comportamiento.TiempoInstrucciones",
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


    #region READ SECTION

    public async Task ReadSection(string documentID)
    {
        DocumentReference docRef =
            db.Collection("Sections")
            .Document(documentID);

        DocumentSnapshot snapshot =
            await docRef.GetSnapshotAsync();

        if (snapshot.Exists)
        {
            Dictionary<string, object> data =
                snapshot.ToDictionary();

            Debug.Log("Documento encontrado");
            Debug.Log("Nombre: " + data["Nombre"]);
        }
        else
        {
            Debug.Log("Documento no existe");
        }
    }

    #endregion
}