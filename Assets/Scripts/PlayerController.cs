using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
   [SerializeField] private float speed;
   private PlayerInput _playerInput;
   private CharacterController _characterController;

   //move controls are stored here
   private InputAction _moveAction;
   private InputAction _jumpAction;
   
   
   private bool _wasdDown = false;
   private Vector2 _move;
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

   private void Jump(InputAction.CallbackContext obj)
   {
      Debug.Log("Jump!");
   }

   private void OnWasdPressed(InputAction.CallbackContext context)
   {
      _wasdDown = true;
      _move = context.ReadValue<Vector2>();
   }
   
   private void OnWasdReleased(InputAction.CallbackContext context)
   {
      _wasdDown = false;
      _move = context.ReadValue<Vector2>();
   }
   private void Move()
   {
      var target = new Vector3(_move.x, _move.y, 0.0f);
      var direction = target.normalized * (speed * Time.deltaTime);
      _characterController.Move(direction);
   }

   private void Update()
   {
      if (_wasdDown) Move();
   }
}
