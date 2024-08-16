using UnityEngine;
using TMPro;  // 导入TextMeshPro命名空间

namespace TreePositioning
{
    public class SetUpTreeState : ITreeState
    {
        private Canvas infoCanvas;
        private TextMeshProUGUI infoText;
        private Tree tree;
        private string name;

        public void EnterState(TreeStateManager treeManager)
        {
            // 创建Tree对象并计算碳储量
            float radius = treeManager.TreeRadius;
            float height = treeManager.TreeHeight;
            string type = treeManager.treeName;  // 你可以根据实际情况调整树的类型

            // 假设宽度是树的直径
            tree = new Tree(radius * 2, height, type);

            float carbonStorage = tree.CalculateCarbonStorage();


            // Calculate the whole kilograms
            //int carbonStorageKg = Mathf.FloorToInt(carbonStorage);

            //// Calculate the remaining grams (after removing the whole kilograms)
            //float carbonStorageGrams = (carbonStorage - carbonStorageKg) * 1000f;

            //Calculate the whole kilograms
            //int carbonStorageKg = 1;
            int carbonStorageKg = Mathf.FloorToInt(carbonStorage);

            // Calculate the remaining grams (after removing the whole kilograms)
            //float carbonStorageGrams = 2;
            int carbonStorageGrams = Mathf.FloorToInt( (carbonStorage - carbonStorageKg) * 1000f);


            // 创建显示信息的世界空间Canvas
            GameObject canvasObject = new GameObject("InfoCanvas");
            infoCanvas = canvasObject.AddComponent<Canvas>();
            infoCanvas.renderMode = RenderMode.WorldSpace;
            infoCanvas.transform.position = treeManager.TreePosition + new Vector3(0, 2, 0);  // 显示在树的位置上方
            infoCanvas.transform.localScale = Vector3.one * 0.01f;

            // 添加TextMeshPro的Text对象
            GameObject textObject = new GameObject("InfoText");
            textObject.transform.SetParent(infoCanvas.transform);
            infoText = textObject.AddComponent<TextMeshProUGUI>();
            infoText.fontSize = 0.2f;
            infoText.alignment = TextAlignmentOptions.Center;
            infoText.text = $"Tree Information\nType: {type}\nHeight: {height:F2} m\nRadius: {radius:F2} m\nCarbon Storage:\n{carbonStorageKg} kg and {carbonStorageGrams} g";

            RectTransform rectTransform = infoText.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(4, 2);  // 设置文本框大小
            rectTransform.anchoredPosition = Vector2.zero;

            // 让Canvas面向相机
            infoCanvas.transform.LookAt(Camera.main.transform);
            infoCanvas.transform.Rotate(0, 180, 0);
        }

        public void UpdateState(TreeStateManager treeManager)
        {
            // 使Canvas面向相机
            infoCanvas.transform.LookAt(Camera.main.transform);
            infoCanvas.transform.Rotate(0, 180, 0);

            // 如果按下Trigger键，重新开始测量新树
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                //GameObject.Destroy(infoCanvas.gameObject);  // 销毁信息面板
                treeManager.SetState(new TalkToAIState());  // 进入测量新树的位置状态
            }
        }
    }
}
