using PowerUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TestProject
{
    public partial class UI_Renderer : MonoBehaviour
    {
        private void RequestSession()
        {
            if (UI_Settings.Argument != null) GameObject.Find("JsonConnect").GetComponent<JsonConnect>().Request();
            else GameObject.Find("JsonConnect").GetComponent<JsonConnect>().Setup();
        }

        IEnumerator LoadDummy()
        {
            yield return new WaitForSeconds(10);

            UI_Settings.input.poolData = new PoolData();
            UI_Settings.input.poolData.stair = new Stair();

            UI_Settings.input.poolData.autoGenerate = new AutoGenerate();
            UI_Settings.input.poolData.autoGenerate.injectorType = "AstralInjector";
            UI_Settings.input.poolData.autoGenerate.skimmerType = "AstralSkimmer175L";
            UI_Settings.input.poolData.autoGenerate.lightCount = 4;
            UI_Settings.input.poolData.autoGenerate.lightPosition = 700;
            UI_Settings.input.poolData.autoGenerate.lightType = "IsobadPar56";

            UI_Settings.input.poolData.type = "Straight";
            UI_Settings.input.poolData.length = 6600;
            UI_Settings.input.poolData.width = 3000;

            UI_Settings.input.poolData.stair.position = "Right";
            UI_Settings.input.poolData.stair.width = 1800;
            UI_Settings.input.poolData.stair.size = "XL";

            UI_Settings._stair_type = "StairStraightXL";
        }

        private void InitializeSettings()
        {
            UI_Settings.formsHtml = new Dictionary<string, Dictionary<string, string>>();
            UI_Settings.formsUnity = new Dictionary<string, Dictionary<string, object>>();
            UI_Settings.buttons = new Dictionary<string, Dictionary<string, object>>();

            UI_Settings.input = new JsonInput();
            UI_Settings.output = null;
        }
    }
}
