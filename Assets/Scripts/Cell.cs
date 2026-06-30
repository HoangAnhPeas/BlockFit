using UnityEngine;

public class Cell : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material normalMat;
    [SerializeField] private Material validMat;
    [SerializeField] private Material invalidMat;

    public int x;
    public int y;

    public bool IsOccupied => OccupiedCube != null;

    public Transform OccupiedCube { get; private set; }

    public void Initialize(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    #region Occupy

    public void SetCube(Transform cube)
    {
        OccupiedCube = cube;
    }

    public void ClearCube()
    {
        if (OccupiedCube != null)
            Destroy(OccupiedCube.gameObject);

        OccupiedCube = null;
    }

    #endregion

    #region Preview

    public void SetPreview(bool valid)
    {
        meshRenderer.sharedMaterial = valid ? validMat : invalidMat;
    }

    public void ClearPreview()
    {
        meshRenderer.sharedMaterial = normalMat;
    }

    #endregion
}