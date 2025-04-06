using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [Header("Character Prefabs")]
    [SerializeField] private GameObject defaultCharacter;
    [SerializeField] private GameObject dashCharacter;
    [SerializeField] private GameObject shieldCharacter;

    [Header("Settings")]
    [SerializeField] private string playerPrefsKey = "SelectedCharacter";
    [SerializeField] private string playerTag = "Player";

    private GameObject currentPlayer;

    void Start()
    {
        SpawnCharacter();
        SetupCamera();
    }

    private void SpawnCharacter()
    {
        int selectedCharacter = PlayerPrefs.GetInt(playerPrefsKey, 0);
        
        GameObject prefabToSpawn = selectedCharacter switch
        {
            1 => dashCharacter,
            2 => shieldCharacter,
            _ => defaultCharacter
        };

        if(prefabToSpawn != null)
        {
            currentPlayer = Instantiate(prefabToSpawn, Vector3.zero, Quaternion.identity);
            currentPlayer.tag = playerTag;
        }
        else
        {
            Debug.LogError("Character prefab not assigned!");
        }
    }

    private void SetupCamera()
    {
        Camera.main.GetComponent<CameraFollow>()?.FindPlayerTarget();
    }
}