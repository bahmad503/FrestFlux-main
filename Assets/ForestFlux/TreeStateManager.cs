using UnityEngine;

namespace TreePositioning
{
    public class TreeStateManager : MonoBehaviour
    {
        public Material lineMaterial;
        public Material sphereMaterial;
        public Material cylinderMaterial;
        public string treeName;
        public Transform rightControllerTransform;
        public Transform leftControllerTransform;

        public Vector3 TreePosition;
        public float TreeHeight;
        public float TreeCircumference;
        public float TreeRadius;

        public GameObject aiPanel;

        private ITreeState currentState;

        private void Start()
        {
            SetState(new TalkToAIState());
        }

        private void Update()
        {
            currentState?.UpdateState(this);
        }

        public void SetState(ITreeState newState)
        {
            currentState = newState;
            currentState.EnterState(this);
        }
    }
}
