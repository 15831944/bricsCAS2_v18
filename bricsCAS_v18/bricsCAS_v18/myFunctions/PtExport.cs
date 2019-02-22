using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using Bricscad.EditorInput;
using Teigha.Geometry;
using CAS.myCAD;
using CAS.myUtilities;
using CAS.myUtilities.myString;

//alias
using _AcAp = Bricscad.ApplicationServices;

namespace CAS.myFunctions
{
    class PtExport
    {
        private readonly string m_Ausgabe = String.Empty;
        private int m_Zähler;
        private readonly Editor m_ed = _AcAp.Application.DocumentManager.MdiActiveDocument.Editor;
        private MyConfig _config = new MyConfig();
        
        readonly myCAD.MyUtilities m_objUtil = new myCAD.MyUtilities();

        public void Start()
        {
            int _digits = _config.GetAppSettingInt("decimals");
            NumberFormatInfo _numbFormat = new NumberFormatInfo()
            {
                NumberDecimalSeparator = _config.GetAppSettingString("Decimal"),
                NumberDecimalDigits = _digits
            };

            SaveFileDialog ddSaveFile = new SaveFileDialog()
            {
                DefaultExt = ".csv",
                Filter = "Punktwolke|*.csv"
            };

            DialogResult diagRes = DialogResult.None;

            if (!Convert.ToBoolean(_config.GetAppSettingBool("useOutputfile")))
                diagRes = ddSaveFile.ShowDialog();
            else
            {
                ddSaveFile.FileName = _config.GetAppSettingString("OutputFile");
                diagRes = DialogResult.OK;
            }

            if (diagRes == DialogResult.OK)
            {
                myCAD.Blöcke.Instance.Init();
                myCAD.Blöcke.Instance.SelectWindow();

                if (Blöcke.Instance.Count > 0)
                {
                    string _Separator = _config.GetAppSettingString("Separator");
                    char _Decimal = Convert.ToChar(_config.GetAppSettingString("Decimal"));
                    try
                    {
                        StreamWriter sw = new StreamWriter(ddSaveFile.FileName, false, Encoding.Default)
                        { NewLine = "\n" };

                        //Header
                        if (Convert.ToBoolean( _config.GetAppSettingBool("useHeader")))
                            sw.WriteLine(_config.GetAppSettingString("Header"));

                        foreach (Messpunkt MP in Blöcke.Instance.LsMP)
                        {
                            //Punktnummer
                            string Zeile = MP.GetAttribute(0).Value + _Separator;
                            Point3d pt3d = new Point3d();

        //                    if (m_Settings.UCScoords)
        //                    {
        //                        //aktuelles BKS setzen
        //                        Matrix3d ucs = m_ed.CurrentUserCoordinateSystem;
        //                        pt3d = MP.Position.TransformBy(ucs.Inverse());
        //                    }
        //                    else
                                pt3d = MP.Pos;

                            //                    if (m_Settings.UCScoords)
                            //                    {
                            //                        switch (m_Settings.AusgabeFormat)
                            //                        {
                            //                            //x,y,z
                            //                            case 0:
                            //                                Zeile += pt3d.X.ToString(m_objUtil.Formatstring(4)).Replace('.', ',') + ";";
                            //                                Zeile += pt3d.Y.ToString(m_objUtil.Formatstring(4)).Replace('.', ',') + ";";
                            //                                Zeile += pt3d.Z.ToString(m_objUtil.Formatstring(4)).Replace('.', ',') + ";";
                            //                                break;

                            //                            //x,z,y
                            //                            case 1:
                            //                                Zeile += pt3d.X.ToString(m_objUtil.Formatstring(4)).Replace('.', ',') + ";";
                            //                                Zeile += pt3d.Z.ToString(m_objUtil.Formatstring(4)).Replace('.', ',') + ";";
                            //                                Zeile += pt3d.Y.ToString(m_objUtil.Formatstring(4)).Replace('.', ',') + ";";
                            //                                break;

                            //                            //x,z,z
                            //                            case 2:
                            //                                Zeile += pt3d.X.ToString(m_objUtil.Formatstring(4)).Replace('.', ',') + ";";
                            //                                Zeile += pt3d.Z.ToString(m_objUtil.Formatstring(4)).Replace('.', ',') + ";";
                            //                                Zeile += pt3d.Z.ToString(m_objUtil.Formatstring(4)).Replace('.', ',') + ";";
                            //                                break;
                            //                        }
                            //                    }
                            //                    else
                            //                    {
                            //                        switch (m_Settings.AusgabeFormat)
                            //                        {
                            //x,y,z
                            //                            case 0:
                            //Rechtswert
                            //Zeile += MP.Pos.X.ToString(m_objUtil.Formatstring(4)).Replace('.', ',') + _Separator;
                            Zeile += MP.Pos.X.ToString("N", _numbFormat) + _Separator;

                            //Hochwert
                            //Zeile += MP.Pos.Y.ToString(m_objUtil.Formatstring(4)).Replace('.', ',') + _Separator;
                            Zeile += MP.Pos.Y.ToString("N", _numbFormat) + _Separator;

                            //Höhe
                            //if (MP.HöheOrg != null)
                            //    //Zeile += MP.HöheOrg.ToString("F3" + 3.ToString()) + _Separator;
                            //    if (!MP.Hdigits.HasValue)
                            //        Zeile += MP.HöheOrg + _Separator;
                            //    else
                            //        //Zeile += MyString.StringValue(MP.HöheOrg, MP.Hdigits.Value) + _Separator;
                            //        Zeile += String.Format(MyString.Formatstring(MP.Hdigits.Value), MP.HöheOrg);
                            //else
                            //    Zeile += _Separator;

                            //Attribute
                            if (MP.AttCount > 1)
                            {
                                for (int i = 1; i < MP.AttCount; i++)
                                {
                                    Zeile += MP.Attribute[i].Value + _Separator;
                                }
                            }

                            m_Zähler += 1;
                            sw.WriteLine(Zeile);
                        }

                        MessageBox.Show(m_Zähler + " Punkte exportiert!");
                        sw.Close();
                    }
                    catch
                    {
                        MessageBox.Show("Ausgabedateiname ungültig!");
                    }
                }
            }        
        }
    }
}
