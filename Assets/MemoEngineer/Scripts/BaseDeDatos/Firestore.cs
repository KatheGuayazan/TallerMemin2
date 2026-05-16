using Firebase.Firestore;
using Firebase.Extensions;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Firestore : MonoBehaviour
{
    FirebaseFirestore db;

    private void Awake()
    {
        db = FirebaseFirestore.DefaultInstance;
    }

    #region CREATE SECTION

    /// <summary>
    /// Crear una nueva sección con ID automático
    /// </summary>
    public async Task<string> CreateSection(string nombre)
    {
        DocumentReference docRef = db.Collection("Sections").Document();

        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            { "Nombre", nombre },

            { "Comportamiento", new Dictionary<string, object>()
                {
                    { "NumeroSecciones", 0 },
                    { "TiempoInstrucciones", 0 }
                }
            },

            { "Estadistica", new Dictionary<string, object>()
                {
                    { "AlimentosRecolectados", 0 },
                    { "HoraInicio", Timestamp.GetCurrentTimestamp() },
                    { "HoraFinal", Timestamp.GetCurrentTimestamp() },
                    { "ToxicosEsquivados", 0 }
                }
            }
        };

        await docRef.SetAsync(data);

        Debug.Log("Section creada");
        Debug.Log("ID: " + docRef.Id);

        return docRef.Id;
    }

    #endregion

    #region UPDATE NOMBRE

    public async Task UpdateNombre(string documentID, string nuevoNombre)
    {
        DocumentReference docRef =
            db.Collection("Sections").Document(documentID);

        await docRef.UpdateAsync("Nombre", nuevoNombre);

        Debug.Log("Nombre actualizado");
    }

    #endregion

    #region UPDATE COMPORTAMIENTO

    public async Task UpdateNumeroSecciones(string documentID, int value)
    {
        DocumentReference docRef =
            db.Collection("Sections").Document(documentID);

        await docRef.UpdateAsync(
            "Comportamiento.NumeroSecciones",
            value
        );

        Debug.Log("NumeroSecciones actualizado");
    }

    public async Task UpdateTiempoInstrucciones(string documentID, float tiempo)
    {
        DocumentReference docRef =
            db.Collection("Sections").Document(documentID);

        await docRef.UpdateAsync(
            "Comportamiento.TiempoInstrucciones",
            tiempo
        );

        Debug.Log("TiempoInstrucciones actualizado");
    }

    #endregion

    #region UPDATE ESTADISTICAS

    public async Task UpdateAlimentos(
        string documentID,
        int cantidad)
    {
        DocumentReference docRef =
            db.Collection("Sections").Document(documentID);

        await docRef.UpdateAsync(
            "Estadistica.ArduinosRecolectados",
            cantidad
        );
    }

    public async Task UpdateToxicos(
        string documentID,
        int cantidad)
    {
        DocumentReference docRef =
            db.Collection("Sections").Document(documentID);

        await docRef.UpdateAsync(
            "Estadistica.ToxicosEsquivados",
            cantidad
        );
    }

    public async Task UpdateHoraInicio(
        string documentID)
    {
        DocumentReference docRef =
            db.Collection("Sections").Document(documentID);

        await docRef.UpdateAsync(
            "Estadistica.HoraInicio",
            Timestamp.GetCurrentTimestamp()
        );
    }

    public async Task UpdateHoraFinal(
        string documentID)
    {
        DocumentReference docRef =
            db.Collection("Sections").Document(documentID);

        await docRef.UpdateAsync(
            "Estadistica.HoraFinal",
            Timestamp.GetCurrentTimestamp()
        );
    }

    #endregion

    #region READ SECTION

    public async Task ReadSection(string documentID)
    {
        DocumentReference docRef =
            db.Collection("Sections").Document(documentID);

        DocumentSnapshot snapshot =
            await docRef.GetSnapshotAsync();

        if (snapshot.Exists)
        {
            Dictionary<string, object> data =
                snapshot.ToDictionary();

            Debug.Log("Documento encontrado");

            Debug.Log("Nombre: " + data["Nombre"]);
        }
    }

    #endregion
}