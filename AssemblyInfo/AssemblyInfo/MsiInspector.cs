using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace AssemblyInfo
{
    public class MsiInspector
    {
        [DllImport("msi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern uint MsiOpenDatabase(string szDatabasePath, IntPtr phPersist, out IntPtr phDatabase);

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        static extern int MsiDatabaseOpenViewW(IntPtr hDatabase, [MarshalAs(UnmanagedType.LPWStr)] string szQuery, out IntPtr phView);

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        static extern int MsiViewExecute(IntPtr hView, IntPtr hRecord);

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        static extern uint MsiViewFetch(IntPtr hView, out IntPtr hRecord);

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        static extern int MsiRecordGetString(IntPtr hRecord, int iField, [Out] StringBuilder szValueBuf, ref int pcchValueBuf);

        [DllImport("msi.dll", ExactSpelling = true)]
        static extern IntPtr MsiCreateRecord(uint cParams);

        [DllImport("msi.dll", ExactSpelling = true)]
        static extern uint MsiCloseHandle(IntPtr hAny);

        internal const uint ERROR_SUCCESS = 0;
        internal const uint ERROR_FILE_NOT_FOUND = 2;
        internal const uint ERROR_INSUFFICIENT_BUFFER = 122;
        internal const uint ERROR_MORE_DATA = 234;
        internal const uint ERROR_NO_MORE_ITEMS = 259;

        public List<Dictionary<int, string>> GetAllProperties(string fileName, string table)
        {
            Debug.WriteLine($"-------------------\r\n{table}\r\n\r\n");
            var sqlStatement = $"SELECT * FROM {table}";
            var phDatabase = IntPtr.Zero;
            var phView = IntPtr.Zero;
            var hRecord = IntPtr.Zero;
            uint uRetCode;

            var bufferSize = 255;

            // Open the MSI database in the input file 
            var val = MsiOpenDatabase(fileName, IntPtr.Zero, out phDatabase);

            hRecord = MsiCreateRecord(1);

            // Open a view on the Property table for the version property 
            var viewVal = MsiDatabaseOpenViewW(phDatabase, sqlStatement, out phView);

            // Execute the view query 
            var exeVal = MsiViewExecute(phView, hRecord);

            // Get the record from the view 
            var retVal = 0;
            uint result = 0;
            var rows = new List<Dictionary<int, string>>();
            while (result == ERROR_SUCCESS)
            {
                var records = new Dictionary<int, string>();
                var bytesRead = bufferSize;
                result = MsiViewFetch(phView, out hRecord);
                if (result == ERROR_NO_MORE_ITEMS)
                    break;

                // Get the version from the data
                var recordNumber = 0;
                while (bytesRead > 0)
                {
                    retVal = (int)ERROR_MORE_DATA;
                    //bytesRead = 255;
                    var sb = new StringBuilder(bytesRead);
                    retVal = MsiRecordGetString(hRecord, recordNumber, sb, ref bytesRead);
                    if (bytesRead > 0)
                    {
                        if (retVal == ERROR_MORE_DATA)
                        {
                            bytesRead++;
                            sb = new StringBuilder(bytesRead, bytesRead);
                            retVal = MsiRecordGetString(hRecord, recordNumber, sb, ref bytesRead);
                        }
                        records.Add(recordNumber, sb.ToString());
                        recordNumber++;
                    }
                }
                Debug.WriteLine($"{string.Join(" ", records.Values)}");
                rows.Add(records);
            }
            uRetCode = MsiCloseHandle(phDatabase);
            uRetCode = MsiCloseHandle(phView);
            uRetCode = MsiCloseHandle(hRecord);

            return rows;
        }

        private string GetMsiProperty(string fileName, string propertyName)
        {
            var sqlStatement = $"SELECT * FROM Property WHERE Property = '{propertyName}'";
            var phDatabase = IntPtr.Zero;
            var phView = IntPtr.Zero;
            var hRecord = IntPtr.Zero;
            uint uRetCode;

            StringBuilder szValueBuf = new StringBuilder();
            var pcchValueBuf = 4096;

            // Open the MSI database in the input file 
            var val = MsiOpenDatabase(fileName, IntPtr.Zero, out phDatabase);

            hRecord = MsiCreateRecord(1);

            // Open a view on the Property table for the version property 
            var viewVal = MsiDatabaseOpenViewW(phDatabase, sqlStatement, out phView);

            // Execute the view query 
            var exeVal = MsiViewExecute(phView, hRecord);

            // Get the record from the view 
            var fetchVal = MsiViewFetch(phView, out hRecord);

            // Get the version from the data 
            var retVal = MsiRecordGetString(hRecord, 2, szValueBuf, ref pcchValueBuf);

            uRetCode = MsiCloseHandle(phDatabase);
            uRetCode = MsiCloseHandle(phView);
            uRetCode = MsiCloseHandle(hRecord);

            return szValueBuf.ToString();
        }
    }
}
