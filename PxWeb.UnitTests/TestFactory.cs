using Value = PCAxis.Paxiom.Value;

namespace PxWeb.UnitTests
{
    [TestClass]
    public class TestFactory
    {
        public Dictionary<string, ItemSelection> GetMenuLookupFolders()
        {
            var dict = new Dictionary<string, ItemSelection>();

            dict.Add("AA0003", new ItemSelection("AA", "AA0003"));
            dict.Add("AA0005", new ItemSelection("AA", "AA0005"));
            dict.Add("AA0003B", new ItemSelection("AA0003", "AA0003B"));
            return dict;
        }


        public Dictionary<string, ItemSelection> GetMenuLookupTables()
        {
            var dict = new Dictionary<string, ItemSelection>();

            dict.Add("AA0003", new ItemSelection("AA", "AA0003"));
            dict.Add("AA0005", new ItemSelection("AA", "AA0005"));
            dict.Add("AA0003B", new ItemSelection("AA0003", "AA0003B"));
            return dict;
        }

        //public Dictionary<string, string> GetMenuLookup()
        //{
        //    var dict = new Dictionary<string, string>();
        //    dict.Add("AA0003", "AA");
        //    dict.Add("AA0005", "AA");
        //    dict.Add("AA0003B", "AA0003");
        //    dict.Add("AA0003C", "AA0003");
        //    dict.Add("AA0003D", "AA0003");
        //    dict.Add("AA0003E", "AA0003");
        //    dict.Add("AA0003F", "AA0003");
        //    dict.Add("AA0003G", "AA0003");
        //    dict.Add("AA0003H", "AA0003");
        //    dict.Add("AA0003I", "AA0003");
        //    dict.Add("AA0003J", "AA0003");
        //    dict.Add("INTGR1KOMKONUTB", "AA0003B");
        //    dict.Add("INTGR1LANKONUTB", "AA0003B");
        //    dict.Add("INTGR1LUAKON", "AA0003B");
        //    dict.Add("INTGR1RIKKONUTB", "AA0003B");
        //    dict.Add("INTGR1URBKON", "AA0003B");
        //    dict.Add("KOMMOTFORV", "AA0003B");
        //    dict.Add("KOMMOTFORVANDEL", "AA0003B");
        //    dict.Add("MOTFLYKTALDKON", "AA0003B");
        //    dict.Add("MOTFLYKTJOBBKON", "AA0003B");
        //    dict.Add("MOTFLYKTLANKON", "AA0003B");
        //    dict.Add("MOTFLYKTUTBKON", "AA0003B");
        //    dict.Add("NYTABTESTMOTFLYKTLAN", "AA0003B");
        //    dict.Add("VBMOTFLYKTLANKON", "AA0003B");
        //    dict.Add("VBTST2MOTFLYKTLANKON", "AA0003B");
        //    dict.Add("INTGR2KOM", "AA0003C");
        //    dict.Add("INTGR2LAN", "AA0003C");
        //    dict.Add("INTGR2LUA", "AA0003C");
        //    dict.Add("INTGR2URB", "AA0003C");
        //    dict.Add("INTGR6KOM", "AA0003D");
        //    dict.Add("INTGR6LANKON", "AA0003D");
        //    dict.Add("INTGR6LUA", "AA0003D");
        //    dict.Add("INTGR6RIKKON", "AA0003D");
        //    dict.Add("INTGR6URB", "AA0003D");
        //    dict.Add("INTGR7KOM", "AA0003D");
        //    dict.Add("FOLKMANGDURBLUA", "AA0003E");
        //    dict.Add("INTGR3KOMS", "AA0003E");
        //    dict.Add("INTGR3KOMU", "AA0003E");
        //    dict.Add("INTGR3LANKONS", "AA0003E");
        //    dict.Add("INTGR3LANKONU", "AA0003E");
        //    dict.Add("INTGR3LUAS", "AA0003E");
        //    dict.Add("INTGR3LUAU", "AA0003E");
        //    dict.Add("INTGR3RIKKONS", "AA0003E");
        //    dict.Add("INTGR3RIKKONU", "AA0003E");
        //    dict.Add("INTGR3URBS", "AA0003E");
        //    dict.Add("INTGR3URBU", "AA0003E");
        //    dict.Add("KOMMOTANTAL", "AA0003E");
        //    dict.Add("INTGR5KOM", "AA0003F");
        //    dict.Add("INTGR5LANKON", "AA0003F");
        //    dict.Add("INTGR5LUA", "AA0003F");

        //    return dict;
        //}

        public PxApiConfigurationOptions GetPxApiConfiguration()
        {
            PxApiConfigurationOptions pxApiConfigurationOptions = new PxApiConfigurationOptions();

            pxApiConfigurationOptions.MaxDataCells = 100000;

            pxApiConfigurationOptions.Languages = new List<Config.Api2.Language>();
            pxApiConfigurationOptions.Languages.Add(new Config.Api2.Language() { Id = "en", Label = "English" });

            return pxApiConfigurationOptions;
        }


        public static PXModel GetPxModel()
        {
            PXModel pxModel = new PXModel();

            Variable timeVar = new Variable("Period", PlacementType.Heading);
            timeVar.IsTime = true;

            timeVar.Values.Add(new Value("2018M01"));
            timeVar.Values.Add(new Value("2018M02"));
            timeVar.Values.Add(new Value("2018M03"));
            timeVar.Values.Add(new Value("2018M04"));
            timeVar.Values.Add(new Value("2018M05"));
            timeVar.Values.Add(new Value("2018M06"));
            timeVar.Values.Add(new Value("2018M07"));
            timeVar.Values.Add(new Value("2018M08"));
            timeVar.Values.Add(new Value("2018M09"));
            timeVar.Values.Add(new Value("2018M10"));
            timeVar.Values.Add(new Value("2018M11"));
            timeVar.Values.Add(new Value("2018M12"));

            pxModel.Meta.AddVariable(timeVar);

            return pxModel;
        }

        public static PXModel GetMinimalModel()
        {
            PXModel pxModel = new PXModel();

            Variable timeVar = new Variable("Period", PlacementType.Heading);
            timeVar.IsTime = true;
            timeVar.Values.Add(new Value("2018M01"));

            pxModel.Meta.AddVariable(timeVar);


            Variable regionVar = new Variable("Region", PlacementType.Stub);
            regionVar.IsTime = false;
            regionVar.Values.Add(new Value("A"));
            regionVar.Values.Add(new Value("B"));

            pxModel.Meta.AddVariable(regionVar);


            pxModel.Meta.AxisVersion = "2018";
            pxModel.Meta.Language = "en";
            pxModel.Meta.SubjectArea = "TST";
            pxModel.Meta.SubjectCode = "TST";
            pxModel.Meta.Matrix = "TST01";
            pxModel.Meta.Title = "Test data";
            pxModel.Meta.Source = "PxTools";
            pxModel.Meta.Contents = "Test data";
            pxModel.Meta.Decimals = 0;
            pxModel.Meta.Description = "Test file";
            pxModel.Meta.DescriptionDefault = false;
            var contentInfo = new ContInfo();
            contentInfo.Units = "Amount";

            pxModel.Meta.ContentInfo = contentInfo;
            pxModel.IsComplete = true;

            pxModel.Data.SetMatrixSize(1, 1);

            pxModel.Data.WriteElement(0, 100);

            return pxModel;
        }
    }
}
