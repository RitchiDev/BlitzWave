using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float m_RotationSpeed = 180;
    [SerializeField] private float m_StartMovementSpeed = 5;
    [SerializeField] private float[] m_Acelerations;
    private float m_Acceleration, m_MovementSpeed;
    private float m_DirectionInput;

    [Header("Misc")]
    [SerializeField] private Camera m_Camera;
    [SerializeField] private Transform m_ThirdPerson;
    [SerializeField] private LayerMask m_ThirdPersonMask;
    [SerializeField] private Transform m_FirstPerson;
    [SerializeField] private LayerMask m_FirstPersonMask;
    [SerializeField] private Toggle m_PerspectiveToggle;
    private bool m_PerspectiveInput;

    private Transform m_World, m_Rotator;
    private Pipe m_CurrentPipe;

    private float m_DistanceTraveled;
    private float m_DeltaToRotation;
    private float m_SystemRotation;
    private float m_WorldRotation, m_AvatarRotation;

    private bool m_IsDead;

    private void Awake()
    {
        m_World = PipeSystem.m_Instance.transform.parent;
        m_Rotator = transform.GetChild(0);

        m_IsDead = true;
        gameObject.SetActive(false);

        m_PerspectiveToggle.onValueChanged.AddListener(delegate { UpdateCameraPerspective(); });
    }

    public void StartGame(int accelerationMode)
    {
        m_DistanceTraveled = 0f;
        m_AvatarRotation = 0f;
        m_SystemRotation = 0f;
        m_WorldRotation = 0f;

        m_CurrentPipe = PipeSystem.m_Instance.SetupFirstPipe();
        SetUpCurrentPipe();

        m_Acceleration = m_Acelerations[accelerationMode];
        m_MovementSpeed = m_StartMovementSpeed;

        m_IsDead = false;
        gameObject.SetActive(true);

        HUD.m_Instance.SetValues(m_DistanceTraveled, m_MovementSpeed);
    }

    public void ContinueGame()
    {
        m_CurrentPipe = PipeSystem.m_Instance.SetupFirstPipe(false);
        SetUpCurrentPipe();

        m_MovementSpeed *= 0.7f;

        m_IsDead = false;
        gameObject.SetActive(true);

        HUD.m_Instance.SetValues(m_DistanceTraveled, m_MovementSpeed);
    }

    private void Update()
    {
        if(!m_IsDead)
        {
            m_MovementSpeed += m_Acceleration * Time.deltaTime;

            float delta = m_MovementSpeed * Time.deltaTime;
            m_DistanceTraveled += delta;

            m_SystemRotation += delta * m_DeltaToRotation;

            if (m_SystemRotation >= m_CurrentPipe.GetCurveAngle())
            {
                delta = (m_SystemRotation - m_CurrentPipe.GetCurveAngle()) / m_DeltaToRotation;
                m_CurrentPipe = PipeSystem.m_Instance.SetUpNextPipe();

                SetUpCurrentPipe();

                m_SystemRotation = delta * m_DeltaToRotation;
            }

            PipeSystem.m_Instance.transform.localRotation = Quaternion.Euler(0f, 0f, m_SystemRotation);

            UpdateAvatarRotation();

            HUD.m_Instance.SetValues(m_DistanceTraveled, m_MovementSpeed);

            //if(m_PerspectiveInput)
            //{
            //    UpdateCameraPerspective();
            //}
        }
    }

    public void TurnOffRotation()
    {
        m_IsDead = true;
    }

    public void Die()
    {
        GameOver.m_Instance.EndGame(m_DistanceTraveled);
        gameObject.SetActive(false);
    }

    private void UpdateAvatarRotation()
    {
        float rotationInput = 0f;

        if(Application.isMobilePlatform)
        {
            if(Input.touchCount == 1)
            {

                rotationInput = Input.GetTouch(0).position.x < Screen.width * 0.5f ? 1f : -1f;

                //if (Input.GetTouch(0).position.x < Screen.width * 0.5f)
                //{
                //    rotationInput = -1f;
                //}
                //else
                //{
                //    rotationInput = 1f;
                //}
            }
        }
        else
        {
            rotationInput = m_DirectionInput;
        }

        m_AvatarRotation += m_RotationSpeed * Time.deltaTime * rotationInput;

        if(m_AvatarRotation < 0f)
        {
            m_AvatarRotation += 360f;
        }
        else if(m_AvatarRotation > 360f)
        {
            m_AvatarRotation -= 360f;
        }

        m_Rotator.localRotation = Quaternion.Euler(m_AvatarRotation, 0f, 0f);
    }

    private void UpdateCameraPerspective()
    {
        if(m_PerspectiveToggle.isOn)
        {
            m_Camera.transform.position = m_ThirdPerson.position;
            m_Camera.cullingMask = m_ThirdPersonMask;
        }
        else
        {
            m_Camera.transform.position = m_FirstPerson.position;
            m_Camera.cullingMask = m_FirstPersonMask;
        }
    }

    private void SetUpCurrentPipe()
    {
        m_DeltaToRotation = 360f / (2f * Mathf.PI * m_CurrentPipe.GetCurveRadius());
        m_WorldRotation += m_CurrentPipe.GetRelativeRotation();
        if(m_WorldRotation < 0f)
        {
            m_WorldRotation += 360f;
        }
        else if(m_WorldRotation >= 360f)
        {
            m_WorldRotation -= 360f;
        }

        m_World.localRotation = Quaternion.Euler(m_WorldRotation, 0f, 0f);
    }

    public void GetDirectionInfo(InputAction.CallbackContext context)
    {
        m_DirectionInput = context.ReadValue<float>();
    }

    public void GetPerspectiveInfo(InputAction.CallbackContext context)
    {
        m_PerspectiveInput = context.performed;
    }
}
