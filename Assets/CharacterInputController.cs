using UnityEngine;
using UnityEngine.InputNew;
using System.Linq;

public class CharacterInputController
	: MonoBehaviour
{
	public ControlMap controlMap;
	public ControlMapEntry lookControlX;
	public ControlMapEntry lookControlY;
	public ControlMapEntry fireControl;
	public float timeBetweenShots = 0.5f;
	public GameObject projectile;

	private ControlMapInstance _controlMapInstance;
	private float _timeOfLastShot;

	public void Awake()
	{
		_controlMapInstance = InputSystem.BindInputs( controlMap ).First();
		_controlMapInstance.Activate();
	}

	public void Update()
	{
		var lookX = _controlMapInstance[ lookControlX ].floatValue;
		var lookY = _controlMapInstance[ lookControlY ].floatValue;

		//Debug.Log( string.Format( "lookX = {0}, lookY = {1}", lookX, lookY ) );

		var fire = _controlMapInstance[ fireControl ].boolValue;
		if ( fire )
		{
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
		newProjectile.transform.position = transform.position;
		newProjectile.transform.rotation = transform.rotation;
		newProjectile.GetComponent< Rigidbody >().AddForce( transform.forward * 2.0f );
	}
}
