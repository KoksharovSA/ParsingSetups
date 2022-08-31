using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsingSetups
{
    class Detail
    {
        public Detail()
        {
        }

        public Detail(string nameDetail, string sizesDetail, string surfaceDetail, string timeOfProcessing, string cuttingLength, string weightDetail, string materialDetail, string bendLength)
        {
            NameDetail = nameDetail ?? throw new ArgumentNullException(nameof(nameDetail));
            SizesDetail = sizesDetail ?? throw new ArgumentNullException(nameof(sizesDetail));
            SurfaceDetail = surfaceDetail ?? throw new ArgumentNullException(nameof(surfaceDetail));
            TimeOfProcessing = timeOfProcessing ?? throw new ArgumentNullException(nameof(timeOfProcessing));
            CuttingLength = cuttingLength ?? throw new ArgumentNullException(nameof(cuttingLength));
            WeightDetail = weightDetail ?? throw new ArgumentNullException(nameof(weightDetail));
            MaterialDetail = materialDetail ?? throw new ArgumentNullException(nameof(materialDetail));
            BendLength = bendLength ?? throw new ArgumentNullException(nameof(bendLength));
        }

        public string NameDetail { get; set; }
        public string SizesDetail { get; set; }
        public string SurfaceDetail { get; set; }
        public string TimeOfProcessing { get; set; }
        public string CuttingLength { get; set; }
        public string WeightDetail { get; set; }
        public string MaterialDetail { get; set; }
        public string BendLength { get; set; }

        public override string ToString()
        {
            return "Название детали: " + NameDetail + "\nРазмер детали: " + SizesDetail + "\nМатериал детали: " + MaterialDetail + "\nПоверхность детали: "
                + SurfaceDetail + "\nМасса детали: " + WeightDetail + "\nВремя обработки: " + TimeOfProcessing + "\nДлина реза: " + CuttingLength + "\nДлина гибов: " + BendLength;
        }

    }
}
