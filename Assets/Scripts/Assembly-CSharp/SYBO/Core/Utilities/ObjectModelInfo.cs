using UnityEngine;

namespace SYBO.Core.Utilities
{
	public struct ObjectModelInfo
	{
		public MeshRenderer MeshRenderer;

		public MeshFilter MeshFilter;

		public ObjectModelInfo(MeshRenderer meshRenderer, MeshFilter meshFilter)
		{
			MeshRenderer = meshRenderer;
			MeshFilter = meshFilter;
		}
	}
}
