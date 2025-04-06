using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class InfiniteGrid : MonoBehaviour
{
    [Header("Player Tracking")]
    public Transform player;
    public float activationDistance = 15f;
    
    [Header("Grid Appearance")]
    public Color gridColor = Color.white;
    public Color backgroundColor = new Color(0,0,0,0);
    public float gridSize = 1f;
    public float lineWidth = 0.05f;
    
    [Header("Performance")]
    [Range(0.1f, 1f)] public float updateFrequency = 0.2f;
    
    private Material gridMaterial;
    private Renderer gridRenderer;
    private float lastUpdateTime;
    private Vector3 lastPlayerPosition;

    private void Awake()
    {
        gridRenderer = GetComponent<Renderer>();
        CreateGridMaterial();
        UpdateGridScale();
    }

    private void CreateGridMaterial()
    {
        // Create a new material with the grid shader
        gridMaterial = new Material(Shader.Find("Custom/InfiniteGrid"));
        gridMaterial.SetColor("_GridColor", gridColor);
        gridMaterial.SetColor("_BackgroundColor", backgroundColor);
        gridMaterial.SetFloat("_GridSize", gridSize);
        gridMaterial.SetFloat("_LineWidth", lineWidth);
        gridRenderer.material = gridMaterial;
    }

    private void Update()
    {
        // Throttle updates for performance
        if (Time.time - lastUpdateTime < updateFrequency) return;
        
        UpdateGridVisibility();
        UpdateGridPosition();
        UpdateGridScale();
        
        lastUpdateTime = Time.time;
    }

    private void UpdateGridVisibility()
    {
        bool shouldShow = Vector3.Distance(player.position, transform.position) < activationDistance;
        gridRenderer.enabled = shouldShow;
    }

    private void UpdateGridPosition()
    {
        // Only update if player moved significantly
        if (Vector3.Distance(player.position, lastPlayerPosition) > gridSize * 0.5f)
        {
            transform.position = new Vector3(
                player.position.x, 
                player.position.y, 
                transform.position.z);
            lastPlayerPosition = player.position;
        }
    }

    private void UpdateGridScale()
    {
        if (Camera.main != null)
        {
            // Add buffer area around the camera view
            float buffer = 2f;
            float height = 2f * Camera.main.orthographicSize + buffer;
            float width = height * Camera.main.aspect + buffer;
            transform.localScale = new Vector3(width, height, 1);
        }
    }

    // Public methods to modify grid appearance at runtime
    public void SetGridColor(Color newColor)
    {
        gridColor = newColor;
        gridMaterial.SetColor("_GridColor", gridColor);
    }

    public void SetGridSize(float newSize)
    {
        gridSize = newSize;
        gridMaterial.SetFloat("_GridSize", gridSize);
    }
}