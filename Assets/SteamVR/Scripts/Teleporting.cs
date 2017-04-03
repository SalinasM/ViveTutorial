using UnityEngine;

public class Teleporting : MonoBehaviour {

	public Transform cameraRigTransform;
	public Transform headTransform; // The camera rig's head
	public Vector3 teleportReticleOffset; // Offset from the floor for the reticle to avoid z-fighting
	public LayerMask teleportMask; // Mask to filter out areas where teleports are and are not allowed

	private SteamVR_TrackedObject trackedObj;

	public GameObject pointerPrefab; // The pointer prefab
	private GameObject pointer; // A reference to the spawned pointer
	private Transform pointerTransform; // The transform component of the pointer for ease of use

	public GameObject teleportReticlePrefab; // Stores a reference to the teleport reticle prefab.
	private GameObject reticle; // A reference to an instance of the reticle
	private Transform teleportReticleTransform; // Stores a reference to the teleport reticle transform for ease of use

	private Vector3 hitPoint; // Point where the raycast hits
	private bool shouldTeleport; // True if there's a valid teleport target

	private SteamVR_Controller.Device Controller
	{
			get { return SteamVR_Controller.Input((int)trackedObj.index); }
	}

	void Awake()
	{
			trackedObj = GetComponent<SteamVR_TrackedObject>();
	}

	// Use this for initialization
	void Start () {
		pointer = Instantiate(pointerPrefab);
		pointerTransform = pointer.transform;
		reticle = Instantiate(teleportReticlePrefab);
		teleportReticleTransform = reticle.transform;
	}

	// Update is called once per frame
	void Update () {
		// Is the touchpad held down?
		if (Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
		{
				RaycastHit hit;

				// Send out a raycast from the controller
				if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100, teleportMask))
				{
						hitPoint = hit.point;

						ShowPointer(hit);

						//Show teleport reticle
						reticle.SetActive(true);
						teleportReticleTransform.position = hitPoint + teleportReticleOffset;

						shouldTeleport = true;
				}
		}
		else // Touchpad not held down, hide pointerPrefab & teleport reticle
		{
				pointer.SetActive(false);
				reticle.SetActive(false);
		}
		// Touchpad released this frame & valid teleport position found
		if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad) && shouldTeleport)
		{
				Teleport();
		}
	}

	//show, scale and fit the pointer between the hitPoint and controller
	private void ShowPointer(RaycastHit hit)
	{
			pointer.SetActive(true);
			pointerTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
			pointerTransform.LookAt(hitPoint);
			pointerTransform.localScale = new Vector3(pointerTransform.localScale.x, pointerTransform.localScale.y,
					hit.distance);
	}

	//function that handles teleporting the user to the selected location. Not using the difference lands user at an incorrect location
	private void Teleport()
	{
			shouldTeleport = false;
			reticle.SetActive(false);
			Vector3 difference = cameraRigTransform.position - headTransform.position;
			difference.y = 0;

			cameraRigTransform.position = hitPoint + difference;
	}

}
