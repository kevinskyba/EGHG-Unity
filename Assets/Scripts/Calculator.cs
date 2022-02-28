using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using KevinSkyba.EGHG.Extensions;

namespace KevinSkyba.EGHG
{
    public class Calculator : MonoBehaviour
    {
        private string currentText = "0";
        public string CurrentText => currentText;

        private string lastText = "";
        private string currentOperator = "";

        private bool isResult = false;

        private TextMeshProUGUI resultTextMesh;

        private void Awake()
        {
            transform.GetComponentsInChildren<Button>().ToList().ForEach((button) =>
            {
                button.onClick.AddListener(() => OnButtonClick(button.GetComponentInChildren<TextMeshProUGUI>().text));
            });
            resultTextMesh = transform.RecursiveFind("ResultText")?.GetComponent<TextMeshProUGUI>();
        }

        public void OnButtonClick(string buttonText)
        {
            switch(buttonText)
            {
                case "/":
                    currentOperator = "/";
                    if (lastText != "") Calculate();
                    return;
                case "x":
                    currentOperator = "x";
                    if (lastText != "") Calculate();
                    return;
                case "-":
                    currentOperator = "-";
                    if (lastText != "") Calculate();
                    return;
                case "+":
                    currentOperator = "+";
                    if (lastText != "") Calculate();
                    return;
            }

            if (buttonText == "=")
            {
                Calculate();
                return;
            }

            if (buttonText == "AC")
            {
                currentText = "0";
                lastText = "";
                currentOperator = "";
                isResult = false;
                return;
            }

            if (isResult)
            {
                lastText = currentText;
                currentText = "0";
            }

            if (buttonText == "." && !currentText.Contains("."))
            {
                currentText = currentText + ".";
                return;
            }

            // If there is an operator selected we first remove the current text
            if (currentOperator != "" && currentText.Length <= 1)
            {
                lastText = currentText;
                currentText = "";
            }

            if (currentText == "0")
            {
                currentText = "";
            }

            currentText = currentText + buttonText[0];
        }

        private void Calculate()
        {
            var a = float.Parse(lastText);
            var b = float.Parse(currentText);

            float result = 0;

            switch (currentOperator)
            {
                case "/":
                    result = a / b;
                    break;
                case "x":
                    result = a * b;
                    break;
                case "-":
                    result = a - b;
                    break;
                case "+":
                    result = a + b;
                    break;
            }

            currentText = result.ToString();
            isResult = true;
            lastText = "";
            currentOperator = "";
        }

        private void Update()
        {
            if (resultTextMesh != null)
                resultTextMesh.text = currentText;
        }
    }
}
