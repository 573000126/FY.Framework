// Copyright (C) 2013 fanyong All rights reserved.	
// Created Date:	2013-04-10
// Modified Date	2013-04-11
// File: 	        ExcelHelper.cs
// Author:	       fanyong@gmail.com
// Version:         V1.0
// Description:   Excel helper
// 注意： 需要添加   1) Microsoft.CSharp.dll   2) Microsoft.Office.Interop.Excel.dll

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.IO;
using Microsoft.Office.Interop.Excel;

namespace BookFilter
{
    /// <summary>
    /// Excel Helper v0.1
    /// </summary>
    public class ExcelHelper
    {
        public ExcelHelper()
        {
            //
        }

        public DataSet Excel2DataSet(string path)
        {
            DataSet ds = new DataSet();
            string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + path + ";" + "Extended Properties=\"Excel 8.0;IMEX=1\";";
            try
            {
                using (OleDbConnection conn = new OleDbConnection(strConn))
                {
                    conn.Open();
                    //返回Excel的架构，包括各个sheet表的名称,类型，创建时间和修改时间等  
                    System.Data.DataTable dtSheetName = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" });

                    //包含excel中表名的字符串数组
                    string[] strTableNames = new string[dtSheetName.Rows.Count];
                    for (int k = 0; k < dtSheetName.Rows.Count; k++)
                    {
                        strTableNames[k] = dtSheetName.Rows[k]["TABLE_NAME"].ToString();
                    }

                    OleDbDataAdapter myCommand = null;
                    System.Data.DataTable dt = new System.Data.DataTable();

                    //从指定的表明查询数据,可先把所有表明列出来供用户选择
                    string strExcel = "select * from [" + strTableNames[0] + "]";
                    myCommand = new OleDbDataAdapter(strExcel, strConn);
                    myCommand.Fill(ds, "table1");
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //RedirectErrorPage("导出excel数据到dataset出错，请联系管理员" + ex);
            }
            return ds;
        }

        /// <summary>  
        /// 通过工作表名 获取数据  
        /// </summary>  
        /// <param name="name"></param>  
        /// <returns></returns>  
        public System.Data.DataTable GetContentBySheetName(OleDbConnection conn, string name)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            OleDbDataAdapter myCommand = null;
            string strExcel = "select * from [" + name + "]";

            using (myCommand = new OleDbDataAdapter(strExcel, conn))
            {
                myCommand.Fill(dt);
                return dt;
            }

        }

        /// <summary>
        /// 加载excel文件到dataset中
        /// </summary>
        /// <param name="path">Excel文件路径</param>
        /// <returns>DataSet</returns>
        public DataSet Excel2DataSetWithSheet(string path)
        {
            DataSet dataSet = new DataSet();
            string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + path + ";" + "Extended Properties=\"Excel 8.0;IMEX=1\";";
            try
            {
                System.Data.DataTable dt = new System.Data.DataTable();

                using (OleDbConnection conn = new OleDbConnection(strConn))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    // 可以过滤隐藏sheet
                    List<string> tableNames = GetSheetNameFromExcel(conn);

                    string[] strArr = new string[tableNames.Count];

                    tableNames.CopyTo(strArr);

                    List<string> sheets = new List<string>();
                    sheets.AddRange(strArr);


                    // 以sheetName新建文件夹
                    for (int i = 0; i < sheets.Count; i++)
                    {
                        //处理sheetName的#号 和 去掉末尾的$ like this:www#16k#net#cn$
                        string curSheetName = sheets[i];
                        curSheetName = curSheetName.Replace('#', '.');
                        curSheetName = curSheetName.TrimEnd('$');

                        //拼接创建的目录
                        int last = path.LastIndexOf('\\');
                        string dir = path.Substring(0, last);
                        string dirPath = dir + "\\" + curSheetName;

                        if (!Directory.Exists(dirPath))
                        {
                            Directory.CreateDirectory(dirPath);
                        }
                    }


                    if (null != tableNames && tableNames.Count > 0)
                    {
                        foreach (string strTable in tableNames)
                        {
                            //获取sheet页的内容
                            dt = GetContentBySheetName(conn, strTable);
                            dt.TableName = strTable;
                            dataSet.Tables.Add(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Excel转化成DataSet异常
                throw ex;
            }
            return dataSet;
        }

        /// <summary>  
        /// 获取Excel 中的工作表  
        /// </summary>  
        /// <returns></returns>  
        public List<string> GetSheetNameFromExcel(OleDbConnection conn)
        {
            System.Data.DataTable dtSheetName = null;
            try
            {
                dtSheetName = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" });
                List<string> strTableNames = new List<string>();
                for (int i = 0; i < dtSheetName.Rows.Count; i++)
                {
                    string s = dtSheetName.Rows[i]["TABLE_NAME"].ToString();

                    //过滤一下没用的表，Excel 默认生成的隐藏文件  
                    if (!s.Contains("_FilterDatabase") && s.LastIndexOf('_') + 1 != s.Length)
                    {
                        strTableNames.Add(s);
                    }
                }
                return strTableNames;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 将List转化成DataSet
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="list">list</param>
        /// <returns>dataset</returns>
        private DataSet ListToDataSet<T>(List<T> list)
        {
            //list is nothing or has nothing, return nothing (or add exception handling)
            if (list == null || list.Count == 0) { return null; }

            //get the type of the first obj in the list
            Type obj = list[0].GetType();

            //now grab all properties
            System.Reflection.PropertyInfo[] properties = obj.GetProperties();

            //make sure the obj has properties, return nothing (or add exception handling)
            if (properties.Length == 0) { return null; }

            //it does so create the dataset and table
            DataSet dataSet = new DataSet();
            System.Data.DataTable dataTable = new System.Data.DataTable();

            //now build the columns from the properties
            System.Data.DataColumn[] columns = new DataColumn[properties.Length];
            for (int i = 0; i < properties.Length; i++)
            {
                columns[i] = new DataColumn(properties[i].Name, properties[i].PropertyType);
            }

            //add columns to table
            dataTable.Columns.AddRange(columns);

            //now add the list values to the table
            foreach (var item in list)
            {
                //create a new row from table
                var dataRow = dataTable.NewRow();

                //now we have to iterate thru each property of the item and retrieve it's value for the corresponding row's cell
                var itemProperties = item.GetType().GetProperties();

                for (int i = 0; i < itemProperties.Length; i++)
                {
                    dataRow[i] = itemProperties[i].GetValue(item, null);
                }

                //now add the populated row to the table
                dataTable.Rows.Add(dataRow);
            }

            //add table to dataset
            dataSet.Tables.Add(dataTable);

            //return dataset
            return dataSet;
        }

        /// <summary>
        /// 下载execl文件，适用于web项目
        /// </summary>
        private void DownloadExcel()
        {
            //Response.Clear();
            //Response.ClearHeaders();
            //Response.Buffer = true;
            //Response.AddHeader("Accept-Language", "zh-cn");
            //// UrlEncode防止文件名出现乱码
            //string fileName = HttpUtility.UrlEncode(this.txtPayMonth.Text + this.ddlDataSourceExport.SelectedItem + "result.csv");
            //Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            //Response.ContentType = "application/octet-stream";
            ////ExportToExcel(ds);  // 把DataSet导出成Excel.csv格式              
            //authorPay.ExportToExcel(ds);
            //Response.Flush();
        }

        /// <summary>
        /// 把DataSet导出到Excel中并且可以分sheet
        /// </summary>
        /// <param name="dataSet">要导出的数据来源</param>
        /// <param name="fileName">导出的Excel名称</param>
        public void DataSetToLocalExcel(DataSet dataSet, string fileName)
        {
            string outputPath = string.Empty;

            bool deleteOldFile = true;
            if (deleteOldFile)
            {
                if (System.IO.File.Exists(outputPath)) { System.IO.File.Delete(outputPath); }
            }

            // Create the Excel Application object
            Application excelApp = new Application();

            // Create a new Excel Workbook
            Workbook excelWorkbook = excelApp.Workbooks.Add(Type.Missing);

            int sheetIndex = 0;

            // 遍历每张 DataTable
            foreach (System.Data.DataTable dt in dataSet.Tables)
            {
                // Create a new Sheet
                Worksheet excelSheet = (Worksheet)excelWorkbook.Sheets.Add(
                    excelWorkbook.Sheets.get_Item(++sheetIndex),
                    Type.Missing, 1, XlSheetType.xlWorksheet);
                excelSheet.Name = dt.TableName;

                //初始化Sheet中的变量
                int rowIndex = 1;
                int colIndex = 1;

                //列出标题
                foreach (DataColumn col in dt.Columns)
                {
                    //LogHelper.Info(" 调用Excel组件，col.ColumnName：" + col.ColumnName );
                    excelApp.Cells[1, colIndex] = col.ColumnName;
                    excelSheet.get_Range(excelApp.Cells[1, colIndex], excelApp.Cells[1, colIndex]).HorizontalAlignment = XlVAlign.xlVAlignCenter;//设置标题格式为居中对齐 
                    colIndex++;
                }

                // Mark the first row as BOLD
                ((Range)excelSheet.Rows[1, Type.Missing]).Font.Bold = true;

                //列出行
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];

                    //新起一行，当前单元格移至行首
                    rowIndex++;
                    colIndex = 1;

                    foreach (DataColumn col in dt.Columns)
                    {
                        if (col.DataType == System.Type.GetType("System.String"))
                        {
                            excelApp.Cells[rowIndex, colIndex] = "'" + row[col.ColumnName].ToString();
                        }
                        else
                        {
                            excelApp.Cells[rowIndex, colIndex] = row[col.ColumnName].ToString();
                        }
                        colIndex++;
                    }

                    // 设置边框
                    // 计算最后一列的字母标识
                    string finalColLetter = string.Empty;
                    string colCharset = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    int colCharsetLen = colCharset.Length;

                    if (dt.Columns.Count > colCharsetLen)
                    {
                        finalColLetter = colCharset.Substring(
                            (dt.Columns.Count - 1) / colCharsetLen - 1, 1);
                    }

                    finalColLetter += colCharset.Substring(
                            (dt.Columns.Count - 1) % colCharsetLen, 1);

                    // 列出范围：标识成 A1:F9 这样的
                    string excelRange = string.Format("A1:{0}{1}",
                        finalColLetter, (dt.Rows.Count + 1));

                    //使用最佳宽度,设置样式
                    Range allDataWithTitleRange = excelSheet.get_Range(excelRange, Type.Missing);
                    allDataWithTitleRange.Select();
                    allDataWithTitleRange.Columns.AutoFit();
                    allDataWithTitleRange.Borders.LineStyle = 1;//将导出Excel加上边框
                }
            }

            excelApp.Application.DisplayAlerts = false;
            // 保存Excel并且关闭excelWorkbook对象
            excelWorkbook.SaveAs(outputPath, XlFileFormat.xlWorkbookNormal, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlExclusive,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            excelWorkbook.Close(true, Type.Missing, Type.Missing);
            excelWorkbook = null;

            // Release the Application object
            excelApp.Quit();
            excelApp = null;

            // 回收未引用对象
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

    }
}