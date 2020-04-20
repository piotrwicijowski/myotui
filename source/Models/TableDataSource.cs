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
        public string Highlight {get; set;}
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
        private static readonly Terminal.Gui.Attribute _highlightColor = Terminal.Gui.Attribute.Make(Color.Red, Color.Black);
        private static readonly Terminal.Gui.Attribute _highlightFocusColor = Terminal.Gui.Attribute.Make(Color.Black, Color.Red);

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
            var focusedAndSelected = container.HasFocus && selected;
            // var currentColor = container.HasFocus ? (selected ? container.ColorScheme.Focus : container.ColorScheme.Normal) : container.ColorScheme.Normal;
            var currentColor = focusedAndSelected ? container.ColorScheme.Focus : container.ColorScheme.Normal;
            var savedColor = currentColor;

            var widths = _columnWidths.Count == 0 ? new List<int>(){width} : CalculateWidths(width, _columnWidths);
            var columnStarts = CalculateColumnStarts(width, widths);
            for(int i = 0; i < columnStarts.Count; i++)
            {
                var columnStart = columnStarts[i];
                container.Move(columnStart,line);
                var columnWidth = widths[i];
                var columnName = _columnMapOrder.Count > 0 ? _columnMapOrder[i] : "value";
                // var columnValue = _contentList[item][columnName] ?? "";
                object columnValue;
                var hasValue = _contentList[item].TryGetValue(columnName, out columnValue);
                columnValue = columnValue ?? "";
                columnValue = columnValue.ToString().Replace(Environment.NewLine," ");
                var newColor = focusedAndSelected ? FocusColors[i % Colors.Count()] : Colors[i % Colors.Count()];
                if(currentColor != newColor){
                    driver.SetAttribute(newColor);
                    currentColor = newColor;
                }
                RenderUstr(driver, columnValue.ToString(), columnWidth, currentColor, focusedAndSelected);
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

		void RenderUstr (ConsoleDriver driver, ustring ustr, int width, Terminal.Gui.Attribute defaultColor, bool focusedAndSelected)
		{
			int byteLen = ustr.Length;
            var highlights = new List<(int start, int end)>();
            if(!string.IsNullOrEmpty(Highlight))
            {
                int pos = 0;
                while(pos < ustr.Length)
                {
                    var highlightStart = ustr.ToString().IndexOf(Highlight,pos, StringComparison.CurrentCultureIgnoreCase);
                    var highlightEnd = highlightStart == -1 ? -1 : highlightStart + Highlight.Length - 1;
                    if(highlightStart >= pos)
                    {
                        highlights.Add((start: highlightStart, end: highlightEnd));
                        pos = highlightEnd + 1;
                    }
                    else
                    {
                        break;
                    }
                }
            }
			int used = 0;
            int highlightsIndex = -1;
            int currentHighlightStart = -1;
            int currentHighlightEnd = -1;
            if(highlights.Count > 0)
            {
                highlightsIndex++;
                currentHighlightStart = highlights[highlightsIndex].start;
                currentHighlightEnd = highlights[highlightsIndex].end;
            }
			for (int i = 0; i < byteLen;) {
                if(used == currentHighlightStart) { driver.SetAttribute(focusedAndSelected ? _highlightFocusColor : _highlightColor); }
				(var rune, var size) = Utf8.DecodeRune (ustr, i, i - byteLen);
				var count = Rune.ColumnWidth (rune);
                if(used+count-1 >= width) { driver.SetAttribute(defaultColor); }
				if (used+count-1 >= width)
					break;
				driver.AddRune (rune);
                if(used == currentHighlightEnd) { 
                    highlightsIndex++;
                    if(highlightsIndex >= highlights.Count){
                        currentHighlightStart = -1;
                        currentHighlightEnd = -1;
                    }
                    else
                    {
                        currentHighlightStart = highlights[highlightsIndex].start;
                        currentHighlightEnd = highlights[highlightsIndex].end;
                    }
                    driver.SetAttribute(defaultColor);
                }
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

        private IList<int> CalculateWidths(int maxWidth, IList<SizeHint> hints)
        {
            // var proportionsSum = proportions.Sum();
            // var absoluteWidths = proportions.SkipLast(1).Select(prop => (int)(maxWidth*prop/proportionsSum)).ToList();
            var totalGapsWidth = hints.Count() - 1;
            var usableWidth = maxWidth - totalGapsWidth;

            var totalFixedWidth = hints.Where(hint => hint.Mode == SizeMode.Fixed).Select(hint => hint.Fixed).Sum();
            var totalAutoWidth = hints.Select((hint,index) => (hint, index)).Where(item => item.hint.Mode == SizeMode.Auto).Select(item => _maxColumnWidths[item.index]).Sum();
            var fillColumnsRatioScale = hints.Where(hint => hint.Mode == SizeMode.Fill).Sum(hint => hint.FillRatio);
            var autoColumnsRatioScale = hints.Where(hint => hint.Mode == SizeMode.Auto).Sum(hint => hint.FillRatio);
            var remainingFlexWidth = usableWidth - totalAutoWidth - totalFixedWidth;

            var absoluteWidths = hints.Select((hint, index) => 
            {
                var unclamped = hint.Mode switch
                {
                    SizeMode.Fixed => hint.Fixed,
                    SizeMode.Auto => _maxColumnWidths[index],
                    SizeMode.Fill => (int)Math.Floor(Helpers.Clamp(value: hint.FillRatio * remainingFlexWidth / fillColumnsRatioScale, min: hint.FillMinPercentage * remainingFlexWidth / 100.0 , max: hint.FillMaxPercentage * remainingFlexWidth / 100.0)),
                };
                return Helpers.Clamp(unclamped, 0, maxWidth);
            });
            var overshoot = absoluteWidths.Sum() - usableWidth;
            absoluteWidths = absoluteWidths.Zip(hints, (calculatedWidth, hint) =>
            {
                var unclamped = hint.Mode switch
                {
                    SizeMode.Fixed => calculatedWidth,
                    SizeMode.Auto => calculatedWidth - (int)Math.Floor(overshoot * hint.FillRatio / (fillColumnsRatioScale + autoColumnsRatioScale)),
                    SizeMode.Fill => calculatedWidth - (int)Math.Floor(overshoot * hint.FillRatio / (fillColumnsRatioScale + autoColumnsRatioScale)),
                };
                return Helpers.Clamp(unclamped, 0, maxWidth);
            });

            return absoluteWidths.ToList();
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