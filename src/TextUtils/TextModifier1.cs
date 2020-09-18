using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.TextFormatting;
using JetBrains.Annotations;

namespace TextUtils
{
    public class TextModifier1 : TextModifier
    {
        private readonly TextRunProperties _properties;
        private XmlLanguage? _xmlLanguage = XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.Name);

        public TextModifier1(int length, [NotNull] TextRunProperties properties)
        {
            Length = length;
            _properties = properties ?? throw new ArgumentNullException(nameof(properties));
        }

        /// <inheritdoc />
        public override int Length { get; }

        /// <inheritdoc />
        public override TextRunProperties Properties
        {
            get
            {
                // Debug.WriteLine("Requested properties");
                return _properties;
            }
        }

        /// <inheritdoc />
        public override TextRunProperties ModifyProperties(TextRunProperties properties)

        {
            Debug.Write($"KFT: {nameof(ModifyProperties)} called with ");
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (properties != null) PropertiesDesc(s => Debug.Write("KFT " + s), properties);
            Debug.WriteLine("");
            if (properties != null)
                return new TextRunProperties1(
                    properties.BackgroundBrush ?? Properties.BackgroundBrush,
                    properties.BaselineAlignment,
                    properties.CultureInfo ?? Properties.CultureInfo,
                    properties.FontHintingEmSize, properties.FontRenderingEmSize,
                    properties.ForegroundBrush ?? Properties.ForegroundBrush,
                    properties.TextDecorations ?? Properties.TextDecorations, 
                    properties.TextEffects ?? properties.TextEffects,
                    properties.Typeface ?? Properties.Typeface);

            return Properties;
        }

        /// <inheritdoc />
        public override FlowDirection FlowDirection { get; }

        /// <inheritdoc />
        public override bool HasDirectionalEmbedding { get; }
        private void PropertiesDesc(Action<string> writeLine, TextRunProperties p)
        {
            if (p.Typeface != null)
            {
                writeLine($"Stretch = {p.Typeface.Stretch}, ");
                writeLine($"Style = {p.Typeface.Style}, ");
                if (p.Typeface.FontFamily != null)
                    writeLine($"Font Family = {p.Typeface.FontFamily.FamilyNames[_xmlLanguage]}, ");
                writeLine($"Typeface name = {p.Typeface.FaceNames[_xmlLanguage]}, ");
            }

            writeLine($"BaselineAlignment = {p.BaselineAlignment}" + ", ");
            writeLine($"ForegroundBrush = {p.ForegroundBrush}, ");
            writeLine($"FontSize = {p.FontRenderingEmSize}");
        }

    }
}