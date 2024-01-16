using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Game.GraphJob {
    public interface IGraphNode {
        string Name { get; set; }
        Group Group { get; set; }
        void Init(Vector2 position, Action<IGraphNode, string> onNameChange);
        void Draw();
        void SetErrorStyle(Color color);
        void ResetStyle();
    }
}
