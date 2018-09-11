using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

public class KukaRig : MonoBehaviour {

	public bool continuous;
	[Range(1, 10)] public int iterations;
	public TrigonometricIK trig;
	public Transform target, arm1, arm2, arm3, arm4, arm5, arm6, end;
	public Vector3 arm1Axis, arm4Axis, arm5Axis, arm6Axis;

	private Transform[] arms = new Transform[0];
	private Quaternion[] defaultLocalRotations = new Quaternion[0];
	private Vector3 trigEndRelToTarget;

	void Start() {
		arms = GetComponentsInChildren<Transform>();
		defaultLocalRotations = new Quaternion[arms.Length];
		StoreDefaultLocalRotations();

		trig.enabled = false;
		trig.solver.target = null;
		trig.solver.IKPositionWeight = 1f;
		trig.solver.IKRotationWeight = 0f;
		trig.fixTransforms = false;

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
			// Direct rotations
			for (int i  = 0; i < 5; i++) {
				RotateTowardsAroundAxis(arm6, end.forward, target.forward, arm6Axis);
				RotateTowardsAroundAxis(arm5, end.up, target.up, arm5Axis);
				RotateTowardsAroundAxis(arm4, end.up, target.up, arm4Axis);
				RotateTowardsAroundAxis(arm1, end.position - arm1.position, target.position - arm1.position, arm1Axis);
			}

			trigEndRelToTarget = end.InverseTransformPoint(trig.solver.bone3.transform.position);

			trig.solver.IKPosition = target.TransformPoint(trigEndRelToTarget);
			trig.solver.bendNormal = Vector3.Cross(Vector3.up, trig.solver.IKPosition - trig.solver.bone1.transform.position);
			trig.solver.Update();

			arm2.rotation = Quaternion.FromToRotation(arm2.right, arm1.right) * arm2.rotation;
			arm3.rotation = Quaternion.FromToRotation(arm3.right, arm2.right) * arm3.rotation;
		}
	}

	private void StoreDefaultLocalRotations() {
		for (int i = 0; i < arms.Length; i++) {
			defaultLocalRotations[i] = arms[i].localRotation;
		}
	}

	private void FixToDefaultLocalRotations() {
		for (int i = 0; i < arms.Length; i++) {
			arms[i].localRotation = defaultLocalRotations[i];
		}
	}

	private static void RotateTowardsAroundAxis(Transform t, Vector3 fromV, Vector3 toV, Vector3 upAxis) {
		Quaternion r = Quaternion.LookRotation(fromV, t.rotation * upAxis);
		Vector3 d = Quaternion.Inverse(r) * toV;
		float angle = Mathf.Atan2(d.x, d.z) * Mathf.Rad2Deg;
		t.rotation = Quaternion.AngleAxis(angle, t.rotation * upAxis) * t.rotation;
	}
}
