using System.Collections.Generic;
using UnityEngine;

public class ConvexMeshGenerator : MonoBehaviour
{
    [SerializeField]
    private int _updateCount = 10;

    [SerializeField]
    private float detectionRadius = 5f; // Taille de l'extraction du mesh

    [SerializeField]
    private LayerMask detectionLayer; // Layer pour détecter la planète

    private Dictionary<Transform, MeshCollider> meshColliderMap = new Dictionary<Transform, MeshCollider>();
    private int _counter = 0;

    void FixedUpdate()
    {
        if (_counter >= _updateCount)
        {
            _counter = 1;
            GenerateMesh();
        }
        else
        {
            _counter++;
        }
    }

    private void GenerateMesh()
    {
        MeshRenderer detectedRenderer = FindMeshRenderer();
        if (detectedRenderer != null)
        {
            GameObject targetObject = detectedRenderer.gameObject;
            Mesh sourceMesh = GetMeshFromRenderer(detectedRenderer);

            if (sourceMesh != null)
            {
                Mesh newMesh = ExtractMeshSegment(sourceMesh, detectedRenderer.transform);

                if (newMesh.vertexCount <= 255) // Limite pour un mesh convex
                {
                    UpdateMeshCollider(detectedRenderer.transform, newMesh);
                }
                else
                {
                    Debug.LogWarning("Le mesh extrait a trop de sommets pour être convex.");
                }
            }
        }
    }

    MeshRenderer FindMeshRenderer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayer);
        foreach (var hit in hits)
        {
            MeshRenderer renderer = hit.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                return renderer;
            }
        }
        return null;
    }

    Mesh GetMeshFromRenderer(MeshRenderer renderer)
    {
        MeshFilter filter = renderer.GetComponent<MeshFilter>();
        return filter ? filter.mesh : null;
    }

    Mesh ExtractMeshSegment(Mesh originalMesh, Transform meshTransform)
    {
        List<Vector3> newVertices = new List<Vector3>();
        List<int> newTriangles = new List<int>();

        Vector3 localPlayerPos = meshTransform.InverseTransformPoint(transform.position);
        float radiusSqr = detectionRadius * detectionRadius;
        Vector3[] vertices = originalMesh.vertices;
        int[] triangles = originalMesh.triangles;

        Dictionary<int, int> vertexMap = new Dictionary<int, int>();

        for (int i = 0; i < triangles.Length; i += 3)
        {
            int v1 = triangles[i], v2 = triangles[i + 1], v3 = triangles[i + 2];
            Vector3 p1 = vertices[v1], p2 = vertices[v2], p3 = vertices[v3];

            if ((p1 - localPlayerPos).sqrMagnitude <= radiusSqr ||
                (p2 - localPlayerPos).sqrMagnitude <= radiusSqr ||
                (p3 - localPlayerPos).sqrMagnitude <= radiusSqr)
            {
                int i1 = GetOrAddVertex(v1, p1, vertexMap, newVertices);
                int i2 = GetOrAddVertex(v2, p2, vertexMap, newVertices);
                int i3 = GetOrAddVertex(v3, p3, vertexMap, newVertices);

                newTriangles.Add(i1);
                newTriangles.Add(i2);
                newTriangles.Add(i3);
            }
        }

        // Limite des 255 sommets pour le MeshCollider convex
        if (newVertices.Count > 255)
        {
            newVertices = newVertices.GetRange(0, 255);

            // Filtrage des triangles pour ne garder que ceux contenant uniquement des indices valides
            List<int> filteredTriangles = new List<int>();
            for (int i = 0; i < newTriangles.Count; i += 3)
            {
                if (newTriangles[i] < 255 && newTriangles[i + 1] < 255 && newTriangles[i + 2] < 255)
                {
                    filteredTriangles.Add(newTriangles[i]);
                    filteredTriangles.Add(newTriangles[i + 1]);
                    filteredTriangles.Add(newTriangles[i + 2]);
                }
            }
            newTriangles = filteredTriangles;
        }

        Mesh newMesh = new Mesh();
        newMesh.vertices = newVertices.ToArray();
        newMesh.triangles = newTriangles.ToArray();
        newMesh.RecalculateNormals();
        newMesh.RecalculateBounds();

        return newMesh;
    }

    int GetOrAddVertex(int originalIndex, Vector3 originalVertex, Dictionary<int, int> vertexMap, List<Vector3> newVertices)
    {
        if (!vertexMap.ContainsKey(originalIndex))
        {
            int newIndex = newVertices.Count;
            vertexMap[originalIndex] = newIndex;
            newVertices.Add(originalVertex);
            return newIndex;
        }
        return vertexMap[originalIndex];
    }

    void UpdateMeshCollider(Transform detectedObject, Mesh newMesh)
    {
        if (!meshColliderMap.TryGetValue(detectedObject, out MeshCollider meshCollider))
        {

            // 🔹 Création d'un nouvel objet enfant pour le MeshCollider
            GameObject colliderObject = new GameObject("GeneratedMeshCollider");

            if (detectedObject.parent != null)
                colliderObject.transform.SetParent(detectedObject.parent, false);

            colliderObject.layer = LayerMask.NameToLayer("Default"); // Assurez-vous que le layer est bien défini

            // 🔹 Ajout du MeshCollider et stockage dans le dictionnaire
            meshCollider = colliderObject.AddComponent<MeshCollider>();
            meshCollider.convex = true;
            meshColliderMap[detectedObject] = meshCollider;
        }

        // 🛠️ Mise à jour du mesh du MeshCollider
        meshCollider.sharedMesh = newMesh;
    }
}