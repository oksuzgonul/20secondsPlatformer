using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
   [SerializeField] private float speed;
   [SerializeField] private float gravityConstant;
   [SerializeField] private float jumpPower;
   private PlayerInput _playerInput;
   private CharacterController _characterController;
   //move controls are stored here
   private InputAction _moveAction;
   private InputAction _jumpAction;
   private bool _wasdDown;
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
      _characterController = GetComponent<CharacterController>();
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
      _moveAction.performed += OnWasdPressed;
      _moveAction.canceled += OnWasdReleased;
      _jumpAction.performed += Jump;
   }
   private void DisableControls()
   {
      _moveAction.performed -= OnWasdPressed;
      _moveAction.canceled -= OnWasdReleased;
      _jumpAction.performed -= Jump;
   }
   private void ResetController()
   {
      StopAllCoroutines();
      _jumpCount = 0;
      _jumpInitiated = false;
      _wasdDown = false;
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
      yield return new WaitUntil(() => _characterController.isGrounded);
      _jumpCount = 0;
      _jumpInitiated = false;
   }
   private IEnumerator EndJump()
   {
      yield return new WaitForSeconds(0.5f);
      _jumpInitiated = false;
      StartCoroutine(WaitUntilLanded());
   }
   private void OnWasdPressed(InputAction.CallbackContext context)
   {
      _wasdDown = true;
      _moveByInput = new Vector3( context.ReadValue<Vector2>().x, 0, 0);
   }
   private void OnWasdReleased(InputAction.CallbackContext context)
   {
      _wasdDown = false;
   }
   private void Move(Vector3 vec, float rate)
   {
      var direction = vec * ( rate * Time.deltaTime);
      _characterController.Move(direction);
   }
   private void Update()
   {
      if (_jumpInitiated) Move(Vector3.up, jumpPower);
      if (_wasdDown) Move(_moveByInput, speed);
      if (!_characterController.isGrounded) Move(Vector3.down, gravityConstant);
   }
}
