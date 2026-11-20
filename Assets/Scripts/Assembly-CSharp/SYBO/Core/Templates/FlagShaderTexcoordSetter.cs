using SYBO.Core.Utilities;
using UnityEngine;

namespace SYBO.Core.Templates
{
	[RequireComponent(typeof(MeshFilter))]
	public class FlagShaderTexcoordSetter : MonoBehaviour
	{
		private void Start()
		{
			MeshFilter component = GetComponent<MeshFilter>();
			if (component == null)
			{
				Debug.LogErrorFormat(this, "FlagShaderTexcoordSetter missing MeshFilter on {0}", base.transform.GetLongName());
				return;
			}
			Mesh sharedMesh = component.sharedMesh;
			if (sharedMesh == null)
			{
				Debug.LogErrorFormat(this, "FlagShaderTexcoordSetter missing a mesh on {0}", base.transform.GetLongName());
				return;
			}
			Vector3[] vertices = sharedMesh.vertices;
			Vector2[] array = new Vector2[vertices.Length];
			for (int i = 0; i < vertices.Length; i++)
			{
				array[i] = new Vector2(vertices[i].y, vertices[i].z);
			}
			sharedMesh.uv2 = array;
		}
	}
}
