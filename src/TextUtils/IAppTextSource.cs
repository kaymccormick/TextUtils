using System.Windows.Media.TextFormatting;

namespace TextUtils
{
    public interface IAppTextSource 
    {
        /// <summary>
        /// 
        /// </summary>
        void Init();

        /// <summary>
        /// 
        /// </summary>
        int Length { get; }

        /// <summary>
        /// 
        /// </summary>
        GenericTextRunProperties? BaseProps { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        TextRunProperties BasicProps();

        TextRun GetTextRun(
            int textSourceCharacterIndex
        );


        /// <summary>
        /// TextFormatter to get text span immediately before specified text source position.
        /// </summary>
        /// <param name="textSourceCharacterIndexLimit">character index to specify where in the source text the text retrieval stops.</param>
        /// <returns>text span immediately before the specify text source character index.</returns>
        /// <remarks> 
        /// Return empty CharacterBufferRange in the text span if the text span immediately before the 
        /// specified position doesn't contain any text (such as inline object or hidden run). 
        /// Return a zero length TextSpan if there is nothing preceding the specified position.
        /// </remarks>
        TextSpan<CultureSpecificCharacterBufferRange> GetPrecedingText(
            int textSourceCharacterIndexLimit
        );


        /// <summary>
        /// TextFormatter to map a text source character index to a text effect character index        
        /// </summary>
        /// <param name="textSourceCharacterIndex"> text source character index </param>
        /// <returns> the text effect index corresponding to the text source character index </returns>
        int GetTextEffectCharacterIndexFromTextSourceCharacterIndex(
            int textSourceCharacterIndex
        );

        /// <summary>
        /// PixelsPerDip at which the text should be rendered. Any class which extends TextSource should update
        /// this property whenever DPI changes for a Per Monitor DPI Aware Application.
        /// </summary>
        double PixelsPerDip { get; set; }

        

    }
}