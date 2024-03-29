using System.Collections.Generic;
using UnityEngine;

namespace Modules.Utils
{
    public static class CatmullRomSpline
    {
        public static IReadOnlyList<Vector3> Create(IReadOnlyList<Vector3> input)
        {
            if (input.Count < 4)
                return input;

            var output = new List<Vector3>();

            for (var index = 0; index < input.Count - 4; index++)
            {
	            var p0 = input[ClampListPos(index)];
	            var p1 = input[ClampListPos(index+1)];
	            var p2 = input[ClampListPos(index+2)];
	            var p3 = input[ClampListPos(index+3)];

	            //The spline's resolution
	            //Make sure it's is adding up to 1, so 0.3 will give a gap, but 0.2 will work
	            float resolution = 0.5f;

	            //How many times should we loop?
	            int loops = Mathf.FloorToInt(1f / resolution);

	            for (int i = 1; i <= loops; i++)
	            {
		            //Which t position are we at?
		            float t = i * resolution;

		            //Find the coordinate between the end points with a Catmull-Rom spline
		            Vector3 newPos = GetCatmullRomPosition(t, p0, p1, p2, p3);

		            output.Add(newPos);

		            //Draw this line segment
		            // Gizmos.DrawLine(lastPos, newPos);

		            //Save this pos so we can draw the next line segment
		            // lastPos = newPos;
	            }
            }

            return output;
	            
	        //Clamp the list positions to allow looping
	        int ClampListPos(int pos)
            {
	            if (pos < 0)
	            {
		            pos = input.Count - 1;
	            }

	            if (pos > input.Count)
	            {
		            pos = 1;
	            }
	            else if (pos > input.Count - 1)
	            {
		            pos = 0;
	            }

	            return pos;
            }
        }
        
	    //Clamp the list positions to allow looping
	    private static Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            //The coefficients of the cubic polynomial (except the 0.5f * which I added later for performance)
            Vector3 a = 2f * p1;
            Vector3 b = p2 - p0;
            Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
            Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

            //The cubic polynomial: a + b * t + c * t^2 + d * t^3
            Vector3 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));

            return pos;
        }
    }
}