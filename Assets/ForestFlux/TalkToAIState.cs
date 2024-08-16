using UnityEngine;

namespace TreePositioning
{
    public class TalkToAIState : ITreeState
    {
        public void EnterState(TreeStateManager treeManager)
        {
            // 激活AI对话面板
            if (treeManager.aiPanel != null)
            {
                treeManager.aiPanel.SetActive(true);
            }

            Debug.Log("进入AI对话状态");
        }

        public void UpdateState(TreeStateManager treeManager)
        {
            // 如果按下A键，结束对话并进入下一状态
            if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
            {
                // 关闭AI对话面板
                if (treeManager.aiPanel != null)
                {
                    treeManager.aiPanel.SetActive(true);
                }

                treeManager.SetState(new DetermineTreePositionState());  // 进入选择树的位置状态
            }
        }
    }
}