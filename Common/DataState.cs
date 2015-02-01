using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitMe.Common
{
    public static class DataState
    {
        #region internals

        private static bool InitialDataFetchSuccess = false;
        private static double InitialDataFetchSpanSecs = 1.0 * 30.0 * 24.0 * 60.0 * 60.0;
        private static double DataFetchSpanSecs = 30.0;
        private static double DataFetchRetrySpanSecs = 5.0;

        private static bool InitalDataSaveSuccess = false;
        private static double InitialDataSaveSpanSecs = 1.0 * 30.0 * 24.0 * 60.0 * 60.0;
        private static double DataSaveSpanSecs = 60.0;
        private static double DataSaveRetrySpanSecs = 5.0;

        private static bool InitialDataLoadSuccess = false;
        private static double InitialDataLoadSpanSecs = 1.0 * 30.0 * 24.0 * 60.0 * 60.0;
        //private static double DataLoadSpanSecs = 60.0;
        private static double DataLoadRetrySpanSecs = 5.0;

        private static double TimeSpanEpsilon = 2.0 * 60.0;
        private static double TimeSpanDelta = InitialDataLoadSpanSecs - TimeSpanEpsilon;

        private static DateTime _lastDataFetchAt = DateTime.UtcNow.AddSeconds(-1.0 * InitialDataFetchSpanSecs);
        private static bool _lastDataFetchSuccess = InitialDataFetchSuccess;

        private static DateTime _lastDataSaveAt = DateTime.UtcNow.AddSeconds(-1.0 * InitialDataSaveSpanSecs);
        private static bool _lastDataSaveSuccess = InitalDataSaveSuccess;

        private static DateTime _lastDataLoadAt = DateTime.UtcNow.AddSeconds(-1.0 * InitialDataLoadSpanSecs);
        private static bool _lastDataLoadSuccess = InitialDataLoadSuccess;

        #endregion

        public static DateTime LastDataFetchAt { set { _lastDataFetchAt = value; } }
        public static bool LastDataFetchSuccess { set { _lastDataFetchSuccess = value; } }

        public static DateTime LastDataSaveAt { set { _lastDataSaveAt = value; } }
        public static bool LastDataSaveSuccess { set { _lastDataSaveSuccess = value; } }

        public static DateTime LastDataLoadAt { set { _lastDataLoadAt = value; } }
        public static bool LastDataLoadSuccess { set { _lastDataLoadSuccess = value; } }

        #region initial_states

        public static bool IsInitialFetch()
        {
            var diff = DateTime.UtcNow - _lastDataFetchAt;
            if (diff.TotalSeconds > TimeSpanDelta)
            {
                return true;
            }
            else
                return false;
        }

        public static bool IsInitialSave()
        {
            var diff = DateTime.UtcNow - _lastDataSaveAt;
            if (diff.TotalSeconds > TimeSpanDelta)
            {
                return true;
            }
            else
                return false;
        }

        public static bool IsInitialLoad()
        {
            var diff = DateTime.UtcNow - _lastDataLoadAt;
            if (diff.TotalSeconds > TimeSpanDelta)
            {
                return true;
            }
            else
                return false;
        }

        #endregion

        #region retry_states

        public static bool IsDataFetchRetry()
        {
            if (IsInitialFetch())
                return false;

            var diff = (DateTime.UtcNow - _lastDataFetchAt).TotalSeconds;
            if (!_lastDataFetchSuccess && diff > DataFetchRetrySpanSecs)
                return true;
            else
                return false;
        }

        public static bool IsDataSaveRetry()
        {
            if (IsInitialSave())
                return false;

            var diff = (DateTime.UtcNow - _lastDataFetchAt).TotalSeconds;
            if (!_lastDataSaveSuccess && diff > DataSaveRetrySpanSecs)
                return true;
            else
                return false;
        }

        public static bool IsDataLoadRetry()
        {
            if (IsInitialLoad())
                return false;

            var diff = (DateTime.UtcNow - _lastDataLoadAt).TotalSeconds;
            if (!_lastDataLoadSuccess && diff > DataLoadRetrySpanSecs)
                return true;
            else
                return false;
        }

        #endregion

        #region due_states

        public static bool IsDataFetchDue()
        {
            if (IsInitialFetch())
                return true;

            if (IsDataFetchRetry())
                return true;

            var diff = DateTime.UtcNow - _lastDataFetchAt;
            if (diff.TotalSeconds > DataFetchSpanSecs)
                return true;
            else
                return false;
        }

        public static bool IsDataSaveDue()
        {
            if (IsInitialSave() && _lastDataFetchSuccess )
                return true;

            if (IsDataSaveRetry())
                return true;

            var now = DateTime.UtcNow;
            var diffSave = (now - _lastDataSaveAt).TotalSeconds;
            var diffFetch = (now - _lastDataFetchAt).TotalSeconds;
            if (diffSave > diffFetch && diffSave > DataSaveSpanSecs)
                return true;
            
            return false;
        }

        public static bool IsDataLoadDue()
        {
            if (IsInitialLoad())
                return true;

            if (IsDataLoadRetry())
                return true;

            var now = DateTime.UtcNow;
            var diffSave = (now - _lastDataSaveAt).TotalSeconds;
            var diffLoad = (now - _lastDataLoadAt).TotalSeconds;
            var diffFetch = (now - _lastDataFetchAt).TotalSeconds;
            if (diffSave > diffFetch && diffSave > DataSaveSpanSecs)
                return true;

            return false;
        }

        #endregion

        #region enforced_states

        public static bool IsDataSaveEnforced()
        {
            if (!_lastDataFetchSuccess)
                return false;
            var diff = (_lastDataSaveAt - _lastDataFetchAt).TotalSeconds;
            if ( diff > 0.0 && _lastDataSaveSuccess)    
                return false;
            return true;
        }

        #endregion

    }
}
