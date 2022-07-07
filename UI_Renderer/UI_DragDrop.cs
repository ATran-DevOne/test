using Leguar.TotalJSON;
using PowerUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TestProject
{
    public class UI_DragDrop : MonoBehaviour
    {
        private Vector3 screenPoint;
        private Vector3 offset;
        private bool freeMode = false;
        private bool dragging = false;
        private bool selected = false;
        private bool valid = true;

        private Addition my;
        private int layerId = -1, additionId = -1;
        private int layerId_new, gridPositionId_new = -1, height_new;
        private Vector3 myPosition;
        

        void Start()
        {
            string[] split = transform.parent.name.Split(':');
            layerId = Int16.Parse(split[0]);
            additionId = Int16.Parse(split[1]);
            if (additionId != 999) my = UI_Settings.output.poolData.layers[layerId].additions[additionId];
            else {
                additionId = UI_Settings.input.poolData.layers[layerId].additions.Count - 1;
                my = UI_Settings.input.poolData.layers[layerId].additions[additionId];
            }

            myPosition = transform.parent.position;
        }

        private void Update()
        {
            if (freeMode)
            {
                OnMouseDrag(); 
                if (UnityEngine.Input.GetMouseButtonDown(0))
                {
                    OnMouseUp();
                }
            }
        }

        private void OnMouseEnter()
        {
            if (dragging) return;
            if (UI_Settings.camera_view == View.PERSPECTIVE) return;
            CreateFlag();
            GenerateDims();
        }

        private void OnMouseExit()
        {
            HideFlag();
            DestroyAll("DimensionDetail");
        }

        void OnMouseDown()
        {
            if (UI_Settings.camera_view == View.PERSPECTIVE) return;
            if (UI_Settings.Argument != null) return;

            screenPoint = new Vector3(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y, screenPoint.z);
            offset = transform.position - UI_Settings.camera_main.ScreenToWorldPoint(new Vector3(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y, screenPoint.z));

            if (additionId >= 0)
            {
                UI_Settings.selection = additionId;
            }

            EnableSpots(true);
        }

        private void OnMouseDrag()
        {
            if (UI_Settings.camera_view == View.PERSPECTIVE) return;
            if (UI_Settings.Argument != null) return;

            Vector3 curScreenPoint = new Vector3(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y, screenPoint.z);
            if (Vector2.Distance(curScreenPoint, screenPoint) < 5f) return;

            valid = true;
            dragging = true;
            HideFlag();

            Vector3 curPosition = UI_Settings.camera_main.ScreenToWorldPoint(curScreenPoint) + offset;
            Vector3 origPosition = transform.position;
            Vector3 myPos = UI_Settings.camera_main.WorldToViewportPoint(curPosition);
            float minDist = Mathf.Infinity;
            Vector3 snapTo = Vector3.zero;

            foreach (Transform go in GameObject.Find("PoolData > Positions").transform)
            {
                if (!go.gameObject.activeSelf) continue;

                Vector3 thisPos = UI_Settings.camera_main.WorldToViewportPoint(go.position);
                float dist = Vector2.Distance(thisPos, myPos);
                if (minDist == 0f || dist < minDist)
                {
                    string[] split = go.name.Split(':');
                    try
                    {
                        bool nextAvail = UI_Settings.output.poolData.layers[Int16.Parse(split[0])].poolGrid.positions[Int16.Parse(split[2]) + 1].avail;
                        if (my.numPositions == 2 && !nextAvail)
                        {
                            continue;
                        }
                    }
                    catch
                    {
                        continue;
                    }

                    minDist = dist;
                    snapTo = go.position;

                    layerId_new = Int16.Parse(split[0]);
                    height_new = Int16.Parse(split[1]);
                    gridPositionId_new = Int16.Parse(split[2]);
                }
            }

            transform.position = origPosition;
            transform.parent.position = snapTo;

            foreach (Transform go in GameObject.Find("PoolData > Subs").transform)
            {
                if (go.name.StartsWith(layerId + ":" + additionId))
                {
                    go.transform.position = origPosition;
                }
            }

            if (freeMode) 
            { 
                if (minDist > 0.03f)
                {
                    valid = false;
                }
            }

            GenerateDims();
        }

        void OnMouseUp()
        {
            if (UI_Settings.camera_view == View.PERSPECTIVE) return;
            if (UI_Settings.Argument != null) return;

            bool occupied = false;
            if (layerId_new >= 0 && gridPositionId_new >= 0)
            {
                int myX = UI_Settings.output.poolData.layers[layerId_new].poolGrid.positions[gridPositionId_new].x;

                foreach (Addition addition in UI_Settings.input.poolData.layers[layerId_new].additions)
                {
                    if (addition.gridPositionId == gridPositionId_new)
                    {
                        occupied = true;
                        continue;
                    }

                    if (my.mirror)
                    {
                        int thisX = UI_Settings.output.poolData.layers[layerId_new].poolGrid.positions[addition.gridPositionId].x;
                        if (thisX == myX && addition.id != additionId)
                        {
                            occupied = true;
                            continue;
                        }
                    }
                }
            }

            UI_Settings.selection = -1;
            DestroyAll("DimensionDetail");
            EnableSpots(false);
            if (!valid || (freeMode && occupied))
            {
                Destroy(transform.parent.gameObject);
                Destroy(gameObject);
                UI_Settings.input.poolData.layers[0].additions.RemoveAt(UI_Settings.input.poolData.layers[0].additions.Count -1);

                if (occupied)
                {
                    object[] paramss = new object[3];
                    paramss[0] = "duplicate";
                    paramss[1] = null;
                    paramss[2] = true;
                    UI.document.Run("showDialog", paramss);
                }
                return;
            }

            if (!freeMode && occupied)
            {
                dragging = false;
                transform.parent.position = myPosition;

                if (gridPositionId_new != my.gridPositionId)
                {
                    object[] paramss = new object[3];
                    paramss[0] = "duplicate";
                    paramss[1] = null;
                    paramss[2] = true;
                    UI.document.Run("showDialog", paramss);
                }
                return;
            }

            if (!dragging)
            {
                if (UI.document.getElementById("dialog-duplicate").style.display != "block")
                {
                    selected = true;

                    object[] paramss = new object[2];
                    paramss[0] = layerId;
                    paramss[1] = my.id;
                    UI.document.Run("changeAddition", paramss);

                    paramss = new object[3];
                    paramss[0] = "addition";
                    paramss[1] = null;
                    paramss[2] = true;
                    UI.document.Run("showDialog", paramss);
                }

                return;
            }

            if (layerId_new != layerId)
            {
                Addition addition = new Addition();

                addition.id = UI_Settings.input.poolData.layers[layerId_new].additions.Count;
                addition.gridPositionId = gridPositionId_new;
                addition.height = height_new;
                addition.addLight = my.addLight;
                addition.groupId = my.groupId;
                addition.mirror = my.mirror;
                addition.numPositions = my.numPositions;
                addition.type = my.type;

                RemoveAddition(layerId, additionId);
                UI_Settings.input.poolData.layers[layerId_new].additions.Add(addition);
            }
            else if (gridPositionId_new > 0)
            {
                int myX = UI_Settings.output.poolData.layers[layerId].poolGrid.positions[my.gridPositionId].x;

                UI_Settings.input.poolData.layers[layerId].additions[additionId].gridPositionId = gridPositionId_new;
                UI_Settings.input.poolData.layers[layerId].additions[additionId].height = height_new;

                if (my.mirror)
                {
                    foreach(Addition addition in UI_Settings.input.poolData.layers[layerId].additions)
                    {
                        int thisX = UI_Settings.output.poolData.layers[layerId].poolGrid.positions[addition.gridPositionId].x;
                        if (thisX == myX && addition.id != additionId)
                        {
                            RemoveAddition(layerId, addition.id, false);
                            break;
                        }
                    }
                }
            }

            UI_Settings.changed = true;
            GameObject.Find("JsonConnect").GetComponent<JsonConnect>().CallVerifier();
            DestroyAll("DimensionDetail");
            Destroy(gameObject);
        }

        public void EnableFreemode()
        {
            Start();
            EnableSpots(true);
            offset = Vector3.zero;
            UI_Settings.selection = additionId;

            freeMode = true;
        }

        private void CreateFlag()
        {
            object[] paramss = new object[5];
            paramss[0] = my.type;
            paramss[1] = "";
            paramss[2] = my.height + layerId * 500 + "mm";
            paramss[3] = layerId;
            paramss[4] = additionId;
            UI.document.Run("renderFlag", paramss);
        }

        private void HideFlag()
        {
            UI.document.Run("hideFlag");
        }

        public void GenerateDims()
        {
            DestroyAll("DimensionDetail");

            List<int> positions = new List<int>();
            int thisHeight, thisLayer, counterPos = 0;
            Vector3 rotation = new Vector3();
            Vector3 position = new Vector3();

            foreach (Transform go in GameObject.Find("PoolData > Additions").transform)
            {
                string[] split = go.gameObject.name.Split(':');
                thisLayer = Int16.Parse(split[0]);
                thisHeight = Int16.Parse(split[2]);

                if (500 * thisLayer + thisHeight == 500 * layerId + my.height)
                {
                    if(UI_Settings.camera_view == View.NORTH || UI_Settings.camera_view == View.SOUTH)
                    {
                        // Add start and end of wall to listing
                        if(positions.Count == 0)
                        {
                            positions.Add(-UI_Settings.output.poolData.length / 2);
                            positions.Add(UI_Settings.output.poolData.length / 2);
                            counterPos = (int)go.transform.position.z;
                            rotation = go.transform.eulerAngles;
                        }

                        if(my.numPositions == 1)
                        {
                            positions.Add((int)go.transform.position.x);
                        }
                        else
                        {
                            int thisPos = (int)go.transform.position.x;
                            positions.Add(thisPos - 150 * (int)UI_Settings.camera_main.transform.right.x);
                        }
                    }
                    else
                    {
                        if (positions.Count == 0) 
                        {
                            positions.Add(-UI_Settings.output.poolData.width / 2);
                            positions.Add(UI_Settings.output.poolData.width / 2);
                            counterPos = (int)go.transform.position.x;
                            rotation = go.transform.eulerAngles;
                        }

                        if (my.numPositions == 1)
                        {
                            positions.Add((int)go.transform.position.z);
                        }
                        else
                        {
                            int thisPos = (int)go.transform.position.z;
                            positions.Add(thisPos - 150 * (int)UI_Settings.camera_main.transform.right.z);
                        }
                    }
                }
            }

            Vector3 thisRotation = rotation;
            thisRotation.x -= 90;
            thisRotation.y -= 90;

            positions.Sort();
            GameObject parent = new GameObject();
            parent.name = "Dimensions detail";
            parent.tag = "DimensionDetail";
            parent.layer = 9;
            for(int i = 1; i < positions.Count; i++)
            {
                position = new Vector3(0, transform.position.y, 0);

                if (UI_Settings.camera_view == View.NORTH || UI_Settings.camera_view == View.SOUTH)
                {
                    position.x = positions[i - 1] + (positions[i] - positions[i - 1]) / 2;
                }
                else
                {
                    position.z = positions[i - 1] + (positions[i] - positions[i - 1]) / 2;
                }

                position += 10f * UI_Settings.camera_main.transform.forward - 200f * UI_Settings.camera_main.transform.up;
                UI_MeshCreate.CreateDimension(parent, DimType.DETAIL, "Measure", position, thisRotation, positions[i] - positions[i - 1]);
            }

            if (my.numPositions == 2) return;

            if (UI_Settings.camera_view == View.NORTH || UI_Settings.camera_view == View.SOUTH)
            {
                position = new Vector3(
                    transform.position.x,
                    (1500f + transform.position.y) / 2f,
                    0
                );
            }
            else
            {
                position = new Vector3(
                    0,
                    (1500f + transform.position.y) / 2f,
                    transform.position.z
                );
            }
            position += 10f * UI_Settings.camera_main.transform.forward + 200f * UI_Settings.camera_main.transform.right;
            thisRotation.x += 90;
            thisRotation.y -= 90;
            thisRotation.z += 90;

            UI_MeshCreate.CreateDimension(parent, DimType.HEIGHT, "Height", position, thisRotation, 1500 - (int)transform.position.y);
        }

        private void EnableSpots(bool state)
        {
            foreach (Transform go in GameObject.Find("PoolData > Positions").transform)
            {
                if (go.name.EndsWith(":"+my.groupId))
                {
                    go.gameObject.SetActive(state);
                }
                else go.gameObject.SetActive(false);
            }
        }

        public static Sub RemoveAddition(int layerId, int additionId, bool removeMirror = true)
        {
            Sub copySub = null;
            Addition my = UI_Settings.input.poolData.layers[layerId].additions[additionId];
            int myX = UI_Settings.output.poolData.layers[layerId].poolGrid.positions[my.gridPositionId].x;

            if (layerId == -1) return null;

            UI_Settings.input.poolData.layers[layerId].additions.RemoveAt(additionId);

            if (my.mirror && removeMirror)
            {
                additionId = 0;
                foreach (Addition addition in UI_Settings.input.poolData.layers[layerId].additions)
                {
                    int thisX = UI_Settings.output.poolData.layers[layerId].poolGrid.positions[addition.gridPositionId].x;
                    if (myX == thisX)
                    {
                        RemoveAddition(layerId, additionId, false);
                        break;
                    }
                    additionId++;
                }
            }

            return copySub;
        }

        private void DestroyAll(string tag)
        {
            try
            {
                GameObject[] go = GameObject.FindGameObjectsWithTag(tag);
                for (int i = 0; i < go.Length; i++)
                {
                    Destroy(go[i]);
                }
            }
            catch
            {

            }
        }
    }
}