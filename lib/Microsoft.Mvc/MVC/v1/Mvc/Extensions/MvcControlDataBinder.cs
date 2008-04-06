namespace System.Web.Mvc {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;

    public static class MvcControlDataBinder {

        /// <summary>
        /// Accepts IEnumerable, IQueryable, IList, DataSet, DataTable, or IDataReader and converts to a Dictionary
        /// for use with list-based controls
        /// </summary>
        /// <param name="dataSource">IEnumerable, IQueryable, IList, DataSet, DataTable, or IDataReader</param>
        /// <param name="keyField">The field to use as the key, or value of the data</param>
        /// <param name="valueField">The fiels to use as the text</param>
        /// <returns>System.Collections.Dictionary</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "This cast is part of a conditional and happens only if the Type involved is complimentary. Therefore it only happens once per call")]
        public static Dictionary<object, object> SourceToDictionary(object dataSource, string keyField, string valueField) {
            Dictionary<object, object> result = new Dictionary<object, object>();

            if (dataSource is DataSet) {
                DataSet ds = dataSource as DataSet;
                result = HandleDataTable(ds.Tables[0], keyField, valueField);

            }
            else if (dataSource is DataTable) {
                DataTable tbl = dataSource as DataTable;
                result = HandleDataTable(tbl, keyField, valueField);

            }
            else if (dataSource is IDataReader) {
                IDataReader rdr = dataSource as IDataReader;
                result = HandleIDataReader(rdr, keyField, valueField);

            }
            else if (dataSource is IList) {
                IList listSource = dataSource as IList;
                result = HandleIList(listSource, keyField, valueField);

            }
            else if (dataSource is Array) {
                IEnumerable arraySource = dataSource as IEnumerable;
                result = HandleArray(arraySource, keyField, valueField);

            }
            else if (dataSource is IQueryable) {
                IQueryable qry = dataSource as IQueryable;
                result = EnumerateSource(qry, keyField, valueField);
            }
            else if (dataSource is IEnumerable) {
                IEnumerable enSource = dataSource as IEnumerable;
                result = EnumerateSource(enSource, keyField, valueField);
            }

            return result;
        }


        private static Dictionary<object, object> HandleIDataReader(IDataReader dataSource, string keyField, string valueField) {
            Dictionary<object, object> result = new Dictionary<object, object>();

            while (dataSource.Read()) {
                if (!String.IsNullOrEmpty(keyField) && !String.IsNullOrEmpty(valueField))
                    result.Add(dataSource[keyField], dataSource[valueField]);
                else {
                    if (dataSource.FieldCount > 1) {
                        result.Add(dataSource[0], dataSource[1]);
                    }
                    else {
                        result.Add(dataSource[0], dataSource[0]);

                    }
                }

            }
            dataSource.Close();

            return result;
        }

        private static Dictionary<object, object> HandleArray(IEnumerable dataSource, string keyField, string valueField) {
            Dictionary<object, object> result = new Dictionary<object, object>();

            //grab the enumerator
            IEnumerator en = dataSource.GetEnumerator();
            object item = null;

            // TODO: Verify. What if the array is empty? What if the array contains null values?

            //test the first element
            while (en.MoveNext()) {
                item = en.Current;
                break;
            }

            if (item.GetType().IsPrimitive || item is String || item is DateTime) {
                result = EnumerateSingleElementSource(dataSource);
            }
            else {
                result = EnumerateSource(dataSource, keyField, valueField);
            }

            return result;
        }


        private static Dictionary<object, object> HandleIList(IList dataSource, string keyField, string valueField) {
            Dictionary<object, object> result = new Dictionary<object, object>();

            //test the first element
            if (dataSource.Count > 0) {
                object item = dataSource[0];
                if (item.GetType().IsPrimitive || item is String || item is DateTime) {
                    result = EnumerateSingleElementSource(dataSource);
                }
                else {
                    result = EnumerateSource(dataSource, keyField, valueField);
                }
            }

            return result;
        }

        private static Dictionary<object, object> EnumerateSingleElementSource(IEnumerable dataSource) {

            Dictionary<object, object> result = new Dictionary<object, object>();
            IEnumerator en = dataSource.GetEnumerator();

            while (en.MoveNext()) {
                result.Add(en.Current, en.Current.ToString());
            }
            return result;
        }


        private static Dictionary<object, object> EnumerateSource(IEnumerable dataSource, string keyField, string valueField) {
            Dictionary<object, object> result = new Dictionary<object, object>();
            IEnumerator en = dataSource.GetEnumerator();
            object keyElement = string.Empty;
            object valueElement = null;
            Type elementType = null;
            PropertyInfo[] props = null;

            int indexer = 0;
            int propCount = 0;

            while (en.MoveNext()) {

                if (elementType == null)
                    elementType = en.Current.GetType();

                if (indexer == 0)
                    propCount = elementType.GetProperties().Count();

                try {
                    if (!String.IsNullOrEmpty(keyField) && !String.IsNullOrEmpty(valueField)) {

                        keyElement = elementType.GetProperty(keyField).GetValue(en.Current, null);
                        valueElement = elementType.GetProperty(valueField).GetValue(en.Current, null);

                    }
                    else if (!String.IsNullOrEmpty(keyField)) {
                        keyElement = elementType.GetProperty(keyField).GetValue(en.Current, null);
                        valueElement = keyElement;

                    }
                    else {
                        if (props == null)
                            props = elementType.GetProperties();
                        //take the first two properties
                        if (propCount > 1) {
                            keyElement = props[0].GetValue(en.Current, null);
                            valueElement = props[1].GetValue(en.Current, null);

                        }
                        else {
                            keyElement = props[0].GetValue(en.Current, null);
                            valueElement = keyElement;
                        }
                    }

                    result.Add(keyElement, valueElement);
                }
                catch (Exception x) {
                    throw new InvalidOperationException(elementType.Name + " doesn't contain one of these properties: " + keyField + "; " + valueField, x);
                }

                indexer++;

            }

            return result;
        }

        private static Dictionary<object, object> HandleDataTable(DataTable dataSource, string keyField, string valueField) {
            Dictionary<object, object> result = new Dictionary<object, object>();

            foreach (DataRow dr in dataSource.Rows) {

                if (String.IsNullOrEmpty(keyField)) {
                    if (dataSource.Columns.Count > 1) {
                        result.Add(dr[0], dr[1]);
                    }
                    else {
                        result.Add(dr[0], dr[0]);
                    }
                }
                else {
                    result.Add(dr[keyField], dr[valueField]);

                }
            }

            return result;
        }
    }
}
