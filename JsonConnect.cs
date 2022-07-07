using Leguar.TotalJSON;
using PowerUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace TestProject
{
    public class JsonConnect : MonoBehaviour
    {
        private JSON excelInput;
        private bool excelCalled = false;
        private float excelDelay = 5f;
        private float excelTimer;

        private void Update()
        {
            if(excelTimer > 0f)
            {
                excelTimer -= Time.deltaTime;
            }
            else if(excelCalled)
            {
                StartCoroutine(PerformCall("excel", excelInput));
                excelCalled = false;
            }

            if (UI_Settings.alignPrices)
            {
                float offset = 20f;
                float colWidth = 0f;
                try
                {
                    Dom.HTMLCollection collection = UI.document.getElementsByClassName("ExcelTable")[0].getElementsByTagName("span");
                    Dom.HTMLCollection collection_td = UI.document.getElementsByClassName("ExcelTable")[0].getElementsByClassName("Data")[0].getElementsByTagName("td");
                    HtmlElement lastColumn = (HtmlElement)collection_td[collection_td.length - 1];
                    colWidth = lastColumn.computedStyle.PixelWidth;

                    foreach (HtmlElement element in collection)
                    {
                        element.style.marginLeft = colWidth - element.computedStyle.InnerWidth - offset + "px";
                    }
                }
                catch { }
                finally
                {
                    UI_Settings.alignPrices = false;
                }
            }
        }

        public void Setup()
        {
            StartCoroutine(PerformCall("setup"));
        }

        public void Request()
        {
            StartCoroutine(PerformCall("request"));
        }

        public void CallVerifier()
        {
            JSON data = JSON.Serialize(UI_Settings.input);
            StartCoroutine(PerformCall("verify", data));
        }

        public void CallExcel()
        {
            excelInput = JSON.Serialize(UI_Settings.input);
            excelCalled = true;
            excelTimer = excelDelay;

            UI.document.getElementsByClassName("Pricing")[0].innerHTML = "...";
            UI.document.getElementsByClassName("ExcelTable")[0].innerHTML = "<tr><td><b>Momenteel niet beschikbaar, de prijzen worden op de achtergrond berekend, een ogenblik geduld...</b></td></tr>";
        }

        private IEnumerator PerformCall(string type, JSON json = null)
        {
            string uri = "";
            if (type == "excel") uri = UI_Settings.url_excel;
            else uri = UI_Settings.url_verifier;

            var webRequest = new UnityWebRequest(uri, "POST");
            webRequest.timeout = 10;
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            if (type == "request")
            {
                json = new JSON();
                json.Add("code", UI_Settings.Argument);
            }

            if (type != "setup")
            {
                string jsonString = json.CreateString();
                byte[] rawData = Encoding.UTF8.GetBytes(jsonString);
                webRequest.uploadHandler = new UploadHandlerRaw(rawData);
            }

            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                JsonOutput result = new JsonOutput();
                result.error = Error.JSON_TIMEOUT;
            }
            else
            {
                JSON jsonResult = JSON.ParseString(webRequest.downloadHandler.text);
                try
                {
                    DeserializeSettings deserializeSettings = new DeserializeSettings();
                    deserializeSettings.RequireAllFieldsArePopulated = false;

                    if (type == "verify" || type == "request")
                    {
                        UI_Settings.output = jsonResult.Deserialize<JsonOutput>(deserializeSettings);

                        if (type == "request")
                        {
                            UI_Settings.input = new JsonInput();
                            UI_Settings.input.session = UI_Settings.output.session;
                            UI_Settings.input.poolData = UI_Settings.output.poolData;
                            UI.document.Run("disableMenu", true);
                        }
                        else
                        {
                            UI.document.Run("disableMenu", false);
                        }
                        ClearInput();

                        if (type == "verify") {
                            UI_Settings.renderMode = RenderMode.BLOCKS;
                        }
                        else 
                        {
                            UI_Settings.renderMode = RenderMode.WORLD;
                        }
                        UI_Settings.update = true;

                        if(UI_Settings.changed)
                        {
                            CallExcel();
                            UI_Settings.changed = false;
                        }
                    }

                    if (type == "excel")
                    {
                        ExcelData data = jsonResult.Deserialize<ExcelData>(deserializeSettings);
                        UI.document.getElementsByClassName("Pricing")[0].innerHTML = data.pricing;
                        UI.document.getElementsByClassName("ExcelTable")[0].innerHTML = data.html;
                        UI.document.getElementById("ExcelVersion").innerHTML = "Versie: " + data.version;
                    }

                    if (type == "setup") 
                    { 
                        JsonOutput jsonOutput = jsonResult.Deserialize<JsonOutput>(deserializeSettings);
                        UI_Settings.input.session = jsonOutput.session;
                        UI_Settings.additionTypes = jsonOutput.additionTypes;
                        UI_Settings.stairTypes = jsonOutput.stairTypes;
                        UI.document.Run("SetupTypes");
                    }
                }
                catch (Exception e)
                {
                    UI_Settings.error = Error.JSON_DESERIALIZE;
                }
            }
        }

        private void ClearInput()
        {
            UI_Settings.input.poolData.autoGenerate = null;
            UI_Settings.input.poolData.layers = new List<Layer>();
            for (int i = 0; i <= 2; i++)
            {
                UI_Settings.input.poolData.layers.Add(new Layer());

                UI_Settings.input.poolData.layers[i].additions = new List<Addition>();
                foreach (Addition addition in UI_Settings.output.poolData.layers[i].additions) 
                {
                    UI_Settings.input.poolData.layers[i].additions.Add(addition);
                }

                UI_Settings.input.poolData.layers[i].subs = new List<Sub>();
                foreach (Sub sub in UI_Settings.output.poolData.layers[i].subs) 
                {
                    UI_Settings.input.poolData.layers[i].subs.Add(sub);
                }
            }
        }
    }
}
