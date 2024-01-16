using UnityEditor;
using UnityEngine.UIElements;

namespace Game.GraphJob {
    public class GraphJobEditorWindow : EditorWindow {

        [MenuItem("Window/Game/Graph Job")]
        public static void Open() {
            GetWindow<GraphJobEditorWindow>("Graph Job");
        }

        private void OnEnable() {
            addGraphView();
            addStyles();
        }

        private void addGraphView() {

            var jobGraphView = new JobGraphView();

            jobGraphView.StretchToParentSize();

            rootVisualElement.Add(jobGraphView);
        }

        private void addStyles() {
            var styleSheet = (StyleSheet)EditorGUIUtility.Load("GameScryptVariables.uss");
            rootVisualElement.styleSheets.Add(styleSheet);
        }
    }
}
