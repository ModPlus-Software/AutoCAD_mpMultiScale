#pragma warning disable SA1600 // Elements should be documented
namespace mpMultiScale
{
    using System;
    using System.Collections.Generic;
    using ModPlusAPI.Interfaces;

    public class ModPlusConnector : IModPlusFunctionInterface
    {
        public SupportedProduct SupportedProduct => SupportedProduct.AutoCAD;
        
        public string Name => "mpMultiScale";

#if A2013
        public string AvailProductExternalVersion => "2013";
#elif A2014
        public string AvailProductExternalVersion => "2014";
#elif A2015
        public string AvailProductExternalVersion => "2015";
#elif A2016
        public string AvailProductExternalVersion => "2016";
#elif A2017
        public string AvailProductExternalVersion => "2017";
#elif A2018
        public string AvailProductExternalVersion => "2018";
#elif A2019
        public string AvailProductExternalVersion => "2019";
#elif A2020
        public string AvailProductExternalVersion => "2020";
#endif

        public string FullClassName => string.Empty;
        
        public string AppFullClassName => string.Empty;
        
        public Guid AddInId => Guid.Empty;
        
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
#pragma warning restore SA1600 // Elements should be documented