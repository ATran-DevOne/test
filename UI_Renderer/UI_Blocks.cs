using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace TestProject
{
    public partial class UI_Renderer : MonoBehaviour
    {
        private void UpdateBlocks()
        {
            if (!UI_Settings.update) 
            {
                return;
            }

            GameObject obj;
            GameObject parentBlocks, parentPositions, parentFloor, parentReinforcement;
            GameObject parentAdditions, parentSubs;
            int id, currentAngle;
            List<int> viewAngles = new List<int>();
            float offsetZ = 0f;

            clearModels();

            currentAngle = AngleFromView(UI_Settings.camera_view);
            viewAngles.Add((currentAngle + 360 - 90) % 360);
            viewAngles.Add(currentAngle);
            viewAngles.Add((currentAngle + 90) % 360);

            parentPositions = new GameObject();
            parentPositions.name = "PoolData > Positions";

            parentAdditions = new GameObject();
            parentAdditions.name = "PoolData > Additions";

            parentSubs = new GameObject();
            parentSubs.name = "PoolData > Subs";

            parentFloor = new GameObject();
            parentFloor.name = "PoolData > Floor";

            parentReinforcement = new GameObject();
            parentReinforcement.name = "PoolData > Reinforcement";

            if (UI_Settings.output.poolData.layers == null) return;
            foreach (Layer layer in UI_Settings.output.poolData.layers)
            {
                id = 0;
                parentBlocks = new GameObject();
                parentBlocks.name = "PoolData > Blocks[layer=" + layer.id + "]";

                if (UI_Settings.renderMode == RenderMode.BLOCKS)
                {
                    foreach (Block block in layer.blocks)
                    {
                        GridPosition gridPosition = UI_Settings.output.poolData.layers[layer.id].poolGrid.positions[block.gridPositionId];
                        
                        obj = createBlock(parentBlocks, block, gridPosition, layer.id, id);
                        if(layer.id != 0) 
                        {
                            SetLayerOnAllRecursive(obj, 9);
                        }

                        if (layer.id == 0 && UI_Settings.output.poolData.floor.mesh != null)
                        {
                            createReinforcement(parentReinforcement, block, gridPosition, id);
                            SetLayerOnAllRecursive(parentReinforcement, 9);
                        }

                        id++;
                    }

                    id = -1;
                    foreach (GridPosition position in layer.poolGrid.positions)
                    {
                        id++;
                        if (!position.avail || ViewFromAngle(position.angle) != UI_Settings.camera_view)
                        {
                            continue;
                        }

                        createSpot(parentPositions, position, layer.id, id);
                    }
                }

                id = -1;
                if (layer.additions.Count > 0)
                {
                    foreach (Addition addition in layer.additions)
                    {
                        id++;

                        GridPosition gridPosition = UI_Settings.output.poolData.layers[layer.id].poolGrid.positions[addition.gridPositionId];
                        if (UI_Settings.Orthogonal() && ViewFromAngle(gridPosition.angle) != UI_Settings.camera_view)
                        {
                            continue;
                        }

                        obj = createAddition(parentAdditions, addition, layer.id, id);
                        SetLayerOnAllRecursive(obj, 9);

                        if (addition.addLight && UI_Settings.renderMode == RenderMode.WORLD)
                        {
                            GameObject light = new GameObject();
                            light.name = "Spotlight";
                            light.transform.parent = obj.transform.parent;
                            light.transform.position = obj.transform.position + 300f * obj.transform.right;
                            light.transform.Rotate(obj.transform.rotation.eulerAngles + new Vector3(10, 90, 0));

                            UnityEngine.Light pl = light.AddComponent<UnityEngine.Light>();
                            pl.type = LightType.Spot;
                            pl.range = 15000f;
                            pl.spotAngle = 120f;
                            pl.color = new Color(200, 200, 200, 255);
                            pl.intensity = 0.01f;
                            pl.renderMode = LightRenderMode.ForcePixel;
                            pl.shadowResolution = UnityEngine.Rendering.LightShadowResolution.Medium;
                            pl.shadows = LightShadows.Hard;
                        }
                    }
                }
            }

            id = -1;
            if (UI_Settings.renderMode == RenderMode.BLOCKS)
            {
                foreach (FloorBlock floorBlock in UI_Settings.output.poolData.floor.floorBlocks)
                {
                    id++;
                    if (UI_Settings.output.poolData.floor.foundation)
                    {
                        createFloorBlock(parentFloor, floorBlock, id);
                        SetLayerOnAllRecursive(parentFloor, 9);
                    }

                    if (UI_Settings.output.poolData.floor.mesh != null)
                    {
                        createReinforcementMesh(parentReinforcement, floorBlock, id);
                        SetLayerOnAllRecursive(parentReinforcement, 9);
                    }
                }
            }

            if ((UI_Settings.output.poolData.width - UI_Settings.output.poolData.stair.width) % 600 > 0)
            {
                offsetZ = 150f;
            }

            GameObject parent = new GameObject();
            parent.name = "PoolData > General";
            createStair(parent, offsetZ);

            obj = createDimensions(parent, offsetZ);
            SetLayerOnAllRecursive(obj, 9);

            obj = createLetters(parent);
            SetLayerOnAllRecursive(obj, 9);

            obj = createWalls(parent);
            SetLayerOnAllRecursive(obj, 10);

            obj = createHighlight(parent);
            SetLayerOnAllRecursive(obj, 10);


            if (UI_Settings.renderMode == RenderMode.WORLD)
            {
                GameObject water = UI_MeshCreate.CreateWater(
                    UI_Settings.output.poolData.type,
                    new Vector3(UI_Settings.output.poolData.length, 1500f, UI_Settings.output.poolData.width),
                    false,
                    new Vector3(UI_Settings.output.poolData.stair.length, 0f, UI_Settings.output.poolData.stair.width),
                    offsetZ
                );
                water.transform.parent = parent.transform;

                MeshRenderer renderer = water.GetComponent<MeshRenderer>();
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.receiveShadows = false;

                GameObject walls = UI_MeshCreate.CreateWater(
                    UI_Settings.output.poolData.type,
                    new Vector3(UI_Settings.output.poolData.length, 1500f, UI_Settings.output.poolData.width),
                    true,
                    new Vector3(UI_Settings.output.poolData.stair.length, 0f, UI_Settings.output.poolData.stair.width),
                    offsetZ
                );
                renderer = walls.GetComponent<MeshRenderer>();
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.material = UI_Settings.materials["Blue"];
                walls.transform.parent = parent.transform;

                GameObject tiling = UI_MeshCreate.CreateTiling(
                    UI_Settings.output.poolData.type,
                    600f,
                    new Vector3(UI_Settings.output.poolData.length, 1515f, UI_Settings.output.poolData.width),
                    new Vector3(UI_Settings.output.poolData.stair.length, 0f, UI_Settings.output.poolData.stair.width),
                    offsetZ
                );

                renderer = tiling.GetComponentsInChildren<MeshRenderer>()[0];
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                renderer.receiveShadows = false;
                renderer.material = UI_Settings.materials["Tiling"];
                renderer.material.mainTextureScale = new Vector2(
                    (float)(UI_Settings.output.poolData.length / UI_Settings.output.poolData.baseSize) / 2f + 2f,
                    (float)(UI_Settings.output.poolData.width / UI_Settings.output.poolData.baseSize) / 2f + 2f);
                renderer.material.mainTextureOffset = new Vector2(0, 0);

                tiling.transform.parent = parent.transform;

                GameObject field = UI_MeshCreate.CreateField(
                    UI_Settings.output.poolData.type,
                    600f,
                    new Vector3(UI_Settings.output.poolData.length, 1500f, UI_Settings.output.poolData.width),
                    new Vector3(UI_Settings.output.poolData.stair.length, 0f, UI_Settings.output.poolData.stair.width),
                    offsetZ
                );

                renderer = field.GetComponentsInChildren<MeshRenderer>()[0];
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                renderer.receiveShadows = false;
                renderer.material = UI_Settings.materials["Grass"];
                renderer.material.mainTextureScale = new Vector2(
                    5000f,
                    5000f
                );
                renderer.material.mainTextureOffset = new Vector2(0, 0);

                field.transform.parent = parent.transform;
            }

        }

        private GameObject createBlock(GameObject parent, Block block, GridPosition gridPosition, int layerId, int blockId)
        {
            Vector3 position = new Vector3(gridPosition.x, 500 * layerId, gridPosition.y);
            position = shiftBlock(position, gridPosition.angle);
            string name = layerId + ":" + blockId + ":" + block.type;
            return createModel(parent, "Block_" + block.type, name, position, gridPosition.angle);
        }

        private void createSpot(GameObject parent, GridPosition position, int layerId, int gridPositionId)
        {
            GameObject go;
            Vector3 spotPos;
            string name;

            for (int i = 1; i <= 3; i++)
            {
                spotPos = new Vector3(position.x, 500 * layerId + 125 * i, position.y);
                spotPos = shiftSpot(spotPos, position.angle);
                name = layerId + ":" + (125 * i) + ":" + gridPositionId + ":0";
                go = createModel(parent, "Spot_", name, spotPos);
                Destroy(go.GetComponent<BoxCollider>());
                go.SetActive(false);
            }

            spotPos = new Vector3(position.x, 500 * layerId + 250, position.y);
            spotPos = shiftSpot(spotPos, position.angle);
            name = layerId + ":250:" + gridPositionId + ":4";
            go = createModel(parent, "Spot_", name, spotPos);
            Destroy(go.GetComponent<BoxCollider>());
            go.SetActive(false);
        }

        public GameObject createAddition(GameObject parent, Addition addition, int layerId = -1, int additionId = 999)
        {
            GridPosition gridPosition = new GridPosition();
            if (addition.gridPositionId > 0)
            {
                gridPosition = UI_Settings.output.poolData.layers[layerId].poolGrid.positions[addition.gridPositionId];
            }
            else
            {
                gridPosition.angle = AngleFromView(UI_Settings.camera_view);
            }

            if (layerId < 0) layerId = 0;
            if (addition.height <= 0) addition.height = 125;

            Vector3 position = new Vector3(gridPosition.x, 500 * layerId + addition.height, gridPosition.y);
            position = shiftAddition(position, gridPosition.angle);
            string name = layerId + ":" + additionId + ":" + addition.height + ":" + addition.type;

            return createModel(parent, "Addition_" + addition.type, name, position, gridPosition.angle);
        }

        public GameObject createSub(GameObject parent, Sub sub, int layerId = -1)
        {
            Addition addition = new Addition();
            GridPosition gridPosition = new GridPosition();
            if (sub.additionId >= 0)
            {
                addition = UI_Settings.output.poolData.layers[layerId].additions[sub.additionId];
                gridPosition = UI_Settings.output.poolData.layers[layerId].poolGrid.positions[addition.gridPositionId];
            }
            else
            {
                layerId = 0;
                sub.id = 999;
                gridPosition.angle = AngleFromView(UI_Settings.camera_view);
            }

            Vector3 position = new Vector3(gridPosition.x, 500 * layerId + addition.height, gridPosition.y);
            position = shiftAddition(position, gridPosition.angle);
            string name = layerId + ":" + sub.id + ":" + sub.type;

            return createModel(parent, "Sub_" + sub.type, name, position, gridPosition.angle);
        }

        private GameObject createFloorBlock(GameObject parent, FloorBlock floorBlock, int floorBlockId = 999)
        {
            Vector3 position = new Vector3(floorBlock.position.x, -(UI_Settings.output.poolData.floor.thickness + 50), floorBlock.position.y);
            string name = floorBlockId + ":" + floorBlock.type;
            return createModel(parent, "Floor_" + floorBlock.type, name, position, floorBlock.position.angle);
        }

        private GameObject createReinforcement(GameObject parent, Block block, GridPosition gridPosition, int floorBlockId)
        {
            GameObject go;
            string blockType = block.type.Replace("Base", "Reinforcement");
            blockType = blockType.Replace("Corner", "Reinforcement");
            blockType = blockType.Replace("Fit", "Reinforcement");

            Vector3 position = new Vector3(gridPosition.x, -(UI_Settings.output.poolData.floor.thickness - 125), gridPosition.y);
            position = shiftBlock(position, gridPosition.angle);

            string name = floorBlockId + ":" + blockType;
            go = createModel(parent, "Reinforcement_" + blockType, name, position, gridPosition.angle);
            go.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            return go;
        }

        private void createReinforcementMesh(GameObject parent, FloorBlock block, int floorBlockId)
        {
            Vector3 position;
            string blockType = "";

            GameObject go = new GameObject();
            go.name = "MeshBundle";

            if (block.type.StartsWith("Side"))
            {
                blockType = "ReinforcementSide4" + block.type.Substring(block.type.Length - 1, 1);
            }
            else if(block.type.StartsWith("Corner"))
            {
                blockType = "ReinforcementCorner44";
            }
            else
            {
                blockType = block.type.Replace("Center", "Reinforcement");
            }

            position = new Vector3(block.position.x, -(UI_Settings.output.poolData.floor.thickness + 50), block.position.y);
            string name = floorBlockId + ":" + blockType;
            
            GameObject go1 = createModel(parent, "ReinforcementMesh_" + blockType, name, position, block.position.angle);
            go1.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            go1.transform.parent = go.transform;

            if (UI_Settings.output.poolData.floor.mesh == "Double") 
            {
                position.y += UI_Settings.output.poolData.floor.thickness - 150;
                GameObject go2 = createModel(parent, "ReinforcementMesh_" + blockType, name, position, block.position.angle);
                go2.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                go2.transform.parent = go.transform;
            }

            go.transform.parent = parent.transform;
        }

        private GameObject createDimensions(GameObject parent, float offsetZ)
        {
            Vector3 rotation = new Vector3(0, 90, 0);
            Vector3 position = new Vector3(0, 0, 0);

            if (UI_Settings.renderMode == RenderMode.WORLD) return new GameObject();

            switch (UI_Settings.camera_view)
            {
                case View.NORTH:
                    rotation.x -= 90;
                    break;
                case View.EAST:
                    rotation.x -= 90;
                    break;
                case View.SOUTH:
                    rotation.x -= 90;
                    rotation.y += 180;
                    break;
                case View.WEST:
                    rotation.x -= 90;
                    rotation.y += 180;
                    break;
            }

            GameObject parent2 = new GameObject();
            parent2.name = "Dimensions";
            parent2.transform.parent = parent.transform;

            int poolWidth = UI_Settings.output.poolData.width;
            int poolLength = UI_Settings.output.poolData.length;
            int stairWidth = UI_Settings.output.poolData.stair.width;

            if (UI_Settings.camera_view == View.PERSPECTIVE ||
                UI_Settings.camera_view == View.EAST ||
                UI_Settings.camera_view == View.WEST)
            {
                if (UI_Settings.output.poolData.type == "Straight" && UI_Settings.output.poolData.stair.width != UI_Settings.output.poolData.width)
                {
                    if (UI_Settings.Orthogonal())
                    {
                        position = new Vector3(0, 0, 0);
                        position += 10f * UI_Settings.camera_main.transform.forward;
                        position.y -= 300 + UI_Settings.output.poolData.floor.thickness;
                    }
                    else
                    {
                        position = new Vector3(0, 0, 0);
                        position.x -= UI_Settings.output.poolData.length / 2 + 500;
                        position.y += 1500f;
                    }
                    UI_MeshCreate.CreateDimension(parent2, DimType.OUTER, "B_trap", position, rotation, stairWidth);
                }

                if (UI_Settings.Orthogonal())
                {
                    position = new Vector3(0, 0, 0);
                    position += 10f * UI_Settings.camera_main.transform.forward;
                    if (UI_Settings.output.poolData.type == "Straight" && UI_Settings.output.poolData.stair.width != UI_Settings.output.poolData.width)
                        position.y -= 850 + UI_Settings.output.poolData.floor.thickness;
                    else position.y -= 300 + UI_Settings.output.poolData.floor.thickness;
                }
                else
                {
                    position = new Vector3(0, 0, 0);
                    if (UI_Settings.output.poolData.type == "Straight" && UI_Settings.output.poolData.stair.width != UI_Settings.output.poolData.width)
                    {
                        position.x -= UI_Settings.output.poolData.length / 2 + 1200;
                    }
                    else
                    {
                        position.x -= UI_Settings.output.poolData.length / 2 + 500;
                    }
                    position.y += 1500f;
                }
                UI_MeshCreate.CreateDimension(parent2, DimType.OUTER, "B", position, rotation, poolWidth);
            }

            if (UI_Settings.camera_view == View.PERSPECTIVE ||
                UI_Settings.camera_view == View.NORTH ||
                UI_Settings.camera_view == View.SOUTH)
            {
                rotation.y -= 90;
                if (UI_Settings.Orthogonal())
                {
                    position = new Vector3(0, 0, 0);
                    position += 10f * UI_Settings.camera_main.transform.forward;
                    position.y -= 300 + UI_Settings.output.poolData.floor.thickness;
                }
                else
                {
                    position = new Vector3(0, 0, 0);
                    position.y += 1500f;
                    position.z -= UI_Settings.output.poolData.width / 2 + 500;
                }
                UI_MeshCreate.CreateDimension(parent2, DimType.OUTER, "L", position, rotation, poolLength);
            }

            return parent2;
        }

        private GameObject createLetters(GameObject parent)
        {
            GameObject parent2 = new GameObject();
            parent2.name = "Wall letters";

            createLetter(parent2, "N", new Vector3(
                0,
                750,
                UI_Settings.output.poolData.width / 2 - 30f),
            0);
            createLetter(parent2, "Z", new Vector3(
                0,
                750,
                -UI_Settings.output.poolData.width / 2 + 30f),
            180);
            createLetter(parent2, "O", new Vector3(
                UI_Settings.output.poolData.length / 2 - 30f,
                750,
                0),
            90);
            createLetter(parent2, "W", new Vector3(
                -UI_Settings.output.poolData.length / 2 + 30f,
                750,
                0),
            -90);

            parent2.transform.parent = parent.transform;
            return parent2;
        }

        private GameObject createWalls(GameObject parent)
        {
            GameObject parent2 = new GameObject();
            parent2.name = "Wall faces";

            createWall(parent2, "N");
            createWall(parent2, "O");
            createWall(parent2, "Z");
            createWall(parent2, "W");

            parent2.transform.parent = parent.transform;
            return parent2;
        }

        private void createLetter(GameObject parent, string letter, Vector3 position, int rotationY)
        {
            GameObject go = new GameObject();
            go.name = "Wall " + letter;
            go.transform.parent = parent.transform;

            TextMesh text = go.AddComponent<TextMesh>();
            text.characterSize = 30f;
            text.text = letter;
            text.font = (Font)Resources.Load("Montserrat-SemiBold", typeof(Font));
            text.fontSize = 500;
            text.anchor = TextAnchor.MiddleCenter;
            text.color = new Color(0f, 0f, 0f, 0.3f);

            go.transform.position = position;
            go.transform.eulerAngles = new Vector3(0, rotationY, 0);

            MeshRenderer mesh = go.GetComponent<MeshRenderer>();
            mesh.material = UI_Settings.materials["Text"];
            mesh.material.shader = Shader.Find("Isotras/Text");
        }

        private void createWall(GameObject parent, string wallName)
        {
            GameObject go = UI_MeshCreate.CreateWall(
                UI_Settings.input.poolData.type,
                wallName,
                new Vector3(UI_Settings.output.poolData.length, 1500f, UI_Settings.output.poolData.width),
                new Vector3(UI_Settings.output.poolData.stair.length, 0f, UI_Settings.output.poolData.stair.width)
            );
            go.transform.parent = parent.transform;

            MeshRenderer mesh = go.GetComponent<MeshRenderer>();
            mesh.material = UI_Settings.materials["FlagActive"];
            mesh.material.SetColor("_Color", new Color(0f, 1f, 0f, 0.7f));
            mesh.material.shader = Shader.Find("Isotras/DiffuseShadowsAlpha");
            mesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        private GameObject createHighlight(GameObject parent)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Plane);
            go.name = "Highlight";

            switch(UI_Settings.camera_view)
            {
                case View.NORTH:
                    go.transform.localPosition = new Vector3(0, 1650, UI_Settings.output.poolData.width / 2 + 150);
                    go.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    go.transform.localScale = new Vector3(UI_Settings.output.poolData.length, 1, UI_Settings.output.poolData.width / 10f);
                    break;

                case View.EAST:
                    go.transform.localPosition = new Vector3(UI_Settings.output.poolData.length / 2 + 150, 1650, 0);
                    go.transform.localRotation = Quaternion.Euler(0, 90, 0);
                    go.transform.localScale = new Vector3(UI_Settings.output.poolData.width, 1, UI_Settings.output.poolData.length / 10f);
                    break;

                case View.SOUTH:
                    go.transform.localPosition = new Vector3(0, 1650, -(UI_Settings.output.poolData.width / 2 + 150));
                    go.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    go.transform.localScale = new Vector3(UI_Settings.output.poolData.length, 1, UI_Settings.output.poolData.width / 10f);
                    break;

                case View.WEST:
                    go.transform.localPosition = new Vector3(-(UI_Settings.output.poolData.length / 2 + 150), 1650, 0);
                    go.transform.localRotation = Quaternion.Euler(0, 90, 0);
                    go.transform.localScale = new Vector3(UI_Settings.output.poolData.width, 1, UI_Settings.output.poolData.length / 10f);
                    break;
            }

            go.transform.parent = parent.transform;

            MeshRenderer mesh = go.GetComponent<MeshRenderer>();
            mesh.material = UI_Settings.materials["FlagActive"];
            mesh.material.SetColor("_Color", new Color(0f, 1f, 0f, 0.3f));
            mesh.material.shader = Shader.Find("Isotras/DiffuseShadowsAlpha");

            return go;
        }

        private GameObject createModel(GameObject parent, string type, string modelName, Vector3 position, int angle = 0, int scaleZ = 1)
        {
            GameObject obj;
            Material mat = null;
            float scale = 1f;
            bool Draggable = false;
            int shift = 0;

            try
            {
                obj = Instantiate(UI_Settings.models[type.Split('_')[1]]);
            }
            catch
            {
                obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            }

            if (type.StartsWith("Block_"))
            {
                mat = UI_Settings.materials["Base"];
            }

            else if (type.StartsWith("Stair_"))
            {
                if (UI_Settings.renderMode == RenderMode.BLOCKS)
                    mat = UI_Settings.materials["Base"];
                else mat = UI_Settings.materials["Blue"];
            }

            else if (type.StartsWith("Floor_"))
            {
                mat = UI_Settings.materials["Base"];
                shift = 150;
            }

            else if (type.StartsWith("Reinforcement_"))
            {
                mat = UI_Settings.materials["Corner"];
            }

            else if (type.StartsWith("ReinforcementMesh_"))
            {
                mat = UI_Settings.materials["Corner"];
                shift = 150;
            }

            else if (type.StartsWith("Addition_"))
            {
                Draggable = true;
            }

            else if (type.StartsWith("Sub_"))
            {
                if (UI_Settings.renderMode == RenderMode.BLOCKS)
                    mat = UI_Settings.materials["FlagActive"];
                else mat = UI_Settings.materials["Base"];
            }

            else
            {
                mat = UI_Settings.materials["Base"];
                scale = 50f;
                scaleZ = 50;
            }

            if (UI_Settings.Orthogonal())
            {
                if (!(type.StartsWith("Block_") || type.StartsWith("Dimension_") || type.StartsWith("Addition") || type.StartsWith("Sub_") || type.StartsWith("Reinforcement_") || type.StartsWith("ReinforcementMesh_")))
                {
                    mat = UI_Settings.materials["Grey"];
                }
                if (type.StartsWith("Block_") && angle != AngleFromView(UI_Settings.camera_view))
                {
                    mat = UI_Settings.materials["Grey"];
                }
            }

            position.x -= UI_Settings.output.poolData.length / 2 + 300 + shift;
            position.z -= UI_Settings.output.poolData.width / 2 + 300 + shift;
            
            obj.name = modelName;
            obj.transform.parent = parent.transform;
            obj.transform.position = position;
            obj.transform.eulerAngles = new Vector3(0, -90 - angle, 0);
            obj.transform.localScale = new Vector3(scale, scale, scaleZ);

            Transform firstChild = null;
            if (!type.StartsWith("Sub"))
            {
                foreach (Transform childObject in obj.transform)
                {
                    childObject.gameObject.AddComponent<MeshCollider>();
                    if (Draggable)
                    {
                        childObject.gameObject.AddComponent<UI_DragDrop>();
                    }
                    firstChild = childObject;
                }
            }

            MeshRenderer renderer = obj.GetComponentsInChildren<MeshRenderer>()[0];
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            if(mat != null) renderer.material = mat;
            
            if (firstChild != null)
                return firstChild.gameObject;
            else return obj;
        }

        private void clearModels()
        {
            List<GameObject> objectsInScene = new List<GameObject>();
            foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
            {
                if (go.name.Contains("PoolData > ") || go.name.Contains("Dimension detail"))
                {
                    Destroy(go);
                }
            }
        }

        private void LoadResources()
        {
            StartCoroutine(LoadAssetBundleFromServer("models"));
            StartCoroutine(LoadAssetBundleFromServer("materials"));
            return;
        }

        public IEnumerator LoadAssetBundleFromServer(string type)
        {
            string url = UI_Settings.url_resources + type + ".unity3d";
            UnityWebRequest webRequest = UnityWebRequestAssetBundle.GetAssetBundle(url, 0);

            yield return webRequest.SendWebRequest();

            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(webRequest);
            if (type == "models")
            {
                UI_Settings.models = new Dictionary<string, GameObject>();
                foreach (GameObject obj in bundle.LoadAllAssets<GameObject>())
                {
                    UI_Settings.models[obj.name] = obj;
                }
            }
            if (type == "materials")
            {
                UI_Settings.materials = new Dictionary<string, Material>();
                foreach (Material mat in bundle.LoadAllAssets<Material>())
                {
                    mat.shader = Shader.Find(mat.shader.name);
                    if(mat.shader.name == "Isotras/Text")
                        mat.SetTexture("_MainTex", fontTexture);
                    UI_Settings.materials[mat.name] = mat;
                }
            }
        }

        private Vector3 shiftBlock(Vector3 position, int angle)
        {
            switch (angle)
            {
                case 90:
                    position.x += 300;
                    break;
                case 180:
                    position.x += 300;
                    position.z += 300;
                    break;
                case 270:
                    position.z += 300;
                    break;
                default:
                    break;
            }
            return position;
        }

        private Vector3 shiftSpot(Vector3 position, int angle = 0)
        {
            switch (angle)
            {
                case 90:
                    position.z += 150;
                    break;
                case 180:
                    position.x += 150;
                    break;
                case 270:
                    position.x += 300;
                    position.z += 150;
                    break;
                default:
                    position.x += 150;
                    position.z += 300;
                    break;
            }
            return position;
        }

        private Vector3 shiftAddition(Vector3 position, int angle = 0)
        {
            switch (angle)
            {
                case 90:
                    position.x += 300;
                    position.z += 150;
                    break;
                case 180:
                    position.x += 150;
                    position.z += 300;
                    break;
                case 270:
                    position.z += 150;
                    break;
                default:
                    position.x += 150;
                    break;
            }
            return position;
        }

        public View ViewFromAngle(int angle)
        {
            switch (angle)
            {
                case 90:
                    return View.EAST;
                case 180:
                    return View.NORTH;
                case 270:
                    return View.WEST;
                default:
                    return View.SOUTH;
            }
        }

        public int AngleFromView(View view)
        {
            switch (view)
            {
                case View.EAST:
                    return 90;
                case View.NORTH:
                    return 180;
                case View.WEST:
                    return 270;
                default:
                    return 0;
            }
        }

        private void SetLayerOnAllRecursive(GameObject obj, int layer)
        {
            obj.layer = layer;
            foreach (Transform child in obj.transform)
            {
                SetLayerOnAllRecursive(child.gameObject, layer);
            }
        }
    }
}
