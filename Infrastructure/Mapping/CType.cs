using System;
using System.Collections.Generic;
using System.Text;

/**********************************************************

*创建人：  liuhd4
*创建时间：2020/4/2 16:21:57
*描述：
*
***********************************************************/
namespace Lenovo.PMS.Infrastruct.Mappings
{
    public class CType
    {
        public static string INT = "int";
        public static string VARCHAR = "varchar";
        public static string NVARCHAR = "nvarchar";
        public static string BIGINT = "bigint";
        public static string DATETIME = "datetime";
        public static string DECIMAL2 = "decimal(18,2)";
        public static string DECIMAL4 = "decimal(18,4)";
        public static string DECIMAL6 = "decimal(18,6)";

        public static string VARCHAR_L(int length)
        {
            return VARCHAR + "(" + length.ToString() + ")";
        }

        public static string NVARCHAR_L(int length)
        {
            return NVARCHAR + "(" + length.ToString() + ")";
        }
    }

}
