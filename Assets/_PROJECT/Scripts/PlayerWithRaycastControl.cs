using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

[RequireComponent(typeof(NetworkTransform))]
[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(CharacterController))]
public class PlayerWithRaycastControl : NetworkBehaviour
{
    [SerializeField] private float walkSpeed = 3.5f;
    [SerializeField] private float runSpeedOffset = 2.0f;
    [SerializeField] private float rotationSpeed = 3.5f;
    [SerializeField] private Vector2 defaultInitialPositionOnPlane = new Vector2(-4, 4);

    [SerializeField] private NetworkVariable<Vector3> networkPositionDirection = new NetworkVariable<Vector3>();
    [SerializeField] private NetworkVariable<Vector3> networkRotationDirection = new NetworkVariable<Vector3>();
    [SerializeField] private NetworkVariable<PlayerState> networkPlayerState = new NetworkVariable<PlayerState>();
    [SerializeField] private NetworkVariable<float> networkPlayerHealth = new NetworkVariable<float>(1000);
    [SerializeField] private NetworkVariable<float> networkPlayerPunchBlend = new NetworkVariable<float>();

    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject rightHand;
    [SerializeField] private float minPunchDistance = 1.0f;

    private CharacterController characterController;
    private Animator animator;

    // Jump 
    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -9.81f;
    private float verticalVelocity;
    private bool isGrounded;

    private Vector3 oldInputPosition = Vector3.zero;
    private Vector3 oldInputRotation = Vector3.zero;
    private PlayerState oldPlayerState = PlayerState.Idle;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError($"{nameof(PlayerWithRaycastControl)} requires a CharacterController. Please add one to the GameObject.");
        }

        animator = GetComponent<Animator>();
    }

    void Start()
    {
        if (IsClient && IsOwner)
        {
            transform.position = new Vector3(
                Random.Range(defaultInitialPositionOnPlane.x, defaultInitialPositionOnPlane.y),
                0,
                Random.Range(defaultInitialPositionOnPlane.x, defaultInitialPositionOnPlane.y)
            );

            var camRoot = transform.Find("PlayerCameraRoot");
            if (camRoot != null)
                PlayerCameraFollow.Instance.FollowPlayer(camRoot);
        }
    }

    void Update()
    {
        if (IsClient && IsOwner)
        {
            ClientInput();
        }

        ClientMoveAndRotate();
        ClientVisuals();
    }

    private void FixedUpdate()
    {
        if (IsClient && IsOwner)
        {
            if (networkPlayerState.Value == PlayerState.Punch && ActivePunchActionKey())
            {
                CheckPunch(leftHand.transform, Vector3.up);
                CheckPunch(rightHand.transform, Vector3.down);
            }
        }
    }

    private void CheckPunch(Transform hand, Vector3 aimDirection)
    {
        RaycastHit hit;
        int layerMask = LayerMask.GetMask("Player");

        if (Physics.Raycast(hand.position, hand.transform.TransformDirection(aimDirection), out hit, minPunchDistance, layerMask))
        {
            Debug.DrawRay(hand.position, hand.transform.TransformDirection(aimDirection) * minPunchDistance, Color.yellow);

            var playerHit = hit.transform.GetComponent<NetworkObject>();
            if (playerHit != null)
            {
                UpdateHealthServerRpc(1, playerHit.OwnerClientId);
            }
        }
        else
        {
            Debug.DrawRay(hand.position, hand.transform.TransformDirection(aimDirection) * minPunchDistance, Color.red);
        }
    }

    private void ClientMoveAndRotate()
    {
        if (characterController == null) return; // safety


        isGrounded = characterController.isGrounded;
        if (isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;

        Vector3 move = Vector3.zero;

        if (networkPositionDirection.Value != Vector3.zero)
        {
            move += networkPositionDirection.Value;
        }
        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        characterController.Move(move * Time.deltaTime);

        if (networkRotationDirection.Value != Vector3.zero)
        {
            transform.Rotate(networkRotationDirection.Value, Space.World);
        }

        if (isGrounded && networkPlayerState.Value == PlayerState.Jump)
        {

            if (IsClient && IsOwner)
            {
                UpdatePlayerStateServerRpc(PlayerState.Idle);
            }
        }
    }

    private void ClientVisuals()
    {
        if (oldPlayerState != networkPlayerState.Value)
        {
            oldPlayerState = networkPlayerState.Value;
            if (animator != null)
            {
                animator.SetTrigger($"{networkPlayerState.Value}");
                if (networkPlayerState.Value == PlayerState.Punch)
                {
                    animator.SetFloat($"{networkPlayerState.Value}Blend", networkPlayerPunchBlend.Value);
                }
            }
        }
    }


    public enum PlayerState
    {
        Idle,
        Walk,
        Run,
        ReverseWalk,
        Punch,
        Jump
    }

    private void ClientInput()
    {
        Vector3 inputRotation = new Vector3(0, Input.GetAxis("Horizontal"), 0);
        Vector3 direction = transform.TransformDirection(Vector3.forward);
        float forwardInput = Input.GetAxis("Vertical");
        Vector3 inputPosition = direction * forwardInput;

        // Jump input 
        if (Input.GetButtonDown("Jump") && isGrounded && characterController != null)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            UpdatePlayerStateServerRpc(PlayerState.Jump);
        }

        // Punch input
        if (ActivePunchActionKey() && forwardInput == 0)
        {
            UpdatePlayerStateServerRpc(PlayerState.Punch);
            return;
        }

        // Walking/running/idle
        if (forwardInput == 0)
            UpdatePlayerStateServerRpc(PlayerState.Idle);
        else if (!ActiveRunningActionKey() && forwardInput > 0 && forwardInput <= 1)
            UpdatePlayerStateServerRpc(PlayerState.Walk);
        else if (ActiveRunningActionKey() && forwardInput > 0 && forwardInput <= 1)
        {
            inputPosition = direction * runSpeedOffset;
            UpdatePlayerStateServerRpc(PlayerState.Run);
        }
        else if (forwardInput < 0)
            UpdatePlayerStateServerRpc(PlayerState.ReverseWalk);

        if (oldInputPosition != inputPosition || oldInputRotation != inputRotation)
        {
            oldInputPosition = inputPosition;
            oldInputRotation = inputRotation;
            UpdateClientPositionAndRotationServerRpc(inputPosition * walkSpeed, inputRotation * rotationSpeed);
        }
    }

    private static bool ActiveRunningActionKey()
    {
        return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    private static bool ActivePunchActionKey()
    {
        return Input.GetKey(KeyCode.Mouse0);
    }

    [ServerRpc]
    public void UpdateClientPositionAndRotationServerRpc(Vector3 newPosition, Vector3 newRotation)
    {
        networkPositionDirection.Value = newPosition;
        networkRotationDirection.Value = newRotation;
    }

    [ServerRpc]
    public void UpdateHealthServerRpc(int takeAwayPoint, ulong clientId)
    {
        var clientWithDamaged = NetworkManager.Singleton.ConnectedClients[clientId]
            .PlayerObject.GetComponent<PlayerWithRaycastControl>();

        if (clientWithDamaged != null && clientWithDamaged.networkPlayerHealth.Value > 0)
        {
            clientWithDamaged.networkPlayerHealth.Value -= takeAwayPoint;
        }

        NotifyHealthChangedClientRpc(takeAwayPoint, new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        });
    }

    [ClientRpc]
    public void NotifyHealthChangedClientRpc(int takeAwayPoint, ClientRpcParams clientRpcParams = default)
    {
        if (IsOwner) return;
        Logger.Instance.LogInfo($"Client got punch {takeAwayPoint}");
    }

    [ServerRpc]
    public void UpdatePlayerStateServerRpc(PlayerState state)
    {
        networkPlayerState.Value = state;
        if (state == PlayerState.Punch)
        {
            networkPlayerPunchBlend.Value = Random.Range(0.0f, 1.0f);
        }
    }
}
