using UnityEngine;

namespace TreePositioning
{
    public class DetermineTreePositionState : ITreeState
    {
        private GameObject positionIndicator;
        private bool haveSetBall = false;

        public void EnterState(TreeStateManager treeManager)
        {
            // 检查是否已有指示器存在，如果没有则创建一个新的
            if (positionIndicator == null)
            {
                positionIndicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                positionIndicator.transform.localScale = Vector3.one * 0.1f;  // 调整大小
                positionIndicator.GetComponent<Renderer>().material = treeManager.sphereMaterial;  // 使用Inspector中的材质
            }

            // 重置指示器和状态
            positionIndicator.SetActive(false);
            haveSetBall = false;
            Debug.Log("进入确定树的位置状态");
        }

        public void UpdateState(TreeStateManager treeManager)
        {
            Vector3 rightControllerPosition = treeManager.rightControllerTransform.position;

            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                treeManager.TreePosition = rightControllerPosition;
                positionIndicator.transform.position = treeManager.TreePosition;
                positionIndicator.SetActive(true);
                Debug.Log("树的位置已设置为: " + treeManager.TreePosition);
                haveSetBall = true;
            }

            Vector2 rightJoystickInput = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick, OVRInput.Controller.RTouch);
            Vector2 leftJoystickInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);

            Vector3 movement = new Vector3(rightJoystickInput.x, leftJoystickInput.y, rightJoystickInput.y) * Time.deltaTime * 2;
            treeManager.TreePosition += movement;

            positionIndicator.transform.position = treeManager.TreePosition;

            if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch) && haveSetBall)
            {
                treeManager.SetState(new SetTreeHeightState(positionIndicator));
            }
        }
    }
}
