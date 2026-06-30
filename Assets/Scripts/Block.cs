using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Block : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IPointerUpHandler
{
    [Header("Visual")]
    [SerializeField] Transform cubePrefab;

    [SerializeField] private Transform visualRoot;

    [SerializeField] float spacing = 1.05f;

    [Header("Drag Feel")]
    [SerializeField] private float followSpeed = 18f;
    [SerializeField] private float liftHeight = 0.5f;

    [Header("Tilt")]
    [SerializeField] private float tiltAmount = 12f;
    [SerializeField] private float tiltSmooth = 12f;

    [Header("Scale")]
    [SerializeField] private float dragScale = 1.12f;

    public IReadOnlyList<Vector2Int> Cells => ShapeData.cells;

    public IReadOnlyList<Transform> Cubes => cubes;

    private Board board;

    public Vector2Int CurrentOrigin { get; set; }
    public bool IsPlaced { get; set; }

    private Vector3 targetPosition;
    private Vector3 dragVelocity;
    private Vector3 lastPosition;

    private Quaternion targetRotation;
    private bool isDragging;

    private Vector3 pointerWorld;

    private readonly List<Transform> cubes = new();

    public ShapeData ShapeData { get; private set; }

    //----------------------------------

    Camera cam;

    Plane dragPlane;

    Vector3 offset;

    Vector3 startPosition;

    //----------------------------------

    public event System.Action<Block> OnPlaced;

    void Awake()
    {
        cam = Camera.main;
    }

    void Start()
    {
        board = FindFirstObjectByType<Board>();
    }

    void Update()
    {
        if (isDragging)
        {
            UpdateDrag();
        }
        else
        {
            UpdateIdle();
        }
    }

    void UpdateDrag()
    {
        // acceleration
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition + Vector3.up * liftHeight,
            followSpeed * Time.deltaTime);

        // velocity
        dragVelocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;

        // tilt
        float tiltX = dragVelocity.y * tiltAmount * 0.05f;
        float tiltY = -dragVelocity.x * tiltAmount * 0.05f;

        targetRotation = Quaternion.Euler(
            tiltX,
            tiltY,
            0);

        visualRoot.localRotation = Quaternion.Slerp(
            visualRoot.localRotation,
            targetRotation,
            tiltSmooth * Time.deltaTime);

        // Scale
        visualRoot.localScale = Vector3.Lerp(
            visualRoot.localScale,
            Vector3.one * dragScale,
            10f * Time.deltaTime);
    }

    void UpdateIdle()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            14f * Time.deltaTime);

        visualRoot.localRotation = Quaternion.Slerp(
            visualRoot.localRotation,
            Quaternion.identity,
            8f * Time.deltaTime);

        visualRoot.localScale = Vector3.Lerp(
            visualRoot.localScale,
            Vector3.one,
            8f * Time.deltaTime);
    }

    public void Build(ShapeData shape)
    {
        ShapeData = shape;

        visualRoot.localRotation = Quaternion.identity;
        visualRoot.localScale = Vector3.one;

        Clear();

        foreach (Vector2Int cell in shape.cells)
        {
            Transform cube = Instantiate(cubePrefab, visualRoot);

            cube.localPosition = new Vector3(
                cell.x * spacing,
                cell.y * spacing,
                0
            );

            cubes.Add(cube);
        }

        targetPosition = transform.position;
        startPosition = transform.position;
    }

    void Clear()
    {
        foreach (Transform cube in cubes)
        {
            Destroy(cube.gameObject);
        }

        cubes.Clear();
    }

    //dragging
    public void OnPointerDown(PointerEventData eventData)
    {
        if (IsPlaced)
        {
                board.Remove(this);
        }

        isDragging = true;

        targetPosition = transform.position;
        lastPosition = transform.position;

        startPosition = transform.position;

        dragPlane = new Plane(
            Vector3.forward,
            transform.position
        );

        Ray ray = cam.ScreenPointToRay(eventData.position);

        if (dragPlane.Raycast(ray, out float distance))
        {
            offset = transform.position - ray.GetPoint(distance);
        }
    }

    //preview
    public void OnDrag(PointerEventData eventData)
    {
        Ray ray = cam.ScreenPointToRay(eventData.position);

        if (dragPlane.Raycast(ray, out float distance))
        {
            pointerWorld = ray.GetPoint(distance);
            targetPosition = pointerWorld + offset;
        }

        Vector2Int origin = board.WorldToGrid(targetPosition); //targetPosition
        board.ShowPreview(this, origin);
    }

    //drop
    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;

        Vector2Int origin = board.WorldToGrid(targetPosition);

        if (board.CanPlace(this, origin))
        {
            board.Place(this, origin);
            OnPlaced?.Invoke(this);
            Destroy(gameObject);

            IsPlaced = true;
        }
        else
        {
            ReturnToStart();
        }
    }

    //return to start position if not placed
    public void ReturnToStart()
    {
        targetPosition = startPosition;
        transform.position = startPosition;
    }

    public void SnapTo(Vector3 worldPos)
    {
        targetPosition = worldPos;
        transform.position = worldPos;
    }
}