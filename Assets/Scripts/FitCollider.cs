using UnityEngine;

public class FitCollider : MonoBehaviour
{
    [SerializeField] private BoxCollider boxCollider;

    public void Init()
    {
        FitColliderToChilds();
    }

    private void FitColliderToChilds()
    {
        // Check for box collider
        if (boxCollider == null) return;

        // Get mesh filter from all model childrens
        MeshFilter[] meshFilters = transform.GetComponentsInChildren<MeshFilter>();
        if (meshFilters.Length == 0) return;

        // Helper to check if we started calculating the box
        bool boundsInitialized = false;
        Bounds localBounds = new Bounds();

        // Loop through each mesh to calculate its points in local space
        foreach (MeshFilter mf in meshFilters)
        {
            // Get the actual mesh data from the MeshFilter
            Mesh mesh = mf.sharedMesh;
            if (mesh == null) continue;

            // Get the 8 corners of the mesh bounds
            Bounds meshBounds = mesh.bounds;
            Vector3[] corners = new Vector3[8];

            // Convert each corner to this object's local space
            Vector3 min = meshBounds.min;
            Vector3 max = meshBounds.max;
            corners[0] = new Vector3(min.x, min.y, min.z);
            corners[1] = new Vector3(min.x, min.y, max.z);
            corners[2] = new Vector3(min.x, max.y, min.z);
            corners[3] = new Vector3(min.x, max.y, max.z);
            corners[4] = new Vector3(max.x, min.y, min.z);
            corners[5] = new Vector3(max.x, min.y, max.z);
            corners[6] = new Vector3(max.x, max.y, min.z);
            corners[7] = new Vector3(max.x, max.y, max.z);

            // Loop through the 8 corners of each mesh
            for (int i = 0; i < 8; i++)
            {
                // Convert the corner from child space to world space, then to parent local space
                Vector3 worldPt = mf.transform.TransformPoint(corners[i]);
                Vector3 localPt = transform.InverseTransformPoint(worldPt);

                // If it's the first corner, start the box here
                if (!boundsInitialized)
                {
                    localBounds = new Bounds(localPt, Vector3.zero);
                    boundsInitialized = true;
                }
                // For other corners, make the box bigger to include them
                else
                {
                    localBounds.Encapsulate(localPt);
                }
            }
        }

        // Apply calculated bounds to the collider
        if (boundsInitialized)
        {
            boxCollider.center = localBounds.center;
            boxCollider.size = localBounds.size;
        }
    }
}
