using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEditor;
using System.Net;


public class FirstPersonController : MonoBehaviour
{
    private Rigidbody rb;
    private int health = 100;
    public GameObject inDialogue;


    #region Camera Movement Variables

    public Camera playerCamera;

    public float fov = 60f;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 50f;

    public bool lockCursor = true;
    public bool crosshair = true;
    public Sprite crosshairImage;
    public Color crosshairColor = Color.white;

    private float yaw = 0.0f;
    private float pitch = 0.0f;
    private Image crosshairObject;
    #endregion

    #region Movement Variables

    public float walkSpeed = 5f;
    public float maxVelocityChange = 10f;

    #region Sprint
    public GameObject sprintPanel;
    RectTransform rt;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public float sprintSpeed = 70f;
    public float sprintDuration = 5f;
    public float sprintCooldown = .5f;
    public float sprintFOV = 80f;
    public float sprintFOVStepTime = 10f;

    // Internal Variables
    private bool isSprinting = false;
    private float sprintRemaining;
    private bool isSprintCooldown = false;
    private float sprintCooldownReset;

    #endregion

    #region Jump

    public KeyCode jumpKey = KeyCode.Space;
    public float jumpPower = 5f;

    // Internal Variables
    public bool isGrounded = false;
    private float lastframey = 0f;
    #endregion

    #region Crouch
    public KeyCode crouchKey = KeyCode.LeftControl;
    public float crouchHeight = .75f;
    public float speedReduction = .5f;

    // Internal Variables
    private bool isCrouched = false;
    private Vector3 originalScale;

    #endregion
    #endregion
    public float slopeLimit = 45f;
    public float slideSpeed = 5f;
    private bool isSliding = false;
    private Vector3 slideVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        crosshairObject = GetComponentInChildren<Image>();

        playerCamera.fieldOfView = fov;
        originalScale = transform.localScale;

        sprintRemaining = sprintDuration;
        sprintCooldownReset = sprintCooldown;
    }

    void Start()
    {
        rt = sprintPanel.GetComponent<RectTransform>();
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        if(crosshair)
        {
            crosshairObject.sprite = crosshairImage;
            crosshairObject.color = crosshairColor;
        }
        else
        {
            crosshairObject.gameObject.SetActive(false);
        }
    }

    float camRotation;

    private void Update()
    {
        #region Camera
        yaw = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * mouseSensitivity;


        pitch -= mouseSensitivity * Input.GetAxis("Mouse Y");
        pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

        transform.localEulerAngles = new Vector3(0, yaw, 0);
        playerCamera.transform.localEulerAngles = new Vector3(pitch, 0, 0);
        #endregion

        #region Sprint
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, sprintRemaining * 20);
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, sprintRemaining * 20/2);
        if (isSprinting)
        {
            rt.gameObject.SetActive(true);
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, sprintFOV, sprintFOVStepTime * Time.deltaTime);

            sprintRemaining -= 1 * Time.deltaTime;
            if (sprintRemaining <= 0)
            {
                isSprinting = false;
                isSprintCooldown = true;
            }
            // Debug.Log(sprintRemaining);
        }
        else if (sprintRemaining == sprintDuration)
        {
            rt.gameObject.SetActive(false);
        }
        else
        {
            rt.gameObject.SetActive(true);
            sprintRemaining = Mathf.Clamp(sprintRemaining += 1 * Time.deltaTime, 0, sprintDuration);
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, fov, sprintFOVStepTime * Time.deltaTime);
        }
            if(isSprintCooldown)
            {
                sprintCooldown -= 1 * Time.deltaTime;
                if (sprintCooldown <= 0)
                {
                    isSprintCooldown = false;
                }
            }
            else
            {
                sprintCooldown = sprintCooldownReset;
            }

        #endregion

        #region Jump
        
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            if (isGrounded)
            {
                rb.AddForce(0f, jumpPower, 0f, ForceMode.Impulse);
                isGrounded = false;
            }
        }

        #endregion

        #region Crouch

            if(Input.GetKeyDown(crouchKey))
            {
                isCrouched = false;
                Crouch();
            }
            else if(Input.GetKeyUp(crouchKey))
            {
                isCrouched = true;
                Crouch();
            }

        #endregion

        CheckGround();
    }

    void FixedUpdate()
    {
        SlideCheck();
        #region Movement
        // Calculate how fast we should be moving
        Vector3 targetVelocity = Vector3.zero;
        if (!inDialogue.activeSelf)
        {
            targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        }

        // All movement calculations while sprint is active
        if (Input.GetKey(sprintKey) && sprintRemaining > 0f && !isSprintCooldown)
        {
            targetVelocity = transform.TransformDirection(targetVelocity) * sprintSpeed;

            // Apply a force that attempts to reach our target velocity
            Vector3 velocity = rb.linearVelocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;

            // Player is only moving when valocity change != 0
            // Makes sure fov change only happens during movement
            if (velocityChange.x != 0 || velocityChange.z != 0)
            {
                isSprinting = true;

                if (isCrouched)
                {
                    Crouch();
                }
            }

            rb.AddForce(velocityChange, ForceMode.VelocityChange);
        }
        // All movement calculations while walking
        else
        {
            isSprinting = false;
            targetVelocity = transform.TransformDirection(targetVelocity) * walkSpeed;

            // Apply a force that attempts to reach our target velocity
            Vector3 velocity = rb.linearVelocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;

            rb.AddForce(velocityChange, ForceMode.VelocityChange);
        }

        #endregion
    }
    private void CheckGround()
    {
        Vector3 origin = new Vector3(transform.position.x, transform.position.y - (transform.localScale.y * .5f), transform.position.z);
        float distance = .75f;

        if (Physics.Raycast(origin, transform.TransformDirection(Vector3.down), out RaycastHit hit, distance))
        {
            //Debug.DrawRay(origin, transform.TransformDirection(Vector3.down) * distance, Color.red);
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        // Check if player is on ground
    }
    private void SlideCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 11f)) // Adjust raycast distance
        {
            Debug.Log("bruh");
            float angle = Vector3.Angle(Vector3.up, hit.normal);

            if (angle > slopeLimit)
            {
                // Calculate slide direction along the slope
                Vector3 slideDirection = Vector3.ProjectOnPlane(Vector3.down, hit.normal).normalized;
                rb.AddForce(slideDirection * slideSpeed, ForceMode.Acceleration);
                rb.AddForce(Vector3.down * slideSpeed);
                Debug.Log("Sliding");
            }
        }
    }
    private void Crouch()
    {
        if (isCrouched)
        {
            transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
            walkSpeed /= speedReduction;

            isCrouched = false;
        }
        else
        {
            transform.localScale = new Vector3(originalScale.x, crouchHeight, originalScale.z);
            walkSpeed *= speedReduction;

            isCrouched = true;
        }
    }
}
