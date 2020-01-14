using UnityEngine;
using System.Collections;

public class ConstantRotation : MonoBehaviour {

	    public float RotationsPerMinuteY=0;
        public float RotationsPerMinuteX=0;
        public float RotationsPerMinuteZ=0;    
 
        void FixedUpdate () {
            Vector3 ConstantRotation = new Vector3 (0,0,0);
           
            if(RotationsPerMinuteX>0) {
                    ConstantRotation.x=6.0f*RotationsPerMinuteX*Time.deltaTime;
            }
           
            if(RotationsPerMinuteZ>0) {
                    ConstantRotation.z=6.0f*RotationsPerMinuteZ*Time.deltaTime;
            }
           
            if(RotationsPerMinuteY>0) {
                    ConstantRotation.y=6.0f*RotationsPerMinuteY*Time.deltaTime;
            }
            transform.Rotate (ConstantRotation); //rotates 50 degrees per second around z axis
       }
}
