using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bricscad.Runtime;
using Teigha.Runtime;
using _AcAp = Bricscad.ApplicationServices;
using _AcEd = Bricscad.EditorInput;
using _AcRx = Teigha.Runtime;
using _AcGe = Teigha.Geometry;
using _AcGi = Teigha.GraphicsInterface;
using _AcWnd = Bricscad.Windows;

[assembly: CommandClass(typeof(CAS.Commands))]
[assembly: ExtensionApplication(typeof(CAS.Commands))]

namespace CAS
{
    public class Commands : IExtensionApplication
    {
        public Commands() { }


        public void Initialize()
        {
            _AcEd.Editor ed = _AcAp.Application.DocumentManager.MdiActiveDocument.Editor;
            CAS.myFunctions.DiaSettings objSettings = new myFunctions.DiaSettings();

            ed.WriteMessage("CAS installed");
        }

        public void Terminate() { }

        //pt import
        [CommandMethod("cas_ptImport")]
        public void CasPtImport()
        {
            myFunctions.PtImport objPtImport = new myFunctions.PtImport();
            objPtImport.Start();
        }

        //pt export
        [CommandMethod("cas_ptExport")]
        public void CasPtExport()
        {
            myFunctions.PtExport objPtExport = new myFunctions.PtExport();
            objPtExport.Start();
        }

        //att Höhe unsichtbar schalten
        [CommandMethod("cas_Hoff")]
        public void CasAtInvisible()
        {
            myCAD.Blöcke.Instance.Init();
            myCAD.Blöcke.Instance.SelectWindow();
            myCAD.Blöcke.Instance.SwitchAtt(myCAD.Blöcke.Mode.HeightOff);
        }

        //att Höhe sichtbar schalten
        [CommandMethod("cas_Hon")]
        public void CasAtVisible()
        {
            myCAD.Blöcke.Instance.Init();
            myCAD.Blöcke.Instance.SelectWindow();
            myCAD.Blöcke.Instance.SwitchAtt(myCAD.Blöcke.Mode.HeightOn);
        }

        //Punkte setzen
        [CommandMethod("cas_setPoint")]
        public void CasSetPoint()
        {
            myFunctions.SetPoint objSetPoint = new myFunctions.SetPoint();
            objSetPoint.Start();
        }

        //Höhen runden
        [CommandMethod("cas_roundHeight")]
        public void CasRoundHeight()
        {
            myFunctions.RoundHeight objRoundHeight = new myFunctions.RoundHeight();
            objRoundHeight.Start();
        }

        //show Settings
        [CommandMethod("cas_Settings")]
        public void CasSettings()
        {
            CAS.myFunctions.DiaSettings objSettings = new myFunctions.DiaSettings();
            objSettings.ShowDialog();
        }

        //shows about box
        [CommandMethod("cas_About")]
        public void CasAbout()
        {
            CASAboutBox objAboutBox = new CASAboutBox();
            objAboutBox.ShowDialog();
        }

        //register CAS
        [CommandMethod("regcas")]
        public void RegisterCas()
        {
            myRegistry.RegApp.Register();
        }

        //unregister CAS
        [CommandMethod("unregcas")]
        public void UnregisterCas()
        {
            myRegistry.RegApp.Unregister();
        }
    }
}
