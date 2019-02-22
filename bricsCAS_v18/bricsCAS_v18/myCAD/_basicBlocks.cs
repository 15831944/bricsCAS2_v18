using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Globalization;
using Bricscad.Runtime;
using Bricscad.EditorInput;
using Teigha.Runtime;
using Teigha.Geometry;
using Teigha.DatabaseServices;
using Bricscad.ApplicationServices;

//alias
using _AcAp = Bricscad.ApplicationServices;
using _AcDb = Teigha.DatabaseServices;
using _AcEd = Bricscad.EditorInput;
using _AcGe = Teigha.Geometry;

using CAS.myUtilities;
using CAS.myUtilities.myString;

namespace CAS.myCAD
{
    public partial class Messpunkt
    {
        private string _Name;
        private string _PNum;
        private Point3d _Pos3d;
        private string _Layer;
        private double[] _scaleFactor;
        private string _HöheOrg;
        private int? _Hdigits = null;
        private BlockReference _blkRef = null;
        private List<Att> _Attributes = new List<Att>();

        //Constructor
        public Messpunkt(string PNum, Point3d Pos, string Höhe)
        {
            _PNum = PNum;

            _HöheOrg = Höhe.Replace(',','.');
            _Pos3d = new Point3d(Pos.X, Pos.Y, Convert.ToDouble(_HöheOrg, CultureInfo.InvariantCulture));
        }

        public Messpunkt(string PNum, Point2d Pos)
        {
            _PNum = PNum;
            _Pos3d = new Point3d(Pos.X, Pos.Y, 0);
        }

        public Messpunkt(string PNum, Point2d Pos, string Höhe)
        {
            _HöheOrg = Höhe.Replace(',', '.');
            _PNum = PNum;
            _Pos3d = new Point3d(Pos.X, Pos.Y, 0);
        }

        public Messpunkt(BlockReference blkRef)
        {
            _blkRef = blkRef;
            _Name = blkRef.Name;
            _Pos3d = blkRef.Position;
            _Layer = blkRef.Layer;
            _scaleFactor = new double[] {blkRef.ScaleFactors.X,
                                         blkRef.ScaleFactors.Y,
                                         blkRef.ScaleFactors.Z};
        }

        public Messpunkt(string PNum, Point3d Pos3d)
        {
            _PNum = PNum;
            _Pos3d = Pos3d;
        }

        //Properties
        /// <summary>
        /// Blocknamen festlegen bzw. abfragen
        /// </summary>
        public string PNum
        {
            get { return this._PNum; }

        }

        public Point3d Pos
        {
            get { return _Pos3d; }
        }

        public string HöheOrg
        {
            get { return _HöheOrg; }
            set { _HöheOrg = value; }
        }

        public int? Hdigits
        {
            get { return _Hdigits; }
        }

        //Attribute
        public List<Att> Attribute
        {
            get { return _Attributes; }
        }

        public int AttCount
        {
            get { return _Attributes.Count; }
        }

        //Methods
        public void AddAttribute(Att att)
        {
            _Attributes.Add(att);
        }

        public Att GetAttribute(int nr)
        {
            Att att = null;
            if (nr <= _Attributes.Count)
                att = _Attributes[nr];

            return att;
        }

        public ErrorStatus RoundHeight(int digits)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            _AcDb.TransactionManager myTm = db.TransactionManager;
            Transaction myT = db.TransactionManager.StartTransaction();
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            ObjectContextManager conTextManager = db.ObjectContextManager;
            ErrorStatus es = ErrorStatus.KeyNotFound;

            if (this.HöheOrg != null)
            {
                if (MyString.Precision(this.HöheOrg) >= digits)
                {
                    try
                    {
                        using (DocumentLock dl = Application.DocumentManager.MdiActiveDocument.LockDocument())
                        {
                            BlockTable bt = (BlockTable)myT.GetObject(db.BlockTableId, OpenMode.ForRead);
                            ObjectId id = bt[BlockTableRecord.ModelSpace];

                            AttributeCollection col = _blkRef.AttributeCollection;

                            foreach (ObjectId attId in col)
                            {
                                AttributeReference attRef = (AttributeReference)myT.GetObject(attId, OpenMode.ForWrite);

                                switch (attRef.Tag)
                                {
                                    case "height":
                                        double Höhe = Convert.ToDouble(this.HöheOrg, CultureInfo.InvariantCulture);
                                        attRef.TextString = Höhe.ToString("F" + digits.ToString());
                                        attRef.Dispose();
                                        _Hdigits = digits;
                                        es = ErrorStatus.OK;
                                        break;
                                }
                            }
                        }
                    }

                    catch { }

                    finally
                    {
                        myT.Commit();
                        myT.Dispose();
                        ed.Regen();
                    }
                }
            }
                return es;
        }

        public void HeightVisible(bool visible)
         {
            Database db = HostApplicationServices.WorkingDatabase;
            _AcDb.TransactionManager myTm = db.TransactionManager;
            Transaction myT = db.TransactionManager.StartTransaction();
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            ObjectContextManager conTextManager = db.ObjectContextManager;

            try
            {
                using (DocumentLock dl = Application.DocumentManager.MdiActiveDocument.LockDocument())
                {
                    BlockTable bt = (BlockTable)myT.GetObject(db.BlockTableId, OpenMode.ForRead);
                    ObjectId id = bt[BlockTableRecord.ModelSpace];

                    AttributeCollection col = _blkRef.AttributeCollection;

                    foreach(ObjectId attId in col)
                    {
                        AttributeReference attRef = (AttributeReference)myT.GetObject(attId, OpenMode.ForWrite);

                        switch(attRef.Tag)
                        {
                            case "height":
                                    attRef.Visible = visible;
                            break;

                            default:
                                break;
                        }
                    }
                }
            }

            catch { }

            finally
            {
                myT.Commit();
                myT.Dispose();
                ed.Regen();
            }
        }

        public void Draw(string block, string Basislayer)
        {
             Database db = HostApplicationServices.WorkingDatabase;
            _AcDb.TransactionManager myTm = db.TransactionManager;
            Transaction myT = db.TransactionManager.StartTransaction();
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            ObjectContextManager conTextManager = db.ObjectContextManager;

            Dictionary<string, Point3d> _attPos = new Dictionary<string, Point3d>();
            List<AttributeDefinition> _attDef = new List<AttributeDefinition>();

            MyLayer objLayer = MyLayer.Instance;
            //objLayer.CheckLayer(Basislayer, true);

            try
            {
                using (DocumentLock dl = Application.DocumentManager.MdiActiveDocument.LockDocument())
                {
                    //Block in Zeichnung einfügen
                    BlockTable bt = (BlockTable)myT.GetObject(db.BlockTableId, OpenMode.ForRead);
                    BlockTableRecord btr = (BlockTableRecord)myT.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                    BlockTableRecord btrDef = (BlockTableRecord)myT.GetObject(bt[block], OpenMode.ForRead);

                    //Attribute aus Blockdefinition übernehmen      
                    if (btrDef.HasAttributeDefinitions)
                    {
                        foreach (ObjectId id in btrDef)
                        {
                            DBObject obj = myT.GetObject(id, OpenMode.ForRead);
                            try
                            {
                                AttributeDefinition ad = (AttributeDefinition)obj;

                                if (ad != null)
                                {
                                    _attPos.Add(ad.Tag, ad.Position);
                                    _attDef.Add(ad);
                                }
                            }
                            catch
                            {
                                try
                                {
                                    Entity ent = (Entity)obj;
                                    //Layer = ent.Layer;
                                }
                                catch { }
                            }
                        }
                    }

                    BlockReference blkRef = new BlockReference(_Pos3d, bt[block])
                    {
                        ScaleFactors = new Scale3d(db.Cannoscale.Scale),
                        Layer = Basislayer
                    };
                    btr.AppendEntity(blkRef);

                    //XData schreiben
                    RegAppTable acRegAppTbl;
                    acRegAppTbl = (RegAppTable)myT.GetObject(db.RegAppTableId, OpenMode.ForRead);

                    if (!acRegAppTbl.Has(Global.Instance.AppName))
                    {
                        using (RegAppTableRecord acRegAppTblRec = new RegAppTableRecord())
                        {
                            acRegAppTblRec.Name = Global.Instance.AppName;

                            acRegAppTbl.UpgradeOpen();
                            acRegAppTbl.Add(acRegAppTblRec);
                            myT.AddNewlyCreatedDBObject(acRegAppTblRec, true);
                        }
                    }

                    using (ResultBuffer rb = new ResultBuffer())
                    {
                        rb.Add(new TypedValue((int)DxfCode.ExtendedDataRegAppName, Global.Instance.AppName));
                        rb.Add(new TypedValue((int)DxfCode.ExtendedDataWorldXCoordinate, _Pos3d));
                        if (_HöheOrg != null)
                            rb.Add(new TypedValue((int)DxfCode.ExtendedDataAsciiString, _HöheOrg));
                        blkRef.XData = rb;
                        rb.Dispose();
                    }

                    myT.AddNewlyCreatedDBObject(blkRef, true);

                    //Attribute befüllen
                    if (_attPos != null)
                    {
                        string[] lsBasislayer = Basislayer.Split('-');
                        string Stammlayer = lsBasislayer[lsBasislayer.Length - 2];

                        for (int i = 0; i < _attPos.Count; i++)
                        {
                            AttributeReference _attRef = new AttributeReference();
                            _attRef.SetDatabaseDefaults();
                            _attRef.SetAttributeFromBlock(_attDef[i], Matrix3d.Identity);
                            _attRef.SetPropertiesFrom(_attDef[i]);

                            Point3d ptBase = new Point3d(blkRef.Position.X + _attRef.Position.X,
                                                         blkRef.Position.Y + _attRef.Position.Y,
                                                         blkRef.Position.Z + _attRef.Position.Z);


                            _attRef.Position = ptBase;

                            string attLayer = String.Empty;

                            KeyValuePair<string, Point3d> keyValuePair = _attPos.ElementAt(i);
                            switch (keyValuePair.Key)
                            {
                                case "number":
                                    _attRef.TextString = _PNum;
                                    _attRef.Layer = Stammlayer + Global.Instance.LayNummer;
                                    break;

                                case "height":
                                    if (_HöheOrg !=null)
                                    {
                                        _attRef.TextString = _HöheOrg;
                                        _attRef.Layer = Stammlayer + Global.Instance.LayHöhe;
                                    }

                                    break;

                                case "date":
                                    _attRef.Layer = Stammlayer + "-Datum";
                                    _attRef.Layer = Stammlayer + Global.Instance.LayDatum;
                                    break;

                                case "code":
                                    _attRef.Layer = Stammlayer + "-Code";
                                    _attRef.Layer = Stammlayer + Global.Instance.LayCode;
                                    break;

                                case "owner":
                                    _attRef.Layer = Stammlayer + "-Hersteller";
                                    break;

                                default:
                                    break;
                            }

                            blkRef.AttributeCollection.AppendAttribute(_attRef);
                            myT.AddNewlyCreatedDBObject(_attRef, true);
                        }
                        blkRef.Dispose();
                    }
                }
            }
            //Block aus Prototypzeichnung holen
            catch { }

            finally
            {
                myT.Commit();              
                myT.Dispose();
            }
        }

        //XData hinzufügen
        static void AddRegAppTableRecord(string regAppName)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = doc.Database;
            Transaction tr = doc.TransactionManager.StartTransaction();

            using (tr)
            {
                RegAppTable rat = (RegAppTable)tr.GetObject(db.RegAppTableId, OpenMode.ForRead, false);
                if (!rat.Has(regAppName))
                {
                    rat.UpgradeOpen();
                    RegAppTableRecord ratr = new RegAppTableRecord {Name = regAppName };
                    rat.Add(ratr);
                    tr.AddNewlyCreatedDBObject(ratr, true);
                }

                tr.Commit();
            }
        }

        public class Att
        {
            private string _Value;
            private Point3d _Pos;
            private string _Layer;

            //Constructor
            public Att() {}

            //Properties
            public string Value
            {
                set { _Value = value; }
                get { return _Value; }
            }

            public Point3d Pos
            {
                set { _Pos = value; }
                get { return _Pos; }
            }

            public string Layer
            {
                set { _Layer = value; }
                get { return Layer; }
            }
        }
    }

    public partial class Blöcke
    {
        private Database m_db = null;
        private Transaction m_myT = null;
        private Editor m_ed = null;

        private List<Messpunkt> ls_MP = new List<Messpunkt>();

        //Constructor (damit kein Default Konstruktor generiert wird!)
        protected Blöcke() { }

        //Properties
        public List<Messpunkt> LsMP
        {
            get { return ls_MP; }
        }

        public int Count
        {
            get { return ls_MP.Count; }
        }

        //Methods
        public void Init()
        {
            //Datenbank initialisieren
            m_db = HostApplicationServices.WorkingDatabase;
            m_myT = m_db.TransactionManager.StartTransaction();
            m_ed = Application.DocumentManager.MdiActiveDocument.Editor;

            //Tabelle wieder löschen
            ls_MP.Clear();
        }

        //Instanz (Singleton)
        public static Blöcke Instance
        {
            get { return BlöckeCreator.CreateInstance; }
        }

        private sealed class BlöckeCreator
        {
            private static readonly Blöcke _Instance = new Blöcke();

            public static Blöcke CreateInstance
            {
                get { return _Instance; }
            }
        }

        public void Dispose()
        {
            try
            {
                m_myT.Commit();
                m_myT.Dispose();
            }
            catch { }
        }

        private void FillTable(SelectionSet ssRes)
        {
            ObjectId[] objID = ssRes.GetObjectIds();

            //SelectionSet iterieren
            foreach (ObjectId blkID in objID)
            {
                using (DocumentLock dl = Application.DocumentManager.MdiActiveDocument.LockDocument())
                {
                    try
                    {
                        m_myT.GetObject(blkID, OpenMode.ForRead);
                        BlockReference blkRef = (BlockReference)m_myT.GetObject(blkID, OpenMode.ForRead);

                        //nur Blöcke mit Attributen berücksichtigen
                        AttributeCollection colAtt = blkRef.AttributeCollection;

                        if (colAtt.Count >= 1)
                        {
                            Messpunkt MP = new Messpunkt(blkRef);

                            //XData lesen
                            RegAppTable acRegAppTbl;
                            acRegAppTbl = (RegAppTable)m_myT.GetObject(m_db.RegAppTableId, OpenMode.ForRead);

                            ResultBuffer rb = new ResultBuffer();
                                rb = blkRef.XData;

                                if (rb == null)
                                    break;

                                foreach(TypedValue tv in rb)
                                {
                                switch (tv.TypeCode)
                                {
                                    case 1000:
                                        try
                                        {
                                            MP.HöheOrg = (string) tv.Value;
                                        }
                                        catch { }

                                        break;
                                }
                            }

                            //Attribute iterieren
                            foreach (ObjectId attID in colAtt)
                            {
                                AttributeReference attRef = (AttributeReference)m_myT.GetObject(attID, OpenMode.ForRead);

                                Messpunkt.Att att = new Messpunkt.Att()
                                {
                                    Value = attRef.TextString,
                                    Pos = attRef.Position,
                                    Layer = attRef.Layer
                                };

                                MP.AddAttribute(att);

                                attRef.Dispose();
                            }

                            LsMP.Add(MP);
                        }
                    }
                    catch { }
                }
            }

          //  m_myT.Commit();

        }   //fillTable
    }
}
