using System;
using UnityEngine;

namespace Kiloo.Common
{
	public static class GizmoUtility
	{
		public static void DrawArrow(Vector3 fromPos, Vector3 toPos)
		{
			DrawArrow(fromPos, toPos, (fromPos - toPos).magnitude * 0.1f);
		}

		public static void DrawArrow(Vector3 fromPos, Vector3 toPos, float arrowHeadSize)
		{
			Vector3 normalized = (fromPos - toPos).normalized;
			Vector3 rhs = ((!Mathf.Approximately(normalized.x, normalized.z)) ? new Vector3(normalized.z, 0f, 0f - normalized.x).normalized : Vector3.right);
			Vector3 axis = Vector3.Cross(normalized, rhs);
			Vector3 vector = normalized * arrowHeadSize;
			Gizmos.DrawLine(fromPos, toPos);
			Gizmos.DrawRay(toPos, Quaternion.AngleAxis(25f, axis) * vector);
			Gizmos.DrawRay(toPos, Quaternion.AngleAxis(-25f, axis) * vector);
		}

		public static void DrawCircle(Vector3 center, float radius)
		{
			Vector3 vector = center + new Vector3(radius, 0f, 0f);
			for (int i = 1; i <= 16; i++)
			{
				float f = (float)i / 16f * (float)Math.PI * 2f;
				Vector3 vector2 = center + new Vector3(Mathf.Cos(f) * radius, 0f, Mathf.Sin(f) * radius);
				Gizmos.DrawLine(vector, vector2);
				vector = vector2;
			}
		}
	}
}
