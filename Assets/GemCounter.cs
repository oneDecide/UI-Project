using UnityEngine;

public class GemCounter : MonoBehaviour

{
    // This script is responsible for counting the gems collected by the player.
    // It will be attached to the player GameObject.

    [Header("Gem Settings")]
    [SerializeField] private int gemCount = 0; // The current number of gems collected
    
    [SerializeField] public GameObject GameWinCanvas = null;

    [SerializeField] public GameObject Player = null;

    public int winningGems = 5; // The number of gems required to win the game

    // Method to add gems to the count
    public void AddGems(int amount)
    {
        gemCount += amount;
        Debug.Log($"Gems collected: {amount}. Total gems: {gemCount}");

        if (gemCount >= winningGems)
        {
            // Call the method to handle game win condition
            GameWinCanvas.SetActive(true);
            Player.SetActive(false);
        }
    }

    // Method to get the current gem count
    public int GetGemCount()
    {
        return gemCount;
    }
}