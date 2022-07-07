using System;
using UnityEngine;

namespace TestProject
{
    public static class UI_MeshCreate
    {
        public static GameObject CreateWater(string poolType, Vector3 dims, bool isWall, Vector3 stair = new Vector3(), float offsetZ = 0)
        {
            GameObject _cube = new GameObject("Cube");
            _cube.AddComponent<MeshRenderer>();
            MeshFilter meshFilter = _cube.AddComponent<MeshFilter>();
            Mesh mesh = meshFilter.mesh;
            float length = dims.x;
            float width = dims.z;
            float height = dims.y;

            if (poolType == "Corner")
            {
                Vector3[] c = new Vector3[8];

                c[0] = new Vector3(-length * .5f, -width * .5f, height * .5f);
                c[1] = new Vector3(length * .5f, -width * .5f, height * .5f);
                c[2] = new Vector3(length * .5f, -width * .5f, -height * .5f);
                c[3] = new Vector3(-length * .5f, -width * .5f, -height * .5f);

                c[4] = new Vector3(-length * .5f, width * .5f, height * .5f);
                c[5] = new Vector3(length * .5f, width * .5f, height * .5f);
                c[6] = new Vector3(length * .5f, width * .5f, -height * .5f);
                c[7] = new Vector3(-length * .5f, width * .5f, -height * .5f);

                Vector3[] vertices = new Vector3[]
                {
                    c[0], c[1], c[2], c[3],
                    c[7], c[4], c[0], c[3],
                    c[4], c[5], c[1], c[0],
                    c[6], c[7], c[3], c[2],
                    c[5], c[6], c[2], c[1],
                    c[7], c[6], c[5], c[4]
                };


                Vector3 up = Vector3.up;
                Vector3 down = Vector3.down;
                Vector3 forward = Vector3.forward;
                Vector3 back = Vector3.back;
                Vector3 left = Vector3.left;
                Vector3 right = Vector3.right;

                Vector3[] normals = new Vector3[]
                {
                    down, down, down, down,
	                left, left, left, left,
	                forward, forward, forward, forward,
	                back, back, back, back,
	                right, right, right, right,
	                up, up, up, up 
                };

                Vector2 uv00 = new Vector2(0f, 0f);
                Vector2 uv10 = new Vector2(1f, 0f);
                Vector2 uv01 = new Vector2(0f, 1f);
                Vector2 uv11 = new Vector2(1f, 1f);

                Vector2[] uvs = new Vector2[]
                {
                    uv11, uv01, uv00, uv10,
	                uv11, uv01, uv00, uv10,
	                uv11, uv01, uv00, uv10,
	                uv11, uv01, uv00, uv10,    
	                uv11, uv01, uv00, uv10,
	                uv11, uv01, uv00, uv10
                };

                int[] triangles;
                if (!isWall)
                {
                    triangles = new int[]
                    {
                        3, 1, 0,        3, 2, 1,
	                    7, 5, 4,        7, 6, 5,
	                    11, 9, 8,       11, 10, 9, 
	                    15, 13, 12,     15, 14, 13,
	                    19, 17, 16,     19, 18, 17,
	                    23, 21, 20,     23, 22, 21
                    };
                }
                else
                {
                    triangles = new int[]
                    {
                        0, 1, 3,        1, 2, 3,
	                    4, 5, 7,        5, 6, 7,
	                    8, 9, 11,       9, 10, 11, 
	                    12, 13, 15,     13, 14, 15,
	                    16, 17, 19,     17, 18, 19,
	                    20, 21, 23,     21, 22, 23
                    };
                }

                //8) Build the Mesh
                mesh.Clear();
                mesh.vertices = vertices;
                mesh.triangles = triangles;
                mesh.normals = normals;
                mesh.uv = uvs;
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
                mesh.Optimize();

                _cube.transform.Translate(0f, 750f, 0f);
                _cube.transform.Rotate(90f, 0f, 0f);
            }

            if (!isWall)
            {
                LiquidVolumeFX.LiquidVolume water = _cube.AddComponent<LiquidVolumeFX.LiquidVolume>();
                water.topology = LiquidVolumeFX.TOPOLOGY.Irregular;
                water.detail = LiquidVolumeFX.DETAIL.DefaultNoFlask;
                water.depthAware = true;
                water.level = 0.95f;
                water.liquidColor1 = new Color(42f / 255f, 245f / 255f, 1f, 42f / 255f);
                water.liquidScale1 = 0.34f;
                water.liquidColor2 = new Color(1f, 1f, 1f, 0f);
                water.murkiness = 0.6f;
                water.emissionBrightness = 0f;
                water.turbulence1 = 1f;
                water.turbulence2 = 0f;
                water.deepObscurance = 4f;
                water.flaskThickness = 0f;
                water.flaskGlossinessExternal = 0f;
                water.flaskGlossinessInternal = 1f;
                water.smokeEnabled = false;
                water.foamThickness = 0f;

                _cube.name = "Water";
            }
            else _cube.name = "Wall";

            return _cube;
        }

        public static GameObject CreateTiling(string poolType, float offset, Vector3 dims, Vector3 stair = new Vector3(), float offsetZ = 0f)
        {
            GameObject _cube = new GameObject("Cube");
            _cube.AddComponent<MeshRenderer>();
            MeshFilter meshFilter = _cube.AddComponent<MeshFilter>();
            Mesh mesh = meshFilter.mesh;
            float length = dims.x;
            float width = dims.z;
            float height = dims.y;
            float offsetY = 5f;


            if (poolType == "Straight" && (int)stair.z != (int)width)
            {
                Vector3[] vertices = new Vector3[16];

                vertices[0] = new Vector3(-(length / 2f + offset), 0, -(stair.z / 2f + offset) - offsetZ);
                vertices[1] = new Vector3(-(length / 2f + offset - stair.x), 0, vertices[0].z);
                vertices[2] = new Vector3(vertices[1].x, 0, -(width / 2f + offset));
                vertices[3] = new Vector3(length / 2f + offset, 0, vertices[2].z);
                vertices[4] = new Vector3(vertices[3].x, 0, -vertices[3].z);
                vertices[5] = new Vector3(vertices[2].x, 0, -vertices[2].z);
                vertices[6] = new Vector3(vertices[1].x, 0, stair.z / 2f + offset - offsetZ);
                vertices[7] = new Vector3(vertices[0].x, 0, vertices[6].z);

                vertices[8] = new Vector3(vertices[0].x + offset, 0, vertices[0].z + offset);
                vertices[9] = new Vector3(vertices[1].x + offset, 0, vertices[1].z + offset);
                vertices[10] = new Vector3(vertices[2].x + offset, 0, vertices[2].z + offset);
                vertices[11] = new Vector3(vertices[3].x - offset, 0, vertices[3].z + offset);
                vertices[12] = new Vector3(vertices[4].x - offset, 0, vertices[4].z - offset);
                vertices[13] = new Vector3(vertices[5].x + offset, 0, vertices[5].z - offset);
                vertices[14] = new Vector3(vertices[6].x + offset, 0, vertices[6].z - offset);
                vertices[15] = new Vector3(vertices[7].x + offset, 0, vertices[7].z - offset);

                Vector3 up = Vector3.up;
                Vector3[] normals = new Vector3[]
                {
                up, up, up, up, up, up, up, up,
                up, up, up, up, up, up, up, up
                };

                float uvx1 = offset / (2f * offset + length);
                float uvx2 = stair.x / (2f * offset + length);
                float uvx3 = (offset + stair.x) / (2f * offset + length);
                float uvx4 = (length + offset) / (2f * offset + length);

                float uvy1 = offset / (2f * offset + width);
                float uvy2 = ((width - stair.z) / 2f - offsetZ) / (2f * offset + width);
                float uvy3 = ((width - stair.z) / 2f + offset - offsetZ) / (2f * offset + width);
                float uvy4 = uvy3 + stair.z / (2f * offset + width);
                float uvy5 = uvy4 + offset / (2f * offset + width);

                Vector2[] uvs = new Vector2[16];
                uvs[0] = new Vector2(0f, uvy2);
                uvs[1] = new Vector2(uvx2, uvs[0].y);
                uvs[2] = new Vector2(uvs[1].x, 0f);
                uvs[3] = new Vector2(1f, uvs[2].y);
                uvs[4] = new Vector2(uvs[3].x, 1f);
                uvs[5] = new Vector2(uvx2, uvs[4].y);
                uvs[6] = new Vector2(uvs[5].x, uvy5);
                uvs[7] = new Vector2(0f, uvs[6].y);

                uvs[8] = new Vector2(uvx1, uvy3);
                uvs[9] = new Vector2(uvx3, uvs[8].y);
                uvs[10] = new Vector2(uvs[9].x, uvy1);
                uvs[11] = new Vector2(uvx4, uvs[10].y);
                uvs[12] = new Vector2(uvs[11].x, 1f - uvs[11].y);
                uvs[13] = new Vector2(uvs[10].x, uvs[12].y);
                uvs[14] = new Vector2(uvs[10].x, uvy4);
                uvs[15] = new Vector2(uvs[8].x, uvs[14].y);

                int[] triangles = new int[]
                {
                2, 9, 10,       2, 10, 3,
                10, 11, 3,      3, 11, 12,
                3, 12, 4,       4, 12, 13,
                4, 13, 5,       5, 13, 14,
                5, 14, 6,       6, 14, 7,
                14, 15, 7,      7, 15, 0,
                15, 8, 0,       0, 8, 9,
                0, 9, 1,        2, 1, 9
                };

                mesh.Clear();
                mesh.vertices = vertices;
                mesh.triangles = triangles;
                mesh.normals = normals;
                mesh.uv = uvs;
                mesh.Optimize();
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();

                _cube.transform.Translate(0f, height + offsetY, 0f);
            }

            _cube.name = "Tiling";
            return _cube;
        }

        public static GameObject CreateWall(string poolType, string wallName, Vector3 dims, Vector3 stair = new Vector3())
        {
            GameObject _wall = new GameObject("Cube");
            _wall.AddComponent<MeshRenderer>();
            MeshFilter meshFilter = _wall.AddComponent<MeshFilter>();
            Mesh mesh = meshFilter.mesh;

            float length = dims.x;
            float width = dims.z;
            float height = dims.y;
            float offset = 30f;

            if (poolType == "Corner")
            {
                Vector3[] c = new Vector3[4];
                if (wallName == "N")
                {
                    c[0] = new Vector3(-length * .5f, width * .5f - offset, height * .5f);
                    c[1] = new Vector3(length * .5f, width * .5f - offset, height * .5f);
                    c[2] = new Vector3(length * .5f, width * .5f - offset, -height * .5f);
                    c[3] = new Vector3(-length * .5f, width * .5f - offset, -height * .5f);
                }

                if (wallName == "O")
                {
                    c[0] = new Vector3(length * .5f - offset, width * .5f, height * .5f);
                    c[1] = new Vector3(length * .5f - offset, -width * .5f, height * .5f);
                    c[2] = new Vector3(length * .5f - offset, -width * .5f, -height * .5f);
                    c[3] = new Vector3(length * .5f - offset, width * .5f, -height * .5f);
                }

                if (wallName == "Z")
                {
                    c[0] = new Vector3(length * .5f, -width * .5f + offset, height * .5f);
                    c[1] = new Vector3(-length * .5f, -width * .5f + offset, height * .5f);
                    c[2] = new Vector3(-length * .5f, -width * .5f + offset, -height * .5f);
                    c[3] = new Vector3(length * .5f, -width * .5f + offset, -height * .5f);
                }

                if (wallName == "W")
                {
                    c[0] = new Vector3(-length * .5f + offset, -width * .5f, height * .5f);
                    c[1] = new Vector3(-length * .5f + offset, width * .5f, height * .5f);
                    c[2] = new Vector3(-length * .5f + offset, width * .5f, -height * .5f);
                    c[3] = new Vector3(-length * .5f + offset, -width * .5f, -height * .5f);
                }

                Vector3[] vertices = new Vector3[]
                {
                    c[0], c[1], c[2], c[3]
                };

                Vector3[] normals = new Vector3[]
                {
                    Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward
                };

                int[] triangles = new int[]
                {
                    2, 1, 0,
                    2, 0, 3
                };

                mesh.Clear();
                mesh.vertices = vertices;
                mesh.triangles = triangles;
                mesh.normals = normals;
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
                mesh.Optimize();

                _wall.transform.Translate(0f, 750f, 0f);
                _wall.transform.Rotate(90f, 0f, 0f);
            }

            _wall.name = "Wall " + wallName;
            return _wall;
        }

        public static GameObject CreateField(string poolType, float offset, Vector3 dims, Vector3 stair = new Vector3(), float offsetZ = 0f)
        {
            offset = 1000000;
            GameObject field = CreateTiling(poolType, offset, dims, stair, offsetZ);
            field.transform.Translate(new Vector3(0, -5f, 0));
            field.name = "Field";
            return field;
        }

        public static void CreateDimension(GameObject parent, DimType type, string var, Vector3 position, Vector3 rotation, int length)
        {
            MeshRenderer meshRenderer;
            GameObject go;
            int moveToCamera = 0, offsetLine = 250, fontSize = 240, width = 25, lineLen = 250;
            Color color = new Color(1f, 1f, 1f, 1f);

            if(type == DimType.DETAIL || type == DimType.HEIGHT)
            {
                offsetLine = 150;
                fontSize = 120;
                width = 15;
                color = new Color(255f, 0f, 0f, 0.88f);
                lineLen = 100;
            }

            GameObject parent2 = new GameObject();
            parent2.name = "Dimension > " + var;
            parent2.transform.position = new Vector3(position.x, position.y, position.z);
            parent2.transform.eulerAngles = rotation;
            parent2.transform.parent = parent.transform;

            CreateDimLine(parent2, new Vector3(0, moveToCamera, 0), length, width, 0, color);
            CreateDimLine(parent2, new Vector3(-length / 2, moveToCamera, 0), lineLen, width, -90, color);
            CreateDimLine(parent2, new Vector3(length / 2, moveToCamera, 0), lineLen, width, -90, color);
            CreateDimTriangle(parent2, new Vector3(-length / 2, moveToCamera, 0), lineLen / 2, false, color);
            CreateDimTriangle(parent2, new Vector3(length / 2, moveToCamera, 0), lineLen / 2, true, color);

            go = new GameObject();
            go.transform.parent = parent2.transform;
            go.name = "Dimension";

            TextMesh text = go.AddComponent<TextMesh>();
            text.characterSize = 10f;
            if(type == DimType.OUTER)
                text.text = var + " = " + Math.Round((float)length / 1000f, 1) + " m";
            else
                text.text = Math.Round((float)length / 10f, 1) + " cm";
            text.font = (Font)Resources.Load("Montserrat-SemiBold", typeof(Font));
            text.fontSize = fontSize;
            text.color = color;
            text.anchor = TextAnchor.MiddleCenter;
            go.transform.localPosition = new Vector3(0, moveToCamera, -offsetLine);
            go.transform.localRotation = Quaternion.Euler(90, 0, 0);

            meshRenderer = go.GetComponent<MeshRenderer>();
            meshRenderer.material = UI_Settings.materials["Text"];
            meshRenderer.material.shader = Shader.Find("Isotras/Text");
        }

        private static GameObject CreateDimLine(GameObject parent, Vector3 position, int length, int width, int rotation, Color color)
        {
            GameObject go = new GameObject();
            go.AddComponent<MeshFilter>();
            go.AddComponent<MeshRenderer>();
            Mesh mesh = go.GetComponent<MeshFilter>().mesh;
            mesh.Clear();
            mesh.vertices = new Vector3[] {
                new Vector3(-0.5f, -0.5f, 0),
                new Vector3(-0.5f, 0.5f, 0),
                new Vector3(0.5f, 0.5f, 0),
                new Vector3(0.5f, -0.5f, 0),
            };
            mesh.uv = new Vector2[] {
                new Vector2(0, 0),
                new Vector2(0, 1),
                new Vector2(1, 1),
                new Vector2(1, 0)
            };
            mesh.triangles = new int[] {
                0, 1, 2,
                2, 3, 0
            };

            go.name = "Line";
            go.transform.parent = parent.transform;
            go.transform.localPosition = position;
            go.transform.localScale = new Vector3(length, width, width);
            go.transform.localRotation = Quaternion.Euler(90, rotation, 0);

            MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
            meshRenderer.material = UI_Settings.materials["Base"];

            if(color.a > 0.9f)
                meshRenderer.material.shader = Shader.Find("Isotras/Text");
            else
                meshRenderer.material.shader = Shader.Find("Isotras/DiffuseShadowsAlpha");
            meshRenderer.material.SetColor("_Color", color);
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            return go;
        }

        private static GameObject CreateDimTriangle(GameObject parent, Vector3 position, int size, bool invert, Color color)
        {
            int rotation = 0;
            GameObject go = new GameObject();
            go.AddComponent<MeshFilter>();
            go.AddComponent<MeshRenderer>();
            Mesh mesh = go.GetComponent<MeshFilter>().mesh;
            mesh.Clear();
            mesh.vertices = new Vector3[] {
                new Vector3(0f, 0f, 0),
                new Vector3(1f, 0.5f, 0),
                new Vector3(1f, -0.5f, 0)
            };
            mesh.uv = new Vector2[] {
                new Vector2(0f, 0.5f),
                new Vector2(1f, 1f),
                new Vector2(0f, 1f)
            };
            mesh.triangles = new int[] {
                0, 1, 2
            };

            go.name = "Triangle";
            go.transform.parent = parent.transform;
            go.transform.localPosition = position;
            go.transform.localScale = new Vector3(size, size, size);

            if (invert) rotation = 180;
            go.transform.localRotation = Quaternion.Euler(90, rotation, 0);

            MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
            meshRenderer.material = UI_Settings.materials["Base"];

            if (color.a > 0.9f)
                meshRenderer.material.shader = Shader.Find("Isotras/Text");
            else
                meshRenderer.material.shader = Shader.Find("Isotras/DiffuseShadowsAlpha");
            meshRenderer.material.SetColor("_Color", color);
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            return go;
        }
    }
}