using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
   [SerializeField] private float speed;
   [SerializeField] private float jumpPower;
   [SerializeField] private LayerMask groundMask;
   [SerializeField] private float screenWrapSize = 4.5f;
   [SerializeField] private AudioSource walkAudio;
   [SerializeField] private AudioSource headAudio;
   [SerializeField] private AudioSource feetAudio;
   [SerializeField] private Camera cameraMain;
   private PlayerInput _playerInput;
   private Rigidbody2D _rB;
   //move controls are stored here
   private InputAction _moveAction;
   private InputAction _jumpAction;
   private Vector3 _moveByInput;
   private Coroutine _waitLandCoroutine;
   //box collider to detect grounded
   private BoxCollider2D _boxCol;
   //animator stuff
   private Animator _animator;
   private static readonly int IsRunning = Animator.StringToHash("IsRunning");
   private static readonly int IsInAir = Animator.StringToHash("IsInAir");
   private void Awake()
   {
      SetScreenWrapSize();
      _playerInput = GetComponent<PlayerInput>();
      _moveAction = _playerInput.actions["Move"];
      _jumpAction = _playerInput.actions["Jump"];
      _rB = GetComponent<Rigidbody2D>();
      _boxCol = GetComponent<BoxCollider2D>();
      _animator = GetComponent<Animator>();
   }

   private void SetScreenWrapSize()
   {
      Vector2 screenBounds =
         cameraMain.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, cameraMain.transform.position.z));
      screenWrapSize = screenBounds.x;
   }
   private void OnEnable()
   {
      EnableControls();
   }
   private void OnDisable()
   {
      DisableControls();
      ResetController();
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
      _moveByInput = Vector3.zero;
      _animator.Rebind();
      _animator.Update(0f);
      FlipAnimation(false);
      _rB.velocity = Vector2.zero;
   }
   private void Jump(InputAction.CallbackContext context)
   {
      if (!IsPlayerGrounded()) return;
      _rB.velocity += Vector2.up * jumpPower;
      _animator.Play("Jump");
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
      Vector3 p = transform.position;
      _rB.velocity = new Vector2(_moveByInput.x * speed, _rB.velocity.y);
      if(p.x < -screenWrapSize || p.x > screenWrapSize) {
         float xPos = Mathf.Clamp(transform.position.x, screenWrapSize, -screenWrapSize);
         transform.position = new Vector3(xPos, p.y, p.z);
      }
      SetAnimParams();
   }

   private void SetAnimParams()
   {
      _animator.SetBool(IsInAir, !IsPlayerGrounded());
      _animator.SetBool(IsRunning, _moveByInput.x != 0 && IsPlayerGrounded());
      FlipAnimation(_moveByInput.x > 0);
   }

   private void FlipAnimation(bool isRight)
   {
      var scale = transform.localScale;
      if ((scale.x > 0 && isRight)||(scale.x < 0 && !isRight))
      {
         scale.x *= -1;
      }
      transform.localScale = scale;
   }

   public bool IsPlayerGrounded()
   {
      return Physics2D.BoxCast(_boxCol.transform.position, _boxCol.size/2, 0f, Vector2.down, .1f, groundMask);
   }

   public void PlayStepSound()
   {
      walkAudio.Play(0);
   }

   public void PlayCollideSound(bool isLanding)
   {
      if (isLanding)
      {
         feetAudio.Play(0);
         return;
      }
      headAudio.Play(0);
   }
}
