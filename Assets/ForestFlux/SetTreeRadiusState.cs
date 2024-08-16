using UnityEngine;
using TMPro;  // 导入TextMeshPro命名空间

namespace TreePositioning
{
    public class SetTreeRadiusState : ITreeState
    {
        private GameObject positionIndicator;
        private GameObject heightMarker;
        private LineRenderer radiusLine;
        private Canvas radiusCanvas;
        private TextMeshProUGUI radiusText;

        public SetTreeRadiusState(GameObject positionIndicator, GameObject heightMarker)
        {
            this.positionIndicator = positionIndicator;
            this.heightMarker = heightMarker;
        }

        public void EnterState(TreeStateManager treeManager)
        {
            // 初始化圆形的线渲染器
            radiusLine = new GameObject("RadiusLine").AddComponent<LineRenderer>();
            radiusLine.startWidth = 0.02f;
            radiusLine.endWidth = 0.02f;
            radiusLine.material = treeManager.lineMaterial;  // 使用Inspector中的材质
            radiusLine.positionCount = 0;
            radiusLine.useWorldSpace = true;

            // 创建世界空间的Canvas
            GameObject canvasObject = new GameObject("RadiusCanvas");
            radiusCanvas = canvasObject.AddComponent<Canvas>();
            radiusCanvas.renderMode = RenderMode.WorldSpace;
            radiusCanvas.transform.position = treeManager.TreePosition + new Vector3(0, 0.3f, 0);  // 在树的位置上方
            radiusCanvas.transform.localScale = Vector3.one * 0.01f;

            // 添加TextMeshPro的Text对象
            GameObject textObject = new GameObject("RadiusText");
            textObject.transform.SetParent(radiusCanvas.transform);
            radiusText = textObject.AddComponent<TextMeshProUGUI>();
            radiusText.fontSize = 0.05f;
            radiusText.alignment = TextAlignmentOptions.Center;
            radiusText.text = "Radius: 0.00 m\nCircumference: 0.00 m\nArea: 0.00 m²";

            RectTransform rectTransform = radiusText.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(2, 2);  // 设置文本框大小
            rectTransform.anchoredPosition = Vector2.zero;

            Debug.Log("进入测量周长状态");
        }

        public void UpdateState(TreeStateManager treeManager)
        {
            // 获取手柄的位置和Y轴高度
            Vector3 handPosition = treeManager.rightControllerTransform.position;
            float handY = handPosition.y;

            // 在高度点和确认位置点的线段上找到相同Y轴的点
            Vector3 lineStart = positionIndicator.transform.position;
            Vector3 lineEnd = heightMarker.transform.position;
            Vector3 projectionPoint = FindPointOnLineAtY(lineStart, lineEnd, handY);

            // 计算从该点到手柄的距离作为半径
            float radius = Vector3.Distance(projectionPoint, handPosition);

            // 计算周长和面积
            float circumference = 2 * Mathf.PI * radius;
            float area = Mathf.PI * Mathf.Pow(radius, 2);

            // 绘制圆形
            DrawCircle(projectionPoint, radius);

            // 更新文本信息
            radiusText.text = $"Radius: {radius:F2} m\nCircumference: {circumference:F2} m\nArea: {area:F2} m²";

            // 使Canvas面向相机
            radiusCanvas.transform.LookAt(Camera.main.transform);
            radiusCanvas.transform.Rotate(0, 180, 0);

            // 如果按下trigger键，确认圆的大小
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                Debug.Log("圆的半径已确认为: " + radius);

                // 保存圆的大小和其他信息
                treeManager.TreeRadius = radius;
                treeManager.TreeCircumference = circumference;

                // 销毁虚线和Canvas
                GameObject.Destroy(radiusLine.gameObject);
                GameObject.Destroy(radiusCanvas.gameObject);

                // 进入下一个状态，显示所有测量信息
                treeManager.SetState(new SetUpTreeState());
            }
        }

        private Vector3 FindPointOnLineAtY(Vector3 start, Vector3 end, float y)
        {
            // 计算线段上的点
            Vector3 direction = end - start;
            float t = (y - start.y) / direction.y;
            return start + t * direction;
        }

        private void DrawCircle(Vector3 center, float radius)
        {
            int segments = 100;  // 圆形的段数，越多越圆滑
            radiusLine.positionCount = segments + 1;

            for (int i = 0; i <= segments; i++)
            {
                float angle = i * 2 * Mathf.PI / segments;
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;
                radiusLine.SetPosition(i, new Vector3(center.x + x, center.y, center.z + z));
            }
        }
    }
}
