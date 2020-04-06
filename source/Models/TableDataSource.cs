using System;
using System.Collections.Generic;
using System.Linq;
using NStack;
using Terminal.Gui;

namespace myotui.Models
{
    public class TableDataSource : IListDataSource
    {
        private IList<IDictionary<string, object>> _contentList;
        private IList<string> _columnMapOrder;
        private IList<double> _columnProportions;
        public int Count => _contentList.Count();

        public TableDataSource(IList<IDictionary<string, object>> contentList, IList<string> columnMapOrder, IList<double> columnPorportions)
        {
            _contentList = contentList;
            _columnMapOrder = columnMapOrder;
            _columnProportions = columnPorportions;
        }
        public bool IsMarked(int item)
        {
            //TODO implement marking
            return false;
        }

        public void Render(ListView container, ConsoleDriver driver, bool selected, int item, int col, int line, int width)
        {
            var widths = CalculateWidths(width, _columnProportions);
            var columnStarts = CalculateColumnStarts(width, widths);
            for(int i = 0; i < columnStarts.Count; i++)
            {
                var columnStart = columnStarts[i];
                container.Move(columnStart,line);
                var columnWidth = widths[i];
                var columnName = _columnMapOrder.Count > 0 ? _columnMapOrder[i] : "value";
                var columnValue = _contentList[item][columnName] ?? "";
                columnValue = columnValue.ToString().Replace(Environment.NewLine," ");
                RenderUstr(driver, columnValue.ToString(), columnWidth);
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
				if (used+count >= width)
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

        private IList<int> CalculateWidths(int maxWidth, IList<double> proportions)
        {
            var proportionsSum = proportions.Sum();
            var absoluteWidths = proportions.SkipLast(1).Select(prop => (int)(maxWidth*prop/proportionsSum)).ToList();
            absoluteWidths.Add(maxWidth - absoluteWidths.Sum());
            return absoluteWidths;
        }

        private IList<int> CalculateColumnStarts(int maxWidth, IList<int> widths)
        {
            return widths.SkipLast(1).Aggregate(new List<int>(){0},(list, current) => 
            {
                var previous = list.Last();
                list.Add(previous + current);
                return list;
            }
            );
        }
    }
}