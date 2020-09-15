using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using CodeExMachina;

namespace TextUtils
{
    internal static class TR
    {
        public static Func<InputTuple, ILineInfo2Base?, TextChange?> CreateDelegate2(BTree<RectInfo> bTree,
            Action<CharInfo, ILineInfo2Base>? takeChar, BTree<TextRunInfo>? btree1)
        {
            TextChange? Delegate1(InputTuple tuple, ILineInfo2Base? line)
            {
                var (textLine, position, _, offset, _, _, change, lineNo1) = tuple;
                // if (offset == null)
                // {
                //     throw new CodeControlException("Offset is null");
                // }
                HandleLine2(textLine, position, lineNo1, offset, change, line, btree1, takeChar);
                return change;
            }

            return
                Delegate1;
        }

        public static void HandleLine2(TextLine? myTextLine,
            Point linePosition, int lineNo, int textStorePosition,
            TextChange? change,
            ILineInfo2Base? curLineInfo = null, BTree<TextRunInfo>? btRuns = null, Action<CharInfo, ILineInfo2Base>? takeChar=null)
        {
            var curPos = linePosition;
            var indexedGlyphRuns = myTextLine?.GetIndexedGlyphRuns();

            // var z = new List<TextRunInfo>();

            // btRuns?.DescendGreaterThan(new TextRunInfo(null, textStorePosition - 1),
            // info =>
            // {
            // z.Add(info);
            // return info.Value < textStorePosition + length;
            // });

            // using (var enum2 = z.GetEnumerator())
            {
                // var moveNext2 = enum2.MoveNext();
                var lineCharIndex = 0;
                var xOrigin = linePosition.X+ myTextLine.Start;
                if (indexedGlyphRuns == null) return;
                TextRunInfo? prevTri = null;
                var lineRun = 0;
                foreach (var glyphRunC in indexedGlyphRuns)
                {
                    var gl = glyphRunC.GlyphRun;
                    var advanceSum = gl.AdvanceWidths.Sum();
                    for (var i = 0; i < gl.Characters.Count; i++)
                    {
                        var i0 = gl.ClusterMap?[i] ?? i;
                        var glAdvanceWidth = gl.AdvanceWidths[i0];
                        var glCharacter = gl.Characters[i];
                        var glCaretStop = gl.CaretStops?[i0];
                        var glyphIndex = gl.GlyphIndices[i0];

                        var storePosition = textStorePosition + lineCharIndex;
                        // Trace.WriteLine("char line " + lineNo + "[" + storePosition + "]: " + glCharacter);

                        var ci = new CharInfo(lineNo, storePosition, lineCharIndex, i,
                            glCharacter, glAdvanceWidth,
                            glCaretStop, xOrigin, linePosition.Y, gl.GlyphTypeface.Height * gl.FontRenderingEmSize, gl.ComputeAlignmentBox(),
                            gl.BaselineOrigin, glyphIndex, gl.GlyphTypeface, gl.FontRenderingEmSize, lineRun);
                        takeChar(ci,curLineInfo);

                        lineCharIndex++;
                        xOrigin += glAdvanceWidth;
                    }

                    lineRun++;
                    // var item = new Rect(curPos, new Size(advanceSum, myTextLine.Height));
                    // if (!moveNext2)
                    // {w
                    //     var sb = new StringBuilder();
                    //     for (var c = curCharInfoNode; c != null; c = c.Next) sb.Append(c.Value.Character);
                    //
                    //     throw new CodeControlException("enumerator empty " + sb);
                    // }

                    // if (prevTri?.TextRun != null && prevTri != null && offset < prevTri.Value + prevTri.TextRun.Length)
                    // {
                    //     prevTri.Rect.Union(item);
                    // }
                    // else
                    // {
                    //     var textRunInfo = new TextRunInfo(offset);
                    //     var tri = btRuns?.Get(textRunInfo);
                    //     if (tri == null)
                    //     {
                    //         var sb = new StringBuilder();
                    //         for (var c = curCharInfoNode; c != null; c = c.Next) sb.Append(c.Value.Character);
                    //         throw new CodeControlException("Unable to find entry for " + offset);
                    //     }
                    //
                    //     tri.Rect = item;
                    //
                    //     rectBtree?.ReplaceOrInsert(new RectInfo(tri.Rect, offset, tri.TextRun));
                    //
                    //     prevTri = tri;
                    // }

                    curPos.X += advanceSum;
                }
            }

#pragma warning disable 8601
#pragma warning restore 8601
        }

        public static void NewMethod2(TextChange? change,
            int textStorePosition,
            ICustomTextSource source,
            double paragraphWidth0,
            Point linePosition,
            int lineNo,
            TextParagraphProperties genericTextParagraphProperties,
            DrawingContext myDc,
            Func<InputTuple, ILineInfo2Base?, TextChange?> delegate1,
            ILineInfo2Base [] lineInfos,
            int liIndex,
            int batchLines,
            DrawingGroup myGroup0,
            object? batchArg, Func<DrawingContext?, DrawingGroup, ILineInfo2Base[], int, bool, object?, bool> batchAction, int? end)
        {

            TextLineBreak? prev = null;

            var myGroup = myGroup0;
            var initialLineSet = true;
            while (textStorePosition < end.GetValueOrDefault(source.Length))
            {
                // Debug.WriteLine(Thread.CurrentThread.ManagedThreadId + ": "+ textStorePosition);
                var paragraphWidth = paragraphWidth0 - linePosition.X;
                if (paragraphWidth < 0) paragraphWidth = 0;

                InputTuple in1 = new InputTuple(null, linePosition,
                    source.AsTextSource(), textStorePosition, null,
                    null, change, lineNo);
                var (height, length, widthIncludingTrailingWhitespace, li,prev1) = DoFormatLine2(source.AsTextSource(),
                    textStorePosition, paragraphWidth, genericTextParagraphProperties, myDc, linePosition, delegate1, in1, CreateLineInfo, lineNo,prev);
                prev = prev1;
              
                // var boundingRect = new Rect(linePosition.X, linePosition.Y, widthWithWs, height);

                lineInfos[liIndex++] = li;

                linePosition.Y += height;
                lineNo++;

                // Update the index position in the text store.
                textStorePosition += length;

                // if (lineNo % (lineCount / 10) == 0)
                // progress?.Report(new UpdateProgress(textStorePosition, lineNo, lineCount));

                if (lineNo <= 0 || lineNo % batchLines != 0) continue;

                if(batchAction(myDc, myGroup, lineInfos, liIndex, initialLineSet, batchArg))
                {
                    myGroup = new DrawingGroup();
                    myDc = myGroup.Open();
                }

                liIndex = 0;
                initialLineSet = false;
            }

            if (lineNo % batchLines != 0)
            {
                batchAction(myDc, myGroup, lineInfos, liIndex, initialLineSet, batchArg);
            }
            // progress?.Report(new UpdateProgress(textStorePosition, lineNo, lineCount));
        }

        private static (double height, int length, double widthIncludingTrailingWhitespace, ILineInfo2Base li, TextLineBreak)
            DoFormatLine2
            (TextSource textSource,
                int textStorePosition,
                double paragraphWidth,
                TextParagraphProperties genericTextParagraphProperties,
                DrawingContext myDc,
                Point linePosition, Func<InputTuple, ILineInfo2Base?, TextChange?> delegate1, InputTuple t,
                Func<int, TextSpan, Rect, TextLine, ILineInfo2Base> createFunc, int lineNo, TextLineBreak? textLineBreak)
        {
            
            int length;
            double widthIncludingTrailingWhitespace;
            double height;
            var textFormatter = Formatter.Value;

            ILineInfo2Base li;
            var myTextLine = textFormatter.FormatLine
            (textSource,
                textStorePosition,
                paragraphWidth + 10,
                genericTextParagraphProperties,
                textLineBreak);
            t.Item1 = myTextLine;
               

            myTextLine.Draw(myDc, linePosition, InvertAxes.None);
            // Debug.WriteLine($"{Thread.CurrentThread.ManagedThreadId}: {linePosition}");
            length = myTextLine.Length;
            height = myTextLine.Height;
            var arg3 = new Rect(new Point(myTextLine.Start, linePosition.Y), new Size(myTextLine.WidthIncludingTrailingWhitespace, myTextLine.Height));
            li = createFunc(lineNo, new TextSpan(textStorePosition, myTextLine.Length),arg3,myTextLine);
            if (delegate1 != null)
            {
                delegate1.Invoke(t, li);
            }
            widthIncludingTrailingWhitespace = myTextLine.WidthIncludingTrailingWhitespace;

            return ( height, length, widthIncludingTrailingWhitespace,li,myTextLine.GetTextLineBreak());

        }

        public static ThreadLocal<TextFormatter> Formatter =>
            new ThreadLocal<TextFormatter>(() => TextFormatter.Create(TextFormattingMode.Ideal));
    }
}