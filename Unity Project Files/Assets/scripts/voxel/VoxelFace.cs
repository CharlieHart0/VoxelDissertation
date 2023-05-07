using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelFace : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    MeshFilter filter;
    public enum FaceDirection
    {
        // X- West  X+ East
        // Z- South Z+ North
        // Y- Down Y+ Up 

        North,
        South,
        East,
        West,
        Up,
        Down
    }

    public FaceDirection direction;
    public Voxel voxel;

    private void Start()
    {
        if (voxel == null)
        {
            voxel = transform.parent.GetComponent<Voxel>();
        }
    }

    private void Update()
    {
        // if view frustum culling is enabled for quad face, in voxel scale mode
        if (WorldSettings.Instance.viewFrustumCulling)
        {
            bool renderThisFace = false;

            // check if any vertices are within screen bounds, if so, set to render face
            foreach (Vector3 VertexPos in filter.mesh.vertices)
            {
                if (HelpfulFunctions.isInViewFrustum(VertexPos + this.transform.position))
                {
                    renderThisFace = true;
                    break;
                }
            }

            meshRenderer.enabled = renderThisFace;
        }
    }

    public void unRenderFace()
    {
        filter = GetComponent<MeshFilter>();
        if (filter != null)
        {
            Destroy(filter);
        }

        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            Destroy(meshRenderer);
        }
    }

    public void renderFace()
    {
        if (voxel.data.block.doNotRender) // if block is not to be rendered, such as air
        {
            return;
        }

        Vector3 bounds = new Vector3((WorldSettings.Instance.voxelSize.x / 2), (WorldSettings.Instance.voxelSize.y / 2), (WorldSettings.Instance.voxelSize.z / 2));


        // only add filter and renderer components if they arent already there (pooled face may already have them)
        filter = GetComponent<MeshFilter>();
        if (filter == null)
        {
            filter = gameObject.AddComponent<MeshFilter>();
        }

        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
        }


        // make sure mesh renderer is enabled - this would otherwise be a problem if getting a
        // pooled face from a chunk outside the view frustum, which was therefore disabled to be hidden
        meshRenderer.enabled = true;
       

        Mesh mesh = new Mesh();


        //Vertices
        List<Vector3> vertices = new();

        // X- West  X+ East
        // Z- South Z+ North
        // Y- Down Y+ Up 

        //Face z+
        if (direction == FaceDirection.North) {

            vertices.Add(new Vector3(bounds.x, -bounds.y, bounds.z));
            vertices.Add(new Vector3(+bounds.x, +bounds.y, +bounds.z));
            vertices.Add(new Vector3(-bounds.x, bounds.y, bounds.z));
            vertices.Add(new Vector3(-bounds.x, -bounds.y, bounds.z));
        }
        //face x-
        else if(direction == FaceDirection.West)
        {
            vertices.Add(new Vector3(-bounds.x, -bounds.y, bounds.z));
            vertices.Add(new Vector3(-bounds.x, bounds.y, bounds.z));
            vertices.Add(new Vector3(-bounds.x, bounds.y, -bounds.z));
            vertices.Add(new Vector3(-bounds.x, -bounds.y, -bounds.z));

        }
        //Face Z-
        else if(direction == FaceDirection.South)
        {
            vertices.Add(new Vector3(-bounds.x, -bounds.y, -bounds.z));
            vertices.Add(new Vector3(-bounds.x, bounds.y, -bounds.z));
            vertices.Add(new Vector3(bounds.x, bounds.y, -bounds.z));
            vertices.Add(new Vector3(bounds.x, -bounds.y, -bounds.z));

        } 
        //Face Y-
        else if(direction == FaceDirection.Down)
        {
            vertices.Add(new Vector3(-bounds.x, -bounds.y, bounds.z));
            vertices.Add(new Vector3(-bounds.x, -bounds.y, -bounds.z));
            vertices.Add(new Vector3(bounds.x, -bounds.y, -bounds.z));
            vertices.Add(new Vector3(bounds.x, -bounds.y, bounds.z));
        }
        //Face Y+
        else if(direction == FaceDirection.Up)
        {
            vertices.Add(new Vector3(-bounds.x, bounds.y, bounds.z));
            vertices.Add(new Vector3(bounds.x, bounds.y, bounds.z));
            vertices.Add(new Vector3(bounds.x, bounds.y, -bounds.z));
            vertices.Add(new Vector3(-bounds.x,bounds.y, -bounds.z));
        }
        //Face X+
        else if(direction == FaceDirection.East)
        {
            vertices.Add(new Vector3(bounds.x, -bounds.y, -bounds.z));
            vertices.Add(new Vector3(bounds.x, bounds.y, -bounds.z));
            vertices.Add(new Vector3(bounds.x, bounds.y, bounds.z));
            vertices.Add(new Vector3(bounds.x, -bounds.y, bounds.z));
        }



        mesh.vertices = vertices.ToArray();


        // Tris
        int[] tris = new int[6] { 0, 1, 2, 0, 2, 3 };
        mesh.triangles = tris;

        //Normals
        Vector3[] normals = new Vector3[4];
        
        //face z+
        if(direction == FaceDirection.South)
        {
            normals = new Vector3[4]
            {
            new Vector3(0,0,1),
            new Vector3(0,0,1),
            new Vector3(0,0,1),
            new Vector3(0,0,1)
            };
        }
        //face x-
        else if (direction == FaceDirection.East)
        {
            normals = new Vector3[4]
            {
            new Vector3(-1,0,0),
            new Vector3(-1,0,0),
            new Vector3(-1,0,0),
            new Vector3(-1,0,0)
            };
        }
        //Face Z-
        else if (direction == FaceDirection.North)
        {
            normals = new Vector3[4]
            {
            new Vector3(0,0,-1),
            new Vector3(0,0,-1),
            new Vector3(0,0,-1),
            new Vector3(0,0,-1)
            };
        }
        //Face Y-
        else if (direction == FaceDirection.Down)
        {
            normals = new Vector3[4]
            {
            new Vector3(0,-1,0),
            new Vector3(0,-1,0),
            new Vector3(0,-1,0),
            new Vector3(0,-1,0)
            };
        }
        //Face Y+
        else if (direction == FaceDirection.Up)
        {
            normals = new Vector3[4]
            {
            new Vector3(0,1,0),
            new Vector3(0,1,0),
            new Vector3(0,1,0),
            new Vector3(0,1,0)
            };
        }
        //Face X+
        else if (direction == FaceDirection.West)
        {
            normals = new Vector3[4]
            {
            new Vector3(1,0,0),
            new Vector3(1,0,0),
            new Vector3(1,0,0),
            new Vector3(1,0,0)
            };
        }

        
        mesh.normals = normals;

        Vector2[] uv = new Vector2[4];


        
            uv = new Vector2[4]{

            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, 0),
           };
         
        

       mesh.uv = uv;

        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;
        meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        

        filter.mesh = mesh;

        // if block state has a material
        if (voxel.data.block.material != null)
        {
            meshRenderer.material = voxel.data.block.material;

            if (voxel.data.block.textureUnwrapType == VoxelData.TextureUnwrapType.Net)
            {
               
                // select the correct texture from the 2d texture array on the material for this face
                switch (direction)
                {
                    case FaceDirection.West:
                        meshRenderer.material.SetFloat("_SliceRange", 12f);
                        break;

                    case FaceDirection.South:
                        meshRenderer.material.SetFloat("_SliceRange", 13f);
                        break;

                    case FaceDirection.East:
                        meshRenderer.material.SetFloat("_SliceRange", 14f);
                        break;

                    case FaceDirection.North:
                        meshRenderer.material.SetFloat("_SliceRange", 15f);
                        break;

                    case FaceDirection.Up:
                        meshRenderer.material.SetFloat("_SliceRange", 8f);
                        break;

                    case FaceDirection.Down:
                        meshRenderer.material.SetFloat("_SliceRange", 9f);
                        break;

                    default:
                        break;

                }
            }
        }

    }
}
