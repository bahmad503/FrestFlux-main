using UnityEngine;
using TMPro;

namespace TreePositioning
{
    public class SetTreeHeightState : ITreeState
    {
        private GameObject positionIndicator;
        private LineRenderer lineRenderer;
        private GameObject heightMarker;
        private GameObject heightIndicator;
        private LineRenderer finalLineRenderer;
        private Canvas worldCanvas;
        private TextMeshProUGUI heightText;

        public SetTreeHeightState(GameObject indicator)
        {
            positionIndicator = indicator;
        }

        public void EnterState(TreeStateManager treeManager)
        {
            // 生成一个非常高的圆柱体，并设置为可见
            heightIndicator = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            heightIndicator.transform.position = treeManager.TreePosition + new Vector3(0, 50, 0);  // 初始高度设置在树位置的上方
            heightIndicator.transform.localScale = new Vector3(0.06f, 50f, 0.06f);
            heightIndicator.GetComponent<Collider>().isTrigger = true;

            // 将Inspector面板中的材质应用到heightIndicator
            if (treeManager.cylinderMaterial != null)
            {
                heightIndicator.GetComponent<Renderer>().material = treeManager.cylinderMaterial;
            }

            // 初始化虚线
            lineRenderer = new GameObject("HeightLine").AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.material = treeManager.lineMaterial;  // 使用Inspector中的材质
            lineRenderer.positionCount = 2;
            lineRenderer.useWorldSpace = true;

            // 设置虚线模式
            lineRenderer.textureMode = LineTextureMode.Tile;
            lineRenderer.material.mainTexture = Resources.Load<Texture2D>("Textures/DashTexture");
            lineRenderer.material.mainTextureScale = new Vector2(1f / lineRenderer.startWidth, 1f);

            // 创建世界空间的Canvas
            GameObject canvasObject = new GameObject("WorldCanvas");
            worldCanvas = canvasObject.AddComponent<Canvas>();
            worldCanvas.renderMode = RenderMode.WorldSpace;
            worldCanvas.transform.position = treeManager.TreePosition + new Vector3(0, 0.2f, 0);
            worldCanvas.transform.localScale = Vector3.one * 0.01f;

            // 添加TextMeshPro的Text对象
            GameObject textObject = new GameObject("HeightText");
            textObject.transform.SetParent(worldCanvas.transform);
            heightText = textObject.AddComponent<TextMeshProUGUI>();
            heightText.fontSize = 0.05f;
            heightText.alignment = TextAlignmentOptions.Center;
            heightText.text = "Height: 0.00 m";

            RectTransform rectTransform = heightText.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(1, 1);
            rectTransform.anchoredPosition = Vector2.zero;
        }

        public void UpdateState(TreeStateManager treeManager)
        {
            Vector3 handPosition = treeManager.rightControllerTransform.position;
            Vector3 handForward = treeManager.rightControllerTransform.forward;

            RaycastHit hit;
            if (Physics.Raycast(handPosition, handForward, out hit))
            {
                lineRenderer.SetPosition(0, handPosition);
                lineRenderer.SetPosition(1, hit.point);

                if (hit.collider.gameObject == heightIndicator)
                {
                    if (heightMarker == null)
                    {
                        heightMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        heightMarker.transform.localScale = Vector3.one * 0.1f;
                        heightMarker.GetComponent<Renderer>().material = treeManager.sphereMaterial;
                    }
                    Vector3 adjustedPosition = new Vector3(treeManager.TreePosition.x, hit.point.y, treeManager.TreePosition.z);
                    heightMarker.transform.position = adjustedPosition;

                    float currentHeight = adjustedPosition.y - treeManager.TreePosition.y;
                    heightText.text = $"Height: {currentHeight:F2} m";

                    worldCanvas.transform.LookAt(Camera.main.transform);
                    worldCanvas.transform.Rotate(0, 180, 0);
                }
            }
            else
            {
                lineRenderer.SetPosition(0, handPosition);
                lineRenderer.SetPosition(1, handPosition + handForward * 100);
            }

            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                if (heightMarker != null)
                {
                    treeManager.TreeHeight = heightMarker.transform.position.y - treeManager.TreePosition.y;
                    heightIndicator.GetComponent<Renderer>().enabled = false;

                    GameObject.Destroy(lineRenderer.gameObject);

                    finalLineRenderer = new GameObject("FinalHeightLine").AddComponent<LineRenderer>();
                    finalLineRenderer.startWidth = 0.05f;
                    finalLineRenderer.endWidth = 0.05f;
                    finalLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                    finalLineRenderer.textureMode = LineTextureMode.Tile;
                    finalLineRenderer.material.mainTexture = Resources.Load<Texture2D>("Textures/DashTexture");
                    finalLineRenderer.material.mainTextureScale = new Vector2(1f / finalLineRenderer.startWidth, 1f);
                    finalLineRenderer.positionCount = 2;
                    finalLineRenderer.useWorldSpace = true;
                    finalLineRenderer.SetPosition(0, positionIndicator.transform.position);
                    finalLineRenderer.SetPosition(1, heightMarker.transform.position);

                    GameObject.Destroy(worldCanvas.gameObject);

                    treeManager.SetState(new SetTreeRadiusState(positionIndicator, heightMarker));
                }
            }
        }
    }
}
