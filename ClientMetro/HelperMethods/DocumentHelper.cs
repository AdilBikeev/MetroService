using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientMetro.HelperMethods
{
    public static class DocumentHelper
    {
        /// <summary>
        /// Разделитель по умолчанию
        /// </summary>
        public static char delimiterDefault = ',';

        /// <summary>
        /// Преобразовывает из элементов строки, разделенными запятой, в List
        /// </summary>
        /// <param name="docList">Строка с элементами разделенными запятыми</param>
        /// <returns>Возвращает представление строки в List<string></string></returns>
        public static List<string> ParseToList(string docList)
        {
            var list = docList.Split(delimiterDefault).ToList();
            return list;
        }

        /// <summary>
        /// Преобразовывает из элементов строки, разделенными символом delimiter, в List
        /// </summary>
        /// <param name="docList">Строка с элементами разделенными символом delimiter</param>
        /// <param name="delimiter">Разделитель элементов строки</param>
        /// <returns>Возвращает представление строки в List<string></returns>
        public static List<string> ParseToList(string docList, char delimiter)
        {
            var list = docList.Split(delimiter).ToList();
            return list;
        }
    }
}
