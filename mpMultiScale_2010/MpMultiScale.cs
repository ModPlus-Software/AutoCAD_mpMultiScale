#if ac2010
using AcApp = Autodesk.AutoCAD.ApplicationServices.Application;
#elif ac2013
using AcApp = Autodesk.AutoCAD.ApplicationServices.Core.Application;
#endif
using System.Collections.Generic;
using mpMsg;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;

namespace mpMultiScale
{
    public class MpMultiScale
    {
        [CommandMethod("ModPlus", "mpMultiScale", CommandFlags.UsePickSet)]
        public void Main()
        {
            var doc = AcApp.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            // Переменная "Копия"
            var isCopy = false;
            try
            {
                // Используем транзикцию
                var tr = db.TransactionManager.StartTransaction();
                using (tr)
                {
                    var opts = new PromptSelectionOptions();
                    opts.Keywords.Add("Копия");
                    var kws = opts.Keywords.GetDisplayString(true);
                    opts.MessageForAdding = "\n" + "Выберите объекты: " + kws;
                    // Implement a callback for when keywords are entered
                    opts.KeywordInput += delegate(object sender, SelectionTextInputEventArgs e)
                    {
                        if (e.Input.Equals("Копия"))
                            isCopy = !isCopy;
                    };
                    var res = ed.GetSelection(opts);
                    if (res.Status != PromptStatus.OK) return;
                    var selSet = res.Value;
                    var _idArray = selSet.GetObjectIds();
                    if (_idArray == null) return;
                    var idArray = isCopy ? SetCopy(_idArray) : _idArray;

                    var doubleOpt = new PromptDoubleOptions("\n" + "Масштаб: ")
                    {
                        AllowNegative = false,
                        AllowNone = false,
                        AllowZero = false
                    };
                    var doubleRes = ed.GetDouble(doubleOpt);
                    if (doubleRes.Status != PromptStatus.OK) return;

                    var pdOpt = new PromptKeywordOptions(string.Empty);
                    var sVal = "Центр"; // Начальное значение
                    pdOpt.AllowArbitraryInput = true;
                    pdOpt.AllowNone = true;
                    pdOpt.SetMessageAndKeywords(
                        "\n" + "Выберите базовую точку: " + "<" + sVal +
                        ">: " + "[Центр/ЛНиз/ЛВерх/ПНиз/ПВерх/СНиз/СВерх/СЛево/СПраво]",
                        "Центр ЛНиз ЛВерх ПНиз ПВерх СНиз СВерх СЛево СПраво");
                    var promptres = ed.GetKeywords(pdOpt);
                    if (promptres.Status != PromptStatus.OK) return;
                    sVal = promptres.StringResult;
                    foreach (var objId in idArray)
                    {
                        var ent = (Entity) tr.GetObject(objId, OpenMode.ForWrite);
                        var extPts = ent.GeometricExtents;
                        var pt1 = extPts.MinPoint;
                        var pt3 = extPts.MaxPoint;
                        var pt = new Point3d();
                        if (sVal.Equals("Центр"))
                            pt = new Point3d((pt1.X + pt3.X)/2, (pt1.Y + pt3.Y)/2, (pt1.Z + pt3.Z)/2);
                        else if (sVal.Equals("ЛНиз"))
                            pt = pt1;
                        else if (sVal.Equals("ЛВерх"))
                            pt = new Point3d(pt1.X, pt3.Y, 0.0);
                        else if (sVal.Equals("ПВерх"))
                            pt = pt3;
                        else if (sVal.Equals("ПНиз"))
                            pt = new Point3d(pt3.X, pt1.Y, 0.0);
                        else if (sVal.Equals("СНиз"))
                            pt = new Point3d((pt1.X + pt3.X)/2, pt1.Y, 0.0);
                        else if (sVal.Equals("СВерх"))
                            pt = new Point3d((pt1.X + pt3.X)/2, pt3.Y, 0.0);
                        else if (sVal.Equals("СЛево"))
                            pt = new Point3d(pt1.X, (pt1.Y + pt3.Y)/2, 0.0);
                        else if (sVal.Equals("СПраво"))
                            pt = new Point3d(pt3.X, (pt1.Y + pt3.Y)/2, 0.0);

                        var mat = Matrix3d.Scaling(doubleRes.Value, pt);
                        ent.TransformBy(mat);
                    }

                    tr.Commit();
                }
            } // try
            catch (Exception ex)
            {
                MpExWin.Show(ex);
            }
        }

        private static ObjectId[] SetCopy(IEnumerable<ObjectId> idArray)
        {
            var list = new List<ObjectId>();
            var doc = AcApp.DocumentManager.MdiActiveDocument;
            var db = doc.Database;

            try
            {
                // Используем транзикцию
                var tr = db.TransactionManager.StartTransaction();
                using (tr)
                {
                    var btr = (BlockTableRecord) tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite, false);
                    foreach (var objId in idArray)
                    {
                        var ent = tr.GetObject(objId, OpenMode.ForWrite).Clone() as Entity;
                        btr.AppendEntity(ent);
                        tr.AddNewlyCreatedDBObject(ent, true);
                        if (ent != null) list.Add(ent.ObjectId);
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                MpExWin.Show(ex);
            }
            return list.ToArray();
        }
    }
}
