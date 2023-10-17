using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
	[Tooltip("Turn on the Gizmos on the Game Window to see the guides.")]
	[Space(10)][Range(0, .5f)]
	public float viewportX = 0.32f;
	[Tooltip("Turn on the Gizmos on the Game Window to see the guides.")]
	[Range(0, .5f)]
	public float viewportY = 0.21f;
	
	[Tooltip("Turn on the Gizmos on the Game Window to see the guides.")]
	[Space(10)][Range(0, .5f)]
	public float maxLimitViewPortX = 0.095f;
	[Tooltip("Turn on the Gizmos on the Game Window to see the guides.")]
	[Range(0, .5f)]
	public float maxLimitViewPortY = 0.32f;

	[Tooltip("Turn on the Gizmos on the Game Window to see the guides.")]
	[Space(10)]	[Range(-.25f, .25f)]
	public float yOffset = -.004f;
	
	[Space(10)]
	[Tooltip("The acceleration when the player is out of the safe area. This will be multiplied by time.fixedDeltaTime.")]
	[SerializeField] private float movementSmoothTime = 10;
	[Tooltip("The acceleration when the player is out of the maximum limit. This will be multiplied by time.fixedDeltaTime.")]
	[SerializeField] private float maxLimitSmoothTime = 5;
	[Tooltip("Check if you want the camera to center the player on the screen if he is unmoving.")]
	[SerializeField] private bool centerCameraOnIdle = true;
	[Tooltip("How much time does the player need to be unmoving to center him on the screen?")]
	[SerializeField] private float timeToCenter = 3f;
	[Tooltip("The acceleration when the camera is centering the player. This will be multiplied by time.fixedDeltaTime.")]
	[SerializeField] private float smoothTimeOnRecenter = 50;

	[SerializeField] private bool drawGizmos = true;

	private Camera cam;

	private Vector2 targetViewPortPoint;
	private Vector3 camNextPos;
	private Vector3 lastTargetPosition;
	private Vector3 currentVelocity;
	private Vector3 movementDelta;
	private float currentTimeToCenter;

	private void OnValidate()
	{
		//Make sure the max limit is greater than the safe area.
		float limitOffset = .05f;
		
		if (maxLimitViewPortX > viewportX - limitOffset)
			maxLimitViewPortX = viewportX - limitOffset;
		
		if (maxLimitViewPortY < viewportY + limitOffset)
			maxLimitViewPortY = viewportY + limitOffset;
	}

	private void Awake()
	{
		cam = GetComponent<Camera>();
		camNextPos = transform.position;
	}
	
	//Change this to Late Update if you prefer.
	private void FixedUpdate()
	{
		movementDelta = target.position - lastTargetPosition;
		targetViewPortPoint = cam.WorldToViewportPoint(target.position);
		float middleYOffset = .5f + yOffset;
		float currentSmoothTime = movementSmoothTime;

		if (targetViewPortPoint.x > 1 - viewportX || targetViewPortPoint.x < viewportX)
		{
			camNextPos.x += movementDelta.x;
		}
		if (targetViewPortPoint.y > viewportY + middleYOffset || targetViewPortPoint.y < middleYOffset - viewportY)
		{
			camNextPos.y += movementDelta.y;
		}

		//Change the current camera acceleration if the player reaches the max limit position on screen.
		if (targetViewPortPoint.x > 1 - maxLimitViewPortX || targetViewPortPoint.x < maxLimitViewPortX || targetViewPortPoint.y > maxLimitViewPortY + middleYOffset || targetViewPortPoint.y < middleYOffset - maxLimitViewPortY)
			currentSmoothTime = maxLimitSmoothTime;

		//If the camera needs to centralize the player on the screen.
		if (centerCameraOnIdle)
		{
			if (movementDelta != Vector3.zero)
				currentTimeToCenter = 0;
			else
				currentTimeToCenter += Time.fixedDeltaTime;

			if (currentTimeToCenter >= timeToCenter)
			{
				camNextPos = target.position;
				currentSmoothTime = smoothTimeOnRecenter;
			}
		}

		camNextPos.z = transform.position.z;
		lastTargetPosition = target.position;
		transform.position = Vector3.SmoothDamp(transform.position, camNextPos, ref currentVelocity, currentSmoothTime * Time.fixedDeltaTime);
	}

	private void OnDrawGizmos()
	{
		if (drawGizmos)
		{
			Vector3 camPos = Camera.main.transform.position;
			float cameraZOffset = .3f;
			float middleOffset = 0.5f + yOffset;

			Gizmos.color = Color.green;
			DrawLimit(viewportX, viewportY);

			Gizmos.color = Color.red;
			DrawLimit(maxLimitViewPortX, maxLimitViewPortY);

			//Function to draw the viewport gizmos
			void DrawLimit(float _viewportX, float _viewportY)
			{
				Vector3 line1Point1 = Camera.main.ViewportToWorldPoint(new Vector3(_viewportX, middleOffset - _viewportY, +cameraZOffset));
				Vector3 line1Point2 = Camera.main.ViewportToWorldPoint(new Vector3(_viewportX, _viewportY + middleOffset, +cameraZOffset));

				Vector3 line2Point1 = Camera.main.ViewportToWorldPoint(new Vector3(1 - _viewportX, middleOffset - _viewportY, +cameraZOffset));
				Vector3 line2Point2 = Camera.main.ViewportToWorldPoint(new Vector3(1 - _viewportX, _viewportY + middleOffset, +cameraZOffset));

				Vector3 line3Point1 = Camera.main.ViewportToWorldPoint(new Vector3(_viewportX, _viewportY + middleOffset, +cameraZOffset));
				Vector3 line3Point2 = Camera.main.ViewportToWorldPoint(new Vector3(1 - _viewportX, _viewportY + middleOffset, +cameraZOffset));

				Vector3 line4Point1 = Camera.main.ViewportToWorldPoint(new Vector3(_viewportX, middleOffset - _viewportY, +cameraZOffset));
				Vector3 line4Point2 = Camera.main.ViewportToWorldPoint(new Vector3(1 - _viewportX, middleOffset - _viewportY, +cameraZOffset));

				Gizmos.DrawLine(line1Point1, line1Point2);
				Gizmos.DrawLine(line2Point1, line2Point2);
				Gizmos.DrawLine(line3Point1, line3Point2);
				Gizmos.DrawLine(line4Point1, line4Point2);
			}
			
		}
	}
}
