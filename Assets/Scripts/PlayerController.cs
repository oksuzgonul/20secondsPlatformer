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
   
   private void Awake()
   {
      _playerInput = GetComponent<PlayerInput>();
      _moveAction = _playerInput.actions["Move"];
      _jumpAction = _playerInput.actions["Jump"];
      _characterController = GetComponent<CharacterController>();
   }

   private void OnEnable()
   {
      _moveAction.performed += OnWasdPressed;
      _moveAction.canceled += OnWasdReleased;
      _jumpAction.performed += Jump;
   }
   
   private void OnDisable()
   {
      _moveAction.performed -= OnWasdPressed;
      _moveAction.canceled -= OnWasdReleased;
      _jumpAction.performed -= Jump;
   }

   private void Jump(InputAction.CallbackContext context)
   {
      Move(Vector3.up, jumpPower);
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
      if (_wasdDown) Move(_moveByInput, speed);
      if (!_characterController.isGrounded) Move(Vector3.down, gravityConstant);
   }
}
