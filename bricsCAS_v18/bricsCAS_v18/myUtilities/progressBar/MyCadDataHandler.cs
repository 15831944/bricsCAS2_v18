using System;
using System.Collections.Generic;
using System.Linq;
using Bricscad.ApplicationServices;
using Teigha.DatabaseServices;
using Teigha.Geometry;

//alias
using _AcAp = Bricscad.ApplicationServices;
using _AcDb = Teigha.DatabaseServices;

namespace ShowProgressBar
{
    public class MyCadDataHandler : ILongProcessingObject
    {
        private enum LongProcessingType
        {
            Type1=0,
            Type2=1,
        }

        private Document _dwg;
        private LongProcessingType _processingType = LongProcessingType.Type1;

        public MyCadDataHandler(Document dwg)
        {
            _dwg = dwg;
        }

        #region public methods

        public void DoWorkWithKnownLoopCount()
        {
            _processingType = LongProcessingType.Type1;

            using (var progress=new ProcessingProgressBar(this))
            {
                progress.Start();
            }
        }

        public void DoWorkWithUnknownLoopCount()
        {
            _processingType = LongProcessingType.Type2;

            using (var progress = new ProcessingProgressBar(this))
            {
                progress.Start();
            }
        }

        #endregion

        #region Implementing ILongProcessingObject interface

        public event LongProcessStarted ProcessingStarted;

        public event LongProcessingProgressed ProcessingProgressed;

        public event EventHandler ProcessingEnded;

        public event EventHandler CloseProgressUIRequested;

        public void DoLongProcessingWork()
        {
            switch(_processingType)
            {
                case LongProcessingType.Type1:
                    LoopThroughModelSpace();
                    break;
                case LongProcessingType.Type2:
                    SearchForTopBlocks("StationLabel", 500);
                    break;
            }
        }

        #endregion

        #region private methods

        private void LoopThroughModelSpace()
        {
            try
            {
                //run 2 long processing loops
                for (int n = 0; n < 2; n++)
                {
                    using (var tran = 
                        _dwg.TransactionManager.StartTransaction())
                    {
                        //Get all entities' ID in ModelSpace
                        BlockTableRecord model = (BlockTableRecord)
                            tran.GetObject(
                            SymbolUtilityServices.GetBlockModelSpaceId(
                            _dwg.Database), OpenMode.ForRead);

                        ObjectId[] entIds = model.Cast<ObjectId>().ToArray();

                        if (ProcessingStarted != null)
                        {
                            string process = n == 0 ? 
                                "Searching ModelSpace for AAAA" : 
                                "Search ModelSpace for BBBB";
                            LongProcessStartedEventArgs e =
                                new LongProcessStartedEventArgs(
                                    process, entIds.Length, true);

                            ProcessingStarted(this, e);
                        }

                        int count = 0;
                        foreach (var entId in entIds)
                        {
                            count++;

                            if (ProcessingProgressed != null)
                            {
                                string progMsg = string.Format(
                                    "{0} out of {1}. {2} remaining...\n" +
                                    "Processing entity: {3}",
                                    count, 
                                    entIds.Length, 
                                    entIds.Length-count, 
                                    entId.ObjectClass.DxfName);

                                LongProcessingProgressEventArgs e =
                                    new LongProcessingProgressEventArgs(progMsg);
                                ProcessingProgressed(this, e);

                                //Since this processing is cancellable, we
                                //test if user clicked the "Stop" button in the 
                                //progressing dialog box
                                if (e.Cancel) break;
                            }

                            //Do something with the entity
                            Entity ent = (Entity)tran.GetObject(
                                entId, OpenMode.ForRead);
                            long s = 0;
                            for (int i = 0; i < 1000000; i++)
                            {
                                s += i * i;
                            }
                            
                        }

                        ProcessingEnded?.Invoke(this, EventArgs.Empty);
                        tran.Commit();
                    }
                }
            }
            finally
            {
                //Make sure the CloseProgressUIRequested event always fires, so
                //that the progress dialog box gets closed because of this event
                CloseProgressUIRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        private void SearchForTopBlocks(string blkName, int targetCount)
        {
            List<ObjectId> blkIds = new List<ObjectId>();

            try
            {
                if (ProcessingStarted != null)
                {
                    string msg = string.Format(
                        "Searching first {0} block refeences: \"{1}\"",
                        targetCount, blkName);
                    LongProcessStartedEventArgs e =
                        new LongProcessStartedEventArgs(msg);

                    ProcessingStarted(this, e);
                }

                using (var tran = _dwg.TransactionManager.StartTransaction())
                {
                    //Get all entities' ID in ModelSpace
                    BlockTableRecord model = (BlockTableRecord)tran.GetObject(
                        SymbolUtilityServices.GetBlockModelSpaceId(
                        _dwg.Database), OpenMode.ForRead);

                    foreach (ObjectId id in model)
                    {
                        if (ProcessingProgressed != null)
                        {
                            string progMsg = string.Format(
                                "{0} found\n" +
                                "Processing entity: {1}",
                                blkIds.Count, id.ObjectClass.DxfName);
                            LongProcessingProgressEventArgs e =
                                new LongProcessingProgressEventArgs(progMsg);
                            ProcessingProgressed(this, e);
                        }

                        if (IsTargetBlock(id, blkName, tran))
                        {
                            blkIds.Add(id);
                            if (blkIds.Count == targetCount) break;
                        }
                    }

                    tran.Commit();
                }

                ProcessingEnded?.Invoke(this, EventArgs.Empty);
            }
            finally
            {
                //Make sure the CloseProgressUIRequested event always fires,
                //so that the progress dialog box gets closed because of 
                //this event
                CloseProgressUIRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        private bool IsTargetBlock(
            ObjectId entId, string blkName, Transaction tran)
        {
            //kill a bit time to allow progress bar effect
            long s = 0;
            for (int i = 0; i < 10000000; i++)
            {
                s += i * i;
            }

            BlockReference blk = tran.GetObject(
                entId, OpenMode.ForRead) as BlockReference;
            if (blk!=null)
            {
                string name;
                if (blk.IsDynamicBlock)
                {
                    BlockTableRecord br = (BlockTableRecord)
                        tran.GetObject(
                        blk.DynamicBlockTableRecord, OpenMode.ForRead);
                    name = br.Name;
                }
                else
                {
                    name = blk.Name;
                }

                return name.ToUpper() == blkName.ToUpper();
            }

            return false;
        }

        #endregion
    }
}
