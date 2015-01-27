using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IKJoint
{
		public Transform Point;
		public float Length;
}

public class IKRadius : MonoBehaviour
{
		public List<Transform> Joints = new List<Transform> () { null, null};
		public Transform Target;
		private List<IKJoint> IKJoints = new List<IKJoint> ();
		float totalLength = 0;
		public float v = 0.1f;

		// Use this for initialization
		void Start ()
		{
				for (int i = 0; i < Joints.Count; i++) {

						IKJoint ikJoint = new IKJoint ();
						ikJoint.Point = Joints [i];
						if (i > 0) {
								ikJoint.Length = Vector3.Distance (Joints [i].position, Joints [i - 1].position);
								Debug.Log (ikJoint.Length);
								totalLength += ikJoint.Length;
						}

						IKJoints.Add (ikJoint);
				}
				
		}
	
		// Update is called once per frame
		void Update ()
		{
				ApplyTargetLimits ();
				
				if (IKJoints.Count > 2) {

						if ((IKJoints.Count - 1) % 2 == 0) { //Joints Even

							
								if (IKJoints.Count == 3) {
										

										SolveIK2Joints (IKJoints [0], IKJoints [1], IKJoints [2]);

										Debug.DrawLine (IKJoints [1].Point.position, IKJoints [0].Point.position, Color.blue);
										Debug.DrawLine (IKJoints [2].Point.position, IKJoints [1].Point.position, Color.blue);
								} else {


					int count = Joints.Count;
					do{

						SolveIKEvenJoints(count);
						Debug.Log("Solved");
						count = count-2;		

					}while(count != 1);
					
								}
				
						} else { //if (IKJoints.Count == 4) { //Joints Odd
								SolveIK3Joints (IKJoints [0], IKJoints [1], IKJoints [2], IKJoints [3]);

								Debug.DrawLine (IKJoints [1].Point.position, IKJoints [0].Point.position, Color.red);
								Debug.DrawLine (IKJoints [2].Point.position, IKJoints [1].Point.position, Color.red);
								Debug.DrawLine (IKJoints [3].Point.position, IKJoints [2].Point.position, Color.red);
			
						}
				
				}
		}

	void SolveIKEvenJoints(int count)
	{
		int middleIdx = Mathf.FloorToInt (count / 2);
		
		IKJoint joint0 = IKJoints [0];
		IKJoint joint1 = new IKJoint ();
		for (int i = 0; i <= middleIdx; i++) {
			joint1.Length += IKJoints [i].Length;
			joint1.Point = IKJoints [i].Point;
		}
		
		IKJoint joint2 = new IKJoint ();
		for (int i = middleIdx+1; i < count; i++) {
			joint2.Length += IKJoints [i].Length;
			joint2.Point = IKJoints [i].Point;
		}
		
		SolveIK2Joints (joint0, joint1, joint2);
		
		Debug.DrawLine (joint1.Point.position, joint0.Point.position, Color.blue);
		Debug.DrawLine (joint2.Point.position, joint1.Point.position, Color.blue);
	}
	
	void SolveIK2Joints (IKJoint joint0, IKJoint joint1, IKJoint joint2)
	{
		Vector2? intersection1;
		Vector2? intersection2;
		
				CircleIntersection (
			joint0.Point.position.x, joint0.Point.position.y, joint1.Length,
			joint2.Point.position.x, joint2.Point.position.y, joint2.Length,
			out intersection1, out intersection2);
		
				if (intersection1.HasValue || intersection2.HasValue) {
			
						Vector2 intersection = intersection2.HasValue ? intersection2.Value : intersection1.Value;
						
						//Set Joint1 on Intersection
						joint1.Point.position = new Vector3 (intersection.x, intersection.y, joint2.Point.position.z);
						//
						//				//Rotate Joint2
						//				float angleJoint2 =  Mathf.Atan2 (
						//					intersection.y - joint2.Point.position.y, 
						//					intersection.x - joint2.Point.position.x);
						//				
						//				joint2.Point.localEulerAngles = new Vector3 (0, 0, angleJoint2 * Mathf.Rad2Deg);
				} else {
						float dTarget = Vector3.Distance (joint1.Point.position, Target.position);
						float distanceFix = 0.99999f;
						float P2x = (Target.position.x - joint1.Point.position.x) * (joint2.Length * distanceFix) / dTarget;
						float P2y = (Target.position.y - joint1.Point.position.y) * (joint2.Length * distanceFix) / dTarget;
						float P2z = joint2.Point.position.z;
			
						joint2.Point.position = new Vector3 (
							P2x + joint1.Point.position.x, 
							P2y + joint1.Point.position.y, 
							P2z + joint1.Point.position.z);
				}
		}

		void SolveIK3Joints (IKJoint joint0, IKJoint joint1, IKJoint joint2, IKJoint joint3)
		{
				float dist = Vector3.Distance (joint0.Point.position, joint3.Point.position);

				dist = Mathf.Clamp (dist, joint0.Length, joint2.Length + joint3.Length);
				
		
				Vector2? intersectionA1;
				Vector2? intersectionA2;
		
				CircleIntersection (
			joint3.Point.position.x, joint3.Point.position.y, dist,
			joint0.Point.position.x, joint0.Point.position.y, joint1.Length,
			out intersectionA1, out intersectionA2);
		
				if (intersectionA1.HasValue || intersectionA2.HasValue) {
			
						Vector2 intersectionA = intersectionA1.HasValue ? intersectionA1.Value : intersectionA2.Value;
			
						//Position Joint1
						joint1.Point.position = new Vector3 (intersectionA.x, intersectionA.y, 0);
			
						//Debug.DrawLine (joint0.Point.position, new Vector3 (intersectionA.x, intersectionA.y, 0), Color.green);
						//					
			
						Vector2? intersectionB1;
						Vector2? intersectionB2;
			
						CircleIntersection (
				intersectionA.x, intersectionA.y, joint2.Length,
				joint3.Point.position.x, joint3.Point.position.y, joint3.Length,
				out intersectionB1, out intersectionB2);
			
						if (intersectionB1.HasValue || intersectionB2.HasValue) {
				
								Vector2 intersectionB = intersectionB2.HasValue ? intersectionB2.Value : intersectionB1.Value;
				
								//Position Joint2
								joint2.Point.position = new Vector3 (intersectionB.x, intersectionB.y, 0);
				
								//Debug.DrawLine (joint3.Point.position, new Vector3 (intersectionB.x, intersectionB.y, 0), Color.green);
				
								//Debug.Log (Vector2.Distance (intersectionB, intersectionA));
				
						}
				}
		}
		
		void ApplyTargetLimits ()
		{
			
				IKJoint originJoint = IKJoints [0];
				IKJoint endJoint = IKJoints [Joints.Count - 1];
				float distance = totalLength;

				float dTarget = Vector3.Distance (originJoint.Point.position, Target.position);
				//float d = Vector3.Distance (originJoint.Point.position, endJoint.Point.position);

				if (dTarget <= distance && Joints.Count > 2) {
						endJoint.Point.position = Target.transform.position;
			
				} else {
						float distanceFix = 0.99999f;
						float P2x = (Target.position.x - originJoint.Point.position.x) * (distance * distanceFix) / dTarget;
						float P2y = (Target.position.y - originJoint.Point.position.y) * (distance * distanceFix) / dTarget;
						float P2z = endJoint.Point.position.z;
			
						endJoint.Point.position = new Vector3 (P2x, P2y, P2z);
				}
		}

		/// <summary>
		///Find the points where the two circles intersect.
		/// </summary>
		/// <param name="cx0">Cx0.</param>
		/// <param name="cy0">Cy0.</param>
		/// <param name="radius0">Radius0.</param>
		/// <param name="cx1">Cx1.</param>
		/// <param name="cy1">Cy1.</param>
		/// <param name="radius1">Radius1.</param>
		/// <param name="intersection1">Intersection1.</param>
		/// <param name="intersection2">Intersection2.</param>
		void CircleIntersection (
		float cx0, float cy0, float radius0,
		float cx1, float cy1, float radius1,
		out Vector2? intersection1, out Vector2? intersection2)
		{

				float dx, dy, dist, a, h, cx2, cy2;

				//Find the distance between the centers.
				dx = cx0 - cx1;
				dy = cy0 - cy1;
				dist = Mathf.Sqrt (dx * dx + dy * dy);
				
				//See how many solutions there are.
				if (dist > radius0 + radius1) {
						//No solutions, the circles are too far apart.
						intersection1 = null;
						intersection2 = null;
						return;
				} else if (dist < Mathf.Abs (radius0 - radius1)) {
						// No solutions, one circle contains the other.
						intersection1 = null;
						intersection2 = null;
						return;
				} else if ((dist == 0) && (radius0 == radius1)) {
						// No solutions, the circles coincide.
						intersection1 = null;
						intersection2 = null;
						return;
				} else {
						//Find a and h.
						a = (radius0 * radius0 - radius1 * radius1 + dist * dist) / (2 * dist);
						h = Mathf.Sqrt (radius0 * radius0 - a * a);

						//Find P2.
						cx2 = cx0 + a * (cx1 - cx0) / dist;
						cy2 = cy0 + a * (cy1 - cy0) / dist;
				
						//Get the points P3.
						intersection1 = new Vector2? (new Vector2 (
							(cx2 + h * (cy1 - cy0) / dist),
							(cy2 - h * (cx1 - cx0) / dist)
						));

						intersection2 = new Vector2? (new Vector2 (
			                (cx2 - h * (cy1 - cy0) / dist),
			        		(cy2 + h * (cx1 - cx0) / dist)
						));

				}
		}
}
