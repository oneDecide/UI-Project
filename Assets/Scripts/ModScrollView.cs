using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(ScrollRect))]
public class ModScrollView : MonoBehaviour, IScrollHandler
{
    [Header("References")]
    [SerializeField] private RectTransform content;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform viewport;

    [Header("Rubberband Settings")]
    [SerializeField] private float rubberbandStrength = 0.5f;
    [SerializeField] private float rubberbandDuration = 0.3f;
    [SerializeField] private AnimationCurve rubberbandCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Scroll Settings")]
    [SerializeField] private float scrollSensitivity = 1f;
    [SerializeField] private float scrollDeceleration = 0.135f;

    private bool isDragging;
    private float normalizedPositionBeforeDrag;
    private Coroutine rubberbandCoroutine;
    private bool isRubberbanding;

   private void Awake()
{
    if (scrollRect == null) scrollRect = GetComponent<ScrollRect>();
    if (content == null) content = scrollRect.content;
    if (viewport == null) viewport = scrollRect.viewport;

    // Correct event registration for Unity's ScrollRect
    scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
    
    // Create event triggers if they don't exist
    EventTrigger trigger = scrollRect.gameObject.GetComponent<EventTrigger>();
    if (trigger == null)
    {
        trigger = scrollRect.gameObject.AddComponent<EventTrigger>();
    }

    // Add begin drag event
    var beginDragEntry = new EventTrigger.Entry();
    beginDragEntry.eventID = EventTriggerType.BeginDrag;
    beginDragEntry.callback.AddListener((data) => { OnBeginDrag((PointerEventData)data); });
    trigger.triggers.Add(beginDragEntry);

    // Add end drag event
    var endDragEntry = new EventTrigger.Entry();
    endDragEntry.eventID = EventTriggerType.EndDrag;
    endDragEntry.callback.AddListener((data) => { OnEndDrag((PointerEventData)data); });
    trigger.triggers.Add(endDragEntry);
}

    public void OnBeginDrag()
    {
        Debug.Log("Scroll Begin Drag");
    }

    private void OnEndDrag()
    {
        Debug.Log("Scroll End Drag");
    }

    private void OnScrollValueChanged(Vector2 position)
    {
        // If we're at the top or bottom and not currently rubberbanding
        if (!isRubberbanding && !isDragging)
        {
            if (position.y <= 0f || position.y >= 1f)
            {
                StartRubberband(position.y <= 0f);
            }
        }
    }

    private void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        normalizedPositionBeforeDrag = scrollRect.verticalNormalizedPosition;
        
        // Stop any ongoing rubberband
        if (rubberbandCoroutine != null)
        {
            StopCoroutine(rubberbandCoroutine);
            isRubberbanding = false;
        }
    }

    private void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        
        // Check if we need to rubberband
        if (scrollRect.verticalNormalizedPosition < 0f || scrollRect.verticalNormalizedPosition > 1f)
        {
            StartRubberband(scrollRect.verticalNormalizedPosition < 0f);
        }
    }

    public void OnScroll(PointerEventData eventData)
    {
        if (!RectTransformUtility.RectangleContainsScreenPoint(viewport, eventData.position, eventData.pressEventCamera))
            return;

        // Apply scroll wheel input
        float scrollDelta = eventData.scrollDelta.y * scrollSensitivity * 0.01f;
        scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition + scrollDelta);

        // If we hit the bounds, start rubberband
        if (scrollRect.verticalNormalizedPosition <= 0f || scrollRect.verticalNormalizedPosition >= 1f)
        {
            StartRubberband(scrollRect.verticalNormalizedPosition <= 0f);
        }
    }

    private void StartRubberband(bool isAtTop)
    {
        if (rubberbandCoroutine != null)
            StopCoroutine(rubberbandCoroutine);

        rubberbandCoroutine = StartCoroutine(RubberbandEffect(isAtTop));
    }

    private IEnumerator RubberbandEffect(bool isAtTop)
    {
        isRubberbanding = true;
        float startPosition = scrollRect.verticalNormalizedPosition;
        float targetPosition = isAtTop ? 0f : 1f;
        float rubberbandDistance = Mathf.Abs(startPosition - targetPosition);
        float elapsedTime = 0f;

        while (elapsedTime < rubberbandDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = rubberbandCurve.Evaluate(elapsedTime / rubberbandDuration);
            
            // Apply rubberband effect (stronger when further out of bounds)
            float currentPosition = Mathf.Lerp(
                startPosition, 
                targetPosition, 
                t * Mathf.Clamp01(rubberbandDistance * rubberbandStrength)
            );

            scrollRect.verticalNormalizedPosition = currentPosition;
            yield return null;
        }

        scrollRect.verticalNormalizedPosition = targetPosition;
        isRubberbanding = false;
        rubberbandCoroutine = null;
    }

    private void Update()
    {
        // Apply deceleration when not dragging and not rubberbanding
        if (!isDragging && !isRubberbanding && scrollRect.velocity.sqrMagnitude > 0.1f)
        {
            scrollRect.velocity *= Mathf.Pow(scrollDeceleration, Time.unscaledDeltaTime);
            
            // Check if we need to start rubberbanding
            if (scrollRect.verticalNormalizedPosition < 0f || scrollRect.verticalNormalizedPosition > 1f)
            {
                StartRubberband(scrollRect.verticalNormalizedPosition < 0f);
            }
        }
    }

    // Call this when adding new mod items to the scroll view
    public void RefreshContentSize()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        Canvas.ForceUpdateCanvases();
    }
}