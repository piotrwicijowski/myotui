using System.Collections.Generic;
using myotui.Models;
using myotui.Models.Config;
using myotui.Views;

namespace myotui
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<ViewNode> ChildrenWithSplitters(this ViewNode parentNode)
        {
            var first = true;
            var autoSplitters = (parentNode.Buffer as LayoutBuffer)?.AutoSplitters ?? false;
            foreach (var child in parentNode.Children)
            {
                if(first || !autoSplitters)
                {
                    first = !first;
                    yield return child;
                }
                else
                {
                    var splitterBuffer = new SplitterBuffer();
                    var splitterNode = new ViewNode
                    {
                        Buffer = splitterBuffer,
                        Parent = parentNode,
                        // Focusable = false,
                        View = new Splitter(),
                        Width = new SizeHint
                        {
                            Mode = SizeMode.Fixed,
                            Fixed = 1,
                        },
                        Height = new SizeHint
                        {
                            Mode = SizeMode.Fixed,
                            Fixed = 1,
                        },
                    };
                    yield return splitterNode;
                    yield return child;

                }
            }
            yield break;
        }

    }
}