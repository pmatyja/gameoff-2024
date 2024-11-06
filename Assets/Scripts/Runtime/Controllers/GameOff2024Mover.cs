using UnityEngine;

namespace Runtime.Controllers
{
	// Modified version of the CMF Mover script;
	
	public class GameOff2024Mover : MonoBehaviour
	{

		//Collider variables;
		[Header("Mover Options :")]
		[Range(0f, 1f)][SerializeField] private float _stepHeightRatio = 0.25f;
		
		[Header("Collider Options :")]
		[SerializeField] float _colliderHeight = 2f;
		[SerializeField] float _colliderThickness = 1f;
		[SerializeField] Vector3 _colliderOffset = Vector3.zero;

		//References to attached collider(s);
		private BoxCollider _boxCollider;
		private SphereCollider _sphereCollider;
		private CapsuleCollider _capsuleCollider;

		//Sensor variables;
		[Header("Sensor Options :")]
		[SerializeField] public CMF.Sensor.CastType SensorType = CMF.Sensor.CastType.Raycast;
		private float _sensorRadiusModifier = 0.8f;
		private int _currentLayer;
		[SerializeField] private bool _isInDebugMode;

		[Header("Sensor Array Options :")]
		[SerializeField] [Range(1, 5)] private int _sensorArrayRows = 1;
		[SerializeField] [Range(3, 10)] private int _sensorArrayRayCount = 6;
		[SerializeField] private bool _sensorArrayRowsAreOffset = false;

		[HideInInspector] public Vector3[] RaycastArrayPreviewPositions;

		//Ground detection variables;
		private bool _isGrounded;

		//Sensor range variables;
		private bool _isUsingExtendedSensorRange  = true;
		private float _baseSensorRange;

		//Current upwards (or downwards) velocity necessary to keep the correct distance to the ground;
		private Vector3 _currentGroundAdjustmentVelocity = Vector3.zero;

		//References to attached components;
		private Collider _collider;
		private Rigidbody _rigidbody;
		private Transform _transform;
		private CMF.Sensor _sensor;
		
		//Multiply all sensor lengths with '_safetyDistanceFactor' to compensate for floating point errors;
		private const float _SAFETY_DISTANCE_FACTOR = 0.001f;

		private void Awake()
		{
			Setup();

			//Initialize sensor;
			_sensor = new CMF.Sensor(_transform, _collider);
			RecalculateColliderDimensions();
			RecalibrateSensor();
		}

		private void Reset ()
		{
			Setup();
		}

		private void OnValidate()
		{
			//Recalculate collider dimensions;
			if(gameObject.activeInHierarchy)
				RecalculateColliderDimensions();

			//Recalculate raycast array preview positions;
			if(SensorType == CMF.Sensor.CastType.RaycastArray)
				RaycastArrayPreviewPositions = 
					CMF.Sensor.GetRaycastStartPositions(_sensorArrayRows, _sensorArrayRayCount, _sensorArrayRowsAreOffset, 1f);
		}

		//Setup references to components;
		private void Setup()
		{
			_transform = transform;

			//If no collider is attached to this gameobject, add a collider;
			if(!_collider)
			{
				if (!gameObject.TryGetComponent(out _collider))
				{
					_collider = gameObject.AddComponent<CapsuleCollider>();
				}
			}

			//If no rigidbody is attached to this gameobject, add a rigidbody;
			if(!_rigidbody)
			{
				if (!gameObject.TryGetComponent(out _rigidbody))
				{
					_rigidbody = gameObject.AddComponent<Rigidbody>();
				}
			}

			_boxCollider = GetComponent<BoxCollider>();
			_sphereCollider = GetComponent<SphereCollider>();
			_capsuleCollider = GetComponent<CapsuleCollider>();

			//Freeze rigidbody rotation and disable rigidbody gravity;
			_rigidbody.freezeRotation = true;
			_rigidbody.useGravity = false;
		}

		//Draw debug information if debug mode is enabled;
		private void LateUpdate()
		{
			if (_isInDebugMode)
			{
				_sensor.DrawDebug();
			}
		}

		//Recalculate collider height/width/thickness;
		public void RecalculateColliderDimensions()
		{
			//Check if a collider is attached to this gameobject;
			if(!_collider)
			{
				//Try to get a reference to the attached collider by calling Setup();
				Setup();

				//Check again;
				if(!_collider)
				{
					Debug.LogWarning($"There is no collider attached to {gameObject.name}!");
					return;
				}				
			}

			//Set collider dimensions based on collider variables;
			if(_boxCollider)
			{
				var size = new Vector3(_colliderThickness, 0f, _colliderHeight);

				_boxCollider.center = _colliderOffset * _colliderHeight;

				size.y = _colliderHeight * (1f - _stepHeightRatio);
				_boxCollider.size = size;

				_boxCollider.center += new Vector3(0f, _stepHeightRatio * _colliderHeight/2f, 0f);
			}
			else if(_sphereCollider)
			{
				_sphereCollider.radius = _colliderHeight/2f;
				_sphereCollider.center = _colliderOffset * _colliderHeight;

				_sphereCollider.center += new Vector3(0f, _stepHeightRatio * _sphereCollider.radius, 0f);
				_sphereCollider.radius *= (1f - _stepHeightRatio);
			}
			else if(_capsuleCollider)
			{
				_capsuleCollider.height = _colliderHeight;
				_capsuleCollider.center = _colliderOffset * _colliderHeight;
				_capsuleCollider.radius = _colliderThickness/2f;

				_capsuleCollider.center += new Vector3(0f, _stepHeightRatio * _capsuleCollider.height/2f, 0f);
				_capsuleCollider.height *= (1f - _stepHeightRatio);

				if(_capsuleCollider.height/2f < _capsuleCollider.radius)
					_capsuleCollider.radius = _capsuleCollider.height/2f;
			}

			//Recalibrate sensor variables to fit new collider dimensions;
			if (_sensor != null)
			{
				RecalibrateSensor();	
			}
		}

		//Recalibrate sensor variables;
		private void RecalibrateSensor()
		{
			//Set sensor ray origin and direction;
			_sensor.SetCastOrigin(GetColliderCenter());
			_sensor.SetCastDirection(CMF.Sensor.CastDirection.Down);

			//Calculate sensor layermask;
			RecalculateSensorLayerMask();

			//Set sensor cast type;
			_sensor.castType = SensorType;

			//Calculate sensor radius/width;
			var radius = _colliderThickness/2f * _sensorRadiusModifier;

			//Fit collider height to sensor radius;
			if(_boxCollider)
				radius = Mathf.Clamp(radius, _SAFETY_DISTANCE_FACTOR, (_boxCollider.size.y/2f) * (1f - _SAFETY_DISTANCE_FACTOR));
			else if(_sphereCollider)
				radius = Mathf.Clamp(radius, _SAFETY_DISTANCE_FACTOR, _sphereCollider.radius * (1f - _SAFETY_DISTANCE_FACTOR));
			else if(_capsuleCollider)
				radius = Mathf.Clamp(radius, _SAFETY_DISTANCE_FACTOR, (_capsuleCollider.height/2f) * (1f - _SAFETY_DISTANCE_FACTOR));

			//Set sensor variables;

			//Set sensor radius;
			_sensor.sphereCastRadius = radius * _transform.localScale.x;

			//Calculate and set sensor length;
			var _length = 0f;
			_length += (_colliderHeight * (1f - _stepHeightRatio)) * 0.5f;
			_length += _colliderHeight * _stepHeightRatio;
			_baseSensorRange = _length * (1f + _SAFETY_DISTANCE_FACTOR) * _transform.localScale.x;
			_sensor.castLength = _length * _transform.localScale.x;

			//Set sensor array variables;
			_sensor.ArrayRows = _sensorArrayRows;
			_sensor.arrayRayCount = _sensorArrayRayCount;
			_sensor.offsetArrayRows = _sensorArrayRowsAreOffset;
			_sensor.isInDebugMode = _isInDebugMode;

			//Set sensor spherecast variables;
			_sensor.calculateRealDistance = true;
			_sensor.calculateRealSurfaceNormal = true;

			//Recalibrate sensor to the new values;
			_sensor.RecalibrateRaycastArrayPositions();
		}

		//Recalculate sensor layermask based on current physics settings;
		private void RecalculateSensorLayerMask()
		{
			var layerMask = 0;
			var objectLayer = gameObject.layer;
 
			//Calculate layermask;
            for (var i = 0; i < 32; i++)
            {
	            if (!Physics.GetIgnoreLayerCollision(objectLayer, i))
	            {
		            layerMask |= (1 << i);
	            }
			}

			//Make sure that the calculated layermask does not include the 'Ignore Raycast' layer;
			var ignoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");
			if (layerMask == (layerMask | (1 << ignoreRaycastLayer)))
			{
				layerMask ^= (1 << ignoreRaycastLayer);
			}
 
			//Set sensor layermask;
            _sensor.layermask = layerMask;

			//Save current layer;
			_currentLayer = objectLayer;
		}

		//Returns the collider's center in world coordinates;
		private Vector3 GetColliderCenter()
		{
			if (!_collider)
			{
				Setup();
			}

			return _collider.bounds.center;
		}

		//Check if mover is grounded;
		//Store all relevant collision information for later;
		//Calculate necessary adjustment velocity to keep the correct distance to the ground;
		private void Check()
		{
			//Reset ground adjustment velocity;
			_currentGroundAdjustmentVelocity = Vector3.zero;

			//Set sensor length;
			if (_isUsingExtendedSensorRange)
			{
				_sensor.castLength = _baseSensorRange + (_colliderHeight * _transform.localScale.x) * _stepHeightRatio;
			}
			else
			{
				_sensor.castLength = _baseSensorRange;
			}
			
			_sensor.Cast();

			//If sensor has not detected anything, set flags and return;
			if(!_sensor.HasDetectedHit())
			{
				_isGrounded = false;
				return;
			}

			//Set flags for ground detection;
			_isGrounded = true;

			//Get distance that sensor ray reached;
			var distance = _sensor.GetDistance();

			//Calculate how much mover needs to be moved up or down;
			var upperLimit = ((_colliderHeight * _transform.localScale.x) * (1f - _stepHeightRatio)) * 0.5f;
			var middle = upperLimit + (_colliderHeight * _transform.localScale.x) * _stepHeightRatio;
			var distanceToGo = middle - distance;

			//Set new ground adjustment velocity for the next frame;
			_currentGroundAdjustmentVelocity = _transform.up * (distanceToGo/Time.fixedDeltaTime);
		}

		//Check if mover is grounded;
		public void CheckForGround()
		{
			//Check if object layer has been changed since last frame;
			//If so, recalculate sensor layer mask;
			if (_currentLayer != gameObject.layer)
			{
				RecalculateSensorLayerMask();
			}

			Check();
		}

		//Set mover velocity;
		public void SetVelocity(Vector3 velocity)
		{
			_rigidbody.linearVelocity = velocity + _currentGroundAdjustmentVelocity;	
		}	

		//Returns 'true' if mover is touching ground and the angle between hte 'up' vector and ground normal is not too steep (e.g., angle < slope_limit);
		public bool IsGrounded() => _isGrounded;

		//Setters;

		//Set whether sensor range should be extended;
		public void SetExtendSensorRange(bool _isExtended) => _isUsingExtendedSensorRange = _isExtended;

		//Set height of collider;
		public void SetColliderHeight(float newColliderHeight)
		{
			if (Mathf.Approximately(_colliderHeight, newColliderHeight)) return;

			_colliderHeight = newColliderHeight;
			RecalculateColliderDimensions();
		}

		//Set thickness/width of collider;
		public void SetColliderThickness(float newColliderThickness)
		{
			if (Mathf.Approximately(_colliderThickness, newColliderThickness)) return;

			newColliderThickness = Mathf.Max(newColliderThickness, 0f);

			_colliderThickness = newColliderThickness;
			RecalculateColliderDimensions();
		}

		//Set acceptable step height;
		public void SetStepHeightRatio(float newStepHeightRatio)
		{
			newStepHeightRatio = Mathf.Clamp01(newStepHeightRatio);
			_stepHeightRatio = newStepHeightRatio;
			RecalculateColliderDimensions();
		}

		//Getters;
		public Vector3 GetGroundNormal() => _sensor.GetNormal();
		public Vector3 GetGroundPoint() => _sensor.GetPosition();
		public Collider GetGroundCollider() => _sensor.GetCollider();
		
	}


}