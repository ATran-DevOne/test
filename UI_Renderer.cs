using PowerUI;
using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace TestProject
{
    public partial class UI_Renderer : MonoBehaviour
    {
        public Texture fontTexture;
        public float screenWidth = 0f;

        void Start()
        {
            Physics.autoSimulation = false;
            Application.targetFrameRate = 30;
            UI.DisableInput = false;
            UI.FilterMode = FilterMode.Bilinear;
            UI.RenderMode = Css.RenderMode.Atlas;

            LoadResources();
            InitializeSettings();

            string[] arguments = GetArguments();
            if(arguments[0] == "config" && arguments[1].Length > 0)
            {
                Regex regex = new Regex("[^a-z0-9]");
                string arg = regex.Replace(arguments[1], "");
                if (arg.Length > 0) UI_Settings.Argument = arg;
            }
        }

        void Update()
        {
            if (UI_Settings.equalizeForm != null || screenWidth != Screen.width)
            {
                EqualizeLabels_Async();
                if (screenWidth != 0f && screenWidth != Screen.width)
                {
                    UI_Settings.alignPrices = true;
                }
                screenWidth = Screen.width;
            }

            if (UI_Settings.initUI)
            {
                StartCoroutine(AsyncInit());
                UI_Settings.initUI = false;
            }

            if (UI_Settings.initExport)
            {
                StartCoroutine(AsyncExport());
                UI_Settings.initExport = false;
            }

            if (UI_Settings.updateCamera)
            {
                GameObject.Find("Main Camera").GetComponent<UI_Camera>().ChangeView();
                GameObject.Find("Main Camera").GetComponent<UI_Camera>().UpdateCulling();
                if (UI_Settings.exportQueue.Count == 0) ActivateOptions("additions", UI_Settings.Orthogonal());
                ToggleViewChanger();

                UI_Settings.updateCamera = false;
            }

            if (UI_Settings.update)
            {
                if (UI_Settings.output.poolData.layers == null) 
                {
                    return;
                }

                if (UI_Settings.camera_view == View.NONE)
                {
                    UI_Settings.camera_view = View.PERSPECTIVE;
                }
                UpdateBlocks();

                UI_Settings.update = false;
                UI_Settings.updateCamera = true;
            }
        }

        private static string[] GetArguments()
        {
            #if (UNITY_WEBGL || UNITY_ANDROID) && !UNITY_EDITOR
                string parameters = Application.absoluteURL.Substring(Application.absoluteURL.IndexOf("?")+1);
                return parameters.Split(new char[] { '&', '=' });
            #else
                return Environment.GetCommandLineArgs();
            #endif
        }
    }
}
