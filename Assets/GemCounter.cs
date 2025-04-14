using UnityEngine;

public class GemCounter : MonoBehaviour

{
    // This script is responsible for counting the gems collected by the player.
    // It will be attached to the player GameObject.

    [Header("Gem Settings")]
    [SerializeField] private int gemCount = 0; // The current number of gems collected

    // Method to add gems to the count
    public void AddGems(int amount)
    {
        gemCount += amount;
        Debug.Log($"Gems collected: {amount}. Total gems: {gemCount}");
    }

    // Method to get the current gem count
    public int GetGemCount()
    {
        return gemCount;
    }
}