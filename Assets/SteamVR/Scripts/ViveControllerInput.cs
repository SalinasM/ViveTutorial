
using UnityEngine;

public class ViveControllerInput : MonoBehaviour {


	private SteamVR_TrackedObject viveController;			 	//Controller
	private GameObject collidingObject;						//Object that you're colliding with (Before grab)
	private GameObject objectInHand;						//Grabbed object

	//Next two functions give you the controller and stores it into trackedObj
	private SteamVR_Controller.Device Controller 
	{
		get { return SteamVR_Controller.Input ((int)viveController.index); }
	}
	
	void Awake()
	{
			viveController = GetComponent<SteamVR_TrackedObject>();	
	}

	//Sets a colliding object to collidingObject for grab
	private void SetCollidingObject(Collider col)
	{
		if (collidingObject || !col.GetComponent<Rigidbody> ()) 
		{
			return;
		}

		collidingObject = col.gameObject;
	}

	//Function that activates when the trigger enters another object
	public void OnTriggerEnter(Collider other)
	{
		SetCollidingObject (other);
	}

	//Function that is called every frame that the trigger is in another object
	public void OnTriggerStay(Collider other)
	{
		SetCollidingObject (other);
	}

	//Function that activates when the trigger leaves another object
	public void OnTriggerExit(Collider other)
	{
		if (!collidingObject) {
			return;
		}

		collidingObject = null;
	}

	private void GrabObject()
	{
		objectInHand = collidingObject;				//set our objectInHand to the collidingObject 
		collidingObject = null;						//reset collidingObject because we are gonna have it in hand now

		var joint = AddFixedJoint ();
		joint.connectedBody = objectInHand.GetComponent<Rigidbody> ();
	}

	private FixedJoint AddFixedJoint()
	{
		FixedJoint fx = gameObject.AddComponent<FixedJoint> ();
		fx.breakForce = 20000;
		fx.breakTorque = 20000;
		return fx;
	}

	private void ReleaseObject()
	{
		if (GetComponent<FixedJoint> ()) {
			GetComponent<FixedJoint>().connectedBody = null;
			Destroy (GetComponent<FixedJoint> ());

			objectInHand.GetComponent<Rigidbody> ().velocity = Controller.velocity;
			objectInHand.GetComponent<Rigidbody> ().angularVelocity = Controller.angularVelocity;
		}

		objectInHand = null;
	}

	//teleport
		
	// Update is called once per frame
	void Update () {
		//
		if (Controller.GetHairTriggerDown()) {
			if (collidingObject) {
				GrabObject ();
			}

		}

		if (Controller.GetHairTriggerUp ()) {
			if (objectInHand) {
				ReleaseObject ();
			}
		}

	}
}
