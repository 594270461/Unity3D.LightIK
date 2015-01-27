using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FastIK : MonoBehaviour
{

		public List<Transform> Joints = new List<Transform> ();
		public Transform Target;
		public float JointLength = 1;
		int i, k, n;
		float a, b, c;

		// Use this for initialization
		void Start ()
		{
	

		}
	
		// Update is called once per frame
		void Update ()
		{
	
				applyIK ();
		}

		void applyIK ()
		{
				float tetaI;
				float tetaK = 0;
				i = 0;
				n = Joints.Count;
				k = n / 2;
				a = k * JointLength;
				b = n * JointLength - a;
		
				Transform joint = Joints [i];
				c = Vector3.Distance (joint.position, Target.position);
		float angle = Mathf.Atan2 (Target.position.y - joint.position.y, Target.position.x - joint.position.x) - (Mathf.PI / 2);
				//float angle = Mathf.Atan(Target.position.y - joint.position.y);
				//float angle = Mathf.Atan( (joint.position.x - Target.position.x) / c );
				//float angle = Vector3.Angle(joint.position,Target.position); //* Mathf.Deg2Rad;

				Debug.Log (angle);
				Debug.DrawLine (joint.position, Target.position, Color.yellow);

				if (c > a + b) {
						
						Joints [0].localEulerAngles = new Vector3 (0, 0, angle * Mathf.Rad2Deg);
						return;
				}
				while (true) {
						float tmp = (a * a) + (b * b) - (c * c);

						if (tmp > 0) {
								tetaK = Mathf.PI - Mathf.Acos (tmp / (2 * a * b));
								float delta = Mathf.Acos (JointLength / (2 * c));
								if (tetaK > 130 * Mathf.Deg2Rad) {
										delta += 20 * Mathf.Deg2Rad;
								} else {
										delta -= 20 * Mathf.Deg2Rad;
								}
								tetaI = angle - delta; 

								joint.localEulerAngles = new Vector3 (0, 0, tetaI * Mathf.Rad2Deg);

								
								i++;

								if (i < Joints.Count) {

										joint = Joints [i];

										n--;
										k = n / 2;
										a = k * JointLength;
										b = n * JointLength - a;
										c = Vector3.Distance (joint.position, Target.position);
								} else {
										break;
								}

						} else { 
								break;
						}

				}

		angle = Mathf.Atan2 (Target.position.y - joint.position.y, Target.position.x - joint.position.x) - (Mathf.PI / 2);
				float deltaB = Mathf.Acos (((a * a) + (c * c) - (b * b)) / (2 * a * c));
				tetaI = angle - deltaB; 

				joint.localEulerAngles = new Vector3 (0, 0, tetaI * Mathf.Rad2Deg);

				Joints [k].localEulerAngles = new Vector3 (0, 0, tetaK * Mathf.Rad2Deg);
		}
}
