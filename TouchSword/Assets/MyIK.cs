using UnityEngine;
using System.Collections;

public class MyIK : MonoBehaviour
{

		public Transform P0;
		public Transform P1;
		public Transform P2;
		public Transform Target;
		private float l1, l2, dx, dy, d, dTarget;
		private bool canMove = true;
		// Use this for initialization
		void Start ()
		{
	
				l1 = Vector3.Distance (P0.transform.position, P1.transform.position);
				l2 = Vector3.Distance (P1.transform.position, P2.transform.position);
				dx = P2.transform.position.x - P0.transform.position.x;
				dy = P2.transform.position.y - P0.transform.position.y;
				//dTarget = Vector3.Distance (P0.transform.position, Target.transform.position);
				d = Vector3.Distance (P0.transform.position, P2.transform.position);

				Debug.Log (l1);
				Debug.Log (l2);
				//Debug.Log(d);
		}
	
		// Update is called once per frame
		void Update ()
		{
	
				dTarget = Vector3.Distance (P0.transform.position, Target.transform.position);

				//if(dTarget > Mathf.Min(l1,l2)){
				if (dTarget <= l1 + l2 && canMove) {
						P2.transform.position = Target.transform.position;
				} else {
						float P2x = (Target.transform.position.x - P0.transform.position.x) * d / dTarget;
						float P2y = (Target.transform.position.y - P0.transform.position.y) * d / dTarget;
						float P2z = P2.transform.position.z;

						P2.transform.position = new Vector3 (P2x, P2y, P2z);
				}

				d = Vector3.Distance (P0.transform.position, P2.transform.position);
				dx = P2.transform.position.x - P0.transform.position.x;
				dy = P2.transform.position.y - P0.transform.position.y;


				//float x = ((-(dx*dx) - (l1*l1) + (l2*l2))/(-2*dx)) + (dy*dy);
				//float y = Mathf.Sqrt( -((dx-x) * (dx-x)) + (l2*l2)) + (dy*dy);
				
				float cosJT = ((d * d) + (l1 * l1) - (l2 * l2)) / (2 * d * l1);
				cosJT = Mathf.Clamp (cosJT, -1, 1);
				float senJT = Mathf.Sqrt (1 - Mathf.Abs(cosJT * cosJT));

				float senDY = dy / d;
				float cosDX = dx / d;
		canMove = (cosJT >= -1 && cosJT <= 1) && (senDY >= -1 && senDY <= 1);
	
		float a = Mathf.Acos (cosJT) + Mathf.Asin (senDY);
//				if (cosDX < 0) {
//			a = a - Mathf.PI/2;
//		}
		//Debug.Log ("cosJT:" + cosJT +" senJT:"+ senJT +" senDY:" + senDY + " cosDx:" + cosDX + " a: " + (a));
		//Debug.Log (" cosDx:" + cosDX + " a: " + (a*Mathf.Rad2Deg));

	


				if (canMove) {

						
						//float senTDx = 
		
						float x = Mathf.Cos (a) * l1;//* Mathf.Sign(cosDX);
						float y = Mathf.Sin (a) * l1;

						Vector3 newPos = new Vector3 (x, y, 0);
						P1.transform.position = newPos;
						Debug.Log (newPos);
				}
			

				
				//}

				Debug.DrawLine (P0.transform.position, P1.transform.position, Color.blue);
				Debug.DrawLine (P1.transform.position, P2.transform.position, Color.red);
				//Debug.DrawLine (P0.transform.position, P2.transform.position, Color.yellow);
		}
}
