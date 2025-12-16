using System;
using UnityEngine;

namespace Grid
{
    public class GridCell : MonoBehaviour
    {
        [NonSerialized] public Transform thisTransform;
        
        public Vector2Int Position => position;
        [SerializeField] private Vector2Int position;

        [NonSerialized] public bool isFree = true;
        [NonSerialized] public bool isAvailable;
        [NonSerialized] public bool isPainted;
        
        [Header("Visual")]
        [SerializeField] private Material activeMaterial;

        private Color startColor;
        private MeshRenderer meshRenderer;

        public void Init()
        {
            thisTransform = transform;
            meshRenderer = GetComponent<MeshRenderer>();
            startColor = meshRenderer.material.color;
        }

        public void SetActive()
        {
            meshRenderer.material = activeMaterial;
            startColor = meshRenderer.material.color;
        }

        public void Paint(bool isPainting = true)
        {
            if (isPainting)
                meshRenderer.material.color = Color.green;
            isPainted = true;
        }

        public void Repaint()
        {
            meshRenderer.material.color = startColor;
            isPainted = false;
        }
    }
}
