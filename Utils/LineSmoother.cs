using System.Collections.Generic;
using UnityEngine;

namespace Modules.Utils
{
	public static class LineSmoother 
	{
		public static Vector3[] SmoothLine( Vector3[] inputPoints, float segmentSize )
		{
			//create curves
			var curveX = new AnimationCurve();
			var curveY = new AnimationCurve();
			var curveZ = new AnimationCurve();

			//create keyframe sets
			var keysX = new Keyframe[inputPoints.Length];
			var keysY = new Keyframe[inputPoints.Length];
			var keysZ = new Keyframe[inputPoints.Length];

			//set keyframes
			for( var i = 0; i < inputPoints.Length; i++ )
			{
				keysX[i] = new Keyframe( i, inputPoints[i].x );
				keysY[i] = new Keyframe( i, inputPoints[i].y );
				keysZ[i] = new Keyframe( i, inputPoints[i].z );
			}

			//apply keyframes to curves
			curveX.keys = keysX;
			curveY.keys = keysY;
			curveZ.keys = keysZ;

			//smooth curve tangents
			for( var i = 0; i < inputPoints.Length; i++ )
			{
				curveX.SmoothTangents( i, 0 );
				curveY.SmoothTangents( i, 0 );
				curveZ.SmoothTangents( i, 0 );
			}

			//list to write smoothed values to
			List<Vector3> lineSegments = new List<Vector3>();

			//find segments in each section
			for( int i = 0; i < inputPoints.Length; i++ )
			{
				//add first point
				lineSegments.Add( inputPoints[i] );

				//make sure within range of array
				if( i+1 < inputPoints.Length )
				{
					//find distance to next point
					var distanceToNext = Vector3.Distance(inputPoints[i], inputPoints[i+1]);

					//number of segments
					var segments = (int)(distanceToNext / segmentSize);

					//add segments
					for( var s = 1; s < segments; s++ )
					{
						//interpolated time on curve
						var time = s/segments + i;

						//sample curves to find smoothed position
						var newSegment = new Vector3( curveX.Evaluate(time), curveY.Evaluate(time), curveZ.Evaluate(time) );

						//add to list
						lineSegments.Add( newSegment );
					}
				}
			}

			return lineSegments.ToArray();
		}
	}
}