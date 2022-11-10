using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
   [SerializeField] private float speed;
   [SerializeField] private float gravityConstant;
   [SerializeField] private float jumpPower;
   private PlayerInput _playerInput;
   private Rigidbody2D _rB;
   //move controls are stored here
   private InputAction _moveAction;
   private InputAction _jumpAction;
   private Vector3 _moveByInput;
   private bool _jumpInitiated;
   private const int MaxJump = 2;
   private int _jumpCount;
   private Coroutine _endJumpCoroutine;
   private Coroutine _waitLandCoroutine;

   private void Awake()
   {
      _playerInput = GetComponent<PlayerInput>();
      _moveAction = _playerInput.actions["Move"];
      _jumpAction = _playerInput.actions["Jump"];
      _rB = GetComponent<Rigidbody2D>();
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
      _jumpInitiated = false;
      _moveByInput = Vector3.zero;
   }
   private void Jump(InputAction.CallbackContext context)
   {
      if (_jumpInitiated) return;
      if (_jumpCount >= MaxJump)
      {
         if (_waitLandCoroutine != null) StopCoroutine(_waitLandCoroutine);
         _waitLandCoroutine = StartCoroutine(WaitUntilLanded());
         return;
      }
      _jumpInitiated = true;
      _jumpCount += 1;
      if (_endJumpCoroutine != null) StopCoroutine(_endJumpCoroutine);
      _endJumpCoroutine = StartCoroutine(EndJump());
   }
   private IEnumerator WaitUntilLanded()
   {
      yield return new WaitUntil(IsPlayerGrounded);
      _jumpCount = 0;
      _jumpInitiated = false;
   }
   private IEnumerator EndJump()
   {
      yield return new WaitForSeconds(0.5f);
      _jumpInitiated = false;
      StartCoroutine(WaitUntilLanded());
   }
   private void Move(InputAction.CallbackContext context)
   {
      _moveByInput = new Vector3( context.ReadValue<Vector2>().x, 0, 0);
   }
   private void FixedUpdate()
   {
      _rB.velocity = new Vector2(_moveByInput.x * speed, _rB.velocity.y);
   }

   public bool IsPlayerGrounded()
   {
      return _rB.velocity.y == 0;
   }
}
