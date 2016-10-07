using UnityEngine;
using System.Collections;
using Leap;

//<summary>
//Ball movement controlls and simple third-person-style camera
//</summary>
public class RollerBall : MonoBehaviour {

	public GameObject ViewCamera = null;
	public AudioClip JumpSound = null;
	public AudioClip HitSound = null;
	public AudioClip CoinSound = null;

	Controller LeapController;

	private Rigidbody mRigidBody = null;
	private AudioSource mAudioSource = null;
	private bool mFloorTouched = false;

	void Start () {
		LeapController = new Controller ();
		mRigidBody = GetComponent<Rigidbody> ();
		mAudioSource = GetComponent<AudioSource> ();
	}

	void FixedUpdate () {
		Frame frame = LeapController.Frame (); 
		Hand hand = null;
		for (int i = 0; i < frame.Hands.Count; ++i) {
			hand = frame.Hands [i];
		}
		if (mRigidBody != null) {
			if (hand.PalmPosition.x > 0f) {
				mRigidBody.AddTorque(0,0,hand.PalmPosition.x * -0.001f);
			}
			if (hand.PalmPosition.z > 0f) {
				mRigidBody.AddTorque(hand.PalmPosition.z * -0.001f,0,0);
			}
			if (hand.PalmPosition.x < 0f) {
				mRigidBody.AddTorque(0,0,hand.PalmPosition.x * -0.001f);
			}
			if (hand.PalmPosition.z < 0f) {
				mRigidBody.AddTorque(hand.PalmPosition.z * -0.001f,0,0);
			}
			if (Input.GetButtonDown("Jump")) {
				if(mAudioSource != null && JumpSound != null){
					mAudioSource.PlayOneShot(JumpSound);
				}
				mRigidBody.AddForce(Vector3.up*200);
			}
		}
		if (ViewCamera != null) {
			Vector3 direction = (Vector3.up*2+Vector3.back)*2;
			RaycastHit hit;
			Debug.DrawLine(transform.position,transform.position+direction,Color.red);
			if(Physics.Linecast(transform.position,transform.position+direction,out hit)){
				ViewCamera.transform.position = hit.point;
			}else{
				ViewCamera.transform.position = transform.position+direction;
			}
			ViewCamera.transform.LookAt(transform.position);
		}
	}

	void OnCollisionEnter(Collision coll){
		if (coll.gameObject.tag.Equals ("Floor")) {
			mFloorTouched = true;
			if (mAudioSource != null && HitSound != null && coll.relativeVelocity.y > .5f) {
				mAudioSource.PlayOneShot (HitSound, coll.relativeVelocity.magnitude);
			}
		} else {
			if (mAudioSource != null && HitSound != null && coll.relativeVelocity.magnitude > 2f) {
				mAudioSource.PlayOneShot (HitSound, coll.relativeVelocity.magnitude);
			}
		}
	}

	void OnCollisionExit(Collision coll){
		if (coll.gameObject.tag.Equals ("Floor")) {
			mFloorTouched = false;
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag.Equals ("Coin")) {
			if(mAudioSource != null && CoinSound != null){
				mAudioSource.PlayOneShot(CoinSound);
			}
			Destroy(other.gameObject);
		}
	}
}
