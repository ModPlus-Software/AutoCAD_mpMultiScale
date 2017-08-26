using mpPInterface;

namespace mpMultiScale
{
    public class Interface : IPluginInterface
    {
        public string Name => "mpMultiScale";
        public string AvailCad => "2017";
        public string LName => "Мультимасштаб";
        public string Description => "Функция позволяет масштабировать каждый из выбранных объектов относительно самих себя";
        public string Author => "Пекшев Александр aka Modis";
        public string Price => "0";
    }
}