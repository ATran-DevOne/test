using PowerUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace TestProject
{
    public class HtmlParameters
    {
        public string poolType = "";
        public int poolLength = 6000;
        public int poolWidth = 3000;

        public string stairType = "";
        public int stairWidth = 1800;

        public string lightType = "";
        public int lightCount = 0;
        public int lightPosition = 0;

        public string injectorType = "";
        public string skimmerType = "";

        public bool foundation = false;
        public string foundationType = "none";
        public string reinforcementType = "none";
    }

    public partial class UI_Renderer : MonoBehaviour
    {
        public void InitializeUI()
        {
            UI_Settings.initUI = true;
        }

        private IEnumerator AsyncInit()
        {
            yield return new WaitForSeconds(5);

            UI_Settings.domElements = new Dictionary<string, Dictionary<string, Dom.Element>>();
            UI_Settings.domElements["forms"] = InitializeForms();
            UI_Settings.domElements["dialogs"] = InitializeDialogs();
            UI_Settings.domElements["overlays"] = InitializeOverlays();
            UI_Settings.domElements["flags"] = InitializeFlags();
            UI_Settings.domElements["menus"] = InitializeMenu();
            UI_Settings.domElements["sliders"] = InitializeSliders();
            UI_Settings.domElements["sliderValues"] = InitializeSliderValues();
            UI_Settings.domElements["menuOptionsAddition"] = InitializeMenu_OptionsAdditions();
            UI_Settings.domElements["menuOptionsSub"] = InitializeByClass("OptionSub");
            UI_Settings.domElements["formOptionsStair"] = InitializeByClass("OptionStair");
            UI_Settings.domElements["formOptionsLight"] = InitializeByClass("OptionLight");
            UI_Settings.domElements["formSubmitsClean"] = InitializeByClass("Clean");
            UI_Settings.domElements["formSubmitsChange"] = InitializeByClass("Change");

            RequestSession();
            if (UI_Settings.Argument != null)
            {
                UI.document.Run("showMenu");
            }
            else
            {
                UI.document.Run("showForm", "type");
            }
        }

        private Dictionary<string, Dom.Element> InitializeForms()
        {
            Dictionary<string, Dom.Element> listing = new Dictionary<string, Dom.Element>();
            foreach (HtmlElement element in UI.document.getElementsByClassName("Form"))
            {
                string id = element.attributes["id"];
                listing[id] = element;
            }
            return listing;
        }

        private Dictionary<string, Dom.Element> InitializeByClass(string type)
        {
            Dictionary<string, Dom.Element> listing = new Dictionary<string, Dom.Element>();
            foreach (HtmlElement element in UI.document.getElementsByClassName(type))
            {
                string id = element.attributes["id"];
                listing[id] = element;
            }
            return listing;
        }

        private Dictionary<string, Dom.Element> InitializeDialogs()
        {
            Dictionary<string, Dom.Element> listing = new Dictionary<string, Dom.Element>();
            foreach (HtmlElement element in UI.document.getElementsByClassName("Dialog"))
            {
                string id = element.attributes["id"];
                listing[id] = element;
            }
            return listing;
        }

        private Dictionary<string, Dom.Element> InitializeOverlays()
        {
            Dictionary<string, Dom.Element> listing = new Dictionary<string, Dom.Element>();
            foreach (HtmlElement element in UI_Settings.domElements["dialogs"].Values)
            {
                if (element.classList.contains("Overlay"))
                {
                    string id = element.attributes["id"];
                    listing[id] = element;
                }
            }
            return listing;
        }

        private Dictionary<string, Dom.Element> InitializeFlags()
        {
            Dictionary<string, Dom.Element> listing = new Dictionary<string, Dom.Element>();
            foreach (HtmlElement element in UI.document.getElementsByClassName("Flag"))
            {
                string id = element.attributes["id"];
                listing[id] = element;
            }
            return listing;
        }

        private Dictionary<string, Dom.Element> InitializeMenu()
        {
            Dictionary<string, Dom.Element> listing = new Dictionary<string, Dom.Element>();
            listing["menu-topleft"] = UI.document.getElementById("menu-topleft");
            listing["menu-topright"] = UI.document.getElementById("menu-topright");
            listing["menu-bottomright"] = UI.document.getElementById("menu-bottomright");
            listing["sidebar-perspective"] = UI.document.getElementById("sidebar-perspective");
            listing["sidebar-orthogonal"] = UI.document.getElementById("sidebar-orthogonal");
            listing["sidebar-realistic"] = UI.document.getElementById("sidebar-realistic");
            return listing;
        }

        private Dictionary<string, Dom.Element> InitializeMenu_OptionsAdditions()
        {
            Dictionary<string, Dom.Element> listing = new Dictionary<string, Dom.Element>();
            listing["0"] = UI.document.getElementsByClassName("OptionAddition").item(0);
            return listing;
        }

        private Dictionary<string, Dom.Element> InitializeSliders()
        {
            Dictionary<string, Dom.Element> listing = new Dictionary<string, Dom.Element>();
            foreach (HtmlElement element in UI.document.getElementsByClassName("RangeSlider"))
            {
                string id = element.attributes["name"];
                foreach (HtmlElement subElement in element.children)
                {
                    if (subElement.Tag == "scrollthumb")
                    {
                        listing[id] = subElement;
                    }
                }
            }
            return listing;
        }

        private Dictionary<string, Dom.Element> InitializeSliderValues()
        {
            Dictionary<string, Dom.Element> listing = new Dictionary<string, Dom.Element>();
            foreach (KeyValuePair<string, Dom.Element> element in UI_Settings.domElements["sliders"])
            {
                Dom.Element currElement = element.Value.parentElement.previousElementSibling.children[0].children[0].children[1];
                listing[element.Key] = currElement;
            }
            return listing;
        }

        public string[] GetKeys(string type)
        {
            string[] listing = new string[UI_Settings.domElements[type].Keys.Count];
            int i = 0; foreach (string key in UI_Settings.domElements[type].Keys)
            {
                listing[i] = key;
                i++;
            }
            return listing;
        }

        public Dom.Element GetElement(string type, string pos)
        {
            return UI_Settings.domElements[type][pos];
        }

        public void ShowElement(string type, string pos, bool showMe)
        {
            if (showMe) UI_Settings.domElements[type][pos].style.display = "block";
            else UI_Settings.domElements[type][pos].style.display = "none";
        }

        public void ShowElements(string type, bool showMe)
        {
            foreach (KeyValuePair<string, Dom.Element> element in UI_Settings.domElements[type])
            {
                if (showMe)
                {
                    if (element.Value.classList.contains("Exclude")) continue;
                    element.Value.style.display = "block";
                }
                else element.Value.style.display = "none";
            }
        }

        public void SetHtml(string type, string pos, string html, string tag = "")
        {
            if (UI_Settings.domElements[type][pos].Tag == "td")
            {
                UI_Settings.domElements[type][pos].innerHTML = html;
                return;
            }

            foreach (HtmlElement child in UI_Settings.domElements[type][pos].children)
            {
                if (tag.Length > 0 && child.Tag != tag) continue;
                child.innerHTML = html;
            }
        }

        public void SetPosition(string type, string pos, int position)
        {
            UI_Settings.domElements[type][pos].style.left = position.ToString() + "%";
        }

        public void ActivateButtons(string type, bool isActive)
        {
            foreach (KeyValuePair<string, Dom.Element> element in UI_Settings.domElements[type])
            {
                if (isActive) element.Value.className = "MenuButton";
                else element.Value.className = "MenuButton Inactive";

                foreach (HtmlElement child in element.Value.children)
                {
                    if (child.Tag == "span")
                    {
                        if (isActive) child.style.display = "none";
                        else child.style.display = "block";
                    }
                }
            }
        }

        public void UpdateOutput(HtmlParameters paramss)
        {
            string[] split;

            if (UI_Settings.output == null)
            {
                UI_Settings.input.poolData = new PoolData();
                UI_Settings.input.poolData.stair = new Stair();

                split = paramss.poolType.Split('-');
                UI_Settings.input.poolData.type = split[0].ToUpper()[0] + split[0].Substring(1);

                UI_Settings.input.poolData.length = paramss.poolLength;
                UI_Settings.input.poolData.width = paramss.poolWidth;

                if (UI_Settings.input.poolData.type == "Corner")
                {
                    UI_Settings.input.poolData.stair.position = split[1].ToUpper()[0] + split[1].Substring(1);
                }

                UI_Settings.input.poolData.autoGenerate = new AutoGenerate();
                UI_Settings.input.poolData.autoGenerate.lightType = paramss.lightType;
                if (paramss.lightType != "none")
                {
                    UI_Settings.input.poolData.autoGenerate.lightCount = paramss.lightCount;
                    UI_Settings.input.poolData.autoGenerate.lightPosition = paramss.lightPosition;
                }

                UI_Settings.input.poolData.autoGenerate.injectorType = paramss.injectorType;
                UI_Settings.input.poolData.autoGenerate.skimmerType = paramss.skimmerType;
            }

            UI_Settings.input.poolData.floor = new Floor();
            if (paramss.foundationType != "none")
            {
                UI_Settings.input.poolData.floor.foundation = true;
            }

            if (paramss.reinforcementType != "none")
            {
                split = paramss.reinforcementType.Split('-');
                UI_Settings.input.poolData.floor.thickness = Convert.ToInt32(split[0]);
                UI_Settings.input.poolData.floor.mesh = split[1].ToUpper()[0] + split[1].Substring(1);
            }

            UI_Settings.input.poolData.stair.width = paramss.stairWidth;
            UI_Settings._stair_type = paramss.stairType;
            UI_Settings.renderMode = RenderMode.BLOCKS;
            UI_Settings.changed = true;
        }

        public void AddAddition(string type, int groupId, int numPositions, bool mirror)
        {
            if (type == "ConcreteFixture" && (UI_Settings.camera_view == View.EAST || UI_Settings.camera_view == View.WEST))
            {
                object[] paramss = new object[3];
                paramss[0] = "mirror";
                paramss[1] = null;
                paramss[2] = true;
                UI.document.Run("showDialog", paramss);
                return;
            }

            GameObject go = GameObject.Find("PoolData > Additions");
            Addition addition = new Addition();
            addition.gridPositionId = 0;
            addition.type = type.ToString();
            addition.groupId = groupId;
            addition.mirror = mirror;
            addition.numPositions = numPositions;
            createAddition(go, addition);

            UI_Settings.input.poolData.layers[0].additions.Add(addition);

            Transform additions = GameObject.Find("PoolData > Additions").transform;
            foreach (Transform transform in additions)
            {
                if (transform.gameObject.name.Contains("999"))
                {
                    transform.gameObject.GetComponentInChildren<UI_DragDrop>().EnableFreemode();
                }
            }
        }

        public void AddSub(string type, int layerId, int additionId)
        {
            Sub sub = new Sub();
            sub.type = type;
            sub.additionId = additionId;

            int matchId = UI_Settings.input.poolData.layers[layerId].subs.FindIndex(f => f.additionId == additionId);
            if (matchId >= 0) UI_Settings.input.poolData.layers[layerId].subs.RemoveAt(matchId);

            if (sub.type != "none")
            {
                UI_Settings.input.poolData.layers[layerId].subs.Add(sub);
            }

            GameObject.Find("JsonConnect").GetComponent<JsonConnect>().CallVerifier();
        }

        public string GetSessionUrl()
        {
            if (UI_Settings.input == null) return "[N/A]";
            return UI_Settings.input.session.code;
        }

        public bool WallSelected()
        {
            return UI_Settings.Orthogonal();
        }

        public void ActivateOptions(string type, bool active)
        {
            if (type == "additions")
            {
                if (UI_Settings.Orthogonal())
                {
                    ActivateSideMenu("additions", active);
                }
                else
                {
                    ActivateSideMenu("additions", false);
                }
            }
        }

        public void ActivateSideMenu(string type, bool active)
        {
            string display;
            if (UI_Settings.Argument != null) return;

            if (type == "additions")
            {
                display = "none";
                if (active) display = "block";
                UI.document.getElementById("AdditionOptions").style.display = display;
            }
        }

        public void SetRenderMode(string type)
        {
            if (type == "blocks")
            {
                UI_Settings.renderMode = RenderMode.BLOCKS;
                UI_Settings.update = true;
            }
            if (type == "world")
            {
                UI_Settings.renderMode = RenderMode.WORLD;
                UI_Settings.update = true;
            }
            ActivateOptions("additions", false);
        }

        public void SetOverlayMode(bool active)
        {
            UI_Settings.camera_overlay = active;
        }

        public void ResetOutput()
        {
            UI_Settings.input = new JsonInput();
            UI_Settings.output = null;
            UI_Settings.Argument = null;
            RequestSession();

            UI_Settings.domElements["menus"]["sidebar-perspective"].style.display = "none";
            UI_Settings.domElements["menus"]["sidebar-orthogonal"].style.display = "none";
            UI_Settings.domElements["menus"]["sidebar-realistic"].style.display = "none";
            
            GameObject.Find("Main Camera").GetComponent<UI_Camera>().Reset();
        }

        public void RemoveAddition(int layerId, int additionId)
        {
            UI_DragDrop.RemoveAddition(layerId, additionId);
            GameObject.Find("JsonConnect").GetComponent<JsonConnect>().CallVerifier();
        }

        public void EvaluateUrl(string url)
        {
            Application.ExternalEval("window.open(\"" + url + "\",\"_blank\")");
        }

        public void DisableMenu(bool state)
        {
            foreach (HtmlElement element in UI.document.getElementsByClassName("SideMenu"))
            {
                foreach (HtmlElement element2 in element.getElementsByClassName("MenuButton"))
                {
                    {
                        if (element2.classList.contains("Perm")) continue;
                        if (state) element2.className = "MenuButton Inactive";
                        else element2.className = "MenuButton";
                    }
                }
            }

            foreach (HtmlElement element in UI.document.getElementsByClassName("TopButton"))
            {
                if (state) element.className = "TopButton Inactive";
                else element.className = "TopButton";
            }
        }

        public void DetailsAddition(string html, bool state, bool showBg)
        {
            if (UI_Settings.selection >= 0) state = false;

            UI.document.getElementById("AdditionDetails").getElementById("AdditionContent").innerHTML = html;
            if (state) UI.document.getElementById("AdditionDetails").style.display = "block";
            else UI.document.getElementById("AdditionDetails").style.display = "none";

            if (showBg && state)
            {
                UI.document.getElementById("AdditionDetailsBg").style.display = "block";
                UI.document.getElementById("AdditionDetails").style.left = "10vw";
                UI.document.getElementById("AdditionDetails").style.width = "80vw";
                UI.document.getElementById("AdditionDetails").style.top = "10vh";
                UI.document.getElementById("AdditionDetails").style.height = "80vh";
                UI.document.getElementById("AdditionDetails").getElementsByClassName("Button")[0].style.display = "block";
            }
            else
            {
                UI.document.getElementById("AdditionDetailsBg").style.display = "none";
                UI.document.getElementById("AdditionDetails").style.left = "0";
                UI.document.getElementById("AdditionDetails").style.width = "100vw";
                UI.document.getElementById("AdditionDetails").style.top = "18vh";
                UI.document.getElementById("AdditionDetails").style.height = "82vh";
                UI.document.getElementById("AdditionDetails").getElementsByClassName("Button")[0].style.display = "none";
            }
        }

        public void EqualizeLabels(string form)
        {
            Dom.Element htmlForm = UI.document.getElementById("form-" + form);
            foreach (HtmlElement child in htmlForm.getElementsByClassName("Large"))
            {
                child.style.height = "auto";
            }
            UI_Settings.equalizeForm = form;
        }

        public void EqualizeLabels_Async()
        {
            int maxHeight = 0;
            bool haveInfo = false;

            if (UI_Settings.equalizeForm == null)
            {
                foreach (HtmlElement form in UI.document.getElementsByClassName("Form"))
                {
                    if (form.style.display != "none")
                    {
                        foreach (HtmlElement child in form.getElementsByClassName("Large"))
                        {
                            child.style.height = "auto";
                        }
                        UI_Settings.equalizeForm = form.id.Replace("form-", string.Empty);
                        return;
                    }
                }
            }


            Dom.Element htmlForm = UI.document.getElementById("form-" + UI_Settings.equalizeForm);

            if (htmlForm == null || (htmlForm != null && htmlForm.getElementsByTagName("label").length == 0))
            {
                UI_Settings.equalizeForm = null;
                return;
            }

            foreach (HtmlElement child in htmlForm.getElementsByClassName("Large"))
            {
                if (child.computedStyle.ContentHeight > maxHeight)
                {
                    maxHeight = (int)child.computedStyle.ContentHeight;
                }

                if (!haveInfo && child.getElementByTagName("div").getElementByTagName("div") != null)
                {
                    haveInfo = true;
                }
            }

            if (maxHeight > 0)
            {
                foreach (HtmlElement child in htmlForm.getElementsByClassName("Large"))
                {
                    if (haveInfo) child.style.height = (maxHeight + Screen.width * 0.03f) + "px";
                    else child.style.height = (maxHeight + Screen.width * 0.01f) + "px";
                }
                UI_Settings.equalizeForm = null;
            }
        }

        public Addition SelectAdditionType(string item)
        {
            return UI_Settings.additionTypes[item];
        }

        public Stair SelectStairType(string item)
        {
            return UI_Settings.stairTypes[item];
        }

        public bool ToggleFoundation()
        {
            UI_Settings.input.poolData.floor.foundation = !UI_Settings.input.poolData.floor.foundation;
            UI_Settings.changed = true;
            GameObject.Find("JsonConnect").GetComponent<JsonConnect>().CallVerifier();
            return UI_Settings.input.poolData.floor.foundation;
        }

        public void ToggleView()
        {
            if (UI_Settings.renderMode == RenderMode.WORLD || UI_Settings.camera_view != View.PERSPECTIVE)
            {
                UI_Settings.renderMode = RenderMode.BLOCKS;
                UI_Settings.camera_view = View.PERSPECTIVE;
            }
            else
            {
                UI_Settings.renderMode = RenderMode.WORLD;
                UI_Settings.camera_view = View.PERSPECTIVE;
            }

            UI_Settings.update = true;
            ToggleViewChanger();
        }

        public string GetViewType()
        {
            return UI_Settings.camera_view.ToString();
        }

        public void AlignPrices()
        {
            UI_Settings.alignPrices = true;
        }

        public void InitializeExport()
        {
            UI_Settings.exportQueue.Add("VIEW PERSPECTIVE");
            UI_Settings.exportQueue.Add("EXPORT");
            UI_Settings.exportQueue.Add("VIEW NORTH");
            UI_Settings.exportQueue.Add("EXPORT");
            UI_Settings.exportQueue.Add("VIEW EAST");
            UI_Settings.exportQueue.Add("EXPORT");
            UI_Settings.exportQueue.Add("VIEW SOUTH");
            UI_Settings.exportQueue.Add("EXPORT");
            UI_Settings.exportQueue.Add("VIEW WEST");
            UI_Settings.exportQueue.Add("EXPORT");
            UI_Settings.exportQueue.Add("POST");
            UI_Settings.exportQueue.Add("RESET");
            UI_Settings.initExport = true;
        }

        IEnumerator AsyncExport()
        {
            yield return new WaitForSeconds(2);

            string task = UI_Settings.exportQueue[0];
            UI_Settings.exportQueue.RemoveAt(0);

            if (UI_Settings.exportQueue.Count > 0)
            {
                StartCoroutine(AsyncExport());
            }

            switch (task)
            {
                case "VIEW PERSPECTIVE":
                    UI_Settings.renderQueue = new List<Texture2D>();
                    UI.document.Run("hideAll");
                    UI_Settings.camera_view = View.PERSPECTIVE;
                    GameObject.Find("JsonConnect").GetComponent<JsonConnect>().CallVerifier();
                    break;

                case "VIEW NORTH":
                    UI_Settings.camera_view = View.NORTH;
                    GameObject.Find("JsonConnect").GetComponent<JsonConnect>().CallVerifier();
                    break;

                case "VIEW EAST":
                    UI_Settings.camera_view = View.EAST;
                    GameObject.Find("JsonConnect").GetComponent<JsonConnect>().CallVerifier();
                    break;

                case "VIEW SOUTH":
                    UI_Settings.camera_view = View.SOUTH;
                    GameObject.Find("JsonConnect").GetComponent<JsonConnect>().CallVerifier();
                    break;

                case "VIEW WEST":
                    UI_Settings.camera_view = View.WEST;
                    GameObject.Find("JsonConnect").GetComponent<JsonConnect>().CallVerifier();
                    break;

                case "EXPORT":
                    GenerateDims();
                    UI_Settings.renderQueue.Add(ScreenCapture.CaptureScreenshotAsTexture(2));
                    break;

                case "POST":
                    StartCoroutine(CacheImages());
                    break;

                case "RESET":
                    UI.document.Run("showMenu", true);
                    UI_Settings.camera_view = View.PERSPECTIVE;
                    GameObject.Find("JsonConnect").GetComponent<JsonConnect>().CallVerifier();
                    break;
            }
        }

        private void GenerateDims()
        {
            if (UI_Settings.camera_view == View.PERSPECTIVE) return;
            foreach(Transform obj in GameObject.Find("PoolData > Additions").transform)
            {
                obj.GetComponentInChildren<UI_DragDrop>().GenerateDims();
            }
        }

        IEnumerator CacheImages()
        {
            WWWForm form = new WWWForm();
            int imageId = 1;
            foreach (Texture2D tex in UI_Settings.renderQueue)
            {
                form.AddBinaryData("file" + imageId, tex.EncodeToJPG(), "image" + imageId, "image/jpg");
                imageId++;
            }

            using (UnityWebRequest request = UnityWebRequest.Post(UI_Settings.url_cache, form))
            {
                yield return request.SendWebRequest();
                if (request.isNetworkError || request.isHttpError)
                {
                    //Debug.Log(request.error);
                }
                else
                {
                    UI.document.Run("requestDocuments");
                }
            }
        }

        public string GetExportUrl()
        {
            return UI_Settings.url_export + "?code=" + UI_Settings.input.session.code;
        }

        public void ToggleViewChanger()
        {
            UI_Settings.domElements["menus"]["sidebar-perspective"].style.display = "none";
            UI_Settings.domElements["menus"]["sidebar-orthogonal"].style.display = "none";
            UI_Settings.domElements["menus"]["sidebar-realistic"].style.display = "none";

            if (UI_Settings.renderMode == RenderMode.WORLD)
            {
                UI_Settings.domElements["menus"]["sidebar-realistic"].style.display = "block";
            }
            else
            {
                if (UI_Settings.camera_view == View.PERSPECTIVE)
                {
                    UI_Settings.domElements["menus"]["sidebar-perspective"].style.display = "block";
                }
                else
                {
                    UI_Settings.domElements["menus"]["sidebar-orthogonal"].style.display = "block";
                }
            }
        }
    }
}
