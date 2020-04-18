using System;
using System.Collections.Generic;
using System.Linq;
using myotui.Models.Config;
using NStack;
using Terminal.Gui;
using Unix.Terminal;

namespace myotui.Models
{
    public class TableDataSource : IListDataSource
    {
        private IList<IDictionary<string, object>> _contentList;
        private IList<string> _columnMapOrder;
        private IList<SizeHint> _columnWidths;
        private IList<int> _maxColumnWidths = new List<int>();
        public int Count => _contentList.Count();
        private static readonly List<Terminal.Gui.Attribute> Colors = new List<Terminal.Gui.Attribute>()
        {
            Terminal.Gui.Attribute.Make(Color.White,   Color.Black),
            Terminal.Gui.Attribute.Make(Color.Brown,   Color.Black),
            Terminal.Gui.Attribute.Make(Color.Green,   Color.Black),
            Terminal.Gui.Attribute.Make(Color.Magenta, Color.Black),
        };
        private static readonly List<Terminal.Gui.Attribute> FocusColors = new List<Terminal.Gui.Attribute>()
        {
            Terminal.Gui.Attribute.Make(Color.Black, Color.White  ),
            Terminal.Gui.Attribute.Make(Color.Black, Color.Brown  ),
            Terminal.Gui.Attribute.Make(Color.Black, Color.Green  ),
            Terminal.Gui.Attribute.Make(Color.Black, Color.Magenta),
        };

        public TableDataSource(IList<IDictionary<string, object>> contentList, IList<string> columnMapOrder, IList<SizeHint> columnWidths)
        {
            _contentList = contentList;
            _columnMapOrder = columnMapOrder;
            _columnWidths = columnWidths;
        }
        public bool IsMarked(int item)
        {
            //TODO implement marking
            return false;
        }

        public void Render(ListView container, ConsoleDriver driver, bool selected, int item, int col, int line, int width)
        {
            var currentColor = container.HasFocus ? (selected ? container.ColorScheme.Focus : container.ColorScheme.Normal) : container.ColorScheme.Normal;
            var savedColor = currentColor;

            var widths = CalculateWidths(width, _columnWidths);
            var columnStarts = CalculateColumnStarts(width, widths);
            for(int i = 0; i < columnStarts.Count; i++)
            {
                var columnStart = columnStarts[i];
                container.Move(columnStart,line);
                var columnWidth = widths[i];
                var columnName = _columnMapOrder.Count > 0 ? _columnMapOrder[i] : "value";
                var columnValue = _contentList[item][columnName] ?? "";
                columnValue = columnValue.ToString().Replace(Environment.NewLine," ");
                var newColor = (container.HasFocus && selected) ? FocusColors[i % Colors.Count()] : Colors[i % Colors.Count()];
                if(currentColor != newColor){
                    driver.SetAttribute(newColor);
                    currentColor = newColor;
                }
                RenderUstr(driver, columnValue.ToString(), columnWidth);
				driver.AddRune(' ');
            }
            if(savedColor != currentColor)
            {
                driver.SetAttribute(savedColor);
            }
        }

        public IDictionary<string, object> this[int key]
        {
            get
            {
                if(_contentList != null && key < _contentList.Count() && key >=0)
                {
                    return _contentList[key];
                }
                else
                {
                    return null;
                }
            }
        }

		void RenderUstr (ConsoleDriver driver, ustring ustr, int width)
		{
			int byteLen = ustr.Length;
			int used = 0;
			for (int i = 0; i < byteLen;) {
				(var rune, var size) = Utf8.DecodeRune (ustr, i, i - byteLen);
				var count = Rune.ColumnWidth (rune);
				if (used+count-1 >= width)
					break;
				driver.AddRune (rune);
				used += count;
				i += size;
			}
			for (; used < width; used++) {
				driver.AddRune (' ');
			}
		}

        public void SetMark(int item, bool value)
        {
            //TODO implement marking
            return;
        }

        private int ColToColumnNumber(int col, IList<int> cumulativeWidths)
        {
            for(int i = cumulativeWidths.Count - 1; i >= 0; i++)
            {
                if(col <= cumulativeWidths[i])
                {
                    return i;
                }
            }
            return 0;
        }

        private IList<int> CalculateWidths(int maxWidth, IList<SizeHint> widths)
        {
            // var proportionsSum = proportions.Sum();
            // var absoluteWidths = proportions.SkipLast(1).Select(prop => (int)(maxWidth*prop/proportionsSum)).ToList();
            var totalGapsWidth = widths.Count() - 1;
            var absoluteWidths = _maxColumnWidths.SkipLast(1).ToList();
            absoluteWidths.Add(maxWidth - absoluteWidths.Sum() - totalGapsWidth);
            return absoluteWidths;
        }

        private IList<int> CalculateColumnStarts(int maxWidth, IList<int> widths)
        {
            return widths.SkipLast(1).Aggregate(new List<int>(){0},(list, current) => 
            {
                var previous = list.Last();
                list.Add(previous + current + 1);
                return list;
            }
            );
        }

        public IDictionary<string, int> GetMaxColumnContentWidths()
        {
            var dictWidths = _contentList.Select(row => row.ToDictionary(kp => kp.Key, kp => kp.Value?.ToString().Length ?? 0));
            var keys = _contentList.FirstOrDefault().Keys;
            return keys.ToDictionary(key => key, key => dictWidths.Select(dict => dict[key]).Max());
        }

        public void SetMaxColumnWidths(IDictionary<string,int> maxColumnWidths)
        {
            _maxColumnWidths = new List<int>(_columnMapOrder.Select(columnName => maxColumnWidths[columnName]));
        }
    }
}