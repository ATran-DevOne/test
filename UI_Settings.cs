using System.Collections.Generic;
using UnityEngine;

namespace TestProject
{
    public static class UI_Settings
    {
        public static string url_main = "/";
        public static string url_resources = url_main + "Bundles/";

        private static string url_json = "/";
        public static string url_verifier = url_json + "verify.php";
        public static string url_excel = url_json + "excel.php";
        public static string url_export = url_json + "export.php";
        public static string url_html = url_json + "Unity/index.php";
        public static string url_cache = url_json + "cache.php";

        public static float ui_fadeLength = 0.5f;

        public static int _selection = -1;
        public static Dictionary<string, Dictionary<string, string>> formsHtml;
        public static Dictionary<string, Dictionary<string, object>> formsUnity;
        public static Dictionary<string, Dictionary<string, object>> buttons;
        public static Dictionary<string, GameObject> models;
        public static Dictionary<string, Material> materials;
        public static JsonInput input;
        public static JsonOutput output;
        
        public static Dictionary<string, Addition> additionTypes;
        public static Dictionary<string, Stair> stairTypes;

        public static bool update, updateCamera, changed, initUI, initExport;
        public static Error error;
        public static Session session;
        public static RenderMode renderMode;
        public static string Argument;
        public static string equalizeForm;

        public static Camera camera_main;
        public static Camera camera_topview;
        private static View _camera_view = View.NONE;
        public static float camera_rotateSpeed_stepX = 0.25f;
        public static float camera_rotateSpeed_stepY = 0.25f;
        public static int camera_rotateSpeed_max = 45;

        private static int _camera_orthoSize = 1000;
        public static int camera_orthoSize_min = 1000;
        public static int _camera_orthoSize_max = 1000;
        public static int camera_orthoSize_step = 250;

        public static int camera_scroll_step = 1000;
        public static float camera_distance_max = 0f;
        public static bool camera_overlay = false;
        public static bool alignPrices = false;
        public static List<string> exportQueue = new List<string>();
        public static List<Texture2D> renderQueue = new List<Texture2D>();

        public static Dictionary<string, Dictionary<string, Dom.Element>> domElements;

        public static View camera_view
        {
            get
            {
                return _camera_view;
            }
            set
            {
                _camera_view = value;
            }
        }

        public static string _stair_type
        {
            get
            {
                return input.poolData.stair.type;
            }
            set
            {
                input.poolData.stair.type = value;
                GameObject.Find("JsonConnect").GetComponent<JsonConnect>().CallVerifier();
            }
        }

        public static int camera_orthoSize
        {
            get
            {
                return _camera_orthoSize;
            }
            set
            {
                _camera_orthoSize = value;
                Camera.main.orthographicSize = _camera_orthoSize;
            }
        }

        public static int camera_orthoSize_max
        {
            get
            {
                return _camera_orthoSize_max;
            }
            set
            {
                if (value > 2500) _camera_orthoSize_max = value;
                else _camera_orthoSize_max = 2500;

                _camera_orthoSize = _camera_orthoSize_max;
                camera_main.orthographicSize = _camera_orthoSize_max;
            }
        }

        public static int selection
        {
            get
            {
                return _selection;
            }
            set
            {
                _selection = value;
            }
        }

        public static bool Orthogonal()
        {
            if (camera_view == View.PERSPECTIVE || camera_view == View.NONE)
                return false;
            else return true;
        }
    }
}
