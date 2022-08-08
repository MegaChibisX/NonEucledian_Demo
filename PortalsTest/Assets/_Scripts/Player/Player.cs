using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public Camera cmr;
    public Rigidbody body;
    protected CapsuleCollider col;
    public Gravity gravity;

    public bool hasInput = false;
    public float moveSpeed = 10f;
    public float jumpForce = 25f;
    public Vector2 cmrRotateSpeed = new Vector2(360f, -270f);

    public float portalCooldown = 0.0f;
    public HashSet<Portal> portalsInProgress;

    public Vector3 up { get { return transform.up * transform.localScale.x; } }
    public Vector3 right { get { return transform.right * transform.localScale.y; } }
    public Vector3 forward { get { return transform.forward * transform.localScale.z; } }
    public float scale { get { return transform.localScale.y; } }

    public void Start()
    {
        cmr = Camera.main;
        body = GetComponent<Rigidbody>();
        col = GetComponentInChildren<CapsuleCollider>();
        gravity = new Gravity(gravity, this);

        portalsInProgress = new HashSet<Portal>();
    }
    private void Update()
    {
        HandleInput();
    }
    private void FixedUpdate()
    {
        ApplyGravity();
        Move();

        if (portalCooldown > 0.0f)
            portalCooldown -= Time.fixedDeltaTime;
    }


    public void HandleInput()
    {
        if (Input.GetButtonDown("Jump"))
            body.velocity = Vector3.ProjectOnPlane(body.velocity, transform.up) + up * jumpForce;
    }
    public void Move()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (input.sqrMagnitude > 1)
            input.Normalize();

        transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, gravity.GlobalNormal()), -gravity.GlobalNormal());
        if (Input.GetMouseButtonDown(2))
        {
            hasInput = !hasInput;
            Cursor.lockState = hasInput ? CursorLockMode.Locked : CursorLockMode.None;
        }
        if (hasInput)
        {
            transform.rotation = Quaternion.AngleAxis(Input.GetAxisRaw("Mouse X") * -cmrRotateSpeed.x * Time.fixedDeltaTime, gravity.GlobalNormal()) * transform.rotation;
            cmr.transform.rotation = Quaternion.AngleAxis(Input.GetAxisRaw("Mouse Y") * cmrRotateSpeed.y * Time.fixedDeltaTime, cmr.transform.right) * cmr.transform.rotation;
        }

        body.velocity = Vector3.Project(body.velocity, transform.up) + (transform.forward * input.y + transform.right * input.x) * moveSpeed * scale;
        SetClosePrimary();
    }
    public void ApplyGravity()
    {
        gravity.ApplyGravity();
    }


    public void Teleport(Portal portal)
    {
        Portal near= portal;
        Portal far = portal.otherSide;

        Vector3 posDiff = Helper.TransformVectorFromTo(near.transform.position - transform.position, near.transform, far.transform);
        posDiff = Helper.TransformScaleFromTo(-posDiff, near.transform, far.transform);
        transform.position = far.transform.position + posDiff;
        transform.rotation = Quaternion.LookRotation(Helper.TransformVectorFromTo(transform.forward, near.transform, far.transform),
                                                     Helper.TransformVectorFromTo(transform.up     , near.transform, far.transform));
        transform.localScale = Helper.TransformScaleFromTo(transform.localScale, near.transform, far.transform);
        body.velocity = Helper.TransformScaleFromTo(body.velocity, near.transform, far.transform);
        portalCooldown = 0.1f;

        gravity = new Gravity(far.gravity, this);

        RenderSettings.skybox = far.skyBox;
    }
    void SetClosePrimary()
    {
        Portal target = null;
        foreach (Portal portal in FindObjectsOfType<Portal>())
        {
            if (target == null || (portal.transform.position - transform.position).sqrMagnitude < (target.transform.position - transform.position).sqrMagnitude)
                target = portal;
        }
    }


    float lastEntryMul = 1;
    public void OnTriggerEnter(Collider other)
    {
        if (portalCooldown <= 0.0f && other.attachedRigidbody && other.attachedRigidbody.GetComponent<Portal>() != null)
        {
            lastEntryMul = Vector3.Dot(other.transform.forward, (other.transform.position - transform.position).normalized);
        }
    }
    public void OnTriggerStay(Collider other)
    {
        if (other.attachedRigidbody && other.attachedRigidbody.GetComponent<Portal>() != null)
        {
            Portal portal = other.attachedRigidbody.GetComponent<Portal>();

            if (portalCooldown <= 0.0f)
            {
                if (Vector3.Dot(other.transform.forward, (other.transform.position - transform.position).normalized) * lastEntryMul < 0 &&
                    !portalsInProgress.Contains(portal))
                {
                    Teleport(portal);
                    lastEntryMul *= -1;

                    if (!portalsInProgress.Contains(portal))
                        portalsInProgress.Add(portal);
                }
            }
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody && other.attachedRigidbody.GetComponent<Portal>() && portalsInProgress.Contains(other.attachedRigidbody.GetComponent<Portal>()))
            portalsInProgress.Remove(other.attachedRigidbody.GetComponent<Portal>());
    }


    public bool isGrounded(float multi = 1.1f)
    {
        return Physics.CheckSphere(transform.position - transform.up * col.height * 0.5f * multi * transform.localScale.y, col.radius * transform.localScale.z, 1 << 8);
    }

}
