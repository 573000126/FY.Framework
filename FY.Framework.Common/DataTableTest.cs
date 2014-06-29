using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace FY.Framework.Common
{
    /// <summary>
    /// 这个类用来测试合并2个表格结果是否是left join
    /// </summary>
    public class DataTableTest
    {
        public DataTableTest()
        {
        }

        /// <summary>
        /// 初始化2个DataTable，用于测试
        /// </summary>
        public List<DataTable> Init2Table(DataTable dt1, DataTable dt2)
        {
            //创建第一张表格，包含3列
            dt1 = new DataTable("TestMergeTable1");
            dt1.Columns.Add(new DataColumn("col1", typeof(System.String)));
            dt1.Columns.Add(new DataColumn("col2", typeof(string)));
            dt1.Columns.Add(new DataColumn("colmerge", typeof(string)));

            //创建第2张表格，包含2列
            dt2 = new DataTable("TestMergeTable2");
            dt2.Columns.Add(new DataColumn("col1", typeof(System.String)));
            dt2.Columns.Add(new DataColumn("col2", typeof(string)));

            //给第1张表格添加三行数据
            DataRow dr1 = dt1.NewRow();
            dr1["col1"] = "A";
            dr1["col2"] = "Apple";
            dt1.Rows.Add(dr1);

            DataRow dr2 = dt1.NewRow();
            dr2["col1"] = "B";
            dr2["col2"] = "Banana";
            dt1.Rows.Add(dr2);

            DataRow dr3 = dt1.NewRow();
            dr3["col1"] = "C";
            dr3["col2"] = "Cup";
            dt1.Rows.Add(dr3);

            //给第2张表格添加2行数据
            DataRow dr4 = dt2.NewRow();
            dr4["col1"] = "A";
            dr4["col2"] = "Amani";
            dt2.Rows.Add(dr4);

            DataRow dr5 = dt2.NewRow();
            dr5["col1"] = "B";
            dr5["col2"] = "Bug";
            dt2.Rows.Add(dr5);

            List<DataTable> listdt = new List<DataTable>();
            listdt.Add(dt1);
            listdt.Add(dt2);

            return listdt;

        }

        /// <summary>
        /// 测试合并Table是否是 left join
        /// </summary>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        /// <returns></returns>
        public DataTable TestMerger(DataTable dt1, DataTable dt2)
        {
            if (dt1 != null && dt1.Rows.Count > 0)
            {
                foreach (DataRow rowdt1 in dt1.Rows)
                {

                    if (dt2 != null && dt2.Rows.Count > 0)
                    {
                        foreach (DataRow rowdt2 in dt2.Rows)
                        {
                            string coldt1 = rowdt1["col1"].ToString();
                            string coldt2 = rowdt2["col1"].ToString();

                            if (coldt1 == coldt2)
                            {
                                rowdt1["colmerge"] = coldt2;
                            }
                        }
                    }
                }
            }

            return dt1;
        }

        /// <summary>
        /// 打印DataTable
        /// </summary>
        /// <param name="dt"></param>
        public void PrintDataTable(DataTable dt)
        {
            //打印结果Table
            if (dt != null && dt.Rows.Count > 0)
            {
                //打印列名
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    Console.Write(dt.Columns[i] + "\t");
                }

                Console.WriteLine();

                //打印每一行
                foreach (DataRow item in dt.Rows)
                {
                    Console.WriteLine(string.Format("{0}\t{1}\t{2}", item["col1"], item["col2"], item["colmerge"]));
                }
            }
        }

        //直接运行这个方法进行测试
        public void RunInClient()
        {
            DataTableTest tmt = new DataTableTest();
            DataTable dt1 = null;
            DataTable dt2 = null;
            List<DataTable> list = tmt.Init2Table(dt1, dt2);

            if (list != null && list.Count > 0)
            {
                dt1 = list[0];
                dt2 = list[1];
            }

            Console.WriteLine("----------2个表格生成完毕：----------");

            //打印生成的表格
            Console.WriteLine("Table1: " + dt1.TableName + "\n");
            tmt.PrintDataTable(dt1);

            Console.WriteLine("\nTable2: " + dt2.TableName + "\n");
            foreach (DataRow rowdt2 in dt2.Rows)
            {
                Console.WriteLine("{0}\t{1}", rowdt2["col1"], rowdt2["col2"]);
            }



            DataTable dtResult = tmt.TestMerger(dt1, dt2);

            Console.WriteLine("\n----------2个表格合并完毕：----------");

            //打印结果表格
            tmt.PrintDataTable(dtResult);
        }
    }
}
