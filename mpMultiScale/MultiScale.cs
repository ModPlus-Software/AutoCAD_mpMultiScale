namespace mpMultiScale
{
    using System.Collections.Generic;
    using Autodesk.AutoCAD.DatabaseServices;
    using Autodesk.AutoCAD.EditorInput;
    using Autodesk.AutoCAD.Geometry;
    using Autodesk.AutoCAD.Runtime;
    using ModPlusAPI;
    using ModPlusAPI.Windows;
    using AcApp = Autodesk.AutoCAD.ApplicationServices.Core.Application;

    public class MultiScale
    {
        private const string LangItem = "mpMultiScale";

        [CommandMethod("ModPlus", "mpMultiScale", CommandFlags.UsePickSet)]
        public void Main()
        {
            Statistic.SendCommandStarting(new ModPlusConnector());

            var doc = AcApp.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;

            // Переменная "Копия"
            var isCopy = false;
            try
            {
                // Используем транзакцию
                var tr = db.TransactionManager.StartTransaction();
                using (tr)
                {
                    var opts = new PromptSelectionOptions();
                    opts.Keywords.Add(Language.GetItem(LangItem, "msg1"));
                    var kws = opts.Keywords.GetDisplayString(true);
                    opts.MessageForAdding = "\n" + Language.GetItem(LangItem, "msg2") + kws;

                    // Implement a callback for when keywords are entered
                    opts.KeywordInput += (sender, e) =>
                    {
                        if (e.Input.Equals(Language.GetItem(LangItem, "msg1")))
                            isCopy = !isCopy;
                    };
                    var res = ed.GetSelection(opts);
                    if (res.Status != PromptStatus.OK)
                        return;
                    var selSet = res.Value;
                    var idArr = selSet.GetObjectIds();
                    if (idArr == null)
                        return;
                    var idArray = isCopy ? SetCopy(idArr) : idArr;

                    var doubleOpt = new PromptDoubleOptions("\n" + Language.GetItem(LangItem, "msg3"))
                    {
                        AllowNegative = false,
                        AllowNone = false,
                        AllowZero = false
                    };
                    var doubleRes = ed.GetDouble(doubleOpt);
                    if (doubleRes.Status != PromptStatus.OK)
                        return;

                    var pdOpt = new PromptKeywordOptions(string.Empty);
                    var sVal = Language.GetItem(LangItem, "kw1"); // Начальное значение
                    pdOpt.AllowArbitraryInput = true;
                    pdOpt.AllowNone = true;

                    // pdOpt.SetMessageAndKeywords(
                    //    "\n" + "Выберите базовую точку: " + "<" + sVal +
                    //    ">: " + "[Центр/ЛНиз/ЛВерх/ПНиз/ПВерх/СНиз/СВерх/СЛево/СПраво]",
                    //    "Центр ЛНиз ЛВерх ПНиз ПВерх СНиз СВерх СЛево СПраво");

                    pdOpt.SetMessageAndKeywords(
                        "\n" + Language.GetItem(LangItem, "msg4") + "<" + sVal +
                        ">: " + "[" +
                        Language.GetItem(LangItem, "kw1") + "/" +
                        Language.GetItem(LangItem, "kw2") + "/" +
                        Language.GetItem(LangItem, "kw3") + "/" +
                        Language.GetItem(LangItem, "kw5") + "/" +
                        Language.GetItem(LangItem, "kw4") + "/" +
                        Language.GetItem(LangItem, "kw6") + "/" +
                        Language.GetItem(LangItem, "kw7") + "/" +
                        Language.GetItem(LangItem, "kw8") + "/" +
                        Language.GetItem(LangItem, "kw9") + "]",
                        Language.GetItem(LangItem, "kw1") + " " +
                        Language.GetItem(LangItem, "kw2") + " " +
                        Language.GetItem(LangItem, "kw3") + " " +
                        Language.GetItem(LangItem, "kw5") + " " +
                        Language.GetItem(LangItem, "kw4") + " " +
                        Language.GetItem(LangItem, "kw6") + " " +
                        Language.GetItem(LangItem, "kw7") + " " +
                        Language.GetItem(LangItem, "kw8") + " " +
                        Language.GetItem(LangItem, "kw9"));
                    var promptResult = ed.GetKeywords(pdOpt);
                    if (promptResult.Status != PromptStatus.OK)
                        return;
                    sVal = promptResult.StringResult;
                    foreach (var objId in idArray)
                    {
                        var ent = (Entity)tr.GetObject(objId, OpenMode.ForWrite);
                        var extPts = ent.GeometricExtents;
                        var pt1 = extPts.MinPoint;
                        var pt3 = extPts.MaxPoint;
                        var pt = default(Point3d);
                        if (sVal.Equals(Language.GetItem(LangItem, "kw1")))
                            pt = new Point3d((pt1.X + pt3.X) / 2, (pt1.Y + pt3.Y) / 2, (pt1.Z + pt3.Z) / 2);
                        else if (sVal.Equals(Language.GetItem(LangItem, "kw2")))
                            pt = pt1;
                        else if (sVal.Equals(Language.GetItem(LangItem, "kw3")))
                            pt = new Point3d(pt1.X, pt3.Y, 0.0);
                        else if (sVal.Equals(Language.GetItem(LangItem, "kw4")))
                            pt = pt3;
                        else if (sVal.Equals(Language.GetItem(LangItem, "kw5")))
                            pt = new Point3d(pt3.X, pt1.Y, 0.0);
                        else if (sVal.Equals(Language.GetItem(LangItem, "kw6")))
                            pt = new Point3d((pt1.X + pt3.X) / 2, pt1.Y, 0.0);
                        else if (sVal.Equals(Language.GetItem(LangItem, "kw7")))
                            pt = new Point3d((pt1.X + pt3.X) / 2, pt3.Y, 0.0);
                        else if (sVal.Equals(Language.GetItem(LangItem, "kw8")))
                            pt = new Point3d(pt1.X, (pt1.Y + pt3.Y) / 2, 0.0);
                        else if (sVal.Equals(Language.GetItem(LangItem, "kw9")))
                            pt = new Point3d(pt3.X, (pt1.Y + pt3.Y) / 2, 0.0);

                        var mat = Matrix3d.Scaling(doubleRes.Value, pt);
                        ent.TransformBy(mat);
                    }

                    tr.Commit();
                }
            }
            catch (Exception ex)
            {
                ExceptionBox.Show(ex);
            }
        }

        private static ObjectId[] SetCopy(IEnumerable<ObjectId> idArray)
        {
            var list = new List<ObjectId>();
            var doc = AcApp.DocumentManager.MdiActiveDocument;
            var db = doc.Database;

            try
            {
                // Используем транзакцию
                var tr = db.TransactionManager.StartTransaction();
                using (tr)
                {
                    var btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite, false);
                    foreach (var objId in idArray)
                    {
                        var ent = tr.GetObject(objId, OpenMode.ForWrite).Clone() as Entity;
                        btr.AppendEntity(ent);
                        tr.AddNewlyCreatedDBObject(ent, true);
                        if (ent != null)
                            list.Add(ent.ObjectId);
                    }

                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                ExceptionBox.Show(ex);
            }

            return list.ToArray();
        }
    }
}
