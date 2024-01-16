using Game.Scene;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using bf = System.Reflection.BindingFlags;

namespace Game.GraphJob {
    public class GraphNode : Node, IGraphNode {

        public string Name { get; set; }
        public List<string> Choices { get; set; }

        public NodeType NodeType { get; set; }
        public Group Group { get; set; }

        private Action<IGraphNode, string> _onNameChange;
        private StyleColor _defaultBackgroundColor;
        private DropdownField _dropdownField;

        public virtual void Init(Vector2 position, Action<IGraphNode, string> onNameChange) {

            Name = "Name";
            Choices = new List<string>();

            _onNameChange = onNameChange;

            base.SetPosition(new Rect(position, Vector2.zero));

            mainContainer.AddToClassList("gj-node__main-container");
            extensionContainer.AddToClassList("gj-node__extensions-container");
        }

        public virtual void Draw() {

            /* TITLE CONTAINER */

            var dialogueNameTextField = new TextField() {
                value = Name
            };
            dialogueNameTextField.RegisterValueChangedCallback(change => {
                _onNameChange?.Invoke(this, change.newValue);
            });
            dialogueNameTextField.AddToClassList("gj-node__textfield");
            dialogueNameTextField.AddToClassList("gj-node__textfield__hidden");

            base.titleContainer.Insert(0, dialogueNameTextField);

            /* INPUT CONTAINER */

            var inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            inputPort.portName = "Do";

            base.inputContainer.Add(inputPort);

            /* EXTENSIONS CONTAINER */

            var customDataContainer = new VisualElement();
            customDataContainer.AddToClassList("gj-node__custom-data-container");

            var textFoldout = new Foldout() {
                text = "Action"
            };

            var objectField = new ObjectField("Scene Instructor") {
                objectType = typeof(SceneInstructor),
                allowSceneObjects = true,
                value = null,
                name = "GameObject",
            };
            objectField.AddToClassList("gj-node__textfield");
            objectField.RegisterValueChangedCallback(onChange);

            _dropdownField = new DropdownField("Methods") {
                name = "Methods",
                choices = new List<string>()
            };
            _dropdownField.AddToClassList("gj-node__textfield");

            textFoldout.Add(objectField);
            textFoldout.Add(_dropdownField);

            customDataContainer.Add(textFoldout);

            base.extensionContainer.Add(customDataContainer);
        }

        public void SetErrorStyle(Color color) {
            _defaultBackgroundColor = mainContainer.style.backgroundColor;
            mainContainer.style.backgroundColor = color;
        }

        public void ResetStyle() {
            mainContainer.style.backgroundColor = _defaultBackgroundColor;
        }

        private void onChange(ChangeEvent<UnityEngine.Object> evt) {
            var methods = getMethods(evt.newValue as SceneInstructor);

            _dropdownField.choices = new List<string>();

            foreach (var item in methods) {
                _dropdownField.choices.Add(item.Name);
            }
        }


        // for more info on this: https://discussions.unity.com/t/how-do-i-generate-a-drop-down-list-of-functions-on-scripts-attached-to-a-game-object/99504/2
        private List<MethodInfo> getMethods(SceneInstructor target) {

            var methods = new List<MethodInfo>();

            if (target == null) return methods;

            var flags = bf.Instance | bf.Public | bf.Default;
            methods.AddRange(target.GetMethods(typeof(void), null, flags));

            return methods;
        }
    }
}