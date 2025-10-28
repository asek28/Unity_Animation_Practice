using UnityEngine;
using UnityEngine.InputSystem;

public class RightMouseOrbit : MonoBehaviour
{
	[Header("Target & Distance")]
	public Transform target; // Assign your player here
	public float distance = 4f;
	public float minDistance = 2f;
	public float maxDistance = 8f;

	[Header("Offsets")]
	public float heightOffset = 4f; 
	public Vector3 additionalOffset = Vector3.zero; 

	[Header("Rotation (applies only while RMB is held)")]
	public float sensitivityX = 120f;
	public float sensitivityY = 120f;
	public float minY = -30f;
	public float maxY = 70f;

	[Header("Options")]
	public bool lockWhenNotHeld = true;
	public bool zoomOnlyWhenHeld = true; 

	private float yaw;   
	private float pitch; 

	void Start()
	{
		
		Vector3 e = transform.rotation.eulerAngles;
		yaw = e.y;
		pitch = e.x;
	}

	void Update()
	{
		var mouse = Mouse.current;
		if (mouse == null) return;

		bool rmb = mouse.rightButton.isPressed;

		// Zoom
		float scroll = mouse.scroll.ReadValue().y;
		if (Mathf.Abs(scroll) > 0.01f && (!zoomOnlyWhenHeld || rmb))
		{
			distance = Mathf.Clamp(distance - scroll * 0.05f, minDistance, maxDistance);
		}

		if (!rmb && lockWhenNotHeld)
		{
			
			ApplyTransform();
			return;
		}

		// Update yaw/pitch while held
		if (rmb)
		{
			Vector2 delta = mouse.delta.ReadValue();
			yaw   += delta.x * sensitivityX * Time.deltaTime;
			pitch -= delta.y * sensitivityY * Time.deltaTime;
			pitch = Mathf.Clamp(pitch, minY, maxY);
		}

		ApplyTransform();
	}

	private void ApplyTransform()
	{
		Vector3 pivot = target != null ? target.position : transform.position + transform.forward * distance;
		pivot += Vector3.up * heightOffset + additionalOffset;
		Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
		Vector3 desiredPos = pivot - rot * Vector3.forward * distance;

		transform.position = desiredPos;
		transform.rotation = rot;
	}
}
