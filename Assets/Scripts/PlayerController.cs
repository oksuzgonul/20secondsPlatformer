using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
   [SerializeField] private float speed;
   [SerializeField] private float jumpPower;
   [SerializeField] private LayerMask groundMask;
   private PlayerInput _playerInput;
   private Rigidbody2D _rB;
   //move controls are stored here
   private InputAction _moveAction;
   private InputAction _jumpAction;
   private Vector3 _moveByInput;
   private const int MaxJump = 2;
   private int _jumpCount;
   private Coroutine _waitLandCoroutine;
   //box collider to detect grounded
   private BoxCollider2D _boxCol;

   private void Awake()
   {
      _playerInput = GetComponent<PlayerInput>();
      _moveAction = _playerInput.actions["Move"];
      _jumpAction = _playerInput.actions["Jump"];
      _rB = GetComponent<Rigidbody2D>();
      _boxCol = GetComponent<BoxCollider2D>();
   }
   private void OnEnable()
   {
      EnableControls();
   }
   private void OnDisable()
   {
      ResetController();
      DisableControls();
   }
   private void EnableControls()
   {
      _moveAction.performed += Move;
      _moveAction.canceled += Move;
      _jumpAction.performed += Jump;
   }
   private void DisableControls()
   {
      _moveAction.performed -= Move;
      _moveAction.canceled -= Move;
      _jumpAction.performed -= Jump;
   }
   private void ResetController()
   {
      StopAllCoroutines();
      _jumpCount = 0;
      _moveByInput = Vector3.zero;
   }
   private void Jump(InputAction.CallbackContext context)
   {
      if (_jumpCount >= MaxJump)
      {
         if (_waitLandCoroutine != null) StopCoroutine(_waitLandCoroutine);
         _waitLandCoroutine = StartCoroutine(WaitUntilLanded());
         return;
      }
      _rB.velocity += Vector2.up * jumpPower;
      _jumpCount += 1;
   }
   private IEnumerator WaitUntilLanded()
   {
      yield return new WaitUntil(IsPlayerGrounded);
      _jumpCount = 0;
   }
   private void Move(InputAction.CallbackContext context)
   {
      var xInput = context.ReadValue<Vector2>().x;
      //less control on air
      if (!IsPlayerGrounded()) xInput /= 2;
      _moveByInput = new Vector3( xInput, 0, 0);
   }
   private void FixedUpdate()
   {
      _rB.velocity = new Vector2(_moveByInput.x * speed, _rB.velocity.y);
   }
   public bool IsPlayerGrounded()
   {
      return Physics2D.BoxCast(_boxCol.transform.position, _boxCol.size, 0f, Vector2.down, .1f, groundMask);
   }
}
