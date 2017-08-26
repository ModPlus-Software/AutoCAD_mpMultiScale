using System.Collections.Generic;
using ModPlusAPI.Interfaces;

namespace mpMultiScale
{
    public class Interface : IModPlusFunctionInterface
    {
        public SupportedProduct SupportedProduct => SupportedProduct.AutoCAD;
        public string Name => "mpMultiScale";
        public string AvailProductExternalVersion => "2017";
        public string ClassName => string.Empty;
        public string LName => "Мультимасштаб";
        public string Description => "Функция позволяет масштабировать каждый из выбранных объектов относительно самих себя";
        public string Author => "Пекшев Александр aka Modis";
        public string Price => "0";
        public bool CanAddToRibbon => true;
        public string FullDescription => "Каждый выбранный объект будет масштабирован относительно указанной точки – Левая Нижняя, Правя Нижняя, Левая Верхняя, Правая Верхняя или Центральная. Точка масштабирования определяется по прямоугольнику, описывающему объект";
        public string ToolTipHelpImage => "mpMultiScale.png";
        public List<string> SubFunctionsNames => new List<string>();
        public List<string> SubFunctionsLames => new List<string>();
        public List<string> SubDescriptions => new List<string>();
        public List<string> SubFullDescriptions => new List<string>();
        public List<string> SubHelpImages => new List<string>();
        public List<string> SubClassNames => new List<string>();
    }
}