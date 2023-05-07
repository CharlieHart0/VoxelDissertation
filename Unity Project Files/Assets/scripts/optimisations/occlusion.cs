using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class occlusion : MonoBehaviour
{
	
	public Material _Material;
	private ComputeBuffer _Reader;
	private ComputeBuffer _Writer;
	private Vector4[] _Element;
	public MeshRenderer _MeshRenderer;
	public MeshFilter _MeshFilter;
	private List<Vector4> _Vertices;

    private void Awake()
    {
		initialise();
    }

    public void initialise()
	{
		_Writer = new ComputeBuffer(1, 16, ComputeBufferType.Default);
		_Element = new Vector4[1];
		_Vertices = new List<Vector4>();

		foreach(Vector3 v3 in _MeshFilter.mesh.vertices)
        {
			Vector3 newv3 = _MeshRenderer.gameObject.transform.TransformPoint(v3);
			_Vertices.Add(new Vector4(newv3.x, newv3.y, newv3.z, 1));
        }

		Graphics.ClearRandomWriteTargets();
		Graphics.SetRandomWriteTarget(1, _Writer, false);

		_Reader = new ComputeBuffer(_Vertices.Count, 16, ComputeBufferType.Default);
		_Reader.SetData(_Vertices.ToArray());
		_Material.SetBuffer("_Reader", _Reader);
		_Material.SetBuffer("_Writer", _Writer);
	}

    private void Update()
    {
		_Writer.GetData(_Element);
		_MeshRenderer.enabled = (Vector4.Dot(_Element[0], _Element[0]) > 0.0f);
		Debug.Log(_Element[0].ToString());
		Debug.Log((Vector4.Dot(_Element[0], _Element[0]) > 0.0f));
		System.Array.Clear(_Element, 0, 1);
		_Writer.SetData(_Element);
	}


}
