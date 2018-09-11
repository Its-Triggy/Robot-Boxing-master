using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

public class UR5RigCCD : MonoBehaviour {

	[System.Serializable]
	public class Arm {
		public bool enabled;
		public Transform transform;
		public Vector3 axis;
		public Vector3 targetAxis;

		public void RotateTowardsAroundAxis(Vector3 fromV, Vector3 toV) {
			if (!enabled) return;

			Quaternion r = Quaternion.LookRotation(fromV, transform.rotation * axis);
			Vector3 d = Quaternion.Inverse(r) * toV;
			float angle = Mathf.Atan2(d.x, d.z) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis(angle, transform.rotation * axis) * transform.rotation;
		}
	}

	public bool continuous;
	[Range(1, 10)] public int iterations;
	public CCDIK ccd;

	public Arm gimbal1, gimbal2, gimbal3;
	public Transform end;
	public Transform target;

	private Transform[] children = new Transform[0];
	private Quaternion[] defaultLocalRotations = new Quaternion[0];

	void Start() {
		children = GetComponentsInChildren<Transform>();
		defaultLocalRotations = new Quaternion[children.Length];
		StoreDefaultLocalRotations();

		ccd.enabled = false;
		ccd.fixTransforms = false;

		Vector3 targetPos = target.position;
		Quaternion targetRot = target.rotation;
		target.position = end.position;
		target.rotation = end.rotation;
		target.position = targetPos;
		target.rotation = targetRot;
	}

	void LateUpdate() {
		// Reset
		if (!continuous) FixToDefaultLocalRotations();

		for (int it = 0; it < iterations; it++) {
			gimbal3.RotateTowardsAroundAxis(end.rotation * gimbal3.targetAxis, target.rotation * gimbal3.targetAxis);
			gimbal2.RotateTowardsAroundAxis(end.rotation * gimbal2.targetAxis, target.rotation * gimbal2.targetAxis);
			gimbal1.RotateTowardsAroundAxis(end.rotation * gimbal1.targetAxis, target.rotation * gimbal1.targetAxis);

			Vector3 ccdRelToTarget =  end.InverseTransformPoint(ccd.solver.bones[ccd.solver.bones.Length - 1].transform.position);
			ccd.solver.IKPosition = target.TransformPoint(ccdRelToTarget);
			ccd.solver.Update();
		}
	}

	private void StoreDefaultLocalRotations() {
		for (int i = 0; i < children.Length; i++) {
			defaultLocalRotations[i] = children[i].localRotation;
		}
	}

	private void FixToDefaultLocalRotations() {
		for (int i = 0; i < children.Length; i++) {
			if (children[i] != target) children[i].localRotation = defaultLocalRotations[i];
		}
	}

	private static void RotateTowardsAroundAxis(Transform t, Vector3 fromV, Vector3 toV, Vector3 upAxis) {
		Quaternion r = Quaternion.LookRotation(fromV, t.rotation * upAxis);
		Vector3 d = Quaternion.Inverse(r) * toV;
		float angle = Mathf.Atan2(d.x, d.z) * Mathf.Rad2Deg;
		t.rotation = Quaternion.AngleAxis(angle, t.rotation * upAxis) * t.rotation;
	}
}
