using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ad.util {
    public static class TreeUtils {
        public static IList<GeneralPurposeTreeNode<T>> BuildSubtree<T>(IEnumerable<GeneralPurposeTreeNode<T>> nodes, GeneralPurposeTreeNode<T> top = null) {
            var stack = new Stack<GeneralPurposeTreeNode<T>>();
            var result = new List<GeneralPurposeTreeNode<T>>();
            if (top != null) {
                stack.Push(top);
                top.Childs = result;
            }
            Func<GeneralPurposeTreeNode<T>> getParent = () => stack.Count == 0 ? null : stack.Peek();
            foreach (var node in nodes) {
                var parent = getParent();
                try {
                    while (parent != null && parent.Id != node.ParentId) {
                        stack.Pop();
                        parent = getParent();
                    }
                    if (parent == null)
                        continue;
                    parent.Childs.Add(node);
                    node.Parent = parent;
                }
                finally {
                    if (parent == null) {
                        result.Add(node);
                        node.Parent = null;
                    }
                    stack.Push(node);
                    node.Childs = new List<GeneralPurposeTreeNode<T>>();
                }
            }
            return result;
        }
    }
}
