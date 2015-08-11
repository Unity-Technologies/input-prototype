using UnityEngine;
using UnityEngine.InputNew;
using System.Linq;

public class CharacterInputController
	: MonoBehaviour
{
	public ControlMap controlMap;
	
	[Space( 10 )]
	public ControlMapEntry moveControlX;
	public ControlMapEntry moveControlY;
	public ControlMapEntry lookControlX;
	public ControlMapEntry lookControlY;
	public ControlMapEntry fireControl;
	
	[Space( 10 )]
	public float moveSpeed = 5;
	public Transform head;
	public GameObject projectile;
	public float timeBetweenShots = 0.5f;

	private ControlMapInstance _controlMapInstance;
	private float _timeOfLastShot;
	private Rigidbody _rigid;
	private Vector2 _rotation = Vector2.zero;

	public void Awake()
	{
		_controlMapInstance = InputSystem.BindInputs( controlMap ).First();
		_controlMapInstance.Activate();
		_rigid = GetComponent< Rigidbody >();
	}
	
	float _lastLookX;
	float _lastLookY;
	public void Update()
	{
		// Move
		if (moveControlX)
		{
			var moveX = _controlMapInstance[ moveControlX ].floatValue;
			var moveY = _controlMapInstance[ moveControlY ].floatValue;
			
			_rigid.velocity = transform.TransformDirection( new Vector3( moveX, 0, moveY ) ) * moveSpeed;
		}
		
		// Look
		var lookX = _controlMapInstance[ lookControlX ].floatValue;
		var lookY = _controlMapInstance[ lookControlY ].floatValue;
		
		// HACK UNTIL MOUSE IS RELATIVE
		lookX = lookX - _lastLookX;
		lookY = lookY - _lastLookY;
		_lastLookX = lookX + _lastLookX;
		_lastLookY = lookY + _lastLookY;
		
		_rotation.y += lookX;
		transform.localEulerAngles = new Vector3( 0, _rotation.y, 0 );
		
		_rotation.x = Mathf.Clamp( _rotation.x - lookY, -89, 89 );
		head.localEulerAngles = new Vector3( _rotation.x, 0, 0 );

		// Fire
		var fire = _controlMapInstance[ fireControl ].boolValue;
		if ( fire )
		{
			Debug.Log ("Fire");
			var currentTime = Time.time;
			var timeElapsedSinceLastShot = currentTime - _timeOfLastShot;
			if ( timeElapsedSinceLastShot > timeBetweenShots )
			{
				_timeOfLastShot = currentTime;
				Fire();
			}
		}
		
		
	}

	private void Fire()
	{
		var newProjectile = Instantiate( projectile );
		newProjectile.transform.position = head.position + head.forward * 0.6f;
		newProjectile.transform.rotation = head.rotation;
		newProjectile.GetComponent< Rigidbody >().AddForce( head.forward * 2.0f );
	}
}
