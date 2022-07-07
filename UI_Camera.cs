using PowerUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TestProject
{
    public class UI_Camera : MonoBehaviour
    {
        private View lastView;
        public GameObject mainLight;

        void Start()
        {
            UI_Settings.camera_main = GetComponent<Camera>();
            UI_Settings.camera_main.enabled = false;
        }

        void Update()
        {
            if (UI_Settings.camera_overlay) 
            {
                return;
            }

            if (UI_Settings.camera_view == View.PERSPECTIVE)
            {
                DetectRays();
                transform.LookAt(new Vector3(0, 750, 0));
            }
            else
            {
                mainLight.GetComponent<Light>().transform.eulerAngles = UI_Settings.camera_main.transform.eulerAngles; 
            }

            UpdateSize();
            UpdateRotation();
        }

        void DetectRays()
        {
            if (UI_Settings.renderMode != RenderMode.BLOCKS) 
            {
                return;
            }

            if (GameObject.Find("Wall faces") != null)
            {
                GameObject.Find("Wall faces/Wall N").layer = 10;
                GameObject.Find("Wall faces/Wall O").layer = 10;
                GameObject.Find("Wall faces/Wall Z").layer = 10;
                GameObject.Find("Wall faces/Wall W").layer = 10;
            }

            if (!EventSystem.current.IsPointerOverGameObject())
            {
                RaycastHit hit;
                string objectName;
                int layerId, blockId;
                Ray ray = UI_Settings.camera_main.ScreenPointToRay(UnityEngine.Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.gameObject != null)
                    {
                        objectName = hit.transform.parent.gameObject.name;
                        try
                        {
                            layerId = int.Parse(objectName.Split(':')[0]);
                            blockId = int.Parse(objectName.Split(':')[1]);

                            if (layerId >= 0 && blockId >= 0)
                            {
                                int gridId = UI_Settings.output.poolData.layers[layerId].blocks[blockId].gridPositionId;
                                int angle = UI_Settings.output.poolData.layers[layerId].poolGrid.positions[gridId].angle;
                                View selectedView = ViewFromAngle(angle);

                                switch (selectedView)
                                {
                                    case View.NORTH:
                                        GameObject.Find("Wall faces/Wall N").layer = 9;
                                        break;

                                    case View.EAST:
                                        GameObject.Find("Wall faces/Wall O").layer = 9;
                                        break;

                                    case View.SOUTH:
                                        GameObject.Find("Wall faces/Wall Z").layer = 9;
                                        break;

                                    case View.WEST:
                                        GameObject.Find("Wall faces/Wall W").layer = 9;
                                        break;
                                }

                                if (UnityEngine.Input.GetMouseButtonDown(0))
                                {
                                    UI_Settings.camera_view = selectedView;
                                    GameObject.Find("JsonConnect").GetComponent<JsonConnect>().CallVerifier();
                                }
                            }
                        }
                        catch
                        {

                        }
                    }
                }
            }
        }

        private Vector3 dragOrigin;
        void UpdateRotation()
        {
            if (UnityEngine.Input.GetMouseButtonDown(1))
            {
                dragOrigin = UnityEngine.Input.mousePosition;
            }

            if (!UnityEngine.Input.GetMouseButton(1)) 
            {
                return;
            }

            float speedX = (UnityEngine.Input.mousePosition.x - dragOrigin.x) * UI_Settings.camera_rotateSpeed_stepX * Time.deltaTime;
            float speedY = (UnityEngine.Input.mousePosition.y - dragOrigin.y) * UI_Settings.camera_rotateSpeed_stepY * Time.deltaTime;
            
            if (Mathf.Abs(speedX) > UI_Settings.camera_rotateSpeed_max * Time.deltaTime)
            {
                speedX = UI_Settings.camera_rotateSpeed_max * Mathf.Sign(speedX) * Time.deltaTime;
            }

            if (Mathf.Abs(speedY) > UI_Settings.camera_rotateSpeed_max * Time.deltaTime)
            {
                speedY = UI_Settings.camera_rotateSpeed_max * Mathf.Sign(speedY) * Time.deltaTime;
            }

            if (UI_Settings.camera_view == View.PERSPECTIVE)
            {
                transform.RotateAround(new Vector3(0, 750, 0), Vector3.up, speedX);

                if (transform.eulerAngles.x <= 20f && speedY < 0f)
                {
                    //
                }
                else if (transform.eulerAngles.x >= 88f && speedY > 0f)
                {
                    //
                }
                else
                {
                    transform.RotateAround(new Vector3(0, 2.5f, 0), transform.right, speedY);
                }
            }
            else
            {
                int invert = 1;
                if (UI_Settings.camera_view == View.NORTH || UI_Settings.camera_view == View.SOUTH)
                {
                    if (UI_Settings.camera_view == View.SOUTH) 
                    {
                        invert = -1;
                    }

                    if (invert * speedX < 0f && transform.position.x > -(UI_Settings.output.poolData._length / 2 - 1000))
                    {
                        transform.Translate(new Vector3(25 * speedX, 0, 0));
                    }

                    if (invert * speedX > 0f && transform.position.x < UI_Settings.output.poolData._length / 2 - 1000)
                    {
                        transform.Translate(new Vector3(25 * speedX, 0, 0));
                    }
                }
                else
                {
                    if (UI_Settings.camera_view == View.EAST) 
                    {
                        invert = -1;
                    }

                    if (invert * speedX < 0f && transform.position.z > -(UI_Settings.output.poolData._width / 2 - 1000))
                    {
                        transform.Translate(new Vector3(25 * speedX, 0, 0));
                    }

                    if (invert * speedX > 0f && transform.position.z < UI_Settings.output.poolData._width / 2 - 1000)
                    {
                        transform.Translate(new Vector3(25 * speedX, 0, 0));
                    }
                }
            }
        }

        void UpdateSize()
        {
            float scroll = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
            float stepSize = 0f;
            if (Mathf.Abs(scroll) > 0f)
            {
                if (UI_Settings.camera_view == View.PERSPECTIVE)
                {
                    stepSize = UI_Settings.camera_scroll_step * scroll;
                    if (stepSize < 0f && Vector3.Distance(transform.position + stepSize * transform.forward, new Vector3(0, 750, 0)) > UI_Settings.camera_distance_max) 
                    {
                        return;
                    }

                    if (stepSize > 0f && Vector3.Distance(transform.position + stepSize * transform.forward, new Vector3(0, 750, 0)) < 5000f)
                    {
                        return;
                    }

                    transform.position += stepSize * transform.forward;
                }
                else
                {
                    stepSize = Mathf.Sign(scroll) * (float)UI_Settings.camera_orthoSize_step;
                    if (stepSize < 0f && UI_Settings.camera_orthoSize + stepSize < UI_Settings.camera_orthoSize_min)
                    {
                        return;
                    }

                    if (stepSize > 0f && UI_Settings.camera_orthoSize + stepSize > UI_Settings.camera_orthoSize_max)
                    {
                        return;
                    }

                    UI_Settings.camera_orthoSize += (int)stepSize;
                }
            }
        }

        public void ChangeView()
        {
            int offset = 0;
            UI_Settings.camera_main.enabled = true;   

            if (UI_Settings.camera_view == lastView) return;
            if (UI_Settings.camera_view == View.NONE) return;
            if (UI_Settings.output == null) return;

            lastView = UI_Settings.camera_view;

            if (UI_Settings.camera_view == View.PERSPECTIVE)
            {
                UI_Settings.camera_main.orthographic = false;
                mainLight.GetComponent<Light>().transform.eulerAngles = new Vector3(45, -30, 0);
                mainLight.GetComponent<Light>().shadows = LightShadows.Hard;
                mainLight.GetComponent<Light>().shadowResolution = UnityEngine.Rendering.LightShadowResolution.Medium;
            }
            else
            {
                UI_Settings.camera_main.orthographic = true;
                mainLight.GetComponent<Light>().shadows = LightShadows.None;
            }

            if (UI_Settings.output.poolData.type == "Corner") 
            {
                offset = 900;
            }

            transform.position = new Vector3(0, 750, 0);
            switch (UI_Settings.camera_view)
            {
                case View.NORTH:
                    transform.position = new Vector3(0, 750, -offset);
                    transform.LookAt(new Vector3(0, 750, UI_Settings.output.poolData.width));
                    UI_Settings.camera_orthoSize_max = UI_Settings.output.poolData.length / 2;
                    break;
                case View.EAST:
                    transform.position = new Vector3(0, 750, 0);
                    transform.LookAt(new Vector3(UI_Settings.output.poolData.length, 750, 0));
                    UI_Settings.camera_orthoSize_max = UI_Settings.output.poolData.width / 2;
                    break;
                case View.SOUTH:
                    transform.position = new Vector3(0, 750, offset);
                    transform.LookAt(new Vector3(0, 750, -UI_Settings.output.poolData.width));
                    UI_Settings.camera_orthoSize_max = UI_Settings.output.poolData.length / 2;
                    break;
                case View.WEST:
                    transform.position = new Vector3(0, 750, 0);
                    transform.LookAt(new Vector3(-UI_Settings.output.poolData.length, 750, 0));
                    UI_Settings.camera_orthoSize_max = UI_Settings.output.poolData.width / 2;
                    break;
                case View.PERSPECTIVE:
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    transform.position = new Vector3(0f, 2000f, 0f);
                    transform.LookAt(new Vector3(0, 750, 0));
                    Bounds bounds = CalculateBounds();
                    bool zoomNeeded = true;
                    float margin = 0.2f;

                    while (zoomNeeded && transform.position.y < 100000f)
                    {
                        transform.Translate(new Vector3(0, 100f, 0));
                        transform.LookAt(new Vector3(0, 750, 0));

                        Vector3 vpp1 = Camera.main.WorldToViewportPoint(bounds.min);
                        Vector3 vpp2 = Camera.main.WorldToViewportPoint(bounds.max);
                        
                        if (vpp1.x >= margin && vpp1.x <= 1f - margin && vpp2.x >= margin && vpp2.x <= 1f - margin)
                        {
                            if (vpp1.y >= margin && vpp1.y <= 1f - margin && vpp2.y >= margin && vpp2.y <= 1f - margin)
                            {
                                zoomNeeded = false;
                            }
                        }
                    }

                    transform.Translate(new Vector3(0f, -300f, 0f));
                    transform.LookAt(new Vector3(0, 750, 0));
                    transform.rotation = Quaternion.Euler(0, 0, 0);

                    UI_Settings.camera_scroll_step = (int)((transform.position.y - 750f) / 10f);
                    UI_Settings.camera_distance_max = Vector3.Distance(transform.position, new Vector3(0, 750, 0));
                    break;
            }
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

        public void UpdateCulling()
        {
            UI_Settings.camera_main.enabled = false;
            UI_Settings.camera_main.cullingMask = ~(1 << 10);
            UI_Settings.camera_main.enabled = true;
        }

        public void Reset()
        {
            UI_Settings.camera_view = View.NONE;
            lastView = View.EAST;
        }

        private Bounds CalculateBounds()
        {
            Bounds bounds = new Bounds(transform.position, Vector3.one);
            Renderer[] renderers = GameObject.Find("PoolData > Blocks[layer=2]").GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
            }
            return bounds;
        }
    }
}