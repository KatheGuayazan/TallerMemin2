using UnityEngine;

public class InstructionsAnalytics : MonoBehaviour
{
    #region VARIABLES

    private float startTime;

    private float totalTime;

    private bool isViewingInstructions;

    #endregion

    #region START VIEW

    public void StartViewingInstructions()
    {
        if (isViewingInstructions) return;

        isViewingInstructions = true;

        startTime = Time.time;

        Debug.Log("Started Viewing Instructions");
    }

    #endregion

    #region STOP VIEW

    public void StopViewingInstructions()
    {
        if (!isViewingInstructions) return;

        isViewingInstructions = false;

        float duration =
            Time.time - startTime;

        totalTime += duration;

        FirestoreEvents.OnUpdateInstructionTime?.Invoke(
            totalTime
        );

        Debug.Log(
            "Stopped Viewing Instructions: " +
            duration
        );
    }

    #endregion
}