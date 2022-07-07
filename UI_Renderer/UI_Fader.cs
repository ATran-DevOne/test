using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestProject
{
    public partial class UI_Renderer : MonoBehaviour
    {
        public void ShowPanel(string panelName)
        {
            foreach (KeyValuePair<string, Dictionary<string, object>> uiForm in UI_Settings.formsUnity)
            {
                GameObject obj = (GameObject)uiForm.Value["Panel"];
                CanvasGroup canvas = obj.GetComponent<CanvasGroup>();
                if (uiForm.Key == panelName)
                    StartCoroutine(Fade(obj, canvas, true));
                else StartCoroutine(Fade(obj, canvas, false));
            }
        }

        public void ShowHelper(string helperName, bool show)
        {
            GameObject objHelper = (GameObject)UI_Settings.buttons[helperName]["Helper"];
            GameObject objOptions = (GameObject)UI_Settings.buttons[helperName]["Options"];
            CanvasGroup canvas = objHelper.GetComponent<CanvasGroup>();
            bool optionsActive = false;
            if (objOptions != null)
            {
                optionsActive = objOptions.GetComponent<CanvasGroup>().isActiveAndEnabled;
            }

            if (show && !optionsActive)
            {
                StartCoroutine(Fade(objHelper, canvas, true));
            }
            else
            {
                HideHelpers();
            }
        }


        void ShowOptions(string buttonName)
        {
            HideOptions(); HideHelpers();
            GameObject obj = (GameObject)UI_Settings.buttons[buttonName]["Options"];
            CanvasGroup canvas = obj.GetComponent<CanvasGroup>();

            if (!canvas.isActiveAndEnabled)
                StartCoroutine(Fade(obj, canvas, true));
            else StartCoroutine(Fade(obj, canvas, false));
        }

        void ShowSelection(bool showMe)
        {
            GameObject obj = (GameObject)UI_Settings.formsUnity["Form_Selected"]["Panel"];
            CanvasGroup cg = obj.GetComponent<CanvasGroup>();
            StartCoroutine(Fade(obj, cg, showMe));
        }

        void ShowInvalid(bool showMe)
        {
            GameObject obj = (GameObject)UI_Settings.formsUnity["Form_Invalid"]["Panel"];
            CanvasGroup cg = obj.GetComponent<CanvasGroup>();
            StartCoroutine(Fade(obj, cg, showMe));
        }

        public void ClearDisplay()
        {
            HidePanels();
            HideButtons();
            HideHelpers();
        }

        public void HidePanels()
        {
            ShowPanel("none");
        }

        public void HideButtons()
        {
            foreach (Transform tf in GameObject.Find("UI/Buttons").transform)
            {
                tf.gameObject.SetActive(false);
            }
        }

        public void HideHelpers()
        {
            foreach (KeyValuePair<string, Dictionary<string, object>> uiButton in UI_Settings.buttons)
            {
                GameObject obj = (GameObject)uiButton.Value["Helper"];
                CanvasGroup canvas = obj.GetComponent<CanvasGroup>();
                StartCoroutine(Fade(obj, canvas, false));
            }
        }

        void HideOptions()
        {
            foreach (KeyValuePair<string, Dictionary<string, object>> uiButton in UI_Settings.buttons)
            {
                GameObject obj = (GameObject)uiButton.Value["Options"];
                if (obj != null)
                {
                    CanvasGroup canvas = obj.GetComponent<CanvasGroup>();
                    StartCoroutine(Fade(obj, canvas, false));
                }
            }
        }

        private IEnumerator Fade(GameObject obj, CanvasGroup canvas, bool showMe)
        {
            if (!obj.activeSelf && showMe)
            {
                canvas.alpha = 0f;
                obj.SetActive(true);
                for (float alpha = 0f; alpha < 1f; alpha += Time.deltaTime / UI_Settings.ui_fadeLength)
                {
                    canvas.alpha = alpha;
                    yield return null;
                }
            }

            if (obj.activeSelf && !showMe)
            {
                canvas.alpha = 1f;
                for (float alpha = 1f; alpha > 0f; alpha -= Time.deltaTime / UI_Settings.ui_fadeLength)
                {
                    canvas.alpha = alpha;
                    yield return null;
                }
                obj.SetActive(false);
            }
        }
    }
}
