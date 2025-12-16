using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Axis {X, Y, Z}
public class MeshFiller : MonoBehaviour
{
    [SerializeField] private Axis axis;
    [SerializeField] private float yMultiplier = 1;
    [SerializeField, Range(-1, 1)] private int sign = 1;
    
    private Vector3[] originalVertices;
    private Mesh mesh;
    private float minVertex;
    
    public float Value { get; private set; }
    
    private MeshRenderer meshRenderer;
    private Vector3[] modifiedVertices;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        mesh = GetComponent<MeshFilter>().mesh;
        originalVertices = mesh.vertices.Clone() as Vector3[];
        switch (axis)
        {
            case Axis.X:
                minVertex = originalVertices.Select(x => x.x).Min();
                break;
            case Axis.Y:
                minVertex = originalVertices.Select(x => x.y).Min();
                break;
            case Axis.Z:
                minVertex = originalVertices.Select(x => x.z).Min();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void HideUpperPart(float yThreshold)
    {
        // var min = isHorizontal ? 0.15f : 0.03f;
        // if (yThreshold < min)
        // {
        //     if (gameObject.activeInHierarchy)
        //         StartCoroutine(CutRoutine());
        // }
        // else
            Cut(yThreshold);
    }

    private IEnumerator CutRoutine()
    {
        Cut(0.15f);
        yield return new WaitForSeconds(0.1f);
        meshRenderer.enabled = false;
    }

    private void Cut(float yThreshold)
    {
        if (!meshRenderer.enabled)
            meshRenderer.enabled = true;

        switch (axis)
        {
            case Axis.X:
                HideUpperPart_Horizontal(yThreshold);
                break;
            case Axis.Y:
                HideUpperPart_Vertical(yThreshold);
                break;
            case Axis.Z:
                HideUpperPart_ZAxis(yThreshold);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void HideUpperPart_Vertical(float yThreshold)
    {
        Value = yThreshold;
        
        if (sign == -1)
            yThreshold = Mathf.Abs(yThreshold - 1);
        yThreshold = yThreshold * (yMultiplier * 2) + minVertex;
        
        var vertices = mesh.vertices;
        var verticesChanged = false;

        for (var i = 0; i < vertices.Length; i++)
        {
            var isCut = sign == -1
                ? originalVertices[i].y < yThreshold && !Mathf.Approximately(vertices[i].y, yThreshold)
                : originalVertices[i].y > yThreshold && !Mathf.Approximately(vertices[i].y, yThreshold);

            var isReCut = sign == -1
                ? originalVertices[i].y >= yThreshold && !Mathf.Approximately(vertices[i].y, originalVertices[i].y)
                : originalVertices[i].y <= yThreshold && !Mathf.Approximately(vertices[i].y, originalVertices[i].y);
            
            if (isCut)
            {
                vertices[i] = new Vector3(originalVertices[i].x, yThreshold, originalVertices[i].z);
                verticesChanged = true;
            }
            else if (isReCut)
            {
                vertices[i] = originalVertices[i];
                verticesChanged = true;
            }
        }

        if (verticesChanged)
        {
            mesh.vertices = vertices;
            mesh.RecalculateBounds();
        }
    }
    
    private void HideUpperPart_Horizontal(float yThreshold)
    {
        Value = yThreshold;
        
        if (sign == -1)
            yThreshold = Mathf.Abs(yThreshold - 1);
        yThreshold = yThreshold * (yMultiplier * 2) + minVertex;
        
        var vertices = mesh.vertices;
        var verticesChanged = false;

        for (var i = 0; i < vertices.Length; i++)
        {
            var isCut = sign == -1
                ? originalVertices[i].x < yThreshold && !Mathf.Approximately(vertices[i].x, yThreshold)
                : originalVertices[i].x > yThreshold && !Mathf.Approximately(vertices[i].x, yThreshold);

            var isReCut = sign == -1
                ? originalVertices[i].x >= yThreshold && !Mathf.Approximately(vertices[i].x, originalVertices[i].x)
                : originalVertices[i].x <= yThreshold && !Mathf.Approximately(vertices[i].x, originalVertices[i].x);
            
            if (isCut)
            {
                vertices[i] = new Vector3(yThreshold, originalVertices[i].y, originalVertices[i].z);
                verticesChanged = true;
            }
            else if (isReCut)
            {
                vertices[i] = originalVertices[i];
                verticesChanged = true;
            }
        }

        if (verticesChanged)
        {
            mesh.vertices = vertices;
            mesh.RecalculateBounds();
        }
    }
    
    private void HideUpperPart_ZAxis(float yThreshold)
    {
        Value = yThreshold;
        
        if (sign == -1)
            yThreshold = Mathf.Abs(yThreshold - 1);
        yThreshold = yThreshold * (yMultiplier * 2) + minVertex;
        
        var vertices = mesh.vertices;
        var verticesChanged = false;
        
        for (var i = 0; i < vertices.Length; i++)
        {
            var isCut = sign == -1
                ? originalVertices[i].z < yThreshold && !Mathf.Approximately(vertices[i].z, yThreshold)
                : originalVertices[i].z > yThreshold && !Mathf.Approximately(vertices[i].z, yThreshold);

            var isReCut = sign == -1
                ? originalVertices[i].z >= yThreshold && !Mathf.Approximately(vertices[i].z, originalVertices[i].z)
                : originalVertices[i].z <= yThreshold && !Mathf.Approximately(vertices[i].z, originalVertices[i].z);
            
            if (isCut)
            {
                vertices[i] = new Vector3(originalVertices[i].x, originalVertices[i].y, yThreshold);
                verticesChanged = true;
            }
            else if (isReCut)
            {
                vertices[i] = originalVertices[i];
                verticesChanged = true;
            }
        }

        if (verticesChanged)
        {
            mesh.vertices = vertices;
            mesh.RecalculateBounds();
        }
    }
}
