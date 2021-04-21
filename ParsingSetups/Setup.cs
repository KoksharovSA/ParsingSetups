using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsingSetups
{
    internal class Setup
    {
        public Setup(string nameSetup, string dirSetup, string materialSetup, string sizeListSetup,
            string timeSetup, string numberOfRunsSetup, string wastePercentageSetup,
            Dictionary<string, int> detailsSetup)
        {
            NameSetup = nameSetup ?? throw new ArgumentNullException(nameof(nameSetup));
            DirSetup = dirSetup ?? throw new ArgumentNullException(nameof(dirSetup));
            MaterialSetup = materialSetup ?? throw new ArgumentNullException(nameof(materialSetup));
            SizeListSetup = sizeListSetup ?? throw new ArgumentNullException(nameof(sizeListSetup));
            TimeSetup = timeSetup ?? throw new ArgumentNullException(nameof(timeSetup));
            NumberOfRunsSetup = numberOfRunsSetup ?? throw new ArgumentNullException(nameof(numberOfRunsSetup));
            WastePercentageSetup = wastePercentageSetup ?? throw new ArgumentNullException(nameof(wastePercentageSetup));
            DetailsSetup = detailsSetup ?? throw new ArgumentNullException(nameof(detailsSetup));
        }

        public Setup()
        {
        }

        public string NameSetup { get; set; }
        public string DirSetup { get; set; }
        public string MaterialSetup { get; set; }
        public string SizeListSetup { get; set; }
        public string TimeSetup { get; set; }
        public string NumberOfRunsSetup { get; set; }
        public string WastePercentageSetup { get; set; }
        public string WasteSMSetup 
        { 
            get 
            {
                double WasteSM = 0;
                double BusinessWasteSM = 0;
                if (BusinessWasteSetup != null && BusinessWasteSetup != "")
                {
                    BusinessWasteSM = (Convert.ToDouble(BusinessWasteSetup.Replace('х', 'x').Split('x')[0].Trim().Replace('.', ',')) / 10) * (Convert.ToDouble(BusinessWasteSetup.Replace('х', 'x').Split('x')[1].Trim().Replace('.', ',')) / 10);
                }
                if (WastePercentageSetup!= null && SizeListSetup != null)
                {
                    WasteSM = (Convert.ToDouble(SizeListSetup.Replace('х', 'x').Split('x')[0].Trim().Replace('.', ',')) /10) * (Convert.ToDouble(SizeListSetup.Replace('х', 'x').Split('x')[1].Trim().Replace('.', ',')) / 10) 
                        * (Convert.ToDouble(WastePercentageSetup.Trim().Replace('.', ','))/100);
                }
                return (WasteSM - BusinessWasteSM).ToString();
            }

        }
        public string BusinessWasteSetup { get; set; }
        public string DateSpellingSetup { get; set; }
        public string DateRunSetup { get; set; }
        public Dictionary<string, int> DetailsSetup = new Dictionary<string, int>();
        

    }
}
