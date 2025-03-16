using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : HealthEntity, IFactioned, IProjectileSource
{
    [Header("PlayerController")]
    [SerializeField] private Faction faction;
    public Faction Faction => faction;

    public float baseMoveSpeed;
    [ReadOnly] public float currentMoveSpeed;

    [SerializeField] private Transform projectileSpawnPoint;
    public Transform ProjectileSpawnPoint => projectileSpawnPoint;

    [Header("Global Links")]
    public Vector3Variable playerPositionVariable;

    private PlayerInputActions controls;
    private InputAction moveInput;
    private InputAction lookInput;
    private InputAction mouse1Input;

    private Vector2 moveDirection;
    private Vector2 lookDirection;
    private Rigidbody rb => GetComponent<Rigidbody>();

    private AbilitiesController abilitiesController => GetComponent<AbilitiesController>();

    protected override void Awake()
    {
        base.Awake();
        controls = new();

        currentMoveSpeed = baseMoveSpeed;
    }

    private void OnEnable()
    {
        moveInput = controls.Player.Move;
        moveInput.Enable();

        lookInput = controls.Player.Look;
        lookInput.Enable();

        mouse1Input = controls.Player.Fire;
        mouse1Input.Enable();
    }

    private void OnDisable()
    {
        moveInput.Disable();
        lookInput.Disable();
        mouse1Input.Disable();
    }

    private void Update()
    {
        moveDirection = moveInput.ReadValue<Vector2>();
        lookDirection = lookInput.ReadValue<Vector2>();

        if (!abilitiesController) return;

        if (mouse1Input.WasPressedThisFrame())
            abilitiesController.PerformMouse01();

        abilitiesController.mouse1WasPressed = mouse1Input.WasPressedThisFrame();
    }

    private void FixedUpdate()
    {
        Vector3 direction = new Vector3(moveDirection.x, 0, moveDirection.y).normalized;
        //rb.velocity = direction * currentMoveSpeed;
        rb.AddForce(direction * currentMoveSpeed);

        Ray mouseRay = Camera.main.ScreenPointToRay(lookDirection);

        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, 0, 0)); //basically, plane orientation and plane world height

        if (groundPlane.Raycast(mouseRay, out float rayLength))
        {
            Vector3 pointToLookAt = mouseRay.GetPoint(rayLength); //get point along mouseRay where it intersects with groundPlane
            transform.LookAt(new Vector3(pointToLookAt.x, transform.position.y, pointToLookAt.z)); //rotate, but do not include y to avoid y axis movement
            abilitiesController.worldPosFromMousePos = pointToLookAt;
        }

        playerPositionVariable.Value = transform.position;
    }
}
