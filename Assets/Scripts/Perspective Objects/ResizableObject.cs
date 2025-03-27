using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizableObject : MonoBehaviour
{
    public float resizeAmount = 0.03f;
    public float resizeInterval = 0.1f; // Add a delay between resizes
    public float minSize = 0.5f;
    public float maxSize = 2.0f;

    private Vector3 initialScale;
    private float lastResizeTime; // Track last resize time

    void Start()
    {
        initialScale = transform.localScale;
        lastResizeTime = 0f;
    }

    public void ResizeUp()
    {
        // Only resize if enough time has passed since last resize
        if (Time.time - lastResizeTime < resizeInterval)
            return;
            
        Vector3 newScale = transform.localScale + Vector3.one * resizeAmount;
        newScale = Vector3.Max(newScale, Vector3.one * minSize); // Ensure minimum size
        newScale = Vector3.Min(newScale, Vector3.one * maxSize); // Ensure maximum size
        transform.localScale = newScale;
        lastResizeTime = Time.time;
    }

    public void ResizeDown()
    {
        // Only resize if enough time has passed since last resize
        if (Time.time - lastResizeTime < resizeInterval)
            return;
            
        Vector3 newScale = transform.localScale - Vector3.one * resizeAmount;
        newScale = Vector3.Max(newScale, Vector3.one * minSize); // Ensure minimum size
        newScale = Vector3.Min(newScale, Vector3.one * maxSize); // Ensure maximum size
        transform.localScale = newScale;
        lastResizeTime = Time.time;
    }
}