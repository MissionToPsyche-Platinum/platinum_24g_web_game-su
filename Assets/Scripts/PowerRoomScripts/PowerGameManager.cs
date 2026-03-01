using UnityEngine;

public class PowerGameManager : MonoBehaviour
{
    // Drag your 4 Cell objects here in the Inspector
    public PowerCell[] allCells; 
    private int currentCellIndex = 0; 

    void Update()
    {
        // Check for Spacebar press
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Make sure we haven't finished all cells yet
            if (currentCellIndex < allCells.Length)
            {
                // Try to stop the current needle
                allCells[currentCellIndex].AttemptCalibration();

                // If the needle successfully stopped in the yellow zone
                if (allCells[currentCellIndex].isCalibrated)
                {
                    currentCellIndex++;
                    Debug.Log("Next Cell!");
                }
            }
        }

        // If all 4 are done, the game is won!
        if (currentCellIndex >= allCells.Length)
        {
            Debug.Log("MINIGAME COMPLETE!");
        }
    }
}