using FastReport.Code;
using FastReport.Utils;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace FastReport
{
    public partial class TextObject
    {

        #region Report Engine

        /// <inheritdoc/>
        public override async Task GetDataAsync(CancellationToken cancellationToken)
        {
            await base.GetDataAsync(cancellationToken);

            // process expressions
            if (AllowExpressions)
            {
                if (!String.IsNullOrEmpty(Brackets))
                {
                    string[] brackets = Brackets.Split(',');
                    FindTextArgs args = new FindTextArgs();
                    args.Text = new FastString(Text);
                    args.OpenBracket = brackets[0];
                    args.CloseBracket = brackets[1];
                    int expressionIndex = 0;
                    while (args.StartIndex < args.Text.Length)
                    {
                        string expression = CodeUtils.GetExpression(args, false);
                        if (expression == null)
                            break;

                        string formattedValue = await CalcAndFormatExpressionAsync(expression, expressionIndex, cancellationToken);

                        args.Text.Remove(args.StartIndex, args.EndIndex - args.StartIndex);
                        args.Text.Insert(args.StartIndex, formattedValue);

                        args.StartIndex += formattedValue.Length;
                        expressionIndex++;
                    }
                    Text = args.Text.ToString();
                }
            }

            // process highlight
            Variant varValue = new Variant(Value);
            foreach (HighlightCondition condition in Highlight)
            {
                try
                {
                    object val = await Report.CalcAsync(condition.Expression, varValue, cancellationToken);
                    if (val != null && (bool)val == true)
                    {
                        ApplyCondition(condition);
                        break;
                    }
                }
                catch (Exception e)
                {
                    throw new Exception(Name + ": " + Res.Get("Messages,ErrorInHighlightCondition") + ": " + condition.Expression, e.InnerException);
                }
            }

            // make paragraph offset
            if (ParagraphOffset != 0)
                Text = MakeParagraphOffset(Text);
            // process AutoShrink
            ProcessAutoShrink();
        }

        #endregion
    }
}