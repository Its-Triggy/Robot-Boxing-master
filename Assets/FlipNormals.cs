using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]

public class FlipNormals : MonoBehaviour {

    void Start()
    {
        //transform.GetComponent<SkinnedMeshRenderer>
        //var mesh = (transform.GetComponent("MeshFilter") as MeshFilter).mesh;
        var mesh = (transform.GetComponent<SkinnedMeshRenderer>() as SkinnedMeshRenderer).sharedMesh;// mesh;
        mesh.uv = mesh.uv.Select(o => new Vector2(1 - o.x, o.y)).ToArray();
        mesh.triangles = mesh.triangles.Reverse().ToArray();
        mesh.normals = mesh.normals.Select(o => -o).ToArray();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
